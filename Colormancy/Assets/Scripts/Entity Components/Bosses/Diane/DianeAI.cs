using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.AI;

public class DianeAI : BossAI
{
    //Controls if there are debug messages when changing states amongst other prints
    public bool DebugMode = true;

    //Ability variables
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

    public enum States
    {
        Slash, Chase, FocusFire, Hamstring
    }
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        Animator = GetComponent<Animator>();
        EnemyHitbox = GetComponent<EnemyHitbox>();
        MeshAgent = GetComponent<NavMeshAgent>();
        StatusEffect = GetComponent<StatusEffectScript>();
        photonView.RPC("SetDianeState", Photon.Pun.RpcTarget.AllViaServer, States.Chase);
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (GetCurrentHealth() <= 0)
            return;
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

    [PunRPC]
    public void SetDianeState(States state)
    {
        if (photonView.IsMine)
        {
            if (state == States.Slash)
            {
                SetState(new DianeSlash(this));
            }
            else if (state == States.Chase)
            {
                SetState(new DianeChase(this));
            }
            else if (state == States.FocusFire)
            {
                SetState(new DianeFocusFire(this));
            }
            else if (state == States.Hamstring)
            {
                SetState(new DianeHamstring(this));
            }
        }
    }
}
