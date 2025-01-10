using System;
using System.Collections;
using System.Collections.Generic;
using ECircuit.Simulation;
using ECircuit.Simulation.Components;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace ECircuit
{
    public class CircuitEditor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The camera to use, leave empty to use the default one")]
        private Camera m_Camera;
        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_Simulator;
        [SerializeField]
        [Tooltip("The prefab to use for connections, cannot be null")]
        private GameObject m_ConnectionPrefab;

        [Header("Controls")]
        [SerializeField]
        private InputActionReference m_InteractAction;
        [SerializeField]
        private InputActionReference m_SpawnDiodeAction;
        [SerializeField]
        private InputActionReference m_SpawnGeneratorAction;
        [SerializeField]
        private InputActionReference m_SpawnLedAction;
        [SerializeField]
        private InputActionReference m_SpawnPushButtonAction;
        [SerializeField]
        private InputActionReference m_SpawnResistorAction;
        [SerializeField]
        private float m_ComponentDeleteDelaySeconds = 0.4f;

        [Header("Component Prefabs")]
        [SerializeField]
        [Tooltip("The prefab to use for diodes")]
        private GameObject m_DiodePrefab;
        [SerializeField]
        [Tooltip("The prefab to use for generators")]
        private GameObject m_GeneratorPrefab;
        [SerializeField]
        [Tooltip("The prefab to use for LEDs")]
        private GameObject m_LedPrefab;
        [SerializeField]
        [Tooltip("The prefab to use for push buttons")]
        private GameObject m_PushButtonPrefab;
        [SerializeField]
        [Tooltip("The prefab to use for resistors")]
        private GameObject m_ResistorPrefab;


        [Header("Runtime Values, DO NOT CHANGE IN EDITOR")]
        [SerializeField]
        [Tooltip("The currently selected connector")]
        private Connector m_SelectedConnector;
        [Tooltip("The initial color of the selected connector")]
        private Color m_SelectedConnectorInitialColor;

        private Coroutine m_HoldInteractCoroutine;

        private readonly List<Action> m_CleanupActions = new();

        private void Awake()
        {
            Assert.IsNotNull(m_ConnectionPrefab, "Connection prefab cannot be null");
            if (m_Camera == null)
            {
                m_Camera = Camera.main;
            }
            InputAction interactAction = m_InteractAction.action;
            interactAction.started += OnInteractActionStarted;
            interactAction.canceled += OnInteractActionCanceled;
            interactAction.performed += OnInteractActionPerformed;
            m_CleanupActions.Add(() =>
            {
                interactAction.started -= OnInteractActionStarted;
                interactAction.canceled -= OnInteractActionCanceled;
                interactAction.performed -= OnInteractActionPerformed;
            });
            AddSpawnComponentAction<Diode>(m_SpawnDiodeAction, m_DiodePrefab);
            AddSpawnComponentAction<Generator>(m_SpawnGeneratorAction, m_GeneratorPrefab);
            AddSpawnComponentAction<Led>(m_SpawnLedAction, m_LedPrefab);
            AddSpawnComponentAction<PushButton>(m_SpawnPushButtonAction, m_PushButtonPrefab);
            AddSpawnComponentAction<Resistor>(m_SpawnResistorAction, m_ResistorPrefab);
            if (m_Simulator == null)
            {
                m_Simulator = FindFirstObjectByType<Simulator>();
            }
        }

        private void OnDestroy()
        {
            foreach (var cleanup in m_CleanupActions)
            {
                cleanup();
            }
            m_CleanupActions.Clear();
        }

        private void OnInteractActionStarted(InputAction.CallbackContext context)
        {
            m_HoldInteractCoroutine ??= StartCoroutine(OnInteractActionHeld());
        }


        private void OnInteractActionCanceled(InputAction.CallbackContext context)
        {
            if (m_HoldInteractCoroutine != null)
            {
                StopCoroutine(m_HoldInteractCoroutine);
                m_HoldInteractCoroutine = null;
            }
        }


        /// <summary>
        /// Executed on "Interact" action (left mouse click by default).
        /// Attempts to select a connector by casting a ray from the mouse position.
        /// </summary>
        private void OnInteractActionPerformed(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }
            if (RaycastComponent(out Connector connector))
            {
                OnConnectorSelect(connector);
                return;
            }
            OnConnectorSelect(null);
        }

        /// <summary>
        /// Executed while the "Interact" action is held (left mouse click by default).
        /// If the mouse is still on the same component after a delay, deletes the component.
        /// </summary>
        private IEnumerator OnInteractActionHeld()
        {
            if (!RaycastComponent(out BaseComponent componentStart))
            {
                yield break;
            }
            yield return new WaitForSeconds(m_ComponentDeleteDelaySeconds);
            // Check that the component is still the same after the delay
            if (RaycastComponent(out BaseComponent componentEnd) && componentStart == componentEnd)
            {
                // Hold "interact" to delete the component
                OnComponentDelete(componentStart);
            }
        }

        private void OnSpawnComponentActionPerformed<T>(GameObject prefab) where T : BaseComponent
        {
            if (!RaycastComponent(out CircuitSurfaceMarker marker, out RaycastHit hit))
            {
                return;
            }

            GameObject obj = Instantiate(prefab, hit.point, Quaternion.identity);
            obj.transform.parent = marker.transform;
            BaseComponent component = obj.GetComponent<T>();
            component.name = component.RandomName();
            component.ComponentName = component.name;
            m_Simulator.NeedSimulation = true;
        }

        private void AddSpawnComponentAction<T>(InputActionReference actionRef, GameObject prefab) where T : BaseComponent
        {
            InputAction action = actionRef.action;
            if (action != null)
            {
                void onPerform(InputAction.CallbackContext context)
                {
                    OnSpawnComponentActionPerformed<T>(prefab);
                }
                action.performed += onPerform;
                m_CleanupActions.Add(() => action.performed -= onPerform);
            }
        }

        private bool RaycastComponent<T>(out T component)
        {
            return RaycastComponent(out component, out _);
        }

        private bool RaycastComponent<T>(out T component, out RaycastHit hit)
        {
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = m_Camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.TryGetComponent(out component))
            {
                return true;
            }
            component = default;
            hit = default;
            return false;
        }

        private void OnConnectorSelect(Connector connector)
        {
            if (connector == m_SelectedConnector)
            {
                // treat select again as unselect
                connector = null;
            }
            var previousConnector = m_SelectedConnector;
            UnselectConnector();
            if (connector == null || previousConnector == null)
            {
                SelectConnector(connector);
            }
            else if (connector.Connection == null || connector.Connection != previousConnector.Connection || !connector.Connection.ConnectedTo.Contains(previousConnector))
            {
                Connect(previousConnector, connector);
            }
            else
            {
                Disconnect(previousConnector, connector);
            }
        }

        private void OnComponentDelete(BaseComponent component)
        {
            Debug.Log($"Deleting component {component}");
            Destroy(component.gameObject);
            m_Simulator.NeedSimulation = true;
        }

        private void UnselectConnector()
        {
            if (m_SelectedConnector != null)
            {
                m_SelectedConnector.GetComponent<Renderer>().material.color = m_SelectedConnectorInitialColor;
                m_SelectedConnector = null;
            }
        }

        private void SelectConnector(Connector connector)
        {
            m_SelectedConnector = connector;
            if (connector != null)
            {
                m_SelectedConnectorInitialColor = connector.GetComponent<Renderer>().material.color;
                connector.GetComponent<Renderer>().material.color = Color.red;
            }
        }

        private void Disconnect(Connector from, Connector to)
        {
            if (from == null || to == null || from == to)
            {
                return; // sanity check
            }
            var connFrom = from.Connection;
            var connTo = to.Connection;

            Debug.Log($"Disconnecting {from.Owner} from {to.Owner}");
            to.Connection = null;
            Connection.DestroyIfInvalid(connFrom);
            Connection.DestroyIfInvalid(connTo);
            m_Simulator.NeedSimulation = true;
        }

        private void Connect(Connector from, Connector to)
        {
            if (from == null || to == null || from == to)
            {
                return; // sanity check
            }
            var connFrom = from.Connection;
            var connTo = to.Connection;

            Debug.Log($"Connecting {from.Owner} to {to.Owner}");
            if (connFrom == null)
            {
                // Instantiate a middle pos between the two connectors
                var pos = (from.transform.position + to.transform.position) / 2;
                GameObject obj = Instantiate(m_ConnectionPrefab, pos, Quaternion.identity);
                obj.name = Connection.RandomName();
                obj.transform.parent = from.transform.parent.parent;
                connFrom = obj.GetComponent<Connection>();
            }

            from.Connection = connFrom;
            to.Connection = connFrom;

            // Special case: the ground connection should always be called "0"
            if (IsGroundConnection(connFrom) && connFrom.name != "0")
            {
                connFrom.name = "0";
            }

            Connection.DestroyIfInvalid(connTo);
            m_Simulator.NeedSimulation = true;
        }

        private bool IsGroundConnection(Connection connection)
        {
            Generator generator = m_Simulator.MainGenerator;
            if (connection == null || generator == null || generator.Negative == null)
            {
                return false;
            }
            return connection.ConnectedTo.Contains(generator.Negative);
        }
    }
}
