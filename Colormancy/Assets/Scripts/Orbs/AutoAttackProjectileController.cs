using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;
public class AutoAttackProjectileController : MonoBehaviour
{
    [SerializeField]
    private float speed;
    private float startTime;
    [SerializeField]
    private float lifetime;
    [SerializeField]
    private float paintRadius;
    public Color playerColor;

    [Space]

    public float attackDamage;
    public float attackMultiplier;
    public bool poisonedAttack;
    public float poisonedAttackDamage;
    public float poisonedAttackDuration;

    public bool canAttackOtherPlayer = false;
    public int shooterID; // AKA the photon view ID of the person who created this projectile, AKA the shooter

    [Space]

    [SerializeField]
    private bool debug;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        Material projectileMaterial = GetComponent<TrailRenderer>().material;
        projectileMaterial.SetColor("_Color", playerColor);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (debug)
        {
            float xpos = 5 * Mathf.Sin(Time.time);
            float ypos = 5 * Mathf.Cos(Time.time);

            transform.position = new Vector3(xpos, 1, ypos);
        }
        else
        {
            transform.position += transform.forward * speed;

            if (Time.time - startTime > lifetime && !debug)
                Destroy(gameObject);
        }

        PaintingManager.PaintSphere(playerColor, transform.position, paintRadius);
    }

    private void OnCollisionEnter(Collision collision)
    {
        bool isEnemy = collision.gameObject.CompareTag("Enemy");
        bool isPlayer = collision.gameObject.CompareTag("Player");

        if (isEnemy || (canAttackOtherPlayer && isPlayer))
        {
            StatusEffectScript status = collision.gameObject.GetComponent<StatusEffectScript>();
            if (status.StatusExists(StatusEffect.StatusType.AutoAttackIncreasedDamage))
                attackMultiplier += 0.5f;

            PhotonView photonView = PhotonView.Get(collision.gameObject);
            if (photonView.ViewID != shooterID)
            {
                // Don't hurt yourself with the projectile
                photonView.RPC("TakeDamage", RpcTarget.All, attackDamage * attackMultiplier);

                if (poisonedAttack)
                    status.RPCApplyStatus(StatusEffect.StatusType.DamageOverTime, poisonedAttackDuration, 1, poisonedAttackDamage);
            }
        }
            
        if ((canAttackOtherPlayer && isPlayer) || !isPlayer)
            Destroy(gameObject);
    }
}
