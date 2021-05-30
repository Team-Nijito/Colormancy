using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.AI;

public class HelenAI : BossAI
{

    //Controls if there are debug messages when changing states amongst other prints
    public bool DebugMode = true;

    public float ShankRange = 3f;
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

    public enum States
    {
        Shank, Chase, Shunpo
    }

    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        if (PhotonNetwork.IsMasterClient)
        {
            Animator = GetComponent<Animator>();
            EnemyHitbox = GetComponent<EnemyHitbox>();
            MeshAgent = GetComponent<NavMeshAgent>();
            StatusEffect = GetComponent<StatusEffectScript>();
            photonView.RPC("SetHelenState", Photon.Pun.RpcTarget.AllViaServer, HelenAI.States.Chase);
        }
    }

    // Update is called once per frame
    new void Update()
    {
        base.Update();
        if (GetCurrentHealth() <= 0)
            return;
        if (!PhotonNetwork.IsMasterClient) return;
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

    [PunRPC]
    public void SetHelenState(States state)
    {
        if (photonView.IsMine)
        {
            if (state == States.Shank)
            {
                SetState(new HelenShank(this));
            } else if (state == States.Chase)
            {
                SetState(new HelenChase(this));
            } else if (state == States.Shunpo)
            {
                SetState(new HelenShunpo(this));
            }
        }
    }
}
