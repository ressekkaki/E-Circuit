using System;
using System.Collections.Generic;
using SpiceSharp.Entities;
using UnityEngine;

namespace ECircuit.Simulation.Components
{
    public abstract class BaseComponent : MonoBehaviour
    {
        public string ComponentName { get => name; set => name = value; }

        /// <summary>
        /// The current flowing through the component in amperes.
        /// </summary>
        public double CurrentCurrent { get; set; } = 0.0;

        public abstract IEnumerable<Connector> Connectors { get; }

        /// <summary>
        /// Realizes the component as a SpiceSharp circuit entity.
        /// </summary>
        public abstract IEnumerable<IEntity> BuildEntity();

        public virtual string RandomName()
        {
            return $"C-{Guid.NewGuid()}";
        }

        public virtual bool IsOverloaded()
        {
            return false;
        }

        private void OnDestroy()
        {
            foreach (var connector in Connectors)
            {
                var connection = connector.Connection;
                connector.Connection = null;
                Connection.DestroyIfInvalid(connection);
            }
        }
    }
}
