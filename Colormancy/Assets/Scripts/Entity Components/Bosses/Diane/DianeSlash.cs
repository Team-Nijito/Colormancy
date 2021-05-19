using System.Collections;
using UnityEngine;
using Photon.Pun;

public class DianeSlash : State
{
    DianeAI m_dianeAI;
    public DianeSlash(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_dianeAI.DebugMode)
            Debug.Log("Start Slash State");

        BossAI.Movement.FaceTarget(BossAI.DirectionToTarget());
        BossAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Slash");
        m_dianeAI.photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, DianeAI.States.Chase);
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        if (!PhotonNetwork.IsMasterClient) return base.Start();

        if (m_dianeAI.DebugMode)
            Debug.Log("Stop Slash State");

        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}