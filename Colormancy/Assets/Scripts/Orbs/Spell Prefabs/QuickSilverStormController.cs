using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSilverStormController : MonoBehaviour
{
    public float duration;
    private float currentTime;

    public bool IsPVPEnabled
    {
        get { return PVPEnabled; }
        set {
            PVPEnabled = value;
            if (value)
            {
                PVPIsNowEnabled();
            }
        }
    }
    private bool PVPEnabled = false;
    
    public PhotonView CasterPView = null;

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

    private void OnTriggerStay(Collider collision)
    {
        Vector3 enemyToStorm = transform.position - collision.transform.position;
        //enemyToStorm = new Vector3(enemyToStorm.x, 0, enemyToStorm.z); // suck, don't propel the enemy

        if (collision.gameObject.CompareTag("Enemy"))
        {
            StatusEffectScript status = collision.gameObject.GetComponent<StatusEffectScript>();
            status.RPCApplyForce(0, "storm_pull", enemyToStorm.normalized, enemyToStorm.sqrMagnitude);
        }
        else if (PVPEnabled && collision.gameObject.CompareTag("Player") && PhotonView.Get(collision.gameObject).ViewID != CasterPView.ViewID)
        {
            StatusEffectScript status = collision.gameObject.GetComponent<StatusEffectScript>();
            status.RPCApplyForce(0, "storm_pull", enemyToStorm.normalized, enemyToStorm.sqrMagnitude);
        }
    }

    /// <summary>
    /// Change the layer so that the tornado may target players.
    /// </summary>
    private void PVPIsNowEnabled()
    {
        gameObject.layer = LayerMask.NameToLayer("Default");
    }
}
