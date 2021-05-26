using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class IndigoOrb : Orb
{
    public IndigoOrb()
    {
        m_OrbShape = SpellShape.ExpandingOrbs;
        m_OrbElement = Element.Darkness;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/IndigoOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        float dmgMultiplier = 1;
        if (hit.GetComponent<StatusEffectScript>().StatusExists(StatusEffect.StatusType.SpellIncreasedDamage))
            dmgMultiplier += OrbValueManager.getGreaterEffectPercentile(Element.Water) / 100f;

        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod * dmgMultiplier);

        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyStatus(StatusEffect.StatusType.Blind, OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level), 0, OrbValueManager.getGreaterEffectPercentile(m_OrbElement));
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Indigo Orbs", typeof(GameObject))) as GameObject;
        g.transform.position = t.position;

        for (int i = 0; i < 16; i++)
        {
            IndigoSpellSphereController sphereController = g.transform.GetChild(i).GetComponent<IndigoSpellSphereController>();
            sphereController.greaterCast = greaterEffectMethod;
            sphereController.lesserCast = lesserEffectMethod;
            sphereController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement);

            if ((i % 2 == 1 && m_Level == 2) || (i % 4 != 0 && m_Level == 1))
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
        }

        IndigoSpellController spellController = g.GetComponent<IndigoSpellController>();
        spellController.playerTransform = t;
    }

    public static object Deserialize(byte[] data)
    {
        IndigoOrb result = new IndigoOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        IndigoOrb o = (IndigoOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
