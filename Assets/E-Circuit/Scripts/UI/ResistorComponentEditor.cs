using System;
using ECircuit.Simulation;
using UnityEngine;
using UnityEngine.UI;

namespace ECircuit.UI
{
    [RequireComponent(typeof(ComponentEditor))]
    public class ResistorComponentEditor : MonoBehaviour
    {
        [SerializeField]
        private Slider m_Slider;

        private Resistor Resistor => GetComponent<ComponentEditor>().Component as Resistor;

        public void Start()
        {
            if (m_Slider == null)
            {
                return;
            }
            m_Slider.onValueChanged.AddListener(OnResistanceChanged);
            m_Slider.value = (float)Resistor.Resistance;
        }

        public void OnResistanceChanged(float value)
        {
            Resistor.Resistance = Math.Floor(value);
        }
    }
}
