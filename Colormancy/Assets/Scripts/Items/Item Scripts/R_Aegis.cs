using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class R_Aegis : Item
{
    float timeToActivate = 4f;
    float currentCooldown;

    bool hasAegis = false;

    public override void Init(GameObject playerGO)
    {
        base.Init(playerGO);
        Types[0] = ItemTypes.DamageTaken;
        currentCooldown = timeToActivate;
    }

    public override void AddItemEffects(ItemManager manager)
    {
    }

    public override void RemoveItemEffects()
    {
    }

    public override float OnTakeDamage(float damageValue)
    {
        currentCooldown = timeToActivate;

        if (hasAegis)
        {
            hasAegis = false;
            //Disables graphic
            PlayerPhotonView.RPC("DisableAegis", Photon.Pun.RpcTarget.All);
            return 0;
        }else
        {
            return damageValue;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!hasAegis)
        {
            if (currentCooldown > 0)
            {
                currentCooldown -= Time.deltaTime;
            }else
            {
                hasAegis = true;
                //Enables graphic
                PlayerPhotonView.RPC("EnableAegis", Photon.Pun.RpcTarget.All);
            }
        }
    }
}
