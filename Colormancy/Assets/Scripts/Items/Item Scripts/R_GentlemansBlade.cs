using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_GentlemansBlade : Item
{
    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        MaxItemAmount = 1;
        KeyToPress = KeyCode.Tab;
        Types[0] = ItemTypes.OnKeyPressed;
    }

    public override void AddItemEffects(ItemManager manager)
    {
    }

    public override void RemoveItemEffects()
    {
    }

    public override void OnKeyPressed()
    {
        PlayerMouse.ToggleBladeAttack();
    }
}
