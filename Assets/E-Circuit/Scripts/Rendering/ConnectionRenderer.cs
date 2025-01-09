using UnityEngine;
using ECircuit.Simulation;
using System.Collections.Generic;
using TMPro;
using System;

namespace ECircuit.Rendering
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
        [SerializeField]
        private GameObject m_VoltageIndicatorPrefab;
        [SerializeField]
        private double m_minThreshold = 0.00001;
        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_simulator;

        private readonly Dictionary<Connector, LineRenderer> m_Wires = new();
        private Connection m_Connection;
        private TextMeshPro m_VoltageText;

        public void Awake()
        {
            m_Connection = GetComponent<Connection>();
            if (m_simulator == null)
            {
                m_simulator = FindFirstObjectByType<Simulator>();
            }
            // check if the prefab has a TextMeshPro component
            if (m_VoltageIndicatorPrefab != null && m_VoltageIndicatorPrefab.GetComponent<TextMeshPro>() == null)
            {
                Debug.LogWarning("Voltage indicator prefab does not have a TextMeshPro component, voltage will not be displayed");
                m_VoltageIndicatorPrefab = null;
            }
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
            if (m_Connection != null && m_Connection.ConnectedTo != null)
            {
                RenderWire();
            }
            if (m_simulator != null && m_VoltageIndicatorPrefab != null)
            {
                RenderVoltage();
            }
        }

        private void RenderWire()
        {
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

        private void RenderVoltage()
        {
            if (m_simulator == null || !m_simulator.DidSimulate)
            {
                if (m_VoltageText != null)
                {
                    Destroy(m_VoltageText.gameObject);
                    m_VoltageText = null;
                }
                return;
            }
            if (m_VoltageText == null)
            {
                m_VoltageText = Instantiate(m_VoltageIndicatorPrefab, transform.position, Quaternion.identity).GetComponent<TextMeshPro>();
                m_VoltageText.transform.SetParent(transform);
                m_VoltageText.transform.localScale = Vector3.one;
            }
            double voltage = m_Connection.CurrentVoltage < m_minThreshold ? 0 : m_Connection.CurrentVoltage;
            // format voltage to 2 decimal places
            m_VoltageText.text = $"{voltage:G3}V";
        }
    }
}
