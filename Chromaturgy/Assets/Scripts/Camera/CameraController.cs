using UnityEngine;

namespace Chromaturgy
{
    public class CameraController : MonoBehaviour
    {
        // This script should be a player component, and a camera should be a child of the player
        [SerializeField]
        private Vector3 m_initialCameraOffset = Vector3.zero;
        [SerializeField]
        private Vector3 m_zoomAmount = Vector3.zero;
        [SerializeField]
        private float m_minZoom = 0f;
        [SerializeField]
        private float m_maxZoom = 150f;
        [SerializeField]
        private float m_rotationAmount = 1f;
        [SerializeField]
        private float m_camSpeed = 10f;

        private GameObject m_TCamera = null; // tracking camera
        private Transform m_TCameraTransform = null; // its transform
        private Vector3 m_newZoom = Vector3.zero;
        private Quaternion m_newRotation = Quaternion.identity;

        private void Start()
        {
            m_TCamera = Camera.main.transform.gameObject;
            m_TCameraTransform = m_TCamera.transform;
            m_newZoom = m_TCameraTransform.localPosition;
            m_newRotation = transform.rotation;

            if (m_TCamera) InitialCameraTrackPlayer();
        }

        private void Update()
        {
            if (m_TCamera)
            {
                HandleCameraZoom();
                HandleCameraRotation();
            }
        }

        private void InitialCameraTrackPlayer()
        {
            m_TCameraTransform.position = transform.position + m_initialCameraOffset + m_newZoom;
            m_TCameraTransform.LookAt(transform);
        }

        private void HandleCameraZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                // zoom out
                if ((m_newZoom - m_zoomAmount).y <= m_maxZoom)
                    m_newZoom -= m_zoomAmount;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                // zoom in
                if ((m_newZoom + m_zoomAmount).y >= m_minZoom)
                    m_newZoom += m_zoomAmount;
            }
            m_TCameraTransform.localPosition = m_newZoom;
        }

        private void HandleCameraRotation()
        {
            if (Input.GetKey(KeyCode.Q))
            {
                m_newRotation *= Quaternion.Euler(Vector3.up * m_rotationAmount);
            }
            else if (Input.GetKey(KeyCode.E))
            {
                m_newRotation *= Quaternion.Euler(Vector3.up * -m_rotationAmount);
            }
            transform.rotation = Quaternion.Lerp(transform.rotation, m_newRotation, Time.deltaTime * m_camSpeed);
        }
    }
}
