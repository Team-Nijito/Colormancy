using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DianeHamstring : State
{
    DianeAI m_dianeAI;
    public DianeHamstring(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Hamstring State");
        BossAI.StatusEffect.RPCApplyForce("Knockback", 1f, BossAI.DirectionToTarget(), 25f);
        BossAI.SetState(new DianeChase(BossAI));
        BossAI.SetAnimationTrigger("Hamstring");
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}
