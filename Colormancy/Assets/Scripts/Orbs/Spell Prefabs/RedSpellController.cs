using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class RedSpellController : MonoBehaviour
{
    public Transform playerTransform;
    private GameObject playerObject;
    [SerializeField]
    private GameObject redBase;
    [SerializeField]
    private GameObject redEdge;
    private Renderer baseMaterial;
    private Renderer edgeMaterial;
    private const Orb.Element element = Orb.Element.Wrath;

    [Space]

    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;

    public bool PVPEnabled = false;
    public PhotonView CasterPView = null;

    [Space]

    private Vector3 startPosition;
    public Vector3 endPosition;
    private Vector3 flatDirection;

    [Space]

    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private float jumpVelocity;
    private float startTime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    [Range(0, 1)]
    private float m_lerp;

    [SerializeField]
    private bool debug;

    private bool landed;
    private bool toggleLanding;

    // Start is called before the first frame update
    void Start()
    {
        baseMaterial = redBase.GetComponent<Renderer>();
        edgeMaterial = redEdge.GetComponent<Renderer>();

        if (!debug)
        {
            baseMaterial.enabled = false;
            edgeMaterial.enabled = false;
        }

        playerObject = playerTransform.gameObject;
        playerObject.GetComponent<PlayerMovement>().enabled = false;
        startPosition = playerTransform.position;

        flatDirection = new Vector3(endPosition.x - startPosition.x, 0, endPosition.z - startPosition.z);
    }

    void OnEnable()
    {
        startTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (toggleLanding && TryGetComponent(out CapsuleCollider capsuleCollider))
            capsuleCollider.enabled = false;

        if (debug)
        {
            baseMaterial.material.SetFloat("_Lerp", m_lerp);
            edgeMaterial.material.SetFloat("_Lerp", m_lerp);
        }

        if (landed)
        {
            // toggles
            playerObject.GetComponent<PlayerMovement>().enabled = true;
            toggleLanding = true;
            landed = false;


            baseMaterial.enabled = true;
            edgeMaterial.enabled = true;

            transform.position = playerTransform.position + Vector3.down;

            PaintingManager.PaintSphere(OrbValueManager.getColor(element), transform.position, OrbValueManager.getPaintRadius(element));

            // search in enemy and player layermasks
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, OrbValueManager.getPaintRadius(element), (1 << 10) | (1 << 9));

            foreach (var hitCollider in hitColliders)
            {
                if (hitCollider.CompareTag("Enemy"))
                {
                    Vector3 PlayerToEnemy = (hitCollider.gameObject.transform.position - transform.position).normalized;
                    float[] vectorData = { PlayerToEnemy.x, PlayerToEnemy.y, PlayerToEnemy.z };

                    greaterCast(hitCollider.gameObject, spellEffectMod, vectorData, CasterPView.transform);
                }
                else if (hitCollider.CompareTag("Player"))
                {
                    if (PVPEnabled && PhotonView.Get(hitCollider.gameObject).ViewID != CasterPView.ViewID)
                    {
                        Vector3 PlayerToEnemy = (hitCollider.gameObject.transform.position - transform.position).normalized;
                        float[] vectorData = { PlayerToEnemy.x, PlayerToEnemy.y, PlayerToEnemy.z };

                        greaterCast(hitCollider.gameObject, spellEffectMod, vectorData, CasterPView.transform);
                    }
                    else
                    {
                        lesserCast(hitCollider.gameObject, spellEffectMod, null);
                    }
                }
            }

            // reset time for destroying object
            startTime = Time.time;
        }

        if (Time.time - startTime < jumpTime && !toggleLanding)
        {
            Vector3 horizontalMovement = flatDirection.normalized * flatDirection.magnitude / jumpTime * Time.deltaTime;
            Vector3 verticalMovement = Vector3.up * (Mathf.Lerp(jumpVelocity, -jumpVelocity, (Time.time - startTime) / jumpTime) * Time.deltaTime);
            playerObject.GetComponent<CharacterController>().Move(horizontalMovement + verticalMovement);
        }
        else
        {
            if (!toggleLanding)
                landed = true;
            else
            {
                baseMaterial.material.SetFloat("_Lerp", m_lerp);
                edgeMaterial.material.SetFloat("_Lerp", m_lerp);
            }
        }

        if (Time.time - startTime > lifetime && !debug && toggleLanding)
            Destroy(gameObject);
    }
}
