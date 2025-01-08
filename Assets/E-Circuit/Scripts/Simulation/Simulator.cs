using System;
using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using System.Linq;
using SpiceSharp.Simulations;
using SpiceSharp.Validation;

namespace ECircuit.Simulation
{
    public class Simulator : MonoBehaviour
    {
        [SerializeField]
        private Generator m_MainGenerator;

        private bool m_DidSimulate = false;

        public Generator MainGenerator { get => m_MainGenerator; }

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
                    name = Connection.RandomName();
                    connection.ConnectionName = name;
                    connectionsByName.Add(name, connection);
                }
            }
        }

        public void SetSimulationNeeded()
        {
            m_DidSimulate = false;
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

            try
            {

                foreach (var _ in dc.Run(scCircuit))
                {
                    foreach (var connection in circuit.Connections)
                    {
                        connection.CurrentVoltage = dc.GetVoltage(connection.ConnectionName);
                    }
                }
            }
            catch (ValidationFailedException e)
            {
                PrintRuleViolations(e);
            }
        }

        private static void PrintRuleViolations(ValidationFailedException e)
        {
            foreach (var rule in e.Rules)
            {
                if (rule.ViolationCount == 0)
                    continue;
                Debug.LogError($"{rule}: ");
                foreach (var violation in rule.Violations)
                {
                    Debug.LogError($"- {violation}");
                    if (violation is FloatingNodeRuleViolation fv)
                    {
                        Debug.LogError($"  Cond. Type: {fv.Type}, Floating: ${fv.FloatingVariable}, Fixed: {fv.FixedVariable}");
                    }
                    else if (violation is VoltageLoopRuleViolation lv)
                    {
                        Debug.LogError($"  Subject: {lv.Subject}, First: {lv.First}, Second: {lv.Second}");
                    }
                    else if (violation is VariablePresenceRuleViolation vv)
                    {
                        Debug.LogError($"  Subject: {vv.Subject}, Variable: {vv.Variable}");
                    }
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
