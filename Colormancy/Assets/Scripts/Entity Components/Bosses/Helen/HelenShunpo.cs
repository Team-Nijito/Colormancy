using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class HelenShunpo : State
{
    HelenAI m_HelenAI;
    public HelenShunpo(BossAI bossAI) : base(bossAI)
    {
        m_HelenAI = (HelenAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Shunpo State");
        m_HelenAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Shunpo", true);
        Debug.Log("WAiting");
        m_HelenAI.StartCoroutine(WaitTime(1f));
        m_HelenAI.MeshAgent.Warp(m_HelenAI.Target.transform.position + (-m_HelenAI.Target.transform.forward * 2));
        m_HelenAI.Movement.FaceTarget(m_HelenAI.DirectionToTarget());
        m_HelenAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Shank");
        m_HelenAI.SetState(new HelenChase(BossAI));
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        m_HelenAI.currentIdleCooldown = 0f;
        m_HelenAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Shunpo", false);
        return base.Stop();
    }

    IEnumerator WaitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
    }
}