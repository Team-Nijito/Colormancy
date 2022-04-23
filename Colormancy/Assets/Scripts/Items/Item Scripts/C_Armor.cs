using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_Armor : Item
{
    float armorAmount = 10f;

    public override void AddItemEffects(ItemManager manager)
    {
        PlayerPhotonView.RPC("AlterArmorValueAdditive", RpcTarget.All, armorAmount);
    }

    public override void RemoveItemEffects()
    {
        PlayerPhotonView.RPC("AlterArmorValueAdditive", RpcTarget.All, -armorAmount);
    }
}
