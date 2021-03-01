using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;
using System.Collections;

public class EnemyChase : MonoBehaviourPun, IPunObservable
{
    // Handles the logic, and movement of the enemy chaser.
    // If there is a hitbox, the damage is done by the hitbox, and the hitbox is a child of a part of the character object.

    #region Variables

    [System.Serializable]
    protected struct HitBox
    {
        public GameObject m_hitBoxObject;
        public DetectHit m_hitBoxScript;
    }

    // Targetting
    protected Transform m_targetPlayer;

    [SerializeField] protected float m_closeDetectionRadius = 1.5f; // used when a player gets too close to an enemy
    [SerializeField] protected float m_detectionRadius = 30f; // used in every other case

    [Range(0, 180)]
    [SerializeField] protected float m_fieldOfView = 30f; // degrees

    [SerializeField] protected float m_attackRange = 3f;

    [SerializeField] protected float m_rememberTargetDuration = 7f;  // how long does an AI "remember" a target once target is out of vision

    protected bool m_rememberTarget = false;
    protected bool m_isForgettingTarget = false;

    protected Coroutine m_forgettingTargetCoroutineRef = null;

    // Movement
    [SerializeField] protected float m_speed = 18f;

    [Tooltip("The speed at which the Run animation is triggered")]
    [SerializeField]
    protected float m_speedTriggerRun = 15f;

    protected AnimationManager.EnemyState m_currentState = AnimationManager.EnemyState.Idle;

    protected Vector3 m_directionToPlayer = Vector3.zero;
    protected float m_angleFromPlayer = 0;
    protected float m_distanceFromPlayer = -1;

    // Attack attributes

    [Tooltip("Include all references to hitboxes here")]
    [SerializeField]
    protected HitBox[] m_hitBoxesArray;

    [SerializeField]
    private int m_numPlayersCanHit = 4; // number of players hitbox can damage in one attack animation

    private int[] m_hurtVictimArray; // keep track of PhotonID of players who hitbox has damaged so we don't damage them
                                     // again during same attack animation

    private int m_hurtVictimArrayIndex = 0; // keep track of first available spot to insert in array

    // Components
    public GameObject m_character = null;

    protected NavMeshAgent m_navMeshAgent;
    protected HealthScript m_hscript;
    protected AnimationManager m_animManager;

    #endregion
    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    protected virtual void Start()
    {
        m_hscript = GetComponent<HealthScript>();
        m_animManager = GetComponent<AnimationManager>();
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        // Override the variables in m_navMeshAgent if they're not set already.
        m_navMeshAgent.speed = m_speed;
        m_navMeshAgent.stoppingDistance = m_attackRange;

        m_hurtVictimArray = new int[m_numPlayersCanHit];
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        Transform target = DetermineTargetPlayer(ref m_distanceFromPlayer);
        PhotonView targetPhotonView = target.gameObject.GetPhotonView();
        if (target && (!m_targetPlayer || m_targetPlayer.gameObject.GetPhotonView().ViewID != targetPhotonView.ViewID))
        {
            // Only send another RPC, if the target this time is different.
            // Also check if player is still here
            if (targetPhotonView.Owner != null && targetPhotonView.Owner.TagObject != null)
            {
                photonView.RPC("DeclareTargetPlayer", RpcTarget.All, target.gameObject.GetPhotonView().ViewID);
            }
        }
        ProcessAIIntent();
    }

    protected virtual void FixedUpdate()
    {
        HandleAIIntent();
    }

    #endregion
    #region Private Methods

    /// <summary>
    /// Determine which player to seek out (if omnipotent), or just check
    /// if there are any players around the enemy (if not omnipotent)
    /// right now just omnipotent b/c we're in an arena of course they know we're here
    /// </summary>
    /// <param name="distanceFromPlayer"></param>
    /// <returns>The transform of the closest player in the server, or null if no players characters exist</returns>
    protected virtual Transform DetermineTargetPlayer(ref float distanceFromPlayer)
    {
        Transform targetTransform = null;
        float targetDistance = -1;

        // Focus on the closest players from all players
        foreach (Player play in PhotonNetwork.PlayerList)
        {
            GameObject playObj = play.TagObject as GameObject;
            if (playObj)
            {
                float tmpDistance = Vector3.Distance(playObj.transform.position, transform.position);
                if (targetTransform)
                {
                    if (tmpDistance < Vector3.Distance(targetTransform.position, transform.position))
                    {
                        targetTransform = playObj.transform;
                        targetDistance = tmpDistance;
                    }
                }
                else
                {
                    targetTransform = playObj.transform;
                    targetDistance = tmpDistance;
                }
            }
        }
        distanceFromPlayer = targetDistance;
        return targetTransform;
    }

    /// <summary>
    /// Consider what the AI will do at any point, and handles AI animation
    /// </summary>
    protected virtual void ProcessAIIntent()
    {
        if (m_targetPlayer)
        {
            m_directionToPlayer = m_targetPlayer.position - transform.position;
            m_angleFromPlayer = Vector3.Angle(m_directionToPlayer, transform.forward);
              
            if (m_distanceFromPlayer < m_closeDetectionRadius)
            {
                // player got too close, they're detected
                // raycast here to see if there is an object between AI and player
                if (canSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else if (m_distanceFromPlayer < m_detectionRadius && m_angleFromPlayer < m_fieldOfView)
            {
                // player is in cone vision
                // raycast here to see if there is an object between AI and player
                if (canSeePlayer())
                {
                    photonView.RPC("PlayerIsDetected", RpcTarget.All);
                }
            }
            else
            {
                if (m_rememberTarget)
                {
                    print("forgetting:");

                    // still remember target, go after them
                    photonView.RPC("StartForgettingTask", RpcTarget.All);
                }

                if (m_rememberTarget || m_isForgettingTarget)
                {
                    // start forgetting the target, but still target them
                    // until AI completely forgets target
                    photonView.RPC("PlayerIsTargetted", RpcTarget.All);
                }
                else
                {
                    // don't see player, just idle for now
                    m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
                }
            }
        }
        else
        {
            m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
        }
    }

    /// <summary>
    /// Primarily used for moving and attacking
    /// </summary>
    protected virtual void HandleAIIntent()
    {
        if (m_targetPlayer)
        {
            m_currentState = m_animManager.GetCurrentState();

            if (Vector3.Distance(m_targetPlayer.position, transform.position) < m_detectionRadius)
            {
                if (m_currentState == AnimationManager.EnemyState.Walk || m_currentState == AnimationManager.EnemyState.Run)
                {
                    m_navMeshAgent.SetDestination(m_targetPlayer.position);
                }
                else if (m_currentState == AnimationManager.EnemyState.Attack)
                {
                    // Stop the agent from moving
                    m_navMeshAgent.SetDestination(transform.position);
                    m_navMeshAgent.velocity = Vector3.zero;
                    
                    // turn towards player when attacking
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer), 0.1f);
                }
            }
        }
    }

    /// <summary>
    /// The AI raycast to see if there is any obstacle between the AI and a player target.
    /// The ray currently originates from the chest of the AI.
    /// </summary>
    /// <returns>Returns true if there is no obstacle between the transform and the target.</returns>
    protected bool canSeePlayer()
    {
        bool canSee = false;
        Ray ray = new Ray(transform.position, m_targetPlayer.transform.position - transform.position);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.transform == m_targetPlayer)
            {
                canSee = true;
            }
        }
        return canSee;
    }


    /// <summary>
    /// (PunRPC) This is invoked in ProcessAIIntent whenever a player is considered "detected"
    /// </summary>
    [PunRPC]
    protected void PlayerIsDetected()
    {
        m_directionToPlayer.y = 0;

        // Reset the m_rememberTarget task
        m_rememberTarget = true;
        if (m_forgettingTargetCoroutineRef != null)
        {
            StopCoroutine(m_forgettingTargetCoroutineRef);
        }

        PlayerIsTargetted();
    }

    /// <summary>
    /// (PunRPC) This is invoked in ProcessAIIntent whenever a player is considered "remembered" instead of "detected"
    /// and is also invoked by PlayerIsDetected()
    /// </summary>
    [PunRPC]
    protected virtual void PlayerIsTargetted()
    {
        if (m_directionToPlayer.magnitude > m_attackRange)
        {
            if (m_speed > m_speedTriggerRun)
            {
                m_animManager.ChangeState(AnimationManager.EnemyState.Run);
            }
            else
            {
                m_animManager.ChangeState(AnimationManager.EnemyState.Walk);
            }
        }
        else
        {
            m_animManager.ChangeState(AnimationManager.EnemyState.Attack);
        }
    }

    /// <summary>
    /// (PunRPC) Starts the forgetting task.
    /// </summary>
    [PunRPC]
    protected void StartForgettingTask()
    {
        m_forgettingTargetCoroutineRef = StartCoroutine(ForgetTargetAfterDuration());
    }

    /// <summary>
    /// Allows the AI to remember and target the player until the player remains out of sight for m_rememberTargetDuration seconds.
    /// </summary>
    protected IEnumerator ForgetTargetAfterDuration()
    {
        m_rememberTarget = false;
        m_isForgettingTarget = true;
        yield return new WaitForSeconds(m_rememberTargetDuration);

        m_isForgettingTarget = false;

        // stop moving towards target
        m_navMeshAgent.SetDestination(transform.position);
    }

    /// <summary>
    /// (PunRPC) Enables all the listed hitboxes for the AI
    /// </summary>
    [PunRPC]
    private void EnableHitBoxes()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.SetActive(true);
        }
    }

    /// <summary>
    /// (PunRPC) Disables all the listed hitboxes for the AI
    /// </summary>
    [PunRPC]
    private void DisableHitBoxes()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.SetActive(false);
        }
        ResetHurtVictimArray();
    }


    /// <summary>
    /// (PunRPC) For hitbox usage, keep track of players (via array) we've attacked during same animation
    /// then we would reset the array and check again during the next animation
    /// </summary>
    /// <param name="playerViewID">Player photon view</param>
    [PunRPC]
    private void InsertHurtVictim(int playerViewID)
    {
        m_hurtVictimArray[m_hurtVictimArrayIndex] = playerViewID;
        m_hurtVictimArrayIndex += 1;
    }

    /// <summary>
    /// Clears the hurt victim array, so that we can damage the victims again in the next attack.
    /// This function is used alongside InsertHurtVictim.
    /// </summary>
    private void ResetHurtVictimArray()
    {
        if (m_hurtVictimArray != null)
        {
            System.Array.Clear(m_hurtVictimArray, 0, m_hurtVictimArray.Length);
        }
        m_hurtVictimArrayIndex = 0;
    }

    #endregion

    #region Public methods

    /// <summary>
    /// RPC Wrapper function for enabling hitbox (EnableHitBoxes)
    /// </summary>
    public void RPCEnableHitBoxes()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("EnableHitBoxes", RpcTarget.All);
        }
    }

    /// <summary>
    /// RPC Wrapper function for disabling hitbox (DisableHitBoxes)
    /// </summary>
    public void RPCDisableHitBoxes()
    {
        if (photonView.IsMine)
        {
            photonView.RPC("DisableHitBoxes", RpcTarget.All);
        }
    }

    /// <summary>
    ///  Wrapper function for inserting players who've the enemy attacked during one animation (InsertHurtVictim)
    /// </summary>
    /// <param name="playerViewID"></param>
    public void RPCInsertHurtVictim(int playerViewID)
    {
        if (photonView.IsMine)
        {
            photonView.RPC("InsertHurtVictim", RpcTarget.All, playerViewID);
        }
    }

    // How to pass GameObject/transform through RPC: https://forum.unity.com/threads/how-to-photon-networking-send-gameobjects-transforms-and-other-through-the-network.343973/
    /// <summary>
    /// (PunRPC) This is used so that the Enemy's target is synced acrossed all players
    /// </summary>
    /// <param name="playerPhotonViewID">Player's photon view</param>
    [PunRPC]
    public void DeclareTargetPlayer(int playerPhotonViewID)
    {
        m_rememberTarget = false; // forget old player
        Transform playerTransform = PhotonView.Find(playerPhotonViewID).transform;
        m_targetPlayer = playerTransform;
    }

    /// <summary>
    /// Check if player can be attacked again during attack animation. Prevents a player from being attacked multiple times during 1 anim.
    /// </summary>
    /// <param name="PhotonID">Player's photon view</param>
    /// <returns>Is a player is a valid target?</returns>
    public bool IsPlayerValidTarget(int PhotonID)
    {
        return !m_hurtVictimArray.Contains(PhotonID) && m_hurtVictimArrayIndex < m_numPlayersCanHit;
    }

    // IPunObservable Implementation
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // Use this information to sync child rotational transform
        // instead of placing PhotonView and PhotonTransfromView on child object
        if (stream.IsWriting)
        {
            if (m_character)
            {
                stream.SendNext(m_character.transform.localRotation);
            }
        }
        else
        {
            if (m_character)
            {
                m_character.transform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }

    #endregion
}
