using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VioletSpellSphereController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;

    public bool PVPEnabled = false;
    public PhotonView CasterPView = null;

    [Space]

    public Vector3 endPosition;
    [SerializeField]
    private float maxHeight;

    private void Start()
    {
        Rigidbody rb = GetComponent<Rigidbody>();

        Vector3 direction = new Vector3(endPosition.x - transform.position.x, 0, endPosition.z - transform.position.z);

        Vector3 yComponent = Vector3.up;
        Vector3 xComponent = direction.normalized;

        rb.velocity = (xComponent + yComponent) * 5; // placeholder value until implementation
    }

    private void Update()
    {
        if (GetComponent<Rigidbody>().velocity == Vector3.zero)
        {
            instantiateCloud();
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
                instantiateCloud();
            }
        }
    }

    private void instantiateCloud()
    {
        GameObject g = GameObject.Instantiate(Resources.Load("Orbs/Violet Cloud"), transform.position + Vector3.up, transform.rotation) as GameObject;
        g.transform.Rotate(Vector3.up, Random.Range(0, 360));

        VioletSpellController spellController = g.GetComponent<VioletSpellController>();

        spellController.greaterCast = greaterCast;
        spellController.lesserCast = lesserCast;
        spellController.spellEffectMod = spellEffectMod;

        // Update the PVPStatus and Caster photon view for the cloud
        spellController.PVPEnabled = PVPEnabled;
        spellController.CasterPView = CasterPView;

        Destroy(gameObject);
    }
}
