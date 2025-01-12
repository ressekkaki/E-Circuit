using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace ECircuit.UI
{
    [RequireComponent(typeof(Toggle))]
    [RequireComponent(typeof(Image))]
    public class ToggleImageColor : MonoBehaviour
    {
        [SerializeField]
        private Color m_OnColor = new(0.278f, 0.098f, 0.870f);
        [SerializeField]
        private Color m_OffColor = Color.black;

        private Toggle m_Toggle;
        private Image m_Image;

        public void Awake()
        {
            m_Toggle = GetComponent<Toggle>();
            m_Image = GetComponent<Image>();
            Assert.IsNotNull(m_Toggle);
            Assert.IsNotNull(m_Image);
        }
        public void OnEnable()
        {
            OnValueChanged(m_Toggle.isOn);
            m_Toggle.onValueChanged.AddListener(OnValueChanged);
        }

        public void OnDisable()
        {
            m_Toggle.onValueChanged.RemoveListener(OnValueChanged);
        }

        private void OnValueChanged(bool isOn)
        {
            m_Image.color = isOn ? m_OnColor : m_OffColor;
        }
    }
}
