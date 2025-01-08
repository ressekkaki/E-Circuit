using UnityEngine;
using ECircuit.Simulation;
using System.Collections.Generic;

namespace ECircuit
{
    /// <summary>
    /// Renders the connections between connectors as straight lines.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(Connection))]
    public class ConnectionRenderer : MonoBehaviour
    {
        [SerializeField]
        private Material m_WireMaterial;

        private readonly Dictionary<Connector, LineRenderer> m_Wires = new();
        private Connection m_Connection;

        public void Awake()
        {
            m_Connection = GetComponent<Connection>();
        }

        public void OnDisable()
        {
            LineRenderer[] childLines = GetComponentsInChildren<LineRenderer>();

            m_Wires.Clear();
            foreach (var line in childLines)
            {
                DestroyImmediate(line.gameObject);
            }
        }

        public void Update()
        {
            if (m_Connection == null || m_Connection.ConnectedTo == null)
            {
                return;
            }
            LineRenderer[] childLines = GetComponentsInChildren<LineRenderer>();

            if (childLines.Length != m_Connection.ConnectedTo.Count)
            {
                foreach (var line in childLines)
                {
                    DestroyImmediate(line.gameObject);
                }
                m_Wires.Clear();
            }

            foreach (var c in m_Connection.ConnectedTo)
            {
                if (c == null || !m_Wires.TryGetValue(c, out LineRenderer wire))
                {
                    wire = new GameObject("Rendered Wire").AddComponent<LineRenderer>();
                    wire.transform.SetParent(transform);
                    wire.positionCount = 2;
                    wire.startWidth = 0.01f;
                    wire.endWidth = 0.01f;
                    wire.materials = new Material[] { m_WireMaterial };
                    m_Wires.Add(c, wire);
                }

                wire.SetPositions(new Vector3[] { transform.position, c.transform.position });
            }
        }
    }
}