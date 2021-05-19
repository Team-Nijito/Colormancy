using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

[System.Serializable]
public class HelenChase : State
{
    EnemyMovement m_EnemyMovement;

    HelenAI m_HelenAI;

    public HelenChase(BossAI bossAI) : base(bossAI)
    {
        m_HelenAI = (HelenAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_HelenAI.DebugMode)
            Debug.Log("Start Chase State");

        m_HelenAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chase", true);
        m_EnemyMovement = BossAI.GetComponent<EnemyMovement>();
        return base.Start();
    }

    public override IEnumerator Update()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Update();

        if (BossAI.Target != null)
        {
            if (BossAI.DistanceToTarget() > 2f)
            {
                BossAI.Movement.MoveToPosition(BossAI.Target.transform.position);
            }
            BossAI.Movement.FaceTarget(BossAI.DirectionToTarget());
        }

        if (m_HelenAI.currentIdleCooldown >= m_HelenAI.IdleCooldown)
        {
            if (m_HelenAI.currentShunpoCooldown >= m_HelenAI.ShunpoCooldown)
            {
                m_HelenAI.photonView.RPC("SetHelenState", Photon.Pun.RpcTarget.AllViaServer, HelenAI.States.Shunpo);
                m_HelenAI.currentShunpoCooldown = 0f;
                m_HelenAI.currentIdleCooldown = 0f;
            } else if (m_HelenAI.currentShankCooldown >= m_HelenAI.ShankCooldown && m_HelenAI.InRangeOfTarget(m_HelenAI.ShankRange))
            {
                m_HelenAI.photonView.RPC("SetHelenState", Photon.Pun.RpcTarget.AllViaServer, HelenAI.States.Shank);
                m_HelenAI.currentShankCooldown = 0f;
                m_HelenAI.currentIdleCooldown = 0f;
            }
        }
        return base.Update();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_HelenAI.DebugMode)
            Debug.Log("Stop Chase State");

        return base.Stop();
    }
}
