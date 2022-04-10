using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndigoSpellSphereController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;

    public bool PVPEnabled = false;
    public PhotonView CasterPView = null;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            greaterCast(collision.gameObject, spellEffectMod, null);
            Destroy(gameObject);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            if (PVPEnabled && PhotonView.Get(collision.gameObject).ViewID != CasterPView.ViewID)
            {
                greaterCast(collision.gameObject, spellEffectMod, null);
                Destroy(gameObject);
            }
            else
            {
                lesserCast(collision.gameObject, spellEffectMod, null);
            }
        }
        else if (!collision.gameObject.CompareTag("Projectile"))
        {
            Destroy(gameObject);
        }
    }
}
