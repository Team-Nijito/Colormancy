using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System;

public class OrbInfo
{
    public Color color;
    public int spellShape;
    public int spellElement;
    public float cooldownMod;
    public float manaMod;
}

public class SpellTest : MonoBehaviourPun
{
    public static List<Orb> orbHistory = new List<Orb>(); // static variable that keep tracks of orbs in between scenes

    List<Orb> orbs = new List<Orb>();

    SpellManager manager;
    ManaScript mana;
    OrbTrayUIController uIController;

    public bool TestingMode = false;

    [SerializeField]
    Dictionary<(System.Type, System.Type, System.Type), float> spellCooldowns = new Dictionary<(System.Type, System.Type, System.Type), float>();

    SpellManager.Spell currentSpell;

    #region Dummy Player Attributes

    static readonly float BASE_ATTACK_SPEED = 1f;
    static readonly float BASE_HEALTH_REGEN = 1f;

    float attackSpeed = BASE_ATTACK_SPEED;
    float healthRegen = BASE_HEALTH_REGEN;

    float _attackSpeedMod = 1f;
    public float AttackSpeedMod
    {
        get => _attackSpeedMod;
        set
        {
            _attackSpeedMod = value;
            attackSpeed = BASE_ATTACK_SPEED * _attackSpeedMod;
            print("Current attack speed: " + attackSpeed);
        }
    }

    float _healthRegenMod = 1f;
    public float HealthRegenMod
    {
        get => _healthRegenMod;
        set
        {
            _healthRegenMod = value;
            healthRegen = BASE_HEALTH_REGEN * _healthRegenMod;
            print("Current health regen: " + healthRegen);
        }
    }

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SpellManager>();
        mana = GetComponent<ManaScript>();

        Initialization();
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
            GetSpellInput();
    }

    void GetSpellInput()
    {
        var input = Input.inputString;
        if (!string.IsNullOrEmpty(input))
        {
            if (int.TryParse(input, out int number))
            {
                number = number - 1; //used for indexing now
                if (number < orbs.Count && number >= 0)
                {
                    photonView.RPC("AddOrb", RpcTarget.All, orbs[number]);
                }
            }
        }

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, 100f, 1 << PaintingManager.paintingMask)) { }
            
            photonView.RPC("TryCastSpell", RpcTarget.All, hit.point);
        }
    }

    public void AddSpellOrb(Orb orb, bool addToOrbHistory = false)
    {
        orbs.Add(orb);
        if (addToOrbHistory)
        {
            orbHistory.Add(orb); // sync across scenes
        }
        if (uIController)
            uIController.AddOrb(orb);
    }
    
    [PunRPC]
    void AddOrb(Orb orb)
    {
        currentSpell = manager.AddOrb(orb);
    }

    [PunRPC]
    void TryCastSpell(Vector3 clickedPosition)
    {
        if (mana.GetEffectiveMana() >= currentSpell.GetManaCost() && !currentSpell.Equals(new SpellManager.Spell()))
        {
            if (spellCooldowns.ContainsKey(currentSpell.GetOrbTuple()))
            {
                if (spellCooldowns[currentSpell.GetOrbTuple()] <= Time.time)
                {
                    CastSpell(clickedPosition);
                }
            }
            else
            {
                CastSpell(clickedPosition);
            }
        }
    }
    
    void CastSpell(Vector3 clickedPosition)
    {
        currentSpell.Cast(transform, clickedPosition);
        spellCooldowns[currentSpell.GetOrbTuple()] = Time.time + currentSpell.GetSpellCooldown();
        mana.ConsumeMana(currentSpell.GetManaCost());
    }

    private void UpdateOrbsFromPreviousScene()
    {
        // update the player's orbs from the previous scene
        foreach (Orb o in orbHistory.ToArray())
        {
            switch (o.OrbElement)
            {
                case Orb.Element.Wrath:
                    AddSpellOrb(new RedOrb());
                    break;
                case Orb.Element.Fire:
                    AddSpellOrb(new OrangeOrb());
                    break;
                case Orb.Element.Light:
                    AddSpellOrb(new YellowOrb());
                    break;
                case Orb.Element.Nature:
                    //AddSpellOrb(new GreenOrb());
                    throw new NotImplementedException("Didn't implement adding " + o.OrbElement + " yet");
                case Orb.Element.Water:
                    AddSpellOrb(new BlueOrb());
                    break;
                case Orb.Element.Poison:
                    AddSpellOrb(new VioletOrb());
                    break;
                case Orb.Element.Earth:
                    //AddSpellOrb(new BrownOrb());
                    throw new NotImplementedException("Didn't implement adding " + o.OrbElement + " yet");
                case Orb.Element.Wind:
                    //AddSpellOrb(new QuickSilverOrb());
                    throw new NotImplementedException("Didn't implement adding " + o.OrbElement + " yet");
                case Orb.Element.Darkness:
                    AddSpellOrb(new IndigoOrb());
                    break;
                default:
                    throw new NotImplementedException("Didn't implement adding " + o.OrbElement + " yet");
            }
        }
    }

    /// <summary>
    /// Call this function upon loading a new scene from an existing scene
    /// To reassign the references.
    /// </summary>
    public void Initialization()
    {
        GameObject gUIController = null;
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            gUIController = GameObject.Find("OrbTray");
        }

        if (gUIController)
        {
            uIController = gUIController.GetComponent<OrbTrayUIController>();
        }

        if (TestingMode)
        {
            AddSpellOrb(new YellowOrb());
            AddSpellOrb(new OrangeOrb());
            AddSpellOrb(new BlueOrb());
            AddSpellOrb(new VioletOrb());
            AddSpellOrb(new IndigoOrb());
        }
        else
        {
            UpdateOrbsFromPreviousScene();
        }
    }

    /// <summary>
    /// Clear out all current spells.
    /// (although this will not update the UI).
    /// </summary>
    public void ResetSpells()
    {
        orbHistory.Clear();
        orbs.Clear();
    }
}
