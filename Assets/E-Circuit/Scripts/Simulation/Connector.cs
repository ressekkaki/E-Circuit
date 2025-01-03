using UnityEngine;
using ECircuit.Simulation.Components;
using System;

namespace ECircuit.Simulation
{

    public class Connector : MonoBehaviour
    {

        [SerializeField]
        private BaseComponent m_Owner = null;

        [SerializeField]
        [Header("DO NOT CHANGE IN EDITOR")]
        [Tooltip("The connection this connector is a part of.")]
        private Connection m_Connection = null;

        public Connection Connection
        {
            set
            {
                if (m_Connection != value)
                {
                    if (m_Connection != null)
                    {
                        var prevConnection = m_Connection;
                        m_Connection = null;
                        prevConnection.ConnectedTo.Remove(this);
                    }
                    if (value != null && !value.ConnectedTo.Contains(this))
                    {
                        value.ConnectedTo.Add(this);
                    }
                }
                m_Connection = value;
            }
            get => m_Connection;
        }

        public BaseComponent Owner
        {
            get => m_Owner;
            set => m_Owner = value;
        }

        public string ConnectionName
        {
            get
            {
                if (m_Connection == null)
                {
                    return $"DISCONNECTED-{Guid.NewGuid()}";
                }
                return m_Connection.ConnectionName;
            }
        }
    }
}
