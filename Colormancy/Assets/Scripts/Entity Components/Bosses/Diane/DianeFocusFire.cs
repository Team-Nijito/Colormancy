using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DianeFocusFire : State
{
    DianeAI m_dianeAI;
    public DianeFocusFire(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Focus Fire State");
        BossAI.StatusEffect.RPCApplyForce(2, "Knockback", BossAI.DirectionToTarget() + Vector3.up * 6, 30f);
        BossAI.SetState(new DianeChase(BossAI));
        BossAI.SetAnimationTrigger("Focus Fire");
        return base.Start();
    }

    public override IEnumerator Update()
    {

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}