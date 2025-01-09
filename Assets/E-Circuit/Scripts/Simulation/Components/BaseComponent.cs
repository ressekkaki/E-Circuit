using System;
using System.Collections.Generic;
using SpiceSharp.Entities;
using UnityEngine;

namespace ECircuit.Simulation.Components
{
    public abstract class BaseComponent : MonoBehaviour
    {
        public string ComponentName { get => name; set => name = value; }

        public abstract IEnumerable<Connector> Connectors { get; }

        /// <summary>
        /// Realizes the component as a SpiceSharp circuit entity.
        /// </summary>
        public abstract IEntity BuildEntity();

        public virtual string RandomName()
        {
            return $"C-{Guid.NewGuid()}";
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
