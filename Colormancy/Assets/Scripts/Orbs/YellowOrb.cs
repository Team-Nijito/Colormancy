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

    public override void CastGreaterEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        PhotonView photonView = PhotonView.Get(hit);
        photonView.RPC("TakeDamage", RpcTarget.All, orbAmount * 20f * spellEffectMod);

        //missing 20% less damage
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount, float spellEffectMod)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t, Vector3 clickedPosition)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Yellow Orbs"), t.position, t.rotation) as GameObject;
        YellowSpellController spellController = g.GetComponent<YellowSpellController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.greaterCastAmt = amounts.Item1;
        spellController.lesserCastAmt = amounts.Item2;
        spellController.spellEffectMod = m_SpellEffectMod;

        spellController.playerTransform = t;

        for (int i = 0; i < 3; i++)
        {
            if (i - amounts.Item3 >= 0)
                GameObject.Destroy(g.transform.GetChild(i).gameObject);
        }

        Debug.Log(amounts.Item3);
    }

    public static object Deserialize(byte[] data)
    {
        YellowOrb result = new YellowOrb();
        result.setColor(new Color(data[0], data[1], data[2]));
        result.setCooldownMod(data[3]);
        result.setShapeManaMod(data[4]);
        result.setSpellEffectMod(data[5]);
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        YellowOrb o = (YellowOrb)customType;
        return new byte[] { (byte)o.getColor().r, (byte)o.getColor().g, (byte)o.getColor().b, (byte)o.getCooldownMod(), (byte)o.getShapeManaMod(), (byte)o.getSpellEffectMod() };
    }
}
