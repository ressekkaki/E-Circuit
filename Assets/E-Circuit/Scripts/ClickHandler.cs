using UnityEngine;
using UnityEngine.Events;

namespace ECircuit
{
    public class ClickHandler : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent m_OnClick;
        [SerializeField]
        private UnityEvent m_OnPress;
        [SerializeField]
        private UnityEvent m_OnRelease;

        public UnityEvent OnClick { get => m_OnClick; }
        public UnityEvent OnPress { get => m_OnPress; }
        public UnityEvent OnRelease { get => m_OnRelease; }
    }
}
