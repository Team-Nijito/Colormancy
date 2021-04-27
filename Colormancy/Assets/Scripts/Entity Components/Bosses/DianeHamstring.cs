using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DianeHamstring : State
{
    DianeAI dianeAI;
    public DianeHamstring(BossAI bossAI) : base(bossAI)
    {
        dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Hamstring State");
        BossAI.StatusEffect.RPCApplyForce("Knockback", 1, BossAI.DirectionToTarget(), 60f);
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
