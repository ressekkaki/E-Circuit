using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using SpiceSharp.Entities;
using System;

namespace ECircuit.Simulation
{
    public class Resistor : BaseComponent
    {
        [SerializeField]
        [Tooltip("The name of the resistor, must be unique. Automatically generated when null.")]
        private string m_ComponentName;

        [SerializeField]
        [Tooltip("Positive connector of the resistor")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the resistor")]
        private Connector m_Negative;

        [SerializeField]
        [Tooltip("The resistance of the resistor, in Ohms")]
        private double m_Resistance = 100.0;

        public override string ComponentName
        {
            get => m_ComponentName;
            set => m_ComponentName = value;
        }

        public override IEnumerable<Connector> Connectors
        {
            get => new Connector[2] { m_Positive, m_Negative };
        }

        public double Resistance
        {
            get => m_Resistance;
            set => m_Resistance = value;
        }

        public override IEntity BuildEntity()
        {
            Debug.Log($"Building Resistor: Name={ComponentName}, Positive={m_Positive.Connection.ConnectionName}, Negative={m_Negative.Connection.ConnectionName}, Resistance={m_Resistance}Ω");
            return new SpiceSharp.Components.Resistor(ComponentName, m_Positive.Connection.ConnectionName, m_Negative.Connection.ConnectionName, m_Resistance);
        }

        public override string RandomName()
        {
            return $"R-{Guid.NewGuid()}";
        }
    }
}
