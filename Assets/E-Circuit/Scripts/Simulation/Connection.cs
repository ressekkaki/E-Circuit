using System;
using System.Collections.Generic;
using UnityEngine;

namespace ECircuit.Simulation
{
    public class Connection : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The UNIQUE name of this connection, automatically generated when null")]
        private string m_ConnectionName;

        [SerializeField]
        [Tooltip("The list of connectors this connector is connected to.")]
        private List<Connector> m_ConnectedTo;

        [Header("Simulation Values DO NOT EDIT")]
        [SerializeField]
        private double m_CurrentVoltage = 0.0;

        public string ConnectionName
        {
            get => m_ConnectionName;
            set => m_ConnectionName = value;
        }

        public List<Connector> ConnectedTo
        {
            get => m_ConnectedTo;
        }

        public void Awake()
        {
            foreach (var connector in m_ConnectedTo)
            {
                connector.Connection = this;
            }
        }

        public string RandomName()
        {
            return $"W-{Guid.NewGuid()}";
        }

        public double CurrentVoltage
        {
            get => m_CurrentVoltage;
            set => m_CurrentVoltage = value;
        }
    }
}
