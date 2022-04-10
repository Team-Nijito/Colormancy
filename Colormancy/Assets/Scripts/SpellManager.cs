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
        float SpellDmgMultiplier;
        (System.Type, System.Type, System.Type) OrbTuple;

        Orb[] orbs;

        public Spell(Orb[] _orbs, float cooldownMultiplier, float damageMultiplier)
        {
            orbs = _orbs;
            
            SpellCooldown = BASE_COOLDOWN * OrbValueManager.getCooldownMod(orbs[2].getElement()) * cooldownMultiplier;
            SpellManaCost = BASE_SPELL_MANA * OrbValueManager.getShapeManaMod(orbs[2].getElement());
            SpellDmgMultiplier = damageMultiplier;
            OrbTuple = (orbs[0].GetType(), orbs[1].GetType(), orbs[2].GetType());
        }

        public void Cast(Transform t, Vector3 clickedPosition, bool PVPEnabled, PhotonView casterView)
        {
            StatusEffectScript status = t.gameObject.GetComponent<StatusEffectScript>();
            status = t.gameObject.GetComponent<StatusEffectScript>();

            // Sync the PVP status and caster photonView
            orbs[0].setPVPStatus(PVPEnabled);
            orbs[1].setPVPStatus(PVPEnabled);
            orbs[2].setPVPStatus(PVPEnabled);

            orbs[0].setCasterPView(casterView);
            orbs[1].setCasterPView(casterView);
            orbs[2].setCasterPView(casterView);

            // exception for quicksilver
            if (orbs[0].getElement() == Orb.Element.Wind)
            {
                GameObject storm = Instantiate(Resources.Load("Orbs/QuickSilver Storm"), clickedPosition, t.rotation) as GameObject;
                QuickSilverStormController g = storm.GetComponent<QuickSilverStormController>();
                g.duration = OrbValueManager.getGreaterEffectDuration(Orb.Element.Wind, OrbValueManager.getLevel(Orb.Element.Wind, orbs[0].getLevel()));
                g.CasterPView = casterView;
                g.IsPVPEnabled = PVPEnabled;
            }
            // exception for red
            if(orbs[2].getElement() != Orb.Element.Wrath)
            {
                status.RPCClearStatusEffect(StatusEffect.StatusType.AutoAttackIncreasedSpeed);
            }

            status.RPCClearStatusEffect(StatusEffect.StatusType.ManaRegeneration);
            status.RPCClearStatusEffect(StatusEffect.StatusType.AmplifySpell);

            orbs[2].CastShape(orbs[0].CastGreaterEffect, orbs[1].CastLesserEffect, t, clickedPosition, SpellDmgMultiplier);
        }

        public float GetSpellCooldown()
        {
            return SpellCooldown;
        }

        public float GetManaCost()
        {
            return SpellManaCost;
        }

        public void PreviewSpell(Transform playerTransform, Vector3 mouseLocation)
        {
            GameObject preview = GameObject.FindGameObjectWithTag("SpellPreview");
            string previewFilePath = "Orbs/Previews/";
            mouseLocation += Vector3.up * 0.1f;
            Vector3 mouseDirection = mouseLocation - playerTransform.position;

            switch (orbs[2].getElement())
            {
                case Orb.Element.Wrath:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "CirclePreview"), mouseLocation, playerTransform.rotation) as GameObject;
                        preview.transform.localScale = Vector3.one * OrbValueManager.getPaintRadius(Orb.Element.Wrath);

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Wrath));
                    }

                    // repeat assignment in case object already exists
                    preview.transform.position = mouseLocation;
                    break;
                case Orb.Element.Fire:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "ArrowPreview"), mouseLocation, playerTransform.rotation) as GameObject;

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Fire));
                    }

                    preview.transform.position = playerTransform.GetChild(0).position + new Vector3(mouseDirection.x, 0, mouseDirection.z).normalized * 4 + Vector3.up * 0.1f;
                    preview.transform.LookAt(preview.transform.position + new Vector3(mouseDirection.x, 0, mouseDirection.z));
                    preview.transform.Rotate(Vector3.up, -90f);
                    break;
                case Orb.Element.Light:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "CurvedArrowPreview"), mouseLocation, playerTransform.rotation) as GameObject;

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Light));
                    }

                    // repeat assignment in case object already exists
                    preview.transform.position = playerTransform.position + Vector3.up * 0.1f;
                    break;
                case Orb.Element.Nature:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "BoxPreview"), mouseLocation, playerTransform.rotation) as GameObject;

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Nature));
                    }

                    preview.transform.position = playerTransform.GetChild(0).position + new Vector3(mouseDirection.x, 0, mouseDirection.z).normalized * 4 + Vector3.up * 0.1f;
                    preview.transform.LookAt(preview.transform.position + new Vector3(mouseDirection.x, 0, mouseDirection.z));
                    break;
                case Orb.Element.Water:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "CirclePreview"), mouseLocation, playerTransform.rotation) as GameObject;
                        preview.transform.localScale = Vector3.one * OrbValueManager.getPaintRadius(Orb.Element.Water);

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Water));
                    }

                    preview.transform.position = playerTransform.position - playerTransform.GetChild(0).forward;
                    break;
                case Orb.Element.Poison:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "CirclePreview"), mouseLocation, playerTransform.rotation) as GameObject;
                        preview.transform.localScale = Vector3.one * OrbValueManager.getPaintRadius(Orb.Element.Poison);

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Poison));
                    }

                    preview.transform.position = playerTransform.position + mouseDirection.normalized * 7;
                    break;
                case Orb.Element.Earth:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "CirclePreview"), mouseLocation, playerTransform.rotation) as GameObject;
                        preview.transform.localScale = Vector3.one * OrbValueManager.getPaintRadius(Orb.Element.Earth);

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Earth));
                    }

                    // repeat assignment in case object already exists
                    preview.transform.position = playerTransform.position + Vector3.up * 0.1f;
                    break;
                case Orb.Element.Wind:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "CirclePreview"), mouseLocation, playerTransform.rotation) as GameObject;
                        preview.transform.localScale = Vector3.one * OrbValueManager.getPaintRadius(Orb.Element.Wind);

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Wind));
                    }

                    // repeat assignment in case object already exists
                    preview.transform.position = mouseLocation;
                    break;
                case Orb.Element.Darkness:
                    if (!preview)
                    {
                        preview = Instantiate(Resources.Load(previewFilePath + "ArrowsPreview"), mouseLocation, playerTransform.rotation) as GameObject;
                        preview.transform.localScale = Vector3.one * OrbValueManager.getPaintRadius(Orb.Element.Darkness);

                        Renderer previewRenderer = preview.GetComponent<Renderer>();
                        previewRenderer.material.SetColor("_Color", OrbValueManager.getColor(Orb.Element.Darkness));
                    }

                    // repeat assignment in case object already exists
                    preview.transform.position = playerTransform.position + Vector3.up * 0.1f;
                    break;
            }
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

    private float m_cooldownMultiplier = 1f;
    private float m_damageMultiplier = 1f;
    
    public void AddCooldownMultiplier(float percentage)
    {
        if (m_cooldownMultiplier + percentage / 100 <= 0)
            return;

        m_cooldownMultiplier += percentage / 100;
    }

    public void AddDamageMultiplier(float percentage)
    {
        if (m_damageMultiplier + percentage / 100 <= 0)
            return;

        m_damageMultiplier += percentage / 100;
    }

    public Spell AddOrb(Orb orb)
    {
        if (photonView.IsMine && PhotonNetwork.IsConnected)
        {
            if (FirstOrb != null)
            {
                FirstOrb.RevertHeldEffect(gameObject);
            }
            FirstOrb = orb;
            FirstOrb.AddHeldEffect(gameObject);

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
            spell = new Spell(spellOrbs, m_cooldownMultiplier, m_damageMultiplier);
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

