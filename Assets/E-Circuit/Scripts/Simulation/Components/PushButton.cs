using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using SpiceSharp.Entities;
using System;

namespace ECircuit.Simulation
{
    public class PushButton : BaseComponent
    {
        [SerializeField]
        [Tooltip("First connector of the button")]
        private Connector m_Connector1;

        [SerializeField]
        [Tooltip("Second connector of the button")]
        private Connector m_Connector2;

        [Header("Runtime Values, DO NOT CHANGE IN EDITOR")]
        [SerializeField]
        [Tooltip("Whether the button is pressed")]
        private bool m_Pressed = false;
        private bool m_WasPressed = false;

        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_simulator;

        private string m_VoltageControlName;
        private string m_VoltageControlPos;

        public bool IsPressed { get => m_Pressed; }

        public override IEnumerable<Connector> Connectors
        {
            get => new Connector[2] { m_Connector1, m_Connector2 };
        }

        public override IEnumerable<IEntity> BuildEntity()
        {
            var conn1 = m_Connector1.ConnectionName;
            var conn2 = m_Connector2.ConnectionName;
            m_VoltageControlName = $"BCTRL-{Guid.NewGuid()}";
            m_VoltageControlPos = $"BCTRL-2-{Guid.NewGuid()}";

            yield return new SpiceSharp.Components.VoltageSource(m_VoltageControlName, m_VoltageControlPos, "0", m_Pressed ? 5.0 : 0.0);
            Debug.Log($"Building Push Button: Name={ComponentName}, Conn1={conn1}, Conn2={conn2}");
            yield return new SpiceSharp.Components.VoltageSwitch(ComponentName, conn1, conn2, m_VoltageControlPos, "0", "PushButtonModel");
        }

        public override string RandomName()
        {
            return $"B-{Guid.NewGuid()}";
        }

        public void Awake()
        {
            if (m_simulator == null)
            {
                m_simulator = FindFirstObjectByType<Simulator>();
            }
        }

        public void Update()
        {
            if (m_Pressed != m_WasPressed)
            {
                m_simulator.NeedSimulation = true;
            }
            m_WasPressed = m_Pressed;
        }
    }
}
