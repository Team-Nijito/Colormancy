using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class DianeChase : State
{
    EnemyMovement EnemyMovement;

    DianeAI dianeAI;

    public DianeChase(BossAI bossAI) : base(bossAI)
    {
        dianeAI = (DianeAI)BossAI;
    }

    public override IEnumerator Start()
    {
        Debug.Log("Chase State");
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", true);
        EnemyMovement = BossAI.GetComponent<EnemyMovement>();
        return base.Start();
    }

    public override IEnumerator Update()
    {
        BossAI.photonView.RPC("SetDestination", Photon.Pun.RpcTarget.All, BossAI.Target.transform.position);
        BossAI.transform.LookAt(BossAI.Target.transform);

        if (dianeAI.currentHamstringCooldown >= dianeAI.HamstringCooldown)
        {
            dianeAI.currentHamstringCooldown = 0f;
            BossAI.SetState(new DianeHamstring(BossAI));
            BossAI.SetTarget(null);
        }

        if (BossAI.InRangeOfTarget(dianeAI.SlashRange) && dianeAI.currentSlashCooldown >= dianeAI.SlashCooldown)
        {
            dianeAI.currentSlashCooldown = 0f;
            BossAI.SetState(new DianeSlash(BossAI));
            BossAI.SetTarget(null);
        }

        if (dianeAI.currentFocusFireCooldown >= dianeAI.FocusFireCooldown)
        {
            dianeAI.currentFocusFireCooldown = 0f;
            BossAI.SetState(new DianeFocusFire(BossAI));
            BossAI.SetTarget(null);
        }

        return base.Update();
    }

    public override IEnumerator Stop()
    {
        BossAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Chasing", false);
        return base.Stop();
    }
}
