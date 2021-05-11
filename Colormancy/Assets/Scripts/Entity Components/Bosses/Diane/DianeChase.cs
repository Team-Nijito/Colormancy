using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

public class DianeChase : State
{
    EnemyMovement m_EnemyMovement;

    DianeAI m_dianeAI;

    public DianeChase(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", true);
        m_EnemyMovement = BossAI.GetComponent<EnemyMovement>();
        return base.Start();
    }

    public override IEnumerator Update()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Update();
        //Chase player
        if (BossAI.Target != null)
        {
            if (BossAI.DistanceToTarget() > 2f)
            {
                BossAI.Movement.MoveToPosition(BossAI.Target.transform.position);
            }
            BossAI.Movement.FaceTarget(BossAI.DirectionToTarget());
        }

        //Attacks
        Debug.Log(m_dianeAI.currentIdleCooldown);
        if (m_dianeAI.currentIdleCooldown >= m_dianeAI.IdleCooldown)
        {
            if (m_dianeAI.currentFocusFireCooldown >= m_dianeAI.FocusFireCooldown)
            {
                m_dianeAI.currentFocusFireCooldown = 0f;
                m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, "Focus Fire");
                BossAI.SetTarget(null);
            }

            if (m_dianeAI.currentHamstringCooldown >= m_dianeAI.HamstringCooldown)
            {
                m_dianeAI.currentHamstringCooldown = 0f;
                m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, "Hamstring");
                BossAI.SetTarget(null);
            }

            if (BossAI.InRangeOfTarget(m_dianeAI.SlashRange) && m_dianeAI.currentSlashCooldown >= m_dianeAI.SlashCooldown)
            {
                m_dianeAI.currentSlashCooldown = 0f;
                m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, "Slash");
                BossAI.SetTarget(null);
            }
        }

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", false);
        return base.Stop();
    }
}
