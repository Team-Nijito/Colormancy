using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
class TimothyBlastOff : State
{
    EnemyMovement m_EnemyMovement;

    TimothyAI m_timAI;

    int nPoints = 100;
    int nEnemiesToSpawn = 2;
    Vector3 furthestPoint;

    public TimothyBlastOff(BossAI bossAI) : base(bossAI)
    {
        m_timAI = (TimothyAI)BossAI;
    }
    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_timAI.DebugMode)
            Debug.Log("Start Blast Off State");

        m_EnemyMovement = BossAI.GetComponent<EnemyMovement>();

        furthestPoint = m_EnemyMovement.GetRandomPosition();
        for (int i = 0; i < nPoints; i++)
        {
            Vector3 rPoint = m_EnemyMovement.GetRandomPosition();
            if ((furthestPoint - m_timAI.transform.position).magnitude < (rPoint - m_timAI.transform.position).magnitude)
            {
                furthestPoint = rPoint;
            }
        }

        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Jump", true);
        m_timAI.StartCoroutine(WaitTime(.25f));

        return base.Start();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_timAI.DebugMode)
            Debug.Log("Stop Blast Off State");

        m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Jump", false);

        return base.Stop();
    }
    IEnumerator WaitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        for (int i = 0; i < nEnemiesToSpawn; i++)
        {
            PhotonNetwork.Instantiate("Enemies/Terminator Bot", m_timAI.transform.position, m_timAI.transform.localRotation);
        }

        m_timAI.MeshAgent.Warp(furthestPoint);
        m_timAI.Movement.FaceTarget(m_timAI.DirectionToTarget());
        m_timAI.photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, TimothyAI.States.Run);
    }
}