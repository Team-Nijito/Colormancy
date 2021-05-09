using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class HelenAI : BossAI
{
    public float ShankRange = 2f;

    public float ShankCooldown = 5f;
    public float ShunpoCooldown = 15f;
    public float HunterOfHeadsCooldown = 45f;
    public float IdleCooldown = 5f;

    [HideInInspector]
    public float currentShankCooldown = 0f;
    [HideInInspector]
    public float currentShunpoCooldown = 0f;
    [HideInInspector]
    public float currentHunterOfHeadsCooldown = 0f;
    [HideInInspector]
    public float currentIdleCooldown = 0f;

    // Start is called before the first frame update
    void Start()
    {
        print("Start");
        Animator = GetComponent<Animator>();
        EnemyHitbox = GetComponent<EnemyHitbox>();
        MeshAgent = GetComponent<NavMeshAgent>();
        StatusEffect = GetComponent<StatusEffectScript>();
        SetState(new HelenChase(this));
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

        currentShankCooldown += Time.deltaTime;
        currentShunpoCooldown += Time.deltaTime;
        currentHunterOfHeadsCooldown += Time.deltaTime;
        currentIdleCooldown += Time.deltaTime;
    }

    GameObject GetTarget()
    {
        GameObject targetPlayer = null;
        float lowestHealth = 10000;
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in photonViews)
        {
            GameObject playObj = view.gameObject;
            HealthScript healthScript = playObj.GetComponent<HealthScript>();
            if (healthScript && playObj.tag == "Player")
            {
                float currentHealth = healthScript.GetEffectiveHealth();
                if (currentHealth < lowestHealth)
                {
                    lowestHealth = currentHealth;
                    targetPlayer = playObj;
                }
            }
        }

        return targetPlayer;
    }
}
