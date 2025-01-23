using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using SpiceSharp.Entities;
using System;

namespace ECircuit.Simulation
{
    public class Led : BaseComponent
    {
        [SerializeField]
        [Tooltip("Positive connector of the LED")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the LED")]
        private Connector m_Negative;

        [SerializeField]
        [Tooltip("The color of the LED")]
        private Color m_Color = Color.red;

        [SerializeField]
        private double m_MinCurrent = 0.001f;

        [SerializeField]
        [Tooltip("The maximum current the LED can handle (before it burns out IRL)")]
        private double m_MaxCurrent = 0.03f;

        public Color Color { get => m_Color; set => m_Color = value; }

        public double MinCurrent { get => m_MinCurrent; }

        public double MaxCurrent { get => m_MaxCurrent; }

        public override IEnumerable<Connector> Connectors
        {
            get => new Connector[2] { m_Positive, m_Negative };
        }

        public override IEnumerable<IEntity> BuildEntity()
        {
            var pos = m_Positive.ConnectionName;
            var neg = m_Negative.ConnectionName;
            Debug.Log($"Building LED: Name={ComponentName}, Positive={pos}, Negative={neg}");
            yield return new SpiceSharp.Components.Diode(ComponentName, pos, neg, "DiodeModel");
        }

        public override string RandomName()
        {
            return $"LED-{Guid.NewGuid()}";
        }

        public override bool IsOverloaded()
        {
            return CurrentCurrent > m_MaxCurrent;
        }
    }
}
