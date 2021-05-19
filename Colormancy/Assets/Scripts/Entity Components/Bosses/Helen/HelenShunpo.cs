using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

class HelenShunpo : State
{
    HelenAI m_HelenAI;
    public HelenShunpo(BossAI bossAI) : base(bossAI)
    {
        m_HelenAI = (HelenAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (m_HelenAI.DebugMode)
                Debug.Log("Start Shunpo State");

            m_HelenAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Shunpo", true);
            m_HelenAI.StartCoroutine(WaitTime(.25f));
        }
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (m_HelenAI.DebugMode)
                Debug.Log("Stop Shunpo State");

            m_HelenAI.photonView.RPC("SetAnimationBool", Photon.Pun.RpcTarget.All, "Shunpo", false);
        }
        return base.Stop();
    }

    IEnumerator WaitTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        m_HelenAI.MeshAgent.Warp(m_HelenAI.Target.transform.position + (-m_HelenAI.Target.transform.forward * 2));
        m_HelenAI.Movement.FaceTarget(m_HelenAI.DirectionToTarget());
        m_HelenAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Shank");
        m_HelenAI.photonView.RPC("SetHelenState", Photon.Pun.RpcTarget.AllViaServer, HelenAI.States.Chase);
    }
}