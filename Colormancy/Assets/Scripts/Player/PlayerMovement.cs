using UnityEngine;
using Photon.Pun;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable
{
    // This script handles movement input and animations
    #region Accessors (c# Properties)

    public bool CanMove { get { return m_canMove; } private set { m_canMove = value; } }
    public bool IsMoving { get { return m_isMoving; } private set { m_isMoving = value; } }
    public bool IsDashing { get { return m_isDashing; } private set { m_isDashing = value; } }

    public Vector3 CurrentMovement { get { return m_movement; } private set { m_movement = value; } }

    public float WalkSpeed { get { return m_walkSpeed; } private set { m_walkSpeed = value; } }
    public float RunSpeed { get { return m_runSpeed; } private set { m_runSpeed = value; } }

    public Animator PlayerAnimator { get { return m_animator; } private set { m_animator = value; } }

    public GameObject BlindPanel { get { return m_blindPanel; } private set { m_blindPanel = value; } }

    #endregion

    #region Variables

    public static GameObject LocalPlayerInstance;

    // Movement
    [SerializeField] private float m_baseWalkSpeed = 8f;
    [SerializeField] private float m_baseRunSpeed = 15f;

    private float m_walkSpeed;
    private float m_runSpeed;

    private bool m_isMoving = false;
    private bool m_canMove = true;
    private bool m_isDashing = false;

    private Vector3 m_movement = Vector3.zero;
    [SerializeField] private readonly float m_gravity = 9.81f;

    private Vector3 m_movementNoGrav = Vector3.zero;
    
    private readonly float m_rotationSpeed = 40f; // from WarriorMovementControllerFREE.cs
    private float m_vSpeed = 0f; // current vertical velocity

    // Blind
    private GameObject m_blindPanel;

    #endregion

    #region Components

    public GameObject m_character = null;

    private Animator m_animator;
    private Transform m_characterTransform;
    private CharacterController m_controller;

    #endregion

    #region MonoBehaviour callbacks

    private void Awake()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator)
        {
            m_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
        if (photonView.IsMine)
        {
            LocalPlayerInstance = gameObject;
        }
        if (m_character)
        {
            m_characterTransform = m_character.transform;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        m_controller = GetComponent<CharacterController>();
        m_blindPanel = GameObject.Find("Canvas").transform.Find("LevelUI").transform.Find("BlindPanel").gameObject;

        // Initially set speed depending on what level we join right away (invoked once upon initial instantiation)
        GameManager levelGM = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetSpeedDependingOnLevel(levelGM.IsLevel);
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessPlayerInput();

            if (!m_blindPanel)
            {
                // ensure that blindPanel is always valid, temp fix for switching between scenes
                GameObject canvas = GameObject.Find("Canvas");
                if (canvas)
                {
                    m_blindPanel = canvas.transform.Find("LevelUI").transform.Find("BlindPanel").gameObject;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            MovePlayer();
        }
    }

    #endregion

    #region Private functions

    // Takes in player's inputs for movement
    private void ProcessPlayerInput()
    {
        if (m_canMove)
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
    }

    /// <summary>
    /// Given m_movement, update the player position (aka actually move the player), also handles animations
    /// </summary>
    private void MovePlayer()
    {
        if (m_canMove)
        {
            if (m_isDashing)
            m_controller.Move(m_movement * m_runSpeed * Time.deltaTime);
            else
            {
                m_controller.Move(m_movement * m_walkSpeed * Time.deltaTime);

                if (m_animator)
                {
                    m_animator.enabled = false;
                    m_animator.enabled = true;
                }
            }

            // check if player is moving by zeroing out the y value of m_movement (ignore gravity) and taking the magnitude
            if (m_movementNoGrav.magnitude > 0f)
            {
                if (m_isDashing)
                {
                    // running
                    m_isMoving = true;
                    
                    if (m_animator)
                    {
                        m_animator.SetBool("Moving", true);
                        m_animator.SetFloat("Velocity Z", m_movementNoGrav.magnitude + 6);
                    }
                }
                else
                {
                    // walking
                    m_isMoving = true;
                    
                    if (m_animator)
                    {
                        m_animator.SetBool("Moving", true);
                        m_animator.SetFloat("Velocity Z", m_movementNoGrav.magnitude);
                    }
                }
            }
            else if (m_isMoving)
            {
                // idle
                m_isMoving = false;
                
                if (m_animator)
                {
                    m_animator.SetBool("Moving", false);
                    m_animator.SetFloat("Velocity Z", 0);
                }
            }

            // align the wizard's position to be the same as the parent GameObject position, and shift it down a bit
            if (m_character)
            {
                m_character.transform.position = transform.position - new Vector3(0, 1, 0);
            }

            m_movement = Vector3.zero; // reset movement after finished moving
        }
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Public setter for runSpeed
    /// </summary>
    /// <param name="newSpeed">New speed to override runSpeed</param>
    public void AlterRunSpeed(float newSpeed)
    {
        m_runSpeed = newSpeed;
    }

    /// <summary>
    /// Public setter for walkSpeed
    /// </summary>
    /// <param name="newSpeed">New speed to override walkSpeed</param>
    public void AlterWalkSpeed(float newSpeed)
    {
        m_walkSpeed = newSpeed;
    }

    /// <summary>
    /// If it's a normal level, set speeds to base speed, otherwise, set speed to double the base speed.
    /// This is done for convenience for walking around in the lobby and narrative scenes.
    /// Invoked by GameManager when transitioning players to new scenes.
    /// </summary>
    /// <param name="isNormalLevel">Is this level normal or not</param>
    public void SetSpeedDependingOnLevel(bool isNormalLevel)
    {
        if (isNormalLevel)
        {
            // If it's a normal level, retrieve base speeds
            m_walkSpeed = m_baseWalkSpeed;
            m_runSpeed = m_baseRunSpeed;
        }
        else
        {
            // Double speed in lobby and narrative levels
            m_walkSpeed = m_baseWalkSpeed * 2f;
            m_runSpeed = m_baseRunSpeed * 2f;
        }
    }

    /// <summary>
    /// Prevent the player from controlling their character.
    /// </summary>
    public void Stun()
    {
        m_canMove = false;
        m_animator.SetBool("Moving", false);
    }

    /// <summary>
    /// Allows the player to controll their character again.
    /// </summary>
    public void UnStun()
    {
        m_canMove = true;
    }

    #endregion

    #region WarriorMovementControllerFree functions

    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~
    // all functions below taken from the warriormovementcontrollerFREE script
    // ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~

    /// <summary>
    /// Print the animation states. I haven't used this much so I'm not sure if this works.
    /// </summary>
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

    /// <summary>
    /// Rotates the character towards the direction it's moving in
    /// </summary>
    private void RotateTowardsMovementDir()
    {
        if (m_movementNoGrav != Vector3.zero)
        {
            if (m_character)
            {
                m_character.transform.rotation = Quaternion.Slerp(m_character.transform.rotation, Quaternion.LookRotation(m_movementNoGrav), Time.deltaTime * m_rotationSpeed);
            }
        }
    }

    #endregion

    #region Photon functions

    // IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Use this information to sync child rotational transform
        // instead of placing PhotonView and PhotonTransfromView on child object
        if (stream.IsWriting)
        {
            if (m_character)
            {
                stream.SendNext(m_characterTransform.localRotation);
            }
        }
        else
        {
            if (m_character)
            {
                m_characterTransform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    #endregion
}
