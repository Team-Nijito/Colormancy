using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class RedOrb : Orb
{
    public RedOrb()
    {
        OrbColor = Color.red;
        OrbShape = SpellShape.Jump;
        CooldownMod = 0.7f;
        OrbElement = Element.Wrath;
        ModAmount = .1f;
        UIPrefab = (GameObject)Resources.Load("Orbs/RedOrbUI");
    }

    public override void AddHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod += ModAmount;
    }

    public override void RevertHeldEffect(SpellTest test)
    {
        test.AttackSpeedMod -= ModAmount;
    }

    public override void CastGreaterEffect(GameObject hit, int orbAmount)
    {
        PhotonView photonView = hit.GetPhotonView();
        photonView.RPC("TakeDamage", RpcTarget.All, (float)orbAmount);
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
            PhotonView photonView = PhotonView.Get(t.gameObject);

            if (Vector3.Dot(hit.normal,Vector3.up) > 0.5)
            {
                Vector3 direction = hit.point - t.position;

                GameObject g = PhotonNetwork.Instantiate("Orbs/Red Area", t.position + Vector3.down, t.rotation);
                RedSpellController spellController = g.GetComponent<RedSpellController>();

                spellController.greaterCast = greaterEffectMethod;
                spellController.lesserCast = lesserEffectMethod;
                spellController.endPosition = hit.point + Vector3.up * 1.6f;
                spellController.playerTransform = t;
            }
        }
    }

    public static object Deserialize(byte[] data)
    {
        RedOrb result = new RedOrb();
        result.OrbColor = new Color(data[0], data[1], data[2]);
        result.CooldownMod = data[3];
        result.ShapeManaMod = data[4];
        result.ModAmount = data[5];
        return result;
    }

    public static byte[] Serialize(object customType)
    {
        RedOrb c = (RedOrb)customType;
        return new byte[] { (byte)c.OrbColor.r, (byte)c.OrbColor.g, (byte)c.OrbColor.b, (byte)c.CooldownMod, (byte)c.ShapeManaMod, (byte)c.ModAmount };
    }
}
