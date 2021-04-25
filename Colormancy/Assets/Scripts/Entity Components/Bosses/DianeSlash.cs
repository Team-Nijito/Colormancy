using System.Collections;
using UnityEngine;

public class DianeSlash : State
{
    public DianeSlash(BossAI bossAI) : base(bossAI)
    {
    }

    public override IEnumerator Start()
    {
        BossAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Slash");
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        return base.Stop();
    }
}