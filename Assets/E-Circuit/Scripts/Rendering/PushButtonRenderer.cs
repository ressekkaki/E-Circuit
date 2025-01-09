using ECircuit.Simulation;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECircuit.Rendering
{
    [RequireComponent(typeof(PushButton))]
    public class PushButtonRenderer : MonoBehaviour
    {
        [SerializeField]
        private Transform m_ButtonObject;

        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_simulator;

        private PushButton m_Button;

        public void Awake()
        {
            m_Button = GetComponent<PushButton>();
            if (m_simulator == null)
            {
                m_simulator = FindFirstObjectByType<Simulator>();
            }
            Assert.IsNotNull(m_ButtonObject);
        }

        public void Update()
        {
            if (m_Button.IsPressed)
            {
                m_ButtonObject.localPosition = new Vector3(0, -0.05f, 0);
            }
            else
            {
                m_ButtonObject.localPosition = new Vector3(0, 0.1f, 0);
            }
        }
    }
}
