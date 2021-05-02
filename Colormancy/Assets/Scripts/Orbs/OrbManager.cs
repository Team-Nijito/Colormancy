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

public class OrbManager : MonoBehaviourPun
{
    public static List<Orb> orbHistory = new List<Orb>(); // static variable that keep tracks of orbs in between scenes

    List<Orb> orbs = new List<Orb>();

    SpellManager manager;
    ManaScript mana;
    OrbTrayUIController uIController;

    [SerializeField]
    private bool TestingMode = false;

    [SerializeField]
    Dictionary<(Type, Type, Type), (float, float)> spellCooldowns = new Dictionary<(Type, Type, Type), (float, float)>(); // key: Orb Tuple, value: (current cooldown, Spell base cooldown)

    SpellManager.Spell currentSpell;

    PlayerMovement playerMoveScript; // need a ref to this component so we can check if we're stunned or not, so we'll prevent casting while stunned

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
        playerMoveScript = GetComponent<PlayerMovement>();
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

        if (Input.GetMouseButtonDown(1) && playerMoveScript.CanMove)
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

    public void RemoveSpellOrb(Orb orb, bool removeFromOrbHistory = false)
    {
        orbs.Remove(orb);
        if (removeFromOrbHistory)
        {
            orbHistory.Remove(orb); // sync across scenes
        }
        if (uIController)
            uIController.RemoveOrb(orb);
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
                if (spellCooldowns[currentSpell.GetOrbTuple()].Item1 <= Time.time)
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

    /// <summary>
    /// (PunRPC) Reduces all current spell cooldowns for the player this component is attached to.
    /// The amount reduced is not a flat amount, but rather a percentage (from 0->100) that it would cooldown by.
    /// So: cooldown = cooldown - percentReduce * BaseCooldown;
    /// </summary>
    /// <param name="percentReduce"></param>
    [PunRPC]
    void ReduceAllCooldowns(float percentReduce)
    {
        print("Reduce cooldowns.");
        if (percentReduce <= 0)
        {
            throw new ArgumentException(string.Format("{0} is a positive percentage, and thus should be greater than zero", percentReduce), "percentReduce");
        }

        // copy list of keys so we can permute while looping (O(2n))
        List<(Type, Type, Type)> keys = new List<(Type, Type, Type)>(spellCooldowns.Keys);
        foreach ((Type, Type, Type) key in keys)
        {
            float baseCooldown = spellCooldowns[key].Item2;
            float currentCooldown = spellCooldowns[key].Item1;
            float newCooldown = currentCooldown - ((percentReduce / 100) * baseCooldown) > 0 ? currentCooldown - ((percentReduce / 100) * baseCooldown) : 0;
            spellCooldowns[key] = (newCooldown, baseCooldown);
        }
    }

    void CastSpell(Vector3 clickedPosition)
    {
        currentSpell.Cast(transform, clickedPosition);
        spellCooldowns[currentSpell.GetOrbTuple()] = (Time.time + currentSpell.GetSpellCooldown(), currentSpell.GetSpellCooldown());

        float cost = currentSpell.GetManaCost();
        if (cost > 0)
        {
            mana.ConsumeMana(currentSpell.GetManaCost());
        }
    }

    private void UpdateOrbsFromPreviousScene()
    {
        // update the player's orbs from the previous scene
        foreach (Orb o in orbHistory.ToArray())
        {
            switch (o.getElement())
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
                    AddSpellOrb(new GreenOrb());
                    break;
                case Orb.Element.Water:
                    AddSpellOrb(new BlueOrb());
                    break;
                case Orb.Element.Poison:
                    AddSpellOrb(new VioletOrb());
                    break;
                case Orb.Element.Earth:
                    AddSpellOrb(new BrownOrb());
                    break;
                case Orb.Element.Wind:
                    AddSpellOrb(new QuickSilverOrb());
                    break;
                case Orb.Element.Darkness:
                    AddSpellOrb(new IndigoOrb());
                    break;
                default:
                    throw new NotImplementedException("Didn't implement adding " + o.getElement() + " yet");
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
            AddSpellOrb(new RedOrb());
            AddSpellOrb(new OrangeOrb());
            AddSpellOrb(new YellowOrb());
            AddSpellOrb(new GreenOrb());
            AddSpellOrb(new BlueOrb());
            AddSpellOrb(new VioletOrb());
            AddSpellOrb(new BrownOrb());
            AddSpellOrb(new QuickSilverOrb());
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
