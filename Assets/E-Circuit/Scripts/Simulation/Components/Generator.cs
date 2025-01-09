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
        [Tooltip("Positive connector of the generator")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the generator")]
        private Connector m_Negative;


        [SerializeField]
        [Tooltip("The voltage of the generator, in Volts")]
        private double m_Voltage = 5.0f;

        public Connector Negative { get => m_Negative; }

        public override IEnumerable<Connector> Connectors
        {
            get => new Connector[2] { m_Positive, m_Negative };
        }

        public double Voltage
        {
            get => m_Voltage;
            set => m_Voltage = value;
        }

        public override IEnumerable<IEntity> BuildEntity()
        {
            var pos = m_Positive.ConnectionName;
            var neg = m_Negative.ConnectionName;
            Debug.Log($"Building Generator: Name={ComponentName}, Positive={pos}, Negative={neg}, Voltage={m_Voltage}V");
            yield return new VoltageSource(ComponentName, pos, neg, m_Voltage);
        }

        public override string RandomName()
        {
            return $"V-{Guid.NewGuid()}";
        }
    }
}
