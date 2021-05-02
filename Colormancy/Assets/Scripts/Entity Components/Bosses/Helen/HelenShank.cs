using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class HelenShank : State
{
    HelenAI m_HelenAI;
    public HelenShank(BossAI bossAI) : base(bossAI)
    {
        m_HelenAI = (HelenAI)BossAI;
    }

    public override IEnumerator Start()
    {

        return base.Start();
    }

    public override IEnumerator Stop()
    {
        m_HelenAI.currentShankCooldown = 0f;
        return base.Stop();
    }
}