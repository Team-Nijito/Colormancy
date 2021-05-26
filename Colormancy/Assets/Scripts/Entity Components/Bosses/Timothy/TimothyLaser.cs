using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
class TimothyLaser : State
{
    EnemyMovement m_EnemyMovement;

    TimothyAI m_timAI;

    public TimothyLaser(BossAI bossAI) : base(bossAI)
    {
        m_timAI = (TimothyAI)BossAI;
    }
    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_timAI.DebugMode)
            Debug.Log("Start Laser State");

        m_EnemyMovement = BossAI.GetComponent<EnemyMovement>();

        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Channel", true);
        m_timAI.StartCoroutine(WaitTime(2.5f));

        return base.Start();
    }
    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_timAI.DebugMode)
            Debug.Log("Stop Laser State");

        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Channel", false);

        return base.Stop();
    }

    IEnumerator WaitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        m_timAI.Movement.FaceTarget(m_timAI.DirectionToTarget());
        Debug.Log("Shooting");
        m_timAI.photonView.RPC("SetAnimationTrigger", RpcTarget.All, "Shoot");
        m_timAI.photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, TimothyAI.States.Run);
    }
}