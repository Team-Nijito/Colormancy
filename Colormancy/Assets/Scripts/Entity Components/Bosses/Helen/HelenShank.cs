using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

class HelenShank : State
{
    HelenAI m_HelenAI;
    public HelenShank(BossAI bossAI) : base(bossAI)
    {
        m_HelenAI = (HelenAI)BossAI;
    }

    public override IEnumerator Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (m_HelenAI.DebugMode)
                Debug.Log("Start Shank State");
            m_HelenAI.Movement.FaceTarget(m_HelenAI.DirectionToTarget());
            m_HelenAI.photonView.RPC("SetAnimationTrigger", Photon.Pun.RpcTarget.All, "Shank");
            m_HelenAI.photonView.RPC("SetHelenState", Photon.Pun.RpcTarget.AllViaServer, HelenAI.States.Chase);
        }
        return base.Start();
    }

    public override IEnumerator Stop()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (m_HelenAI.DebugMode)
                Debug.Log("Stop Shank State");
        }
        return base.Stop();
    }
}