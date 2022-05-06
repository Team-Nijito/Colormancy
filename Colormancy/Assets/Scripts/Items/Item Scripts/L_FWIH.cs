using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class L_FWIH : Item
{
    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        MaxItemAmount = 1;
        Types[0] = ItemTypes.DamageMultiplier;
        Types.Add(ItemTypes.DamageDealt);
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

    public override void OnDamageDealt(GameObject hit, Transform sourcePosition)
    {
        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        Vector3 launchVector = hit.transform.position - sourcePosition.position;
        launchVector.y = 0;
        status?.RPCApplyForce(1f, "red_orb", launchVector, 40f);

    }
}
