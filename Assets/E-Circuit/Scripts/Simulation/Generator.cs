using UnityEngine;

namespace ECircuit.Simulation
{
    public class Generator : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Positive connector of the generator")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the generator")]
        private Connector m_Negative;
    }
}
