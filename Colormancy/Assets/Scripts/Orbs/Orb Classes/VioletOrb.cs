﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class VioletOrb : Orb
{
    public VioletOrb()
    {
        m_OrbShape = SpellShape.Cloud;
        m_OrbElement = Element.Poison;
        m_UIPrefab = (GameObject)Resources.Load("Orbs/VioletOrbUI");
    }

    public override void CastGreaterEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        StatusEffectScript status = hit.GetComponent<StatusEffectScript>();
        // currently stacks, should not
        status.RPCApplyOrStackDoT(true, OrbValueManager.getGreaterEffectDamage(m_OrbElement, m_Level) * spellEffectMod, OrbValueManager.getGreaterEffectDuration(m_OrbElement, m_Level), "Poison");
        //status.RPCApplySlowdown("Slow", 10, orbLevel * 2 + 3);
    }

    public override void CastLesserEffect(GameObject hit, float spellEffectMod, float[] data)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, Transform t, Vector3 clickedPosition)
    {
        Vector3 direction = clickedPosition - t.position;

        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Violet Sphere"), t.position + direction.normalized, t.rotation) as GameObject;
        VioletSpellSphereController spellController = g.GetComponent<VioletSpellSphereController>();

        spellController.greaterCast = greaterEffectMethod;
        spellController.lesserCast = lesserEffectMethod;
        spellController.spellEffectMod = OrbValueManager.getShapeEffectMod(m_OrbElement);

        spellController.endPosition = clickedPosition;
    }

    public static object Deserialize(byte[] data) {
        VioletOrb result = new VioletOrb();
        result.setLevel(data[0]);
        return result;
    }

    public static byte[] Serialize(object customType) {
        VioletOrb o = (VioletOrb)customType;
        return new byte[] { (byte)o.getLevel() };
    }

}
