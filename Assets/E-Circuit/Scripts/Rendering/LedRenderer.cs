using System;
using ECircuit.Simulation;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECircuit.Rendering
{
    [RequireComponent(typeof(Led))]
    public class LedRenderer : MonoBehaviour
    {
        [SerializeField]
        private Renderer m_LightBar;

        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_simulator;

        private Led m_Led;

        public void Awake()
        {
            m_Led = GetComponent<Led>();
            if (m_simulator == null)
            {
                m_simulator = FindFirstObjectByType<Simulator>();
            }
            Assert.IsNotNull(m_LightBar);
        }

        public void Update()
        {
            if (m_simulator == null)
            {
                return;
            }

            double current = m_Led.CurrentCurrent;
            if (current < m_Led.MinCurrent || current > m_Led.MaxCurrent)
            {
                m_LightBar.material.color = Color.black;
            }
            else
            {
                double intensity = Math.Clamp(current / m_Led.MaxCurrent, 0.0, 1.0);
                Color.RGBToHSV(m_Led.Color, out float h, out float s, out _);
                m_LightBar.material.color = Color.HSVToRGB(h, s, (float)intensity);
            }
        }
    }
}
