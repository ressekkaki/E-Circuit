using ECircuit.Simulation;
using UnityEngine;

namespace ECircuit.Simulation
{
    public class Resistor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Positive connector of the resistor")]
        private Connector m_Positive;

        [SerializeField]
        [Tooltip("Negative connector of the resistor")]
        private Connector m_Negative;
    }
}
