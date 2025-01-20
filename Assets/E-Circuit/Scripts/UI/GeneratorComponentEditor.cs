using System;
using ECircuit.Simulation.Components;
using UnityEngine;
using UnityEngine.UI;

namespace ECircuit.UI
{
    [RequireComponent(typeof(ComponentEditor))]
    public class GeneratorComponentEditor : MonoBehaviour
    {
        [SerializeField]
        private Slider m_Slider;

        private Generator Generator => GetComponent<ComponentEditor>().Component as Generator;

        public void Start()
        {
            if (m_Slider == null)
            {
                return;
            }
            m_Slider.onValueChanged.AddListener(OnVoltageChanged);
            m_Slider.value = (float)Generator.Voltage;
        }

        public void OnVoltageChanged(float value)
        {
            Generator.Voltage = Math.Floor(value);
        }
    }
}
