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
        [Tooltip("Positive connector of the resistor")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the resistor")]
        private Connector m_Negative;

        [SerializeField]
        [Tooltip("The resistance of the resistor, in Ohms")]
        private double m_Resistance = 100.0;

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
            var pos = m_Positive.ConnectionName;
            var neg = m_Negative.ConnectionName;
            Debug.Log($"Building Resistor: Name={ComponentName}, Positive={m_Positive.ConnectionName}, Negative={m_Negative.ConnectionName}, Resistance={m_Resistance}Ω");
            return new SpiceSharp.Components.Resistor(ComponentName, pos, neg, m_Resistance);
        }

        public override string RandomName()
        {
            return $"R-{Guid.NewGuid()}";
        }
    }
}
