using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public abstract class BossAI : StateMachine
{
    #region Public Variables
    public GameObject Target { get; protected set; }
    public EnemyMovement Movement { get => GetComponent<EnemyMovement>(); }
    public Animator Animator { get; protected set; }
    public EnemyHitbox EnemyHitbox { get; protected set; }
    public NavMeshAgent MeshAgent { get; protected set; }
    public StatusEffectScript StatusEffect { get; protected set; }
    //public PhotonView PhotonView { get; protected set; }
    #endregion
        
    #region Public Methods

    /// <summary>
    /// Sets Target variable of boss, returns true if successful, false otherwise
    /// </summary>
    public bool SetTarget(GameObject target)
    {
        if (target == null) return false;
        Target = target;
        return true;
    }

    public float DistanceToTarget()
    {
        float dist = 10000000;
        if (Target == null)
        {
            Debug.LogError("BossAI:: Need to set Target before getting distance!");
            return dist;
        }
        dist = Vector3.Distance(transform.position, Target.transform.position);
        return dist;
    }

    public Vector3 DirectionToTarget()
    {
        Vector3 dir = Target.transform.position - transform.position;
        return dir;
    }

    public bool InRangeOfTarget(float range)
    {
        return DistanceToTarget() <= range;
    }
    #endregion

    #region RPC Methods

    /// <summary>
    /// Calls Animator.SetBool using "animation" and "value" as params for the boss
    /// </summary>
    /// <param name="animation"></param>
    /// <param name="value"></param>
    [PunRPC]
    public void SetAnimationBool(string animation, bool value)
    {
        if (photonView.IsMine)
            Animator.SetBool(animation, value);
    }

    /// <summary>
    /// Calls Animator.SetTrigger using "animation" for the boss
    /// </summary>
    /// <param name="animation"></param>

    [PunRPC]
    public void SetAnimationTrigger(string animation)
    {
        if (photonView.IsMine)
            Animator.SetTrigger(animation);
    }

    /// <summary>
    /// Sets the destination for the MeshAgent of the boss
    /// </summary>
    /// <param name="pos"></param>
    [PunRPC]
    public void SetDestination(Vector3 pos)
    {
        if (photonView.IsMine)
            MeshAgent.destination = pos;
    }
    #endregion
}
