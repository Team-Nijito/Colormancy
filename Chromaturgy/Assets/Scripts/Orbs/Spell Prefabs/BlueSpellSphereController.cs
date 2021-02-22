using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlueSpellSphereController : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if (GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            GameObject orbs = GameObject.Instantiate(Resources.Load("Orbs/Blue Ink", typeof(GameObject)), transform.position, Quaternion.identity) as GameObject;
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint[] points = new ContactPoint[8];
        collision.GetContacts(points);

        foreach (ContactPoint point in points)
        {
            if (point.normal == Vector3.up)
            {
                GameObject orbs = GameObject.Instantiate(Resources.Load("Orbs/Blue Ink", typeof(GameObject)), point.point + Vector3.down / 10, Quaternion.identity) as GameObject;
                orbs.transform.RotateAround(orbs.transform.position, Vector3.up, Random.Range(0, 360));
                Destroy(gameObject);
            }
        }
    }
}
