using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DianeFocusFire : State
{
    DianeAI m_dianeAI;
    public DianeFocusFire(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_dianeAI.DebugMode)
            Debug.Log("Start Focus Fire State");

        BossAI.StatusEffect.RPCApplyForce("Knockback", 2, BossAI.DirectionToTarget() + Vector3.up * 6, 30f);
        m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, DianeAI.States.Chase);
        BossAI.SetAnimationTrigger("Focus Fire");
        return base.Start();
    }

    public override IEnumerator Update()
    {

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_dianeAI.DebugMode)
            Debug.Log("Stop Focus Fire State");

        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}