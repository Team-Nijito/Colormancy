using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DianeIdle : State
{
    DianeAI dianeAI;
    public DianeIdle(BossAI bossAI) : base(bossAI)
    {
        dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Idle State");
        dianeAI.currentIdleCooldown = 0f;
        return base.Start();
    }

    public override IEnumerator Update()
    {
        if (dianeAI.currentIdleCooldown >= dianeAI.IdleCooldown)
        {
            dianeAI.SetState(new DianeChase(BossAI));
        }
        return base.Update();
    }

    public override IEnumerator Stop()
    {
        return base.Stop();
    }
}
