using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_SUPrincipal : Item
{
    GameObject lastAutoAttack;

    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        MaxItemAmount = 1;
        KeyToPress = KeyCode.Space;
        Types[0] = ItemTypes.OnKeyPressed;
        Types.Add(ItemTypes.OnAutoAttack);
    }

    public override void AddItemEffects(ItemManager manager)
    {
    }

    public override void RemoveItemEffects()
    {
    }

    public override void OnAutoAttack(GameObject shotAuto)
    {
        lastAutoAttack = shotAuto;
    }

    public override void OnKeyPressed()
    {
        if (lastAutoAttack != null)
        {
            PlayerMovement.m_controller.enabled = false;
            PlayerPhotonView.gameObject.transform.position = lastAutoAttack.transform.position;
            PlayerMovement.m_controller.enabled = true;
        }
        else
        {
            Debug.LogError("SUPrincipal::OnKeyPressed No auto attack referenced");
        }
    }
}
