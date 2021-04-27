using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class DianeAI : BossAI
{
    [SerializeField]
    float SlashRange = 2f;

    [SerializeField]
    float SLASH_COOLDOWN = 5f;
    [SerializeField]
    float HAMSTRING_COOLDOWN = 15f;

    float currentSlashCooldown;
    float currentHamstringCooldown;

    // Start is called before the first frame update
    void Start()
    {
        Animator = GetComponent<Animator>();
        EnemyHitbox = GetComponent<EnemyHitbox>();
        MeshAgent = GetComponent<NavMeshAgent>();

        currentSlashCooldown = SLASH_COOLDOWN;
        currentHamstringCooldown = HAMSTRING_COOLDOWN;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target == null)
        {
            GameObject target = GetTarget();
            SetTarget(target);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            SetState(new DianeChase(this));
        }

        if (currentHamstringCooldown >= HAMSTRING_COOLDOWN)
        {
            currentHamstringCooldown = 0f;
            SetState(new DianeHamstring(this));
            Target = null;
        }

        if (DistanceToTarget() < SlashRange && currentSlashCooldown >= SLASH_COOLDOWN)
        {
            currentSlashCooldown = 0f;
            SetState(new DianeSlash(this));
            Target = null;
        }

        if (State != null)
            State.Update();

        //Tick Cooldowns
        currentSlashCooldown += Time.deltaTime;
        currentHamstringCooldown += Time.deltaTime;
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
