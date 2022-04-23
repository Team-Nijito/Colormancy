using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PocketWatch : Item
{
    float cooldownModifierAmount = 10f;

    public override void AddItemEffects(ItemManager manager)
    {
        PlayerOrbManager.AddSpellCooldownModifier(cooldownModifierAmount);
    }

    public override void RemoveItemEffects()
    {
        PlayerOrbManager.AddSpellCooldownModifier(-cooldownModifierAmount);
    }
}
