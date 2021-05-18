using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DianeChase : State
{
    EnemyMovement m_EnemyMovement;

    DianeAI m_DianeAI;

    public DianeChase(BossAI bossAI) : base(bossAI)
    {
        m_DianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", true);
        m_EnemyMovement = BossAI.GetComponent<EnemyMovement>();
        return base.Start();
    }

    public override IEnumerator Update()
    {
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
        if (m_DianeAI.currentIdleCooldown >= m_DianeAI.IdleCooldown)
        {
            if (m_DianeAI.currentFocusFireCooldown >= m_DianeAI.FocusFireCooldown)
            {
                m_DianeAI.currentFocusFireCooldown = 0f;
                BossAI.SetState(new DianeFocusFire(BossAI));
                BossAI.SetTarget(null);
            }

            if (m_DianeAI.currentHamstringCooldown >= m_DianeAI.HamstringCooldown)
            {
                m_DianeAI.currentHamstringCooldown = 0f;
                BossAI.SetState(new DianeHamstring(BossAI));
                BossAI.SetTarget(null);
            }

            if (BossAI.InRangeOfTarget(m_DianeAI.SlashRange) && m_DianeAI.currentSlashCooldown >= m_DianeAI.SlashCooldown)
            {
                m_DianeAI.currentSlashCooldown = 0f;
                BossAI.SetState(new DianeSlash(BossAI));
                BossAI.SetTarget(null);
            }
        }

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", false);
        return base.Stop();
    }
}
