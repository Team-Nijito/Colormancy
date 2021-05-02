using System.Collections;
using UnityEngine;

public class DianeSlash : State
{
    DianeAI m_dianeAI;
    public DianeSlash(BossAI bossAI) : base(bossAI)
    {
        m_dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Slash State");
        BossAI.Movement.FaceTarget(BossAI.DirectionToTarget());
        BossAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Slash");
        BossAI.SetState(new DianeChase(BossAI));
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        m_dianeAI.currentIdleCooldown = 0f;
        return base.Stop();
    }
}