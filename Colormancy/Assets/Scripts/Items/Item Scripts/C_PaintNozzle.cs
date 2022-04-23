using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C_PaintNozzle : Item
{
    float projectileSizeIncreasePercentage = 2f;

    public override void AddItemEffects(ItemManager manager)
    {
        PlayerProjectileSpawner.sizeModifier += projectileSizeIncreasePercentage;
    }

    public override void RemoveItemEffects()
    {
        PlayerProjectileSpawner.sizeModifier -= projectileSizeIncreasePercentage;
    }
}
