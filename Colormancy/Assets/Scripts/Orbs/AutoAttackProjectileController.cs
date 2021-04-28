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

    [SerializeField]
    private bool debug;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;
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
        if (collision.gameObject.CompareTag("Enemy"))
        {
            PhotonView photonView = PhotonView.Get(collision.gameObject);
            photonView.RPC("TakeDamage", RpcTarget.All, 10f);
        }
            
        if (!collision.gameObject.CompareTag("Player"))
            Destroy(gameObject);
    }
}
