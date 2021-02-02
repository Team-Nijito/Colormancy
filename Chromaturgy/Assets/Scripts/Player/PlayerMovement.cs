using UnityEngine;
using Photon.Pun;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviourPunCallbacks
{
    // This script handles movement input and animations

    public enum PlayerState
    {
        Idle = 0,
        Move = 1,
        Jump = 2,
        DoubleJump = 3,
        Fall = 4,
    }

    // Components
    public GameObject m_character = null;
    public PlayerState m_playerState;

    [HideInInspector] public Animator m_animator;
    private CharacterController m_controller;

    // Movement
    public float m_walkSpeed = 0f;
    public float m_runSpeed = 0f;

    [HideInInspector] public bool m_isMoving = false;
    [HideInInspector] public bool m_canMove = true;
    [HideInInspector] public bool m_canAction = true;
    [HideInInspector] public bool m_isDashing = false;

    [HideInInspector] public Vector3 m_movement = Vector3.zero;

    [SerializeField] private float m_gravity = 9.81f;

    private Vector3 m_movementNoGrav = Vector3.zero;
    private readonly float m_rotationSpeed = 40f; // from WarriorMovementControllerFREE.cs
    private float m_vSpeed = 0f; // current vertical velocity

    private void Awake()
    {
        m_animator = GetComponentInChildren<Animator>();
        m_animator.updateMode = AnimatorUpdateMode.AnimatePhysics;
        m_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
    }


    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_playerState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
            ProcessPlayerInput();
        else
        {
            return;
        }
    }

    private void LateUpdate()
    {
        MovePlayer();
    }

    // Takes in player's iputs for movement
    private void ProcessPlayerInput()
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

        // Calculate gravity + update m_vSpeed and movement.y
        m_vSpeed -= m_gravity * Time.deltaTime;
        if (m_controller.isGrounded) m_vSpeed = 0;
        m_movement.y = m_vSpeed;

        // Determine whether the player is currently dashing
        if (Input.GetKey(KeyCode.LeftShift)) m_isDashing = true;
        else m_isDashing = false;

        m_movementNoGrav = new Vector3(m_movement.x, 0f, m_movement.z); // for animation purposes
        RotateTowardsMovementDir();
    }

    // Given m_movement, update the player position (aka actually move the player)
    // also handles animations
    private void MovePlayer()
    {
        if (m_canMove)
        {
            if (m_isDashing)
                m_controller.Move(m_movement * m_runSpeed * Time.deltaTime);
            else
            {
                m_controller.Move(m_movement * m_walkSpeed * Time.deltaTime);
                m_animator.enabled = false;
                m_animator.enabled = true;
            }

            // check if player is moving by zeroing out the y value of m_movement (ignore gravity) and taking the magnitude
            if (m_movementNoGrav.magnitude > 0f)
            {
                if (m_isDashing)
                {
                    // running
                    m_isMoving = true;
                    m_animator.SetBool("Moving", true);
                    m_animator.SetFloat("Velocity Z", m_movementNoGrav.magnitude + 6);
                }
                else
                {
                    // walking
                    m_isMoving = true;
                    m_animator.SetBool("Moving", true);
                    m_animator.SetFloat("Velocity Z", m_movementNoGrav.magnitude);
                }
            }
            else if (m_isMoving)
            {
                // idle
                m_isMoving = false;
                m_animator.SetBool("Moving", false);
                m_animator.SetFloat("Velocity Z", 0);
            }

            // align the wizard's position to be the same as the parent GameObject position, and shift it down a bit
            m_character.transform.position = transform.position - new Vector3(0, 1, 0);
        }
    }

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // all functions below taken from the warriormovementcontrollerFREE script
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    public void AnimatorDebug()
    {
        Debug.Log("ANIMATOR SETTINGS---------------------------");
        Debug.Log("Moving: " + m_animator.GetBool("Moving"));
        Debug.Log("Strafing: " + m_animator.GetBool("Strafing"));
        Debug.Log("Aiming: " + m_animator.GetBool("Aiming"));
        Debug.Log("Stunned: " + m_animator.GetBool("Stunned"));
        Debug.Log("Blocking: " + m_animator.GetBool("Blocking"));
        Debug.Log("Jumping: " + m_animator.GetInteger("Jumping"));
        Debug.Log("Action: " + m_animator.GetInteger("Action"));
        Debug.Log("Velocity X: " + m_animator.GetFloat("Velocity X"));
        Debug.Log("Velocity Z: " + m_animator.GetFloat("Velocity Z"));
    }

    private void RotateTowardsMovementDir()
    {
        if (m_movementNoGrav != Vector3.zero)
        {
            m_character.transform.rotation = Quaternion.Slerp(m_character.transform.rotation, Quaternion.LookRotation(m_movementNoGrav), Time.deltaTime * m_rotationSpeed);
        }
    }
}
