using System;
using ECircuit.Simulation;
using UnityEngine;
using UnityEngine.Assertions;

namespace ECircuit.UI
{
    public class UIEventsController : MonoBehaviour
    {
        [SerializeField]
        private CircuitEditor m_CircuitEditor;
        [SerializeField]
        private Simulator m_Simulator;
        [SerializeField]
        private EditModePanel m_EditModePanel;
        [SerializeField]
        private GameObject m_CircuitErrorPanel;

        public CircuitMode CircuitMode
        {
            get => m_CircuitEditor.CircuitMode;
            set
            {
                if (value != m_CircuitEditor.CircuitMode)
                {
                    switch (value)
                    {
                        case CircuitMode.Edit:
                            SwitchToEditMode();
                            break;
                        case CircuitMode.Simulation:
                            SwitchToSimulationMode();
                            break;
                    }
                }
            }
        }

        public void Awake()
        {
            Assert.IsNotNull(m_CircuitEditor);
            Assert.IsNotNull(m_Simulator);
            SwitchToEditMode();
        }

        private void SwitchToEditMode()
        {
            m_Simulator.NeedSimulation = false;
            m_Simulator.enabled = false;
            m_CircuitEditor.CircuitMode = CircuitMode.Edit;
            if (m_EditModePanel != null)
            {
                m_EditModePanel.gameObject.SetActive(true);
                m_EditModePanel.Open();
            }
            if (m_CircuitErrorPanel)
            {
                m_CircuitErrorPanel.SetActive(false);
            }
        }

        private void SwitchToSimulationMode()
        {
            m_CircuitEditor.CircuitMode = CircuitMode.Simulation;
            if (m_EditModePanel != null)
            {
                m_EditModePanel.Close();
            }
            if (m_CircuitErrorPanel)
            {
                m_CircuitErrorPanel.SetActive(m_Simulator.HasErrors);
            }
            m_Simulator.NeedSimulation = true;
            m_Simulator.enabled = true;
        }

        public void Update()
        {
            var shouldDisplayError = m_Simulator.HasErrors && CircuitMode == CircuitMode.Simulation;
            if (m_CircuitErrorPanel && m_CircuitErrorPanel.activeSelf != shouldDisplayError)
            {
                m_CircuitErrorPanel.SetActive(shouldDisplayError);
            }
        }

        #region UI Event Callbacks

        public void OnEditCircuitModeChanged(bool isOn)
        {
            if (isOn)
            {
                CircuitMode = CircuitMode.Edit;
            }
        }

        public void OnSimulationModeChanged(bool isOn)
        {
            if (isOn)
            {
                CircuitMode = CircuitMode.Simulation;
            }
        }

        public void OnSelectionEditActionChanged(bool isOn)
        {
            if (isOn)
            {
                m_CircuitEditor.EditAction = EditAction.Selection;
            }
        }

        public void OnSpawnDiodeEditActionChanged(bool isOn)
        {
            if (isOn)
            {
                m_CircuitEditor.EditAction = EditAction.SpawnDiode;
            }
        }

        public void OnSpawnGeneratorEditActionChanged(bool isOn)
        {
            if (isOn)
            {
                m_CircuitEditor.EditAction = EditAction.SpawnGenerator;
            }
        }

        public void OnSpawnLedEditActionChanged(bool isOn)
        {
            if (isOn)
            {
                m_CircuitEditor.EditAction = EditAction.SpawnLed;
            }
        }

        public void OnSpawnPushButtonEditActionChanged(bool isOn)
        {
            if (isOn)
            {
                m_CircuitEditor.EditAction = EditAction.SpawnPushButton;
            }
        }

        public void OnSpawnResistorEditActionChanged(bool isOn)
        {
            if (isOn)
            {
                m_CircuitEditor.EditAction = EditAction.SpawnResistor;
            }
        }

        #endregion UI Event Callbacks
    }
}
