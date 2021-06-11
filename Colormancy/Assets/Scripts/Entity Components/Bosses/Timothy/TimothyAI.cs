using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class TimothyAI : BossAI
{
    //Controls if there are debug messages when changing states amongst other prints
    public bool DebugMode = true;

    public float BlastOffRange = 5f;

    public float RunDistance = 10f;
    public float ChaseDistance = 15f;
    public float ChannelTime = 2.5f;

    public float LaserCooldown = 10f;
    public float BlastOffCooldown = 20f;
    public float MMarchCooldown = 40f;
    public float IdleCooldown = 5f;

    public GameObject LaserObject;

    [HideInInspector]
    public float currentLaserCooldown = 0f;
    [HideInInspector]
    public float currentBlastOffCooldown = 0f;
    [HideInInspector]
    public float currentMMarchCooldown = 0f;
    [HideInInspector]
    public float currentIdleCooldown = 0f;

    public enum States
    {
        Run, Laser, BlastOff, MachineMarch
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
            photonView.RPC("SetTimothyState", RpcTarget.AllViaServer, States.Run);
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

        currentLaserCooldown += Time.deltaTime;
        currentBlastOffCooldown += Time.deltaTime;
        currentMMarchCooldown += Time.deltaTime;
        currentIdleCooldown += Time.deltaTime;
    }

    GameObject GetTarget()
    {
        GameObject targetPlayer = null;
        float closestPlayer = 10000;
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in photonViews)
        {
            GameObject playObj = view.gameObject;
            HealthScript healthScript = playObj.GetComponent<HealthScript>();
            if (healthScript && playObj.tag == "Player") /* Is a player */
            {
                float dist = Vector3.Distance(transform.position, playObj.transform.position);
                if (dist < closestPlayer)
                {
                    closestPlayer = dist;
                    targetPlayer = playObj;
                }
            }
        }

        return targetPlayer;
    }

    [PunRPC]
    public void SetTimothyState(States state)
    {
        if (photonView.IsMine)
        {
            if (state == States.Laser)
            {
                SetState(new TimothyLaser(this));
            }
            else if (state == States.BlastOff)
            {
                SetState(new TimothyBlastOff(this));
            }
            else if (state == States.MachineMarch)
            {
                SetState(new TimothyMachineMarch(this));
            }
            else if (state == States.Run)
            {
                SetState(new TimothyRun(this));
            }
        }
    }
}
