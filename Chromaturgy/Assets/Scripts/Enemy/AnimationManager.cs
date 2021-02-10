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

    // Animation times (try not to alter these values outside of the script)
    public float idleTime;
    public float walkTime;
    public float runTime;
    public float attackTime;
    public float deathTime;

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
                m_animator.Play(ENEMY_IDLE);
                break;
            case EnemyState.Walk:
                m_animator.Play(ENEMY_WALK);
                break;
            case EnemyState.Run:
                m_animator.Play(ENEMY_RUN);
                break;
            case EnemyState.Attack:
                m_animator.Play(ENEMY_ATTACK);
                break;
            case EnemyState.Death:
                m_animator.Play(ENEMY_DEATH);
                break;
        }
        m_currentState = newState;
    }

    public EnemyState GetCurrentState()
    {
        return m_currentState;
    }

    // Function by johnnieZombie https://forum.unity.com/threads/how-to-find-animation-clip-length.465751/
    public void UpdateAnimClipTimes()
    {
        AnimationClip[] clips = m_animator.runtimeAnimatorController.animationClips;
        foreach (AnimationClip clip in clips)
        {
            switch (clip.name)
            {
                case "Attack":
                    attackTime = clip.length;
                    break;
                case "Death":
                    deathTime = clip.length;
                    break;
                case "Idle":
                    idleTime = clip.length;
                    break;
                case "Walk":
                    walkTime = clip.length;
                    break;
                case "Run":
                    runTime = clip.length;
                    break;
            }
        }
    }
}
