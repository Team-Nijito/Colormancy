using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletSpellSphereController : MonoBehaviour
{
    public Vector3 endPosition;

    [SerializeField]
    private float maxHeight;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 direction = new Vector3(endPosition.x - transform.position.x, 0, endPosition.z - transform.position.z);
        float dx = direction.magnitude;
        float dy = maxHeight - endPosition.y;

        Vector3 yComponent = Vector3.up * Mathf.Sqrt(-2f * -9.81f * (maxHeight - transform.position.y));

        float tx1 = -yComponent.y / -9.81f;
        float tx2 = Mathf.Sqrt(2 * -dy / -9.81f);
        Vector3 xComponent = direction.normalized * (dx / (tx1 + tx2));

        rb.velocity = xComponent + yComponent;
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint point = collision.GetContact(0);
        
        if (point.normal == Vector3.up)
        {
            GameObject orbs = GameObject.Instantiate(Resources.Load("Orbs/Violet Cloud", typeof(GameObject)), transform.position + Vector3.up, transform.rotation) as GameObject;
            Destroy(gameObject);
        }
    }
}
