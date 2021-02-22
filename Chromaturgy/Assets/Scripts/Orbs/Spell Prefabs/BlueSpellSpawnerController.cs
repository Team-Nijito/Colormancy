using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BlueSpellSpawnerController : MonoBehaviour
{
    public Transform playerTransform;
    private Transform modelTransform;

    [SerializeField]
    private int frequency;
    private int tick;

    [SerializeField]
    private float lifeTime;
    private float currentTime;

    [SerializeField]
    private float bMultiplier;
    [SerializeField]
    private float dMultiplier;

    // Start is called before the first frame update
    void Start()
    {
        // use mage rotation
        modelTransform = playerTransform.GetChild(0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = playerTransform.position;

        if (tick == frequency - 1)
        {
            Vector3 behind = -modelTransform.forward;

            GameObject orbs = GameObject.Instantiate(Resources.Load("Orbs/Blue Sphere", typeof(GameObject)), transform.position + Vector3.up + behind, Quaternion.identity) as GameObject;
            orbs.GetComponent<Rigidbody>().velocity = behind * bMultiplier + Vector3.down * dMultiplier;
        }

        if (currentTime > lifeTime)
            Destroy(gameObject);

        tick = (tick + 1) % frequency;
        currentTime += Time.deltaTime;
    }
}
