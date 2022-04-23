using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerProjectileSpawner : MonoBehaviourPun
{
    //[HideInInspector]
    public float sizeModifier = 1f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject SpawnProjectile(Orb.Element element, string prefabLocation, Vector3 position, Quaternion rotation)
    {
        GameObject g = Instantiate(Resources.Load(prefabLocation), position, rotation) as GameObject;
        Vector3 baseScale = g.transform.localScale;
        Vector3 modifiedScale = new Vector3(baseScale.x * sizeModifier, baseScale.z * sizeModifier, baseScale.z * sizeModifier);
        g.transform.localScale = modifiedScale;

        return g;
    }
}
