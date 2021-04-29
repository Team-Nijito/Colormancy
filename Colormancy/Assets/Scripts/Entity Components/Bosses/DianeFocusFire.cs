using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DianeFocusFire : State
{
    DianeAI dianeAI;
    public DianeFocusFire(BossAI bossAI) : base(bossAI)
    {
        dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Focus Fire State");
        BossAI.StatusEffect.RPCApplyForce("Knockback", 1, BossAI.DirectionToTarget() + Vector3.up, 30f);
        BossAI.SetState(new DianeChase(BossAI));
        return base.Start();
    }

    public override IEnumerator Update()
    {

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        return base.Stop();
    }
}