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
                m_Text.text = $"{m_Component.CurrentCurrent * 1000.0:G3} mA";
                m_Text.enabled = true;
            }
            else
            {
                m_Text.enabled = false;
            }
        }
    }
}
