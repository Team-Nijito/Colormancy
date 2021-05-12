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
        Debug.Log("Shank State");
        m_HelenAI.Movement.FaceTarget(m_HelenAI.DirectionToTarget());
        m_HelenAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Shank");
        m_HelenAI.SetState(new HelenChase(BossAI));
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        m_HelenAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}