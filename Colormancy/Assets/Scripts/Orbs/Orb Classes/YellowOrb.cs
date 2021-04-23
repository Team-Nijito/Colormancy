using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class YellowOrb : Orb
{
    public YellowOrb()
    {
        m_OrbElement = Element.Light;
        m_OrbShape = SpellShape.OrbitingOrbs;
        m_UIPrefab = (GameObject) Resources.Load("Orbs/YellowOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level));

        //missing 20% less damage
    }

    public override void CastLesserEffect(GameObject hit, int orbLevel, float spellEffectMod)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) levels, Transform t, Vector3 clickedPosition)
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Yellow Orbs"), t.position, t.rotation) as GameObject;
        YellowSpellController spellController = g.GetComponent<YellowSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastLevel = levels.Item1;
        spellController.lesserCastLevel = levels.Item2;
        spellController.spellEffectMod = OrbValueManager.getSpellEffectMod(m_OrbElement);

        spellController.playerTransform = t;

        for (int i = 0; i < 3; i++)
        {
            if (i - levels.Item3 >= 0)
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
        }
    }

    public static object Deserialize(byte[] data)
    {
        YellowOrb result = new YellowOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        YellowOrb o = (YellowOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }
}
