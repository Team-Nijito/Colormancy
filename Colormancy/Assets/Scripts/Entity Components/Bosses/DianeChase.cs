using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DianeChase : State
{
    EnemyMovement EnemyMovement;

    public DianeChase(BossAI bossAI) : base(bossAI)
    {
    }

    public override IEnumerator Start()
    {
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", true);
        EnemyMovement = BossAI.GetComponent<EnemyMovement>();
        return base.Start();
    }

    public override IEnumerator Update()
    {
        BossAI.photonView.RPC("SetDestination", Photon.Pun.RpcTarget.All, BossAI.Target.transform.position);
        BossAI.transform.LookAt(BossAI.Target.transform);

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", false);
        return base.Stop();
    }
}
