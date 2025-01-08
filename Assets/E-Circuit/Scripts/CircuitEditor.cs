using ECircuit.Simulation;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.InputSystem;

namespace ECircuit
{
    public class CircuitEditor : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The input actions asset, leave empty to use the default one")]
        private InputActionAsset m_inputActions;
        [SerializeField]
        [Tooltip("The camera to use, leave empty to use the default one")]
        private Camera m_camera;
        [SerializeField]
        [Tooltip("The simulator to use, leave empty to use the default one")]
        private Simulator m_simulator;
        [SerializeField]
        [Tooltip("The prefab to use for connections, cannot be null")]
        private GameObject m_connectionPrefab;

        [Header("Runtime Values, DO NOT CHANGE IN EDITOR")]
        [SerializeField]
        [Tooltip("The currently selected connector")]
        private Connector m_selectedConnector;
        [Tooltip("The initial color of the selected connector")]
        private Color m_selectedConnectorInitialColor;

        private InputAction m_SelectConnectorAction;

        private void Awake()
        {
            Assert.IsNotNull(m_connectionPrefab, "Connection prefab cannot be null");
            if (m_camera == null)
            {
                m_camera = Camera.main;
            }
            m_SelectConnectorAction ??= m_inputActions.FindActionMap("Player").FindAction("Select Connector", throwIfNotFound: true);
            m_SelectConnectorAction.performed += OnSelectConnectorAction;
            if (m_simulator == null)
            {
                m_simulator = FindFirstObjectByType<Simulator>();
            }
        }

        private void OnDestroy()
        {
            m_SelectConnectorAction.performed -= OnSelectConnectorAction;
        }

        /// <summary>
        /// Executed on "Select Connector" action (left mouse click by default).
        /// Attempts to select a connector by casting a ray from the mouse position.
        /// </summary>
        private void OnSelectConnectorAction(InputAction.CallbackContext context)
        {
            if (!context.performed)
            {
                return;
            }
            Vector2 mousePosition = Mouse.current.position.ReadValue();
            Ray ray = m_camera.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit) && hit.collider.gameObject.TryGetComponent<Connector>(out var connector))
            {
                OnConnectorSelect(connector);
                return;
            }
            OnConnectorSelect(null);
        }

        private void OnConnectorSelect(Connector connector)
        {
            if (connector == m_selectedConnector)
            {
                // treat select again as unselect
                connector = null;
            }
            var previousConnector = m_selectedConnector;
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

        private void UnselectConnector()
        {
            if (m_selectedConnector != null)
            {
                m_selectedConnector.GetComponent<Renderer>().material.color = m_selectedConnectorInitialColor;
                m_selectedConnector = null;
            }
        }

        private void SelectConnector(Connector connector)
        {
            m_selectedConnector = connector;
            if (connector != null)
            {
                m_selectedConnectorInitialColor = connector.GetComponent<Renderer>().material.color;
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
            m_simulator.SetSimulationNeeded();
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
                GameObject obj = Instantiate(m_connectionPrefab, pos, Quaternion.identity);
                obj.name = Connection.RandomName();
                obj.transform.parent = from.transform.parent.parent;
                connFrom = obj.GetComponent<Connection>();
            }

            from.Connection = connFrom;
            to.Connection = connFrom;

            // Special case: the ground connection should always be called "0"
            if (connFrom.ConnectedTo.Contains(m_simulator.MainGenerator.Negative) && connFrom.name != "0")
            {
                connFrom.name = "0";
            }

            Connection.DestroyIfInvalid(connTo);
            m_simulator.SetSimulationNeeded();
        }
    }
}
