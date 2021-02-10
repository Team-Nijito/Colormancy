using UnityEngine;
using Photon.Pun;

public class EnemyChase : MonoBehaviourPun, IPunObservable
{
    // Handles the logic, and movement of the enemy chaser.
    // If there is a hitbox, the damage is done by the hitbox, and the hitbox is a child of a part of the character object.

    // Targetting
    private Transform m_targetPlayer;

    [SerializeField] private float m_detectionRadius = 30f;

    [SerializeField] private float m_attackRange = 3f;

    // Movement
    [SerializeField] private float m_speed = 18f;

    [Tooltip("The speed at which the Run animation is triggered")]
    [SerializeField] 
    private float m_speedTriggerRun = 15f;

    private AnimationManager.EnemyState m_currentState = AnimationManager.EnemyState.Idle;

    private Vector3 m_directionToPlayer = Vector3.zero;
    private float m_distanceFromPlayer = -1;

    // Attack attributes
    [Tooltip("Include all references to hitboxes here")]
    [SerializeField]
    private GameObject[] m_hitBoxesArray;

    private float m_attackAnimLength;

    [SerializeField]
    private float m_attackActiveStart; // 2.767 // 0.29

    [SerializeField]
    private float m_attackActiveEnd; // 0.31

    // Components
    public GameObject m_character = null;
    private Transform m_characterTransform;

    private Rigidbody m_rbody;
    private HealthScript m_hscript;
    private AnimationManager m_animManager;

    // Start is called before the first frame update
    void Start()
    {
        m_rbody = GetComponent<Rigidbody>();
        m_hscript = GetComponent<HealthScript>();
        m_animManager = GetComponent<AnimationManager>();
        
        if (m_character)
        {
            m_characterTransform = m_character.transform;
        }
        if (m_animManager)
        {
            m_attackAnimLength = m_animManager.attackTime;
        }
    }

    // Update is called once per frame
    void Update()
    {
        Transform target = DetermineTargetPlayer(ref m_distanceFromPlayer);
        if (target && (!m_targetPlayer || m_targetPlayer.gameObject.GetPhotonView().ViewID != target.gameObject.GetPhotonView().ViewID))
        {
            // Only send another RPC, if the target this time is different.
            photonView.RPC("DeclareTargetPlayer", RpcTarget.All, target.gameObject.GetPhotonView().ViewID);
        }
        ProcessAIIntent();
    }

    private void FixedUpdate()
    {
        HandleAIIntent();
    }

    // Determine which player to seek out (if omnipotent), or just check
    // if there are any players around the enemy (if not omnipotent)
    // right now just omnipotent b/c we're in an arena of course they know we're here
    private Transform DetermineTargetPlayer(ref float distanceFromPlayer)
    {
        Transform targetTransform = null;
        float targetDistance = -1;

        // Focus on the closest players from all players
        foreach (GameObject player in m_hscript.m_gameManager.FetchPlayerGameObjects())
        {
            if (player)
            {
                float tmpDistance = Vector3.Distance(player.transform.position, transform.position);
                if (targetTransform)
                {
                    if (tmpDistance < Vector3.Distance(targetTransform.position, transform.position))
                    {
                        targetTransform = player.transform;
                        targetDistance = tmpDistance;
                    }
                }
                else
                {
                    targetTransform = player.transform;
                    targetDistance = tmpDistance;
                }
            }
        }
        distanceFromPlayer = targetDistance;
        return targetTransform;
    }

    // Consider what the AI will do at any point, and handles AI animation
    private void ProcessAIIntent()
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

    // Primarily used for moving and attacking
    private void HandleAIIntent()
    {
        if (m_targetPlayer)
        {
            m_currentState = m_animManager.GetCurrentState();

            if (Vector3.Distance(m_targetPlayer.position, transform.position) < m_detectionRadius)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(m_directionToPlayer), 0.1f);

                if (m_currentState == AnimationManager.EnemyState.Walk || m_currentState == AnimationManager.EnemyState.Run)
                {
                    m_rbody.AddForce(transform.forward * m_speed);
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

    [PunRPC]
    public void EnableHitBoxes()
    {
        foreach (GameObject hitBox in m_hitBoxesArray)
        {
            hitBox.SetActive(true);
        }
    }

    [PunRPC]
    public void DisableHitBoxes()
    {
        foreach (GameObject hitBox in m_hitBoxesArray)
        {
            hitBox.SetActive(false);
        }
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
                //stream.SendNext(m_characterTransform.localPosition);
                stream.SendNext(m_characterTransform.localRotation);
            }
        }
        else
        {
            if (m_character)
            {
                //m_characterTransform.localPosition = (Vector3)stream.ReceiveNext();
                m_characterTransform.localRotation = (Quaternion)stream.ReceiveNext();
            }
        }
    }
}
