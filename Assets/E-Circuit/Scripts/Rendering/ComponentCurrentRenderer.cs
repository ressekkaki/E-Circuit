using System;
using ECircuit.Simulation;
using ECircuit.Simulation.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECircuit.Rendering
{
    public class ComponentCurrentRenderer : MonoBehaviour
    {
        [SerializeField]
        private BaseComponent m_Component;
        [SerializeField]
        private TextMeshPro m_Text;
        [SerializeField]
        private double m_minThreshold = 0.0001;
        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_simulator;


        public void Awake()
        {
            if (m_simulator == null)
            {
                m_simulator = FindFirstObjectByType<Simulator>();
            }
            Assert.IsNotNull(m_Text);
        }

        // Update is called once per frame
        public void Update()
        {
            if (m_simulator == null)
            {
                return;
            }

            if (m_simulator.DidSimulate)
            {
                double current = m_Component.CurrentCurrent < m_minThreshold ? 0 : m_Component.CurrentCurrent;
                m_Text.text = $"{current * 1000.0:G3} mA";
                m_Text.color = m_Component.IsOverloaded() ? Color.red : Color.white;
                m_Text.enabled = true;
            }
            else
            {
                m_Text.enabled = false;
            }
        }
    }
}
