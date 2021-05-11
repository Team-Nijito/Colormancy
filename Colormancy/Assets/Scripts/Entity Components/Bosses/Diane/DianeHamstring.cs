using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class DianeHamstring : State
{
    DianeAI m_dianeAI;
    public DianeHamstring(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();
        Debug.Log("Hamstring State");
        BossAI.StatusEffect.RPCApplyForce("Knockback", 1f, BossAI.DirectionToTarget(), 25f);
        m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, "Chase");
        BossAI.SetAnimationTrigger("Hamstring");
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();
        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}
