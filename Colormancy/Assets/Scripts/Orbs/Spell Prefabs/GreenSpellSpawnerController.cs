using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreenSpellSpawnerController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastLevel;
    public int lesserCastLevel;
    public float spellEffectMod;
    private const Orb.Element element = Orb.Element.Nature;

    private Vector3 raycastOrigin;

    [SerializeField]
    private float vRaycastHeight;
    [SerializeField]
    private float fRaycastDistance;
    [SerializeField]
    private float hPlacement;
    [SerializeField]
    private float displacement;

    [SerializeField]
    private int iterations;
    private int currentIteration;
    [SerializeField]
    private int ticksPerIteration;
    private int currentTick;
    private bool spawnVine;

    [Space]

    private float startTime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    private float spherePaintRadius;

    [SerializeField]
    private bool debug;
    [SerializeField]
    private bool startAnimation;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
        currentTick = 0;
        currentIteration = 0;
        spawnVine = true;

        raycastOrigin = transform.position + Vector3.up;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Time.time - startTime > lifetime && !debug)
            Destroy(gameObject);

        if (spawnVine)
        {
            if (!Physics.Raycast(raycastOrigin, transform.forward, fRaycastDistance, PaintingManager.paintingMask))
            {
                Vector3 potentialOrigin = raycastOrigin + transform.forward * fRaycastDistance;
                RaycastHit hit;

                if (Physics.Raycast(potentialOrigin, Vector3.down, out hit, 100f, 1 << PaintingManager.paintingMask))
                {
                    raycastOrigin = hit.point + Vector3.up;

                    GameObject vine = Instantiate(Resources.Load<GameObject>("Orbs/Green Vine"), hit.point, transform.rotation);
                    vine.transform.parent = transform;

                    if (currentIteration % 2 == 0)
                    {
                        vine.transform.position += transform.right * hPlacement;
                        vine.transform.Rotate(Vector3.up, 180f);
                    }
                    else
                    {
                        vine.transform.position -= transform.right * hPlacement;
                    }

                    PaintingManager.PaintSphere(OrbValueManager.getColor(element), vine.transform.position, spherePaintRadius);

                    vine.transform.position += transform.right * displacement * Random.Range(-1, 1);
                    vine.transform.position += transform.forward * displacement * Random.Range(-1, 1);
                    vine.transform.Rotate(Vector3.up, Random.Range(-10, 10));
                }
                else
                    currentIteration = iterations;
            }
            else
                currentIteration = iterations;

            spawnVine = false;
        }

        // check to see if animation ended
        for (int i = 0; i < transform.childCount; ++i)
        {
            GameObject vine = transform.GetChild(i).gameObject;
            Animator vineAnim = vine.GetComponent<Animator>();
            if (vineAnim.GetCurrentAnimatorStateInfo(0).IsName("Empty"))
                Destroy(vine);
        }

        // handle ticks
        if (currentIteration < iterations)
        {
            currentTick++;
            if (currentTick >= ticksPerIteration)
            {
                currentTick = 0;
                currentIteration++;
                spawnVine = true;
            }
        }

        if (debug && startAnimation)
        {
            startAnimation = false;
            currentIteration = 0;
            currentTick = 0;

            raycastOrigin = transform.position + Vector3.up;

            for (int i = 0; i < transform.childCount; ++i)
                Destroy(transform.GetChild(i).gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
            greaterCast(collision.gameObject, greaterCastLevel, spellEffectMod);
        else if (collision.gameObject.tag.Equals("Player"))
            lesserCast(collision.gameObject, lesserCastLevel, spellEffectMod);
    }
}
