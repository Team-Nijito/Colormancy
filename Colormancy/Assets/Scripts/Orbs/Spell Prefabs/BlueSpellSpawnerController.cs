using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlueSpellSpawnerController : MonoBehaviour
{
    public Transform playerTransform;
    private Transform modelTransform;

    [Space]

    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;

    [Space]

    [SerializeField]
    private float lifeTime;
    private float currentTime;

    [Space]

    [SerializeField]
    private float bMultiplier;
    [SerializeField]
    private float dMultiplier;
    [SerializeField]
    private int frequency;
    private int tick;
    private List<GameObject> entitiesEntered;

    // Start is called before the first frame update
    void Start()
    {
        // use mage rotation
        modelTransform = playerTransform.GetChild(0);

        entitiesEntered = new List<GameObject>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (tick == frequency - 1 && currentTime < lifeTime)
        {
            Vector3 behind = -modelTransform.forward;

            GameObject sphere = GameObject.Instantiate(Resources.Load("Orbs/Blue Sphere", typeof(GameObject)), playerTransform.position + Vector3.up + behind, Quaternion.identity) as GameObject;
            sphere.GetComponent<Rigidbody>().velocity = behind * bMultiplier + Vector3.down * dMultiplier;
            sphere.GetComponent<BlueSpellSphereController>().spawner = transform;
            sphere.transform.parent = transform;
        }

        if (transform.childCount == 0)
            Destroy(gameObject);

        tick = (tick + 1) % frequency;
        currentTime += Time.deltaTime;
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Enemy"))
        {
            if (!entitiesEntered.Contains(collider.gameObject))
            {
                entitiesEntered.Add(collider.gameObject);
                greaterCast(collider.gameObject, spellEffectMod, null);
            }
        }
        else if (collider.gameObject.CompareTag("Player"))
        {
            if (!entitiesEntered.Contains(collider.gameObject))
            {
                entitiesEntered.Add(collider.gameObject);
                lesserCast(collider.gameObject, spellEffectMod, null);
            }
        }
    }
}
