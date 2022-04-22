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

    public List<Orb> orbs = new List<Orb>();

    [SerializeField]
    [Range(0f, 100f)]
    float m_spellCooldownModifier = 0f;

    ItemManager itemManager;
    SpellManager manager;
    ManaScript mana;
    OrbTrayUIController uIController;

    private readonly bool TestingMode = true;

    [SerializeField]
    Dictionary<(Type, Type, Type), (float, float)> spellCooldowns = new Dictionary<(Type, Type, Type), (float, float)>(); // key: Orb Tuple, value: (current cooldown, Spell base cooldown)

    SpellManager.Spell currentSpell;

    PlayerMovement playerMoveScript; // need a ref to this component so we can check if we're stunned or not, so we'll prevent casting while stunned

    [SerializeField]
    private HealthScript m_playerHealthScript = null; // need to ref to this component so we can fetch the GameManager in the scene, and then check if PVP is enabled

    private bool m_PVPEnabled = false;

    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponent<SpellManager>();
        mana = GetComponent<ManaScript>();
        itemManager = GetComponent<ItemManager>();
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

        if (Input.GetMouseButton(1) && playerMoveScript.CanMove)
        {
            if (currentSpell.GetOrbTuple().Item3 != null)
            {
                if (spellCooldowns.ContainsKey(currentSpell.GetOrbTuple()))
                {
                    if (spellCooldowns[currentSpell.GetOrbTuple()].Item1 <= Time.time)
                    {
                        currentSpell.PreviewSpell(transform, GetMouseLocation());
                    }
                }
                else
                {
                    currentSpell.PreviewSpell(transform, GetMouseLocation());
                }
                 
            }
        }

        if (Input.GetMouseButtonUp(1) && playerMoveScript.CanMove)
        {
            Destroy(GameObject.FindGameObjectWithTag("SpellPreview"));
            photonView.RPC("TryCastSpell", RpcTarget.All, GetMouseLocation());
        }
    }

    private Vector3 GetMouseLocation()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, 100f, 1 << PaintingManager.paintingMask | 1))
        {
            return hit.point;
        }
        else
        {
            Vector3 planeCenter = transform.position - Vector3.up * 0.4f;
            Vector3 planeNormal = Vector3.up;

            float t = Vector3.Dot((planeCenter - ray.origin), planeNormal) / Vector3.Dot(ray.direction, planeNormal);

            return ray.origin + ray.direction * t;
        }
    }

    public void AddSpellOrb(Orb orb, bool addToOrbHistory = false)
    {
        orb.setCasterPView(PhotonView.Get(gameObject));
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
        RemoveOrbFromList(orbs, orb);
        if (removeFromOrbHistory)
        {
            RemoveOrbFromList(orbHistory, orb); // sync across scenes
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
        // Pass in the PVPStatus and our photonView whenever we invoke Cast.
        currentSpell.SpellDmgMultiplier = itemManager.DoDamageMultipliers(currentSpell.SpellDmgMultiplier);
        currentSpell.Cast(transform, clickedPosition, m_PVPEnabled, photonView);
        spellCooldowns[currentSpell.GetOrbTuple()] = (Time.time + currentSpell.GetSpellCooldown() * (1f - (m_spellCooldownModifier / 100f)), currentSpell.GetSpellCooldown() * (1f - (m_spellCooldownModifier / 100f)));

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
    /// Helper function (original code orb removal snippet written by Branden, and translated into helper function by William)
    /// </summary>
    /// <param name="orbList">The list of orbs to search and permute</param>
    /// <param name="markedOrb">The orb to remove</param>
    private void RemoveOrbFromList(List<Orb> orbList, Orb markedOrb)
    {
        Orb elementToRemove = null;
        foreach (Orb searchOrb in orbList)
        {
            if (searchOrb.getElement() == markedOrb.getElement())
            {
                elementToRemove = searchOrb;
            }
        }
        orbList.Remove(elementToRemove);
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
            gUIController = GameObject.Find("OrbTrayPanel");
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

        m_playerHealthScript.gameManagerUpdated += UpdateGameManager;
    }

    /// <summary>
    /// attach this to an event in HealthScript whenever the GameManager is updated
    /// </summary>
    private void UpdateGameManager(GameManager temp)
    {
        photonView.RPC("SetPVPStatusAndPhotonView", RpcTarget.All, temp.TypeOfLevel == GameManager.LevelTypes.PVP);
    }

    /// <summary>
    /// Clear out all current orbs.
    /// </summary>
    public void ResetOrbs()
    {
        // clean up the UI
        if (uIController)
        {
            foreach (Orb o in orbs)
            {
                uIController.RemoveOrb(o);
            }
        }

        orbHistory.Clear();
        orbs.Clear();
    }

    /// <summary>
    /// (PunRPC) This sets the PVP status (according to the GM) and photon view of the caster for each orb.
    /// This needs to be a PunRPC call so that the PVPEnabled bool would be accurately replicated across all clients
    /// and the spell projectiles's layers would be updated accordingly.
    /// </summary>
    [PunRPC]
    public void SetPVPStatusAndPhotonView(bool PVPEnabled)
    {
        //print($"RPC call result for {photonView.name}: {PVPEnabled}: orb count: {orbs.Count}");

        m_PVPEnabled = true;

        // Don't update the orbs for all players, because on client side, the client can only see their own orbs
        // Not the orbs of others (your orb list and cooldowns are not synced across the server).
    }


    public void AddSpellCooldownModifier(float deltaAmount)
    {
        m_spellCooldownModifier += deltaAmount;
        m_spellCooldownModifier = Mathf.Clamp(m_spellCooldownModifier, 0f, 100f);
    }    

    public void SetSpellCooldownModifier(float modifier)
    {
        if (modifier >= 0f && modifier <= 100f)
        {
            m_spellCooldownModifier = modifier;
        }
    }
}
