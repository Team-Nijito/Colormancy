using ExitGames.Client.Photon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
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

    SpellTest test;
    Stack<Orb> currentSpellOrbs = new Stack<Orb>();

    public Orb FirstOrb { get; private set; }

    public Spell AddOrb(Orb orb)
    {
        if (FirstOrb != null)
        {
            FirstOrb.RevertHeldEffect(test);
        }

        FirstOrb = orb;
        currentSpellOrbs.Push(orb);

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
    }

    public bool TestCreateSpell(out Spell spell)
    {
        if (currentSpellOrbs.Count >= 3)
        {
            Orb[] spellOrbs = new Orb[] { currentSpellOrbs.Pop(), currentSpellOrbs.Pop(), currentSpellOrbs.Pop() };
            currentSpellOrbs.Push(spellOrbs[2]);
            currentSpellOrbs.Push(spellOrbs[1]);
            currentSpellOrbs.Push(spellOrbs[0]);

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

    public void ClearOrbs()
    {
        currentSpellOrbs.Clear();
    }
}

