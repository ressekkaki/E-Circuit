using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ECircuit.UI
{
    public class SliderField : MonoBehaviour
    {
        [SerializeField]
        private string m_ValuePattern = "{0,3:F0}Ω";

        [SerializeField]
        private TextMeshProUGUI m_ValueText;

        [SerializeField]
        private Slider m_Slider;

        public void Awake()
        {
            Assert.IsNotNull(m_ValueText);
            Assert.IsNotNull(m_Slider);
        }

        public void OnValueChanged(float value)
        {
            m_ValueText.text = string.Format(m_ValuePattern, value);
        }
    }
}
