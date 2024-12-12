using UnityEngine;

namespace ECircuit.Simulation
{
    public class Simulator : MonoBehaviour
    {
        [SerializeField]
        private Generator m_MainGenerator;

        private bool m_DidSimulate = false;


        public void Awake()
        {
            m_DidSimulate = false;
        }

        public void OnDisable()
        {
            m_DidSimulate = false;
        }

        public void OnEnable()
        {
            m_DidSimulate = false;
        }

        public void Update()
        {
            if (m_DidSimulate)
            {
                return;
            }
            m_DidSimulate = true;
            BuildCircuit();
            Simulate();
        }

        private void BuildCircuit()
        {
            // collect all components
        }

        private void Simulate()
        {
            // do stuff...
        }
    }

}
