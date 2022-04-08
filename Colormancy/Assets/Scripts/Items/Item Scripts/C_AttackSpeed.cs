using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_AttackSpeed : Item
{
    float attackSpeedAmount = 20f;

    public override void AddItemEffects(ItemManager manager)
    {
        PlayerAttack.AddAttackSpeedMultiplier(attackSpeedAmount);
    }

    public override void RemoveItemEffects()
    {
        PlayerAttack.AddAttackSpeedMultiplier(-attackSpeedAmount);
    }
}
