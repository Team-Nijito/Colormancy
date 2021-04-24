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
        (System.Type, System.Type, System.Type) OrbTuple;

        Orb[] orbs;


        public Spell(Orb[] _orbs)
        {
            orbs = _orbs;
            
            SpellCooldown = BASE_COOLDOWN * orbs[2].getCooldownMod();
            SpellManaCost = BASE_SPELL_MANA * orbs[2].getShapeManaMod();
            OrbTuple = (orbs[0].GetType(), orbs[1].GetType(), orbs[2].GetType());
        }

        public void Cast(Transform t, Vector3 clickedPosition)
        {
            orbs[2].CastShape(orbs[0].CastGreaterEffect, orbs[1].CastLesserEffect, (orbs[0].getLevel(), orbs[1].getLevel(), orbs[2].getLevel()), t, clickedPosition);
        }

        public float GetSpellCooldown()
        {
            return SpellCooldown;
        }

        public float GetManaCost()
        {
            return SpellManaCost;
        }

        public (System.Type, System.Type, System.Type) GetOrbTuple()
        {
            return OrbTuple;
        }
    }

    OrbUIController uiController;

    OrbManager orbManager;
    List<Orb> currentSpellOrbs = new List<Orb>();

    public Orb FirstOrb { get; private set; }

    public Spell AddOrb(Orb orb)
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            FirstOrb = orb;

            uiController.AddOrb(orb);
        }
        
        currentSpellOrbs.Add(orb);


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
        orbManager = GetComponent<OrbManager>();

        Initialization();
    }

    public bool TestCreateSpell(out Spell spell)
    {
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

    /// <summary>
    /// Call this whenever we load into any new scene.
    /// </summary>
    public void Initialization()
    {
        //print("Looking for uiController");
        uiController = FindObjectOfType<OrbUIController>();
    }
}

