using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class DianeAI : BossAI
{
    
    public float SlashRange = 2f;
    
    public float SlashCooldown = 5f;
    public float HamstringCooldown = 15f;
    public float FocusFireCooldown = 30f;
    public float IdleCooldown = 5f;

    [HideInInspector]
    public float currentSlashCooldown = 0f;
    [HideInInspector]
    public float currentHamstringCooldown = 0f;
    [HideInInspector]
    public float currentFocusFireCooldown = 0f;
    [HideInInspector]
    public float currentIdleCooldown = 0f;


    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        EnemyHitbox = GetComponent<EnemyHitbox>();
        MeshAgent = GetComponent<NavMeshAgent>();
        StatusEffect = GetComponent<StatusEffectScript>();
        SetState(new DianeChase(this));
    }

    // Update is called once per frame
    void Update()
    { 
        if (Target == null)
        {
            GameObject target = GetTarget();
            SetTarget(target);
        }

        if (State != null)
            State.Update();

        //Tick Cooldowns
        currentSlashCooldown += Time.deltaTime;
        currentHamstringCooldown += Time.deltaTime;
        currentFocusFireCooldown += Time.deltaTime;
        currentIdleCooldown += Time.deltaTime;
    }

    //Gets target (For Diane that means the player with the highest HP)
    GameObject GetTarget()
    {
        GameObject targetPlayer = null;
        float highestHealth = 0;
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in photonViews)
        {
            GameObject playObj = view.gameObject;
            HealthScript healthScript = playObj.GetComponent<HealthScript>();
            if (healthScript && playObj.tag == "Player")
            {
                float currentHealth = healthScript.GetEffectiveHealth();
                if (currentHealth > highestHealth)
                {
                    highestHealth = currentHealth;
                    targetPlayer = playObj;
                }
            }
        }

        return targetPlayer;
    }
}
