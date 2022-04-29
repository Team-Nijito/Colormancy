using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_WeaversWay : Item
{
    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        Types[0] = ItemTypes.DamageMultiplier;
        Types.Add(ItemTypes.SpellCast);
        MaxItemAmount = 1;
    }

    public override void AddItemEffects(ItemManager manager)
    {
    }

    public override void RemoveItemEffects()
    {
    }

    public override float DoDamageMultiplier(float baseDamageMultiplier)
    {
        return 0;
    }

    public override void OnSpellCast()
    {
        PlayerPhotonView.RPC("ReduceAllCooldowns", Photon.Pun.RpcTarget.All, 10);
    }
}
