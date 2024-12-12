using UnityEngine;

namespace ECircuit.Simulation
{

    public class Connector : MonoBehaviour
    {
        [SerializeField]
        [Header("DO NOT CHANGE IN EDITOR")]
        [Tooltip("The connection this connector is a part of.")]
        private Connection m_Connection = null;

        public Connection Connection
        {
            set
            {
                if (m_Connection != null && m_Connection != value)
                {
                    var prevConnection = m_Connection;
                    m_Connection = null;
                    prevConnection.ConnectedTo.Remove(this);
                }
                m_Connection = value;
            }
            get => m_Connection;
        }
    }
}
