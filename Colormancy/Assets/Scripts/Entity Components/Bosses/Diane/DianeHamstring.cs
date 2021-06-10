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

        BossAI.StatusEffect.RPCApplyForce(1f, "Knockback", BossAI.DirectionToTarget(), 25f);
        BossAI.SetAnimationTrigger("Hamstring");

        m_dianeAI.StartCoroutine(StopHamstring());

        return base.Start();
    }

    IEnumerator StopHamstring()
    {
        yield return new WaitForSeconds(1.5f);
        m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, DianeAI.States.Chase);
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
