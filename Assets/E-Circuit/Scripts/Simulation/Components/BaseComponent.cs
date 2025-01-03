using System;
using System.Collections.Generic;
using SpiceSharp.Entities;
using UnityEngine;

namespace ECircuit.Simulation.Components
{
    public abstract class BaseComponent : MonoBehaviour
    {
        public abstract string ComponentName { get; set; }

        public abstract IEnumerable<Connector> Connectors { get; }

        /// <summary>
        /// Realizes the component as a SpiceSharp circuit entity.
        /// </summary>
        public abstract IEntity BuildEntity();

        public virtual string RandomName()
        {
            return $"C-{Guid.NewGuid()}";
        }
    }
}
