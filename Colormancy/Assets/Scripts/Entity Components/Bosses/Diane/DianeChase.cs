using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DianeChase : State
{
    EnemyMovement EnemyMovement;

    DianeAI m_dianeAI;

    public DianeChase(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Chase State");
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", true);
        EnemyMovement = BossAI.GetComponent<EnemyMovement>();
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
        if (m_dianeAI.currentIdleCooldown >= m_dianeAI.IdleCooldown)
        {
            if (m_dianeAI.currentFocusFireCooldown >= m_dianeAI.FocusFireCooldown)
            {
                m_dianeAI.currentFocusFireCooldown = 0f;
                BossAI.SetState(new DianeFocusFire(BossAI));
                BossAI.SetTarget(null);
            }

            if (m_dianeAI.currentHamstringCooldown >= m_dianeAI.HamstringCooldown)
            {
                m_dianeAI.currentHamstringCooldown = 0f;
                BossAI.SetState(new DianeHamstring(BossAI));
                BossAI.SetTarget(null);
            }

            if (BossAI.InRangeOfTarget(m_dianeAI.SlashRange) && m_dianeAI.currentSlashCooldown >= m_dianeAI.SlashCooldown)
            {
                m_dianeAI.currentSlashCooldown = 0f;
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
