using UnityEngine;
using Photon.Pun;
using System.Collections;
using System;
using UnityEngine.UI;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviourPunCallbacks, IPunObservable, IStatusEffects
{
    // This script handles movement input and animations

    #region Variables

    public static GameObject LocalPlayerInstance;

    public enum PlayerState
    {
        Idle = 0,
        Move = 1,
        Fall = 2,
    }

    // Movement
    [SerializeField] private float m_walkSpeed = 18f;
    [SerializeField] private float m_runSpeed = 20f;

    [HideInInspector] public bool m_isMoving = false;
    [HideInInspector] public bool m_canMove = true;
    [HideInInspector] public bool m_isDashing = false;

    [HideInInspector] public Vector3 m_movement = Vector3.zero;

    [SerializeField] private float m_gravity = 9.81f;

    private Vector3 m_movementNoGrav = Vector3.zero;
    
    private readonly float m_rotationSpeed = 40f; // from WarriorMovementControllerFREE.cs
    private float m_vSpeed = 0f; // current vertical velocity

    // Knockback
    private Vector3 m_impact = Vector3.zero; // knockback handling

    [SerializeField] private float m_characterMass = 1.0f;
    [SerializeField] private float m_impactDecay = 5f; // how quickly impact "goes away"

    // Slowdown
    private float m_originalWalkSpeed;
    private float m_originalRunSpeed;

    // Stun / blind
    private Task m_stunTask = null; // only permit one stun coroutine at a time
    private Task m_blindTask = null;

    private GameObject m_blindPanel;

    #endregion

    #region Components

    public GameObject m_character = null;
    public PlayerState m_playerState;

    [HideInInspector] public Animator m_animator;
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
        m_playerState = PlayerState.Idle;
        m_blindPanel = GameObject.Find("Canvas").transform.Find("LevelUI").transform.Find("BlindPanel").gameObject;

        m_originalRunSpeed = m_runSpeed;
        m_originalWalkSpeed = m_walkSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            ProcessPlayerInput();
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            MovePlayer();
            ApplyExternalForce();
        }
    }

    #endregion

    #region Private functions

    /// <summary>
    /// Blinds the player by putting up a black screen.
    /// </summary>
    /// <param name="duration">Duration of blind.</param>
    private IEnumerator BlindForDuration(float duration)
    {
        // apply blind
        m_blindPanel.SetActive(true);
        yield return new WaitForSecondsRealtime(duration);
        // disable blind
        m_blindPanel.SetActive(false);
    }

    // Takes in player's iputs for movement
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

    /// <summary>
    /// Applies the external forces stored in m_impact to the character.
    /// Modified script from aldonaletto: https://answers.unity.com/questions/242648/force-on-character-controller-knockback.html.
    /// This IEnumerator works well with StartCoroutine.
    /// </summary>
    private void ApplyExternalForce()
    {
        // apply the impact force:
        if (m_impact.magnitude > 0.2)
        {
            m_controller.Move(m_impact * Time.deltaTime);
        }
        // consumes the impact energy each cycle:
        m_impact = Vector3.Lerp(m_impact, Vector3.zero, m_impactDecay * Time.deltaTime);
    }

    /// <summary>
    /// Applies slowdown, waits for duration, then reverts slowdown.
    /// This IEnumerator works well with StartCoroutine.
    /// </summary>
    /// <param name="percentReductionSpeed">Range(0,100f). What percentage will we reduce the character's speed by? 50%?</param>
    /// <param name="duration">How long the slowdown will last.</param>
    private IEnumerator ApplySlowdownForDuration(float percentReductionSpeed, float duration)
    {
        float percent = ((100 - percentReductionSpeed) / 100);
        // Apply slowdown
        m_walkSpeed *= percent;
        m_runSpeed *= percent;

        yield return new WaitForSecondsRealtime(duration);
        // Revert slowdown
        m_walkSpeed /= percent;
        m_runSpeed /= percent;
    }

    /// <summary>
    /// Stuns the character, waits for duration, then character can move again.
    /// This IEnumerator doesn't work well with StartCoroutine, go with Task instead.
    /// (having multiple coroutines may mean that player may escape stun early if stunned repeatedly)
    /// </summary>
    /// <param name="duration">Duration of stun.</param>
    private IEnumerator StunForDuration(float duration)
    {
        // apply stun
        m_canMove = false;
        m_animator.SetBool("Moving", false);
        yield return new WaitForSecondsRealtime(duration);
        // revert stun
        m_canMove = true;
    }

    #endregion

    #region Public functions

    /// <summary>
    /// Is like the CS:GO flashbang, but in black.
    /// </summary>
    /// <param name="duration">Duration of blind</param>
    [PunRPC]
    public void ApplyBlind(float duration)
    {
        if (m_blindTask == null)
        {
            m_blindTask = new Task(BlindForDuration(duration));
        }
        else
        {
            m_blindTask.Stop();
            m_blindTask = new Task(BlindForDuration(duration));
        }
    }

    /// <summary>
    /// (PunRPC) Apply a force to this character.
    /// </summary>
    /// <param name="dir">The direction of the force</param>
    /// <param name="force">The magnitude of the force</param>
    [PunRPC]
    public void ApplyForce(Vector3 dir, float force)
    {
        dir.Normalize();
        if (dir.y < 0)
        {
            dir.y = -dir.y; // reflect down force on the ground
        }
        m_impact += dir.normalized * force / m_characterMass;
    }

    /// <summary>
    /// (PunRPC) Apply a slowdown to this character for a duration, then changes the character's speed to its speed before the slowdown. Stackable.
    /// </summary>
    /// <param name="percentReduction">Range(0,100f). What percentage will we reduce the character's speed by? 50%?</param>
    /// <param name="duration">How long the slowdown will last.</param>
    [PunRPC]
    public void ApplySlowdown(float percentReduction, float duration)
    {
        StartCoroutine(ApplySlowdownForDuration(percentReduction, duration));
    }

    /// <summary>
    /// (PunRPC) Stuns a character and prevent them from moving.
    /// </summary>
    /// <param name="duration">How long the stun will last.</param>
    [PunRPC]
    public void ApplyStun(float duration)
    {
        if (m_stunTask == null)
        {
            m_stunTask = new Task(StunForDuration(duration));
        }
        else
        {
            m_stunTask.Stop();
            m_stunTask = new Task(StunForDuration(duration));
        }
    }

    /// <summary>
    /// Stop all ongoing Tasks or coroutines, and reset all variables before status effects.
    /// </summary>
    public void StopAllTasks()
    {
        if (m_stunTask != null)
        {
            m_stunTask.Stop();
            m_canMove = true;
        }
        if (m_blindTask != null)
        {
            m_blindTask.Stop();
            m_blindPanel.SetActive(false);
        }
        StopAllCoroutines();
        m_walkSpeed = m_walkSpeed < m_originalWalkSpeed ? m_originalWalkSpeed : m_walkSpeed;
        m_runSpeed = m_runSpeed < m_originalRunSpeed ? m_originalRunSpeed : m_runSpeed;
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
