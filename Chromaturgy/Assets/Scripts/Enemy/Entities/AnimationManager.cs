using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    // Manages an enemy's animation
    
    private Animator m_animator;
    private EnemyState m_currentState = EnemyState.Idle;
    
    public enum EnemyState
    {
        Idle,
        Walk,
        Run,
        Attack,
        Death
    }

    // Animation names
    const string ENEMY_IDLE = "Idle";
    const string ENEMY_WALK = "Walk";
    const string ENEMY_RUN = "Run";
    const string ENEMY_ATTACK = "Attack";
    const string ENEMY_DEATH = "Death";

    // Enable any of these animations to be played / or not
    [SerializeField]
    private bool m_EnemyIdleExist = true;
    [SerializeField]
    private bool m_EnemyWalkExist = true;
    [SerializeField]
    private bool m_EnemyRunExist = true;
    [SerializeField]
    private bool m_EnemyAttackExist = true;
    [SerializeField]
    private bool m_EnemyDeathExist = true;

    private void Start()
    {
        m_animator = GetComponent<Animator>();
        if (m_animator)
        {
            m_animator.cullingMode = AnimatorCullingMode.CullUpdateTransforms;
        }
    }

    /// <summary>
    /// Changes the enemy's current state and changes its animation accordingly
    /// </summary>
    /// <param name="newState"></param>
    public void ChangeState(EnemyState newState)
    {
        // stop the same animation from interupting itself
        if (m_currentState == newState) return;

        switch (newState)
        {
            case EnemyState.Idle:
                if (m_EnemyIdleExist)
                {
                    m_animator.Play(ENEMY_IDLE);
                }
                break;
            case EnemyState.Walk:
                if (m_EnemyWalkExist)
                {
                    m_animator.Play(ENEMY_WALK);
                }
                break;
            case EnemyState.Run:
                if (m_EnemyRunExist)
                {
                    m_animator.Play(ENEMY_RUN);
                }
                break;
            case EnemyState.Attack:
                if (m_EnemyAttackExist)
                {
                    m_animator.Play(ENEMY_ATTACK);
                }
                break;
            case EnemyState.Death:
                if (m_EnemyDeathExist)
                {
                    m_animator.Play(ENEMY_DEATH);
                }
                break;
        }
        m_currentState = newState;
    }

    public EnemyState GetCurrentState()
    {
        return m_currentState;
    }
}
