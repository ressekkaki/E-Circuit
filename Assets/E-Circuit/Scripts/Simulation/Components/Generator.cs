using System;
using System.Collections.Generic;
using SpiceSharp.Components;
using SpiceSharp.Entities;
using UnityEngine;

namespace ECircuit.Simulation.Components
{
    public class Generator : BaseComponent
    {
        [SerializeField]
        [Tooltip("The name of the resistor, must be unique. Automatically generated when null.")]
        private string m_ComponentName;

        [SerializeField]
        [Tooltip("Positive connector of the generator")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the generator")]
        private Connector m_Negative;


        [SerializeField]
        [Tooltip("The voltage of the generator, in Volts")]
        private double m_Voltage = 5.0f;

        public override string ComponentName
        {
            get => m_ComponentName;
            set => m_ComponentName = value;
        }

        public override IEnumerable<Connector> Connectors
        {
            get => new Connector[2] { m_Positive, m_Negative };
        }

        public double Voltage
        {
            get => m_Voltage;
            set => m_Voltage = value;
        }

        public override IEntity BuildEntity()
        {
            Debug.Log($"Building Generator: Name={ComponentName}, Positive={m_Positive.Connection.ConnectionName}, Negative={m_Negative.Connection.ConnectionName}, Voltage={m_Voltage}V");
            return new VoltageSource(ComponentName, m_Positive.Connection.ConnectionName, m_Negative.Connection.ConnectionName, m_Voltage);
        }

        public override string RandomName()
        {
            return $"V-{Guid.NewGuid()}";
        }
    }
}
