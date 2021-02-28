using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SpellManager : MonoBehaviourPun
{
    public struct Spell
    {
        static float BASE_SPELL_MANA = 33f;
        static float BASE_COOLDOWN = 10f;

        float SpellCooldown;
        float SpellManaCost;
        (Orb, Orb, Orb) OrbTuple;

        Dictionary<Orb.Element, int> orbDict;

        Orb[] orbs;


        public Spell(Orb[] _orbs)
        {
            orbDict = new Dictionary<Orb.Element, int>();
            foreach (Orb orb in _orbs)
            {
                if (orbDict.ContainsKey(orb.OrbElement))
                    orbDict[orb.OrbElement]++;
                else
                    orbDict[orb.OrbElement] = 1;
            }

            orbs = _orbs;
            
            SpellCooldown = BASE_COOLDOWN * orbs[2].CooldownMod;
            SpellManaCost = BASE_SPELL_MANA * orbs[2].ShapeManaMod;
            OrbTuple = (orbs[0], orbs[1], orbs[2]);
        }

        public void Cast(Transform t)
        {
            orbs[2].CastShape(orbs[0].CastGreaterEffect, orbs[1].CastLesserEffect, (orbDict[orbs[0].OrbElement], orbDict[orbs[1].OrbElement], orbDict[orbs[2].OrbElement]), t);
        }

        public float GetSpellCooldown()
        {
            return SpellCooldown;
        }

        public float GetManaCost()
        {
            return SpellManaCost;
        }

        public (Orb, Orb, Orb) GetOrbTuple()
        {
            return OrbTuple;
        }
    }

    OrbUIController controller;

    SpellTest test;
    List<Orb> currentSpellOrbs = new List<Orb>();

    public Orb FirstOrb { get; private set; }

    public Spell AddOrb(Orb orb)
    {
        if (FirstOrb != null)
        {
            FirstOrb.RevertHeldEffect(test);
        }
        
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            controller.AddOrb(orb);
        }
        
        FirstOrb = orb;
        currentSpellOrbs.Add(orb);

        FirstOrb.AddHeldEffect(test);

        if (TestCreateSpell(out Spell spell))
        {
            return spell;
        }
        else
        {
            return new Spell();
        }
    }

    private void Start()
    {
        test = GetComponent<SpellTest>();
        //if (photonView.IsMine)
        //{
            print("Looking for uiCOntorller");
            controller = FindObjectOfType<OrbUIController>();
        //}
    }

    public bool TestCreateSpell(out Spell spell)
    {
        print("Current orb count: " + currentSpellOrbs.Count);
        if (currentSpellOrbs.Count >= 3)
        {
            Orb[] spellOrbs = new Orb[] { currentSpellOrbs[currentSpellOrbs.Count - 1], currentSpellOrbs[currentSpellOrbs.Count - 2], currentSpellOrbs[currentSpellOrbs.Count - 3] };

            //Create and return new spell from orbs
            spell = new Spell(spellOrbs);
            return true;
        }
        else
        {
            spell = new Spell();
            return false;
        }
    }

    //public bool TryCreateSpell(out Spell spell)
    //{
    //    if (currentSpellOrbs.Count >= 3)
    //    {
    //        Orb[] spellOrbs = new Orb[] { currentSpellOrbs.Pop(), currentSpellOrbs.Pop(), currentSpellOrbs.Pop() };
            
    //        //Create and return new spell from orbs
    //        spell = new Spell(spellOrbs);
    //        currentSpellOrbs.Clear();
    //        return true;
    //    } else
    //    {
    //        spell = new Spell();
    //        return false;
    //    }
    //}
}

