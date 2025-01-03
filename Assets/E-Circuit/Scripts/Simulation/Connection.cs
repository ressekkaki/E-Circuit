using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECircuit.Simulation
{
    public class Connection : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The list of connectors this connector is connected to.")]
        private List<Connector> m_ConnectedTo;

        [Header("Simulation Values DO NOT EDIT")]
        [SerializeField]
        private double m_CurrentVoltage = 0.0;

        public string ConnectionName
        {
            get => name;
            set => name = value;
        }

        public IList<Connector> ConnectedTo
        {
            get => m_ConnectedTo;
        }

        public void Awake()
        {
            m_ConnectedTo ??= new List<Connector>();
            foreach (var connector in m_ConnectedTo)
            {
                connector.Connection = this;
            }
        }

        private void OnDestroy()
        {
            List<Connector> toRemove = new(m_ConnectedTo);
            foreach (var connector in toRemove)
            {
                connector.Connection = null;
            }
        }

        public static string RandomName()
        {
            return $"W-{Guid.NewGuid()}";
        }

        public double CurrentVoltage
        {
            get => m_CurrentVoltage;
            set => m_CurrentVoltage = value;
        }

        public static void DestroyIfInvalid(Connection connection)
        {
            if (connection != null && connection.m_ConnectedTo.Count < 2)
            {
                Destroy(connection.gameObject);
            }
        }
    }
}
