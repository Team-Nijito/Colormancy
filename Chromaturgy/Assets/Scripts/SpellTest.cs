using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellTest : MonoBehaviour
{
    List<Orb> orbs = new List<Orb>();
    SpellManager manager;

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
        orbs.Add(new IndigoOrb());
        orbs.Add(new YellowOrb());
        orbs.Add(new VioletOrb());
    }

    // Update is called once per frame
    void Update()
    {
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
                    manager.AddOrb(orbs[number]);
                }
            }
        }

        if (Input.GetMouseButtonDown(0))
        {
            if (manager.TryCreateSpell(out SpellManager.Spell spell))
            {
                spell.Cast(transform);
            }
        }
    }
}
