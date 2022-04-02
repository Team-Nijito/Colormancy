using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementAugment : Item
{
    private void Start()
    {
        itemWeight = 2f;
    }

    public override void AddItemEffects()
    {
        playerMovement.AlterWalkSpeed(playerMovement.WalkSpeed + 5);
    }

    public override void RemoveItemEffects()
    {
        playerMovement.AlterWalkSpeed(playerMovement.WalkSpeed - 5);
    }
}
