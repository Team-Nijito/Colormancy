using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public abstract class StateMachine : MonoBehaviourPunCallbacks
{
    protected State State;

    public void SetState(State state)
    {
        if (State != null)
        {
            StartCoroutine(State.Stop());
        }
        State = state;
        StartCoroutine(State.Start());
    }
}
