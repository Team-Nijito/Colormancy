using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class State
{
    protected BossAI BossAI;

    public State(BossAI bossAI)
    {
        BossAI = bossAI;
    }

    public virtual IEnumerator Start()
    {
        yield break;
    }

    public virtual IEnumerator Stop()
    {
        yield break;
    }

    public virtual IEnumerator Update()
    {
        yield break;
    }
}
