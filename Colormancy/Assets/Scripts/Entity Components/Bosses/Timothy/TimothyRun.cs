using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class TimothyRun : State
{
    EnemyMovement m_EnemyMovement;

    TimothyAI m_timAI;

    public TimothyRun(BossAI bossAI) : base(bossAI)
    {
        m_timAI = (TimothyAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_timAI.DebugMode)
            Debug.Log("Start Chase State");
        m_EnemyMovement = BossAI.GetComponent<EnemyMovement>();
        return base.Start();
    }

    public override IEnumerator Update()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Update();

        if (BossAI.Target != null)
        {
            if (BossAI.DistanceToTarget() < m_timAI.RunDistance)
            {
                m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Run", true);
                Vector3 dirToPlayer = m_timAI.transform.position - m_timAI.Target.transform.position;
                Vector3 newPos = m_timAI.transform.position + dirToPlayer;

                BossAI.Movement.MoveToPosition(newPos);
                BossAI.Movement.FaceTarget(dirToPlayer);
            } else if (BossAI.DistanceToTarget() > m_timAI.ChaseDistance)
            {
                m_timAI.photonView.RPC("SetAnimationBool", RpcTarget.All, "Run", true);
                Vector3 dirToPlayer = Vector3.Normalize(m_timAI.Target.transform.position - m_timAI.transform.position);
                Vector3 newPos = m_timAI.transform.position + (dirToPlayer * 4); // 4 "units" in direction of player

                BossAI.Movement.MoveToPosition(newPos);
                BossAI.Movement.FaceTarget(dirToPlayer);
            }
            else
            {
                m_timAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Run", false);
            }
            BossAI.Movement.FaceTarget(BossAI.DirectionToTarget());
        }

        if (m_timAI.currentIdleCooldown >= m_timAI.IdleCooldown)
        {
            if (m_timAI.currentLaserCooldown >= m_timAI.LaserCooldown)
            {
                m_timAI.photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, TimothyAI.States.Laser);
                m_timAI.currentLaserCooldown = 0f;
                m_timAI.currentIdleCooldown = 0f;
            }
            else if (m_timAI.currentBlastOffCooldown >= m_timAI.BlastOffCooldown && m_timAI.InRangeOfTarget(m_timAI.BlastOffRange))
            {
                m_timAI.photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, TimothyAI.States.BlastOff);
                m_timAI.currentBlastOffCooldown = 0f;
                m_timAI.currentIdleCooldown = 0f;
            }
            //else if (m_timAI.currentMMarchCooldown >= m_timAI.MMarchCooldown)
            //{
            //    m_timAI.photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, TimothyAI.States.MachineMarch);
            //    m_timAI.currentMMarchCooldown = 0f;
            //    m_timAI.currentIdleCooldown = 0f;
            //}
        }
        return base.Update();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_timAI.DebugMode)
            Debug.Log("Stop Chase State");

        m_timAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Run", false);
        return base.Stop();
    }
}
