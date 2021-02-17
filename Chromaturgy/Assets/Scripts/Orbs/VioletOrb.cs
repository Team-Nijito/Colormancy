using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletOrb : Orb
{
    public VioletOrb()
    {
        OrbColor = Color.yellow;
        OrbShape = SpellShape.OrbitingOrbs;
        CooldownMod = 1.3f;
        OrbElement = Element.Light;
        ModAmount = .1f;
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.HealthRegenMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.HealthRegenMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void CastLesserEffect(GameObject hit, int orbAmount)
    {
        throw new System.NotImplementedException();
    }

    public override void CastShape(GreaterCast greaterEffectMethod, LesserCast lesserEffectMethod, (int, int, int) amounts, Transform t)
    {
        //Cast specific orb shape depending on shapeAmnt
        //For any enemies hit
        //greaterEffectMethod(enemy game object, greaterEffectAmnt);
        //For any allies hit 
        //lesserEffectMethod(ally game object, lesserEffectAmnt);
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, 1 << PaintingManager.paintingMask))
        {
            GameObject orbs = GameObject.Instantiate(Resources.Load("Orbs/Violet Cloud", typeof(GameObject)), hit.point + Vector3.up, t.rotation) as GameObject;
        }

    }

}
