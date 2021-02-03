using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellManager : MonoBehaviour
{
    public struct Spell
    {
        static float BASE_COOLDOWN = 10f;

        Orb[] orbs;
        Dictionary<Orb.Element, int> orbDict;
        
        public float SpellCooldown { get; private set; }

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
        }

        public void Cast(Transform t)
        {
            orbs[2].CastShape(orbs[0].CastGreaterEffect, orbs[1].CastLesserEffect, (orbDict[orbs[0].OrbElement], orbDict[orbs[1].OrbElement], orbDict[orbs[2].OrbElement]), t);
        }
    }

    SpellTest test;
    Stack<Orb> currentSpellOrbs = new Stack<Orb>();

    public Orb FirstOrb { get; private set; }

    public void AddOrb(Orb orb)
    {
        if (FirstOrb != null)
        {
            FirstOrb.RevertHeldEffect(test);
        }

        FirstOrb = orb;
        currentSpellOrbs.Push(orb);

        FirstOrb.AddHeldEffect(test);
        print("First orb now " + orb);
    }

    private void Start()
    {
        test = GetComponent<SpellTest>();
    }

    public bool TryCreateSpell(out Spell spell)
    {
        if (currentSpellOrbs.Count >= 3)
        {
            Orb[] spellOrbs = new Orb[] { currentSpellOrbs.Pop(), currentSpellOrbs.Pop(), currentSpellOrbs.Pop() };
            
            //Create and return new spell from orbs
            Debug.Log(string.Format("Creating spell with orbs: {0}, {1}, {2}", spellOrbs[0], spellOrbs[1], spellOrbs[2]));
            spell = new Spell(spellOrbs);
            currentSpellOrbs.Clear();
            return true;
        } else
        {
            spell = new Spell();
            return false;
        }
    }
}

