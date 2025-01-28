using System.Collections;
using LitMotion;
using LitMotion.Extensions;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECircuit.UI
{
    public class EditModePanel : MonoBehaviour
    {
        private RectTransform m_RectTransform;
        
        private bool m_Closing = false;
        
        public void Awake()
        {
            m_RectTransform = GetComponent<RectTransform>();
            Assert.IsNotNull(m_RectTransform);
        }
        
        public void Start()
        {
            Open();
        }

        public void Open()
        {
            m_Closing = false;
            transform.position += Vector3.left * 10000;
            StartCoroutine(OpenAsync());
        }
        
        private IEnumerator OpenAsync()
        {
            // Force update canvas to get the correct width
            Canvas.ForceUpdateCanvases();
            yield return null; // Wait for the next frame
            LMotion.Create(-m_RectTransform.rect.width, 0, 0.3f)
                .WithEase(Ease.InOutCubic)
                .BindToPositionX(transform)
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
            await LMotion.Create(0, -m_RectTransform.rect.width, 0.3f)
                .WithEase(Ease.InOutCubic)
                .BindToPositionX(transform)
                .AddTo(gameObject)
                .ToAwaitable();
            gameObject.SetActive(false);
        }
    }
}