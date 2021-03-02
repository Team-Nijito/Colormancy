using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(PhotonView))]
[RequireComponent(typeof(EnemyAnimationManager))]
[DisallowMultipleComponent]
public class EnemyMovement : MonoBehaviourPun, IPunObservable
{
    // This class is responsible for AI movement variables
    // The AI utilizes the Unity Navmesh and the Photon framework to sync its position

    #region Accessors (c# Properties)

    public float Speed { get { return m_speed; } protected set { m_speed = value; } }
    public float SpeedTriggerRun { get { return m_speedTriggerRun; } protected set { m_speedTriggerRun = value; } }
    
    public EnemyAnimationManager.EnemyState CurrentAnimState { get { return m_currentAnimState; } protected set { m_currentAnimState = value; } }

    public Vector3 DirectionToPlayer { get { return m_directionToPlayer; } protected set { m_directionToPlayer = value; } }
    public float AngleFromPlayer { get { return m_angleFromPlayer; } protected set { m_angleFromPlayer = value; } }
    public float DistanceFromPlayer { get { return m_distanceFromPlayer; } protected set { m_distanceFromPlayer = value; } }

    #endregion

    #region Variables

    [SerializeField] protected float m_speed = 18f;

    [Tooltip("The speed at which the Run animation is triggered")]
    [SerializeField] protected float m_speedTriggerRun = 15f;

    protected EnemyAnimationManager.EnemyState m_currentAnimState = EnemyAnimationManager.EnemyState.Idle;

    protected Vector3 m_directionToPlayer = Vector3.zero;
    protected float m_angleFromPlayer = 0;
    protected float m_distanceFromPlayer = -1;

    #endregion

    #region Components

    protected GameObject m_character;
    protected NavMeshAgent m_navMeshAgent;
    protected EnemyAnimationManager m_animManager;

    #endregion

    #region MonoBehaviour callbacks

    // Start is called before the first frame update
    private void Start()
    {
        m_navMeshAgent = GetComponent<NavMeshAgent>();

        // Override the speed variable in m_navMeshAgent if it's not set already.
        m_navMeshAgent.speed = m_speed;
    }

    #endregion

    #region Setters

    public void SetAngleFromPlayer(float newAngle)
    {
        m_angleFromPlayer = newAngle;
    }

    public void SetCurrentAnimState(EnemyAnimationManager.EnemyState newState)
    {
        m_currentAnimState = newState;
    }

    public void SetDistanceFromPlayer(float dist)
    {
        m_distanceFromPlayer = dist;
    }

    public void SetDirectionToPlayer(Vector3 newDir)
    {
        m_directionToPlayer = newDir;
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
