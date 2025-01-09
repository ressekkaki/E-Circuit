using UnityEngine;

namespace ECircuit
{
    public class Billboard : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The camera to use, leave empty to use the default one")]
        private Camera m_camera;

        private void Awake()
        {
            if (m_camera == null)
            {
                m_camera = Camera.main;
            }
        }

        public void Start()
        {
            transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
        }

        public void Update()
        {
            transform.LookAt(transform.position + m_camera.transform.rotation * Vector3.forward, m_camera.transform.rotation * Vector3.up);
        }
    }
}
