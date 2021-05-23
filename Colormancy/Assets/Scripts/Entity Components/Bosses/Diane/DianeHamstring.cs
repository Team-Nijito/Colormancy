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

        if (m_dianeAI.DebugMode)
            Debug.Log("Start Hamstring State");

        Debug.Log("Hamstring State");
        BossAI.StatusEffect.RPCApplyForce(1f, "Knockback", BossAI.DirectionToTarget(), 25f);
        m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, DianeAI.States.Chase);
        BossAI.SetAnimationTrigger("Hamstring");
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Stop();

        if (m_dianeAI.DebugMode)
            Debug.Log("Stop Hamstring State");

        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}
