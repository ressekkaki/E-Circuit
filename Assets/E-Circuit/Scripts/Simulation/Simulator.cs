using System;
using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using System.Linq;
using SpiceSharp.Simulations;

namespace ECircuit.Simulation
{
    public class Simulator : MonoBehaviour
    {
        [SerializeField]
        private Generator m_MainGenerator;

        private bool m_DidSimulate = false;


        public void Awake()
        {
            m_DidSimulate = false;
        }

        public void OnDisable()
        {
            m_DidSimulate = false;
        }

        public void OnEnable()
        {
            m_DidSimulate = false;
        }

        public void Update()
        {
            if (m_DidSimulate)
            {
                return;
            }
            m_DidSimulate = true;
            var circuit = GatherCircuitComponents();
            Debug.Log($"Gathered circuit components, {circuit.Components.Count} components and {circuit.Connections.Count} connections");
            CheckCircuitNames(circuit);
            Simulate(circuit);
        }

        private Circuit GatherCircuitComponents()
        {
            var toVisit = new List<BaseComponent> { m_MainGenerator };
            var allComponents = new HashSet<BaseComponent>();
            var allConnections = new HashSet<Connection>();

            while (toVisit.Count > 0)
            {
                var current = toVisit[^1];
                toVisit.RemoveAt(toVisit.Count - 1);

                if (current == null || allComponents.Contains(current))
                {
                    continue;
                }
                allComponents.Add(current);

                foreach (var connector in current.Connectors)
                {
                    if (connector.Connection == null || allConnections.Contains(connector.Connection))
                    {
                        continue;
                    }
                    allConnections.Add(connector.Connection);
                    toVisit.AddRange(connector.Connection.ConnectedTo.Select(c => c.Owner));
                }
            }

            return new Circuit(allComponents, allConnections);
        }

        private static void CheckCircuitNames(Circuit circuit)
        {
            var componentsByName = new Dictionary<string, BaseComponent>();
            var connectionsByName = new Dictionary<string, Connection>();

            foreach (var component in circuit.Components)
            {
                var name = component.ComponentName;
                bool needNewName = name == null || name.Length == 0;

                if (!needNewName && componentsByName.ContainsKey(name))
                {
                    Debug.LogError($"Duplicate name for circuit components: {name}");
                    needNewName = true;
                }
                if (needNewName)
                {
                    name = component.RandomName();
                    component.ComponentName = name;
                    componentsByName.Add(name, component);
                }
            }

            foreach (var connection in circuit.Connections)
            {
                var name = connection.ConnectionName;
                bool needNewName = name == null || name.Length == 0;

                if (!needNewName && componentsByName.ContainsKey(name))
                {
                    Debug.LogError($"Duplicate name for circuit connection: {name}");
                    needNewName = true;
                }
                if (needNewName)
                {
                    name = connection.RandomName();
                    connection.ConnectionName = name;
                    connectionsByName.Add(name, connection);
                }
            }
        }

        private void Simulate(Circuit circuit)
        {
            if (m_MainGenerator == null)
            {
                Debug.LogError("Main generator is not set, cannot simulate");
                return;
            }

            var scCircuit = new SpiceSharp.Circuit(circuit.Components.Select(c => c.BuildEntity()));
            var dc = new DC("dc", new List<ISweep> { new ParameterSweep(m_MainGenerator.ComponentName, new List<double> { 5.0 }) });

            foreach (var _ in dc.Run(scCircuit))
            {
                foreach (var connection in circuit.Connections)
                {
                    connection.CurrentVoltage = dc.GetVoltage(connection.ConnectionName);
                }
            }
        }
    }

    class Circuit
    {
        public ISet<BaseComponent> Components;
        public ISet<Connection> Connections;

        public Circuit(ISet<BaseComponent> components, ISet<Connection> connections)
        {
            Components = components;
            Connections = connections;
        }
    }

}
