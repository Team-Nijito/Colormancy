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
        m_timAI.StartCoroutine(ChannelLaser());

        return base.Start();
    }
    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_timAI.DebugMode)
            Debug.Log("Stop Laser State");

        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Shoot", false);

        return base.Stop();
    }

    IEnumerator ChannelLaser()
    {
        yield return new WaitForSeconds(m_timAI.ChannelTime);

        m_timAI.Movement.FaceTarget(m_timAI.DirectionToTarget());
        if (m_timAI.DebugMode)
            Debug.Log("Shooting");
        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Channel", false);
        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Shoot", true);
        m_timAI.LaserObject.SetActive(true);

        m_timAI.StartCoroutine(StopShooting());
    }

    IEnumerator StopShooting()
    {
        yield return new WaitForSeconds(1.85f);
        m_timAI.LaserObject.SetActive(false);
        m_timAI.photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, TimothyAI.States.Run);
    }
}