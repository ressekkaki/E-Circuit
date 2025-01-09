using System.Collections.Generic;
using UnityEngine;
using ECircuit.Simulation.Components;
using SpiceSharp.Entities;
using System;

namespace ECircuit.Simulation
{
    public class Diode : BaseComponent
    {
        [SerializeField]
        [Tooltip("Positive connector of the diode")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the diode")]
        private Connector m_Negative;

        public override IEnumerable<Connector> Connectors
        {
            get => new Connector[2] { m_Positive, m_Negative };
        }

        public override IEnumerable<IEntity> BuildEntity()
        {
            var pos = m_Positive.ConnectionName;
            var neg = m_Negative.ConnectionName;
            Debug.Log($"Building Diode: Name={ComponentName}, Positive={pos}, Negative={neg}");
            yield return new SpiceSharp.Components.Diode(ComponentName, pos, neg, "DiodeModel");
        }

        public override string RandomName()
        {
            return $"D-{Guid.NewGuid()}";
        }
    }
}
