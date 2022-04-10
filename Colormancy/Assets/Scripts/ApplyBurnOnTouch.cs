using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplyBurnOnTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        bool isPlayer = other.gameObject.CompareTag("Player");
        bool isEnemy = other.gameObject.layer == LayerMask.NameToLayer("Enemy");

        print(isPlayer);

        if (isPlayer || isEnemy)
        {
            StatusEffectScript statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();

            if (PhotonNetwork.InRoom)
            {
                // online - we launched from the launcher and then joined the room
                statEffectScript.RPCApplyStatus(StatusEffect.StatusType.DamageOverTime, 3, 0.25f, 2, "Burn");
            }
            else
            {
                // offline testing (you've clicked play on the current scene that's not the launcher) is currently not supported
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        bool isPlayer = other.gameObject.CompareTag("Player");
        bool isEnemy = other.gameObject.layer == LayerMask.NameToLayer("Enemy");

        print(isPlayer);

        if (isPlayer || isEnemy)
        {
            StatusEffectScript statEffectScript = other.gameObject.GetComponent<StatusEffectScript>();

            if (PhotonNetwork.InRoom)
            {
                statEffectScript.RPCApplyStatus(StatusEffect.StatusType.DamageOverTime, 3, 0.25f, 2, "Burn");
            }
            else
            {
                // offline testing (you've clicked play on the current scene that's not the launcher) is currently not supported
            }
        }
    }
}
