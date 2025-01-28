using System.Collections;
using ECircuit.Simulation.Components;
using LitMotion;
using LitMotion.Extensions;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Events;

namespace ECircuit.UI
{
    public class ComponentEditor : MonoBehaviour
    {
        [SerializeField] private BaseComponent m_Component;

        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private UnityEvent m_OnClose;

        private RectTransform m_RectTransform;

        private bool m_Closing = false;

        public BaseComponent Component
        {
            get => m_Component;
            set => m_Component = value;
        }

        public UnityEvent OnClose => m_OnClose;

        public string Title
        {
            get => m_Title.text;
            set => m_Title.text = value;
        }

        public void Awake()
        {
            Assert.IsNotNull(m_Title);
            m_RectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(m_RectTransform);
        }

        public void Start()
        {
            m_Closing = false;
            transform.position += Vector3.down * 10000;
            StartCoroutine(OpenAsync());
        }

        private IEnumerator OpenAsync()
        {
            // Force update canvas to get the correct height
            Canvas.ForceUpdateCanvases();
            yield return null; // Wait for the next frame
            LMotion.Create(-m_RectTransform.rect.height, 0, 0.3f)
                .WithEase(Ease.InOutCubic)
                .BindToPositionY(transform)
                .AddTo(gameObject);
        }

        public void Close()
        {
            CloseAsync().GetAwaiter();
        }

        public async Awaitable CloseAsync()
        {
            if (m_Closing)
            {
                return;
            }
            m_Closing = true;
            await LMotion.Create(0, -m_RectTransform.rect.height, 0.3f)
                .WithEase(Ease.InOutCubic)
                .BindToPositionY(transform)
                .AddTo(gameObject)
                .ToAwaitable();
            m_OnClose.Invoke();
            gameObject.SetActive(false);
        }
    }
}