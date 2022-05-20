using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelBossPodium : Podium
{


    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            manager.PopUp(messages, images, true);
            manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.SummonBoss);
        }
    }
}
