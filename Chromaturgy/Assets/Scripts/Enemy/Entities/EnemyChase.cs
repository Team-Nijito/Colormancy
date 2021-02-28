using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

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

    [SerializeField] protected float m_detectionRadius = 30f;

    [SerializeField] protected float m_attackRange = 3f;

    // Movement
    [SerializeField] protected float m_speed = 18f;

    [Tooltip("The speed at which the Run animation is triggered")]
    [SerializeField]
    protected float m_speedTriggerRun = 15f;

    protected AnimationManager.EnemyState m_currentState = AnimationManager.EnemyState.Idle;

    protected Vector3 m_directionToPlayer = Vector3.zero;
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

    protected bool m_isAttacking = false; // don't interrupt attacking animation

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

    // Determine which player to seek out (if omnipotent), or just check
    // if there are any players around the enemy (if not omnipotent)
    // right now just omnipotent b/c we're in an arena of course they know we're here
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

    // Consider what the AI will do at any point, and handles AI animation
    protected virtual void ProcessAIIntent()
    {
        // don't interrupt enemy attack animation, so that players have a chance to dodge attacks
        if (!m_isAttacking)
        {
            if (m_targetPlayer)
            {
                m_directionToPlayer = m_targetPlayer.position - transform.position;

                if (m_distanceFromPlayer < m_detectionRadius)
                {
                    m_directionToPlayer.y = 0;

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
                        m_isAttacking = true;
                    }
                }
                else
                {
                    m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
                }
            }
            else
            {
                m_animManager.ChangeState(AnimationManager.EnemyState.Idle);
            }
        }
    }

    // Primarily used for moving and attacking
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

    // How to pass GameObject/transform through RPC: https://forum.unity.com/threads/how-to-photon-networking-send-gameobjects-transforms-and-other-through-the-network.343973/
    /// <summary>
    /// This is used so that the Enemy's target is synced acrossed all players
    /// </summary>
    /// <param name="playerPhotonViewID"></param>
    [PunRPC]
    public void DeclareTargetPlayer(int playerPhotonViewID)
    {
        Transform playerTransform = PhotonView.Find(playerPhotonViewID).transform;
        m_targetPlayer = playerTransform;
    }

    // see wrapper functions in public methods
    [PunRPC]
    private void EnableHitBoxes()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.SetActive(true);
        }
    }

    // see wrapper functions in public methods
    [PunRPC]
    private void DisableHitBoxes()
    {
        foreach (HitBox hitBox in m_hitBoxesArray)
        {
            hitBox.m_hitBoxObject.SetActive(false);
        }
        ResetHurtVictimArray();
        m_isAttacking = false;
    }

    // Clears the hurt victim array, so that we can damage the victims again in the next attack
    private void ResetHurtVictimArray()
    {
        if (m_hurtVictimArray != null)
        {
            System.Array.Clear(m_hurtVictimArray, 0, m_hurtVictimArray.Length);
        }
        m_hurtVictimArrayIndex = 0;
    }

    // for hitbox usage, keep track of players (via array) we've attacked during same animation
    // then we would reset the array and check again during the next animation
    [PunRPC]
    private void InsertHurtVictim(int playerViewID)
    {
        m_hurtVictimArray[m_hurtVictimArrayIndex] = playerViewID;
        m_hurtVictimArrayIndex += 1;
    }

    #endregion

    #region Public methods
    
    // Wrapper function for enabling hitbox
    public void RPCEnableHitBoxes()
    {
        photonView.RPC("EnableHitBoxes", RpcTarget.All);
    }

    // Wrapper function for disabling hitbox
    public void RPCDisablehitBoxes()
    {
        photonView.RPC("DisableHitBoxes", RpcTarget.All);
    }

    // Wrapper function for inserting players who've the enemy attacked during one animation
    public void RPCInsertHurtVictim(int playerViewID)
    {
        photonView.RPC("InsertHurtVictim", RpcTarget.All, playerViewID);
    }


    /// <summary>
    /// Check if player can be attacked again during attack animation. Prevents a player from being attacked multiple times during 1 anim.
    /// </summary>
    /// <param name="PhotonID"></param>
    /// <returns>Whether a player is a valid target</returns>
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
