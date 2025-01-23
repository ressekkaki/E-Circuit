using System;
using ECircuit.Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace ECircuit.UI
{
    [RequireComponent(typeof(ComponentEditor))]
    public class LedComponentEditor : MonoBehaviour
    {
        [SerializeField]
        private Slider m_Slider;

        private Led Led => GetComponent<ComponentEditor>().Component as Led;

        public void Start()
        {
            if (m_Slider == null)
            {
                return;
            }
            m_Slider.onValueChanged.AddListener(OnColorChanged);
            Color.RGBToHSV(Led.Color, out float hue, out float lol, out float lol2);
            m_Slider.value = hue;
        }

        public void OnColorChanged(float value)
        {
            Color.RGBToHSV(Led.Color, out _, out float saturation, out float brightness);
            Led.Color = Color.HSVToRGB(value, saturation, brightness);
        }
    }
}
