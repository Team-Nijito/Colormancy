using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_TargetAnalyzer : Item
{
    float doubleDamageChance = .15f;

    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        Types[0] = ItemTypes.DamageMultiplier;
    }

    public override void AddItemEffects(ItemManager manager)
    {
    }

    public override void RemoveItemEffects()
    {
    }

    public override float DoDamageMultiplier(float baseDamageMultiplier)
    {
        float randomNumber = Random.value;

        if (randomNumber < doubleDamageChance)
        {
            Debug.Log("WAGWAN");
            baseDamageMultiplier += 1;
        }

        return baseDamageMultiplier;
    }
}
