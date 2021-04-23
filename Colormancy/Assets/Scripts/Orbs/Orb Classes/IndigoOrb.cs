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

    public override void CastGreaterEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level));

        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        status.RPCApplyBlind(orbLevel);
    }

    public override void CastLesserEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) levels, Transform t, Vector3 clickedPosition)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Indigo Orbs", typeof(GameObject))) as GameObject;
        g.transform.position = t.position;

        for (int i = 0; i < 16; i++)
        {
            IndigoSpellSphereController sphereController = g.transform.GetChild(i).GetComponent<IndigoSpellSphereController>();
            sphereController.greaterCast = greaterEffectMethod;
            sphereController.lesserCast = lesserEffectMethod;
            sphereController.greaterCastLevel = levels.Item1;
            sphereController.lesserCastLevel = levels.Item2;
            sphereController.spellEffectMod = OrbValueManager.getSpellEffectMod(m_OrbElement);

            if ((i % 2 == 1 && levels.Item3 == 2) || (i % 4 != 0 && levels.Item3 == 1))
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
