using ECircuit.Simulation.Components;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace ECircuit.UI
{
    public class ComponentEditor : MonoBehaviour
    {
        [SerializeField]
        private BaseComponent m_Component;

        [SerializeField]
        private TextMeshProUGUI m_Title;
        [SerializeField]
        private UnityEvent m_OnClose;

        public BaseComponent Component { get => m_Component; set => m_Component = value; }

        public UnityEvent OnClose { get => m_OnClose; }

        public string Title
        {
            get => m_Title.text;
            set => m_Title.text = value;
        }

        public void Awake()
        {
            Assert.IsNotNull(m_Title);
        }

        public void Close()
        {
            gameObject.SetActive(false);
            m_OnClose.Invoke();
        }
    }
}
