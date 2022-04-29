using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_SplatJacket : Item
{
    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        Types[0] = ItemTypes.DamageTaken;
    }

    public override void AddItemEffects(ItemManager manager)
    {
    }

    public override void RemoveItemEffects()
    {

    }

    public override float OnTakeDamage(float damageValue)
    {
        var v = Enum.GetValues(typeof(Orb.Element));
        System.Random r = new System.Random();
        Orb.Element randomElement = (Orb.Element)v.GetValue(r.Next(v.Length));
        PaintingManager.PaintSphere(OrbValueManager.getColor(randomElement), transform.position, 3.5f);
        return damageValue;
    }
}
