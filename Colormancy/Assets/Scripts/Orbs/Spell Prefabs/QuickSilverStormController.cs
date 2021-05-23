using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSilverStormController : MonoBehaviour
{
    public float duration;
    private float currentTime;

    private void Start()
    {
        currentTime = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (currentTime > duration)
            Destroy(gameObject);

        currentTime += Time.deltaTime;
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 enemyToStorm = transform.position - collision.transform.position;
            StatusEffectScript status = collision.gameObject.GetComponent<StatusEffectScript>();
            status.RPCApplyForce(0, "storm_pull", enemyToStorm.normalized, enemyToStorm.sqrMagnitude);
        }
    }
}
