using System.Collections;
using UnityEngine;

public class DianeSlash : State
{
    DianeAI dianeAI;
    public DianeSlash(BossAI bossAI) : base(bossAI)
    {
        dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Slash State");
        BossAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Slash");
        BossAI.SetState(new DianeChase(BossAI));
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        return base.Stop();
    }
}