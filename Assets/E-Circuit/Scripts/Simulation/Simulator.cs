using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using System.Linq;
using SpiceSharp.Simulations;
using SpiceSharp.Validation;
using SpiceSharp.Components;
using System;
using JetBrains.Annotations;

namespace ECircuit.Simulation
{
    public class Simulator : MonoBehaviour
    {
        [SerializeField]
        private GameObject m_CircuitRoot;

        [SerializeField]
        [Tooltip("The main generator of the circuit, leave empty to use the first generator found")]
        private Generator m_MainGenerator;

        public Generator MainGenerator { get => m_MainGenerator; }

        public GameObject CircuitRoot
        {
            get => m_CircuitRoot;
            set { m_CircuitRoot = value; }
        }
        public bool NeedSimulation { get; set; } = true;
        public bool DidSimulate { get; private set; } = false;

        public void Awake()
        {
            NeedSimulation = true;
            DidSimulate = false;
        }

        public void OnDisable()
        {
            NeedSimulation = true;
            DidSimulate = false;
        }

        public void OnEnable()
        {
            NeedSimulation = true;
            DidSimulate = false;
        }

        public void Update()
        {
            if (!NeedSimulation)
            {
                return;
            }
            NeedSimulation = false;
            DidSimulate = false;
            FindMainGenerator();
            var circuit = GatherCircuitComponents();
            Debug.Log($"Gathered circuit components, {circuit.Components.Count} components and {circuit.Connections.Count} connections");
            CheckCircuitNames(circuit);
            RenameGroundConnection();
            Simulate(circuit);
        }

        private void FindMainGenerator()
        {
            if (m_CircuitRoot != null && m_MainGenerator == null)
            {
                m_MainGenerator = m_CircuitRoot.GetComponentInChildren<Generator>(true);
            }
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

        private void RenameGroundConnection()
        {
            // // Special case: the ground connection should always be called "0"
            if (m_MainGenerator == null)
            {
                return;
            }
            var groundConnection = m_MainGenerator.Negative.Connection;

            if (groundConnection != null && groundConnection.ConnectionName != "0")
            {
                groundConnection.name = "0";
            }
        }

        public void SetSimulationNeeded()
        {
            NeedSimulation = false;
        }

        private void Simulate(Circuit circuit)
        {
            if (m_MainGenerator == null)
            {
                Debug.LogError("Main generator is not set, cannot simulate");
                return;
            }
            var scCircuit = new SpiceSharp.Circuit(circuit.Components.SelectMany(c => c.BuildEntity()))
            {
                new DiodeModel("DiodeModel"),
            };
            var dc = new DC("dc", new List<ISweep> { new ParameterSweep(m_MainGenerator.ComponentName, new List<double> { m_MainGenerator.Voltage }) });

            // Export the current (in Amps) of each component
            Dictionary<BaseComponent, RealPropertyExport> currentExports = circuit.Components.ToDictionary(c => c, c => new RealPropertyExport(dc, c.ComponentName, "i"));

            try
            {

                foreach (var _ in dc.Run(scCircuit))
                {
                    foreach (var connection in circuit.Connections)
                    {
                        connection.CurrentVoltage = dc.GetVoltage(connection.ConnectionName);
                    }
                    foreach (var entry in currentExports)
                    {
                        entry.Key.CurrentCurrent = Math.Abs(entry.Value.Value);
                    }
                }
                DidSimulate = true;
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
