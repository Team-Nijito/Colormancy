using UnityEngine;
using Photon.Pun;

namespace Chromaturgy
{
    // namespace is needed otherwise Unity will say that there's already a definition for CameraController
    public class CameraController : MonoBehaviourPunCallbacks
    {
        // This script should be a player component

        #region Variables

        public enum CameraZoom
        {
            Stationary,
            In,
            Out
        }

        public enum CameraSpin
        {
            Stationary,
            Left,
            Right
        }

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
        [SerializeField]
        private float m_cameraShake = 0f;

        private GameObject m_TCamera = null; // tracking camera
        private Transform m_TCameraTransform = null; // its transform
        private Vector3 m_TCameraSaved; // saved reference to position
        private Vector3 m_newZoom = Vector3.zero;
        private Quaternion m_newRotation = Quaternion.identity;

        private bool m_isFollowing;
        private CameraZoom m_currentZoom = CameraZoom.Stationary;
        private CameraSpin m_currentSpin = CameraSpin.Stationary;

        [SerializeField]
        private string m_sceneToDeactivateCharacter = "YouWinScene";

        [Tooltip("This should only be true if this is a spectating camera, which is a camera that is not attached to any player.")]
        [SerializeField]
        private bool m_isSpectateCamera = false;

        private GameObject m_cameraPrefab; // Instantiate a camera prefab once a player returns back from a level

        private Vector3 m_movement = Vector3.zero;

        private PlayerMovement m_playerMovementScript;

        #endregion

        #region MonoBehaviour callbacks

        private void Start()
        {
            if (photonView.IsMine)
            {
                m_playerMovementScript = GetComponent<PlayerMovement>();
                StartFollowing();
            }
            m_cameraPrefab = Resources.Load<GameObject>("Main Camera"); // load the camera resource before we might need it
        }

        private void Update()
        {
            if (SceneManagerHelper.ActiveSceneName == m_sceneToDeactivateCharacter)
            {
                gameObject.SetActive(false);
            }
            else if (photonView.IsMine)
            {
                if (!Camera.main)
                {
                    Instantiate(m_cameraPrefab);
                    StartFollowing();
                }

                if (m_TCamera && m_isFollowing && (m_isSpectateCamera || (!m_isSpectateCamera && m_playerMovementScript.CanMove) ))
                {
                    HandleCameraZoomInputs();
                    HandleCameraRotationInputs();

                    if (m_isSpectateCamera)
                    {
                        HandleCameraMoveInputs();
                    }
                }
            }
        }

        void FixedUpdate()
        {
            if (photonView.IsMine)
            {
                if (m_TCamera && m_isFollowing)
                {
                    // Load saved camera position at start of each update after the first shake occurs.
                    if (m_cameraShake != 0f)
                    {
                        m_TCameraTransform.localPosition = m_TCameraSaved;
                    }
                    HandleCameraZoom();
                    HandleCameraRotation();
                    HandleCameraShake();
                }

                if (m_isSpectateCamera)
                {
                    HandleCameraMove();
                }
            }
        }

        #endregion

        #region Private functions

        private void InitialCameraTrackPlayer()
        {
            m_TCameraTransform.position = transform.position + m_initialCameraOffset;
            m_TCameraTransform.LookAt(transform);
        }

        private void HandleCameraMoveInputs()
        {
            // Normalize movement vector, so that we don't move faster when
            // we move diagonally
            m_movement = new Vector3
            {
                x = Input.GetAxisRaw("Horizontal"),
                y = 0,
                z = Input.GetAxisRaw("Vertical")
            }.normalized;

            //CONVERT direction from local to world relative to camera
            m_movement = Camera.main.transform.TransformDirection(m_movement);

            m_movement.y = 0; // Don't move up and down.
        }

        private void HandleCameraMove()
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + m_movement * m_camSpeed, Time.deltaTime * m_camSpeed);
        }

        private void HandleCameraZoomInputs()
        {
            m_currentZoom = CameraZoom.Stationary;
            if (Input.GetAxis("Mouse ScrollWheel") < 0)
            {
                m_currentZoom = CameraZoom.Out;
            }
            else if (Input.GetAxis("Mouse ScrollWheel") > 0)
            {
                m_currentZoom = CameraZoom.In;
            }
        }

        private void HandleCameraZoom()
        {
            if (m_currentZoom != CameraZoom.Stationary)
            {
                if (m_currentZoom == CameraZoom.In)
                {
                    // zoom in
                    if ((m_newZoom + m_zoomAmount).y >= m_minZoom)
                        m_newZoom += m_zoomAmount;
                }
                else
                {
                    // zoom out
                    if ((m_newZoom - m_zoomAmount).y <= m_maxZoom)
                        m_newZoom -= m_zoomAmount;
                }
                m_TCameraTransform.localPosition = m_newZoom;
            }
        }

        private void HandleCameraRotationInputs()
        {
            m_currentSpin = CameraSpin.Stationary;
            if (Input.GetKey(KeyCode.Q))
            {
                m_currentSpin = CameraSpin.Left;
            }
            else if (Input.GetKey(KeyCode.E))
            {
                m_currentSpin = CameraSpin.Right;
            }
        }

        private void HandleCameraRotation()
        {
            if (m_currentSpin != CameraSpin.Stationary)
            {
                if (m_currentSpin == CameraSpin.Right)
                {
                    m_newRotation *= Quaternion.Euler(Vector3.up * m_rotationAmount);
                }
                else
                {
                    m_newRotation *= Quaternion.Euler(Vector3.up * -m_rotationAmount);
                }
                transform.rotation = Quaternion.Lerp(transform.rotation, m_newRotation, Time.deltaTime * m_camSpeed);
            }
        }

        private void HandleCameraShake()
        {
            // Save a reference of the current camera position before the shaking occurs.
            m_TCameraSaved = m_TCameraTransform.localPosition;

            // Camera stops shaking after a certain low value is reached.
            if (m_cameraShake > 0.05f)
            {
                m_TCameraTransform.localPosition = m_TCameraTransform.localPosition + Random.insideUnitSphere * m_cameraShake;
                m_cameraShake *= 0.9f;
            }
            
        }
        #endregion

        #region Public functions

        // Used to set the rotation so that the rotation doesn't "snap" unexpectedly
        // Example case: when respawning, the player will be rotated (to face the spawn's forward direction)
        // but when you turn the camera afterwards, it may snap quickly
        public void ResetRotation(Quaternion newRotation)
        {
            m_newRotation = newRotation;
        }

        public void StartFollowing()
        {
            // set variables
            m_TCamera = Camera.main.transform.gameObject;
            m_TCameraTransform = m_TCamera.transform;
            m_TCameraTransform.parent = transform;
            m_newRotation = transform.rotation;

            m_TCamera.name = "PlayerCamera"; // change name so that we won't destroy this camera on scene load

            // add component to enable camera transparency behavior
            CameraTransparency ct = m_TCamera.gameObject.AddComponent<CameraTransparency>() as CameraTransparency;

            InitialCameraTrackPlayer();
            m_newZoom = m_TCameraTransform.localPosition;

            m_isFollowing = true;
        }

        // Sets the magnitude at which the camera will shake. Value decays over time.
        // Should only be called with values between 0 and 1.
        public void SetCameraShake(float shakeAmount)
        {
            m_cameraShake = shakeAmount;
        }

        // Is this CameraController object a camera rig without a "player object"?
        public void SetIsSpectateCamera(bool isSpectateCamera)
        {
            photonView.Owner.TagObject = gameObject; // we're the player object now
            m_isSpectateCamera = isSpectateCamera;
        }

        #endregion
    }
}
