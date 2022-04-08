using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public float spellEffectMod;
    private const Orb.Element element = Orb.Element.Earth;

    [Space]

    [SerializeField]
    private AnimationCurve ImpactLerp;
    [SerializeField]
    private AnimationCurve WaveLerp;

    private GameObject impact;
    private GameObject wave;
    private GameObject rocks;

    private Material impactMat;
    private Material waveMat;

    public bool PVPEnabled = false;
    public PhotonView CasterPView = null;

    [Space]

    private float startTime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    private bool debug;
    [SerializeField]
    private bool startAnimation;

    // Start is called before the first frame update
    void Start()
    {
        startTime = Time.time;

        impact = transform.GetChild(0).gameObject;
        impactMat = impact.GetComponent<Renderer>().material;

        wave = transform.GetChild(1).gameObject;
        waveMat = wave.GetComponent<Renderer>().material;

        rocks = transform.GetChild(2).gameObject;
        rocks.GetComponent<ParticleSystem>().Play();

        PaintingManager.PaintSphere(OrbValueManager.getColor(element), transform.position, OrbValueManager.getPaintRadius(element));
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time - startTime;

        if (Time.time - startTime > lifetime && !debug)
            Destroy(gameObject);

        impactMat.SetFloat("_Lerp", WaveLerp.Evaluate(currentTime));

        waveMat.SetFloat("_Lerp", WaveLerp.Evaluate(currentTime));

        // apply curve to capsule collider
        if (TryGetComponent(out CapsuleCollider collider))
        {
            collider.radius = WaveLerp.Evaluate(currentTime) * 3;

            // remove collider if no wave
            if (currentTime > 1f)
                collider.enabled = false;
        }

        if (debug && startAnimation)
        {
            startAnimation = false;
            startTime = Time.time;
            rocks.GetComponent<ParticleSystem>().Play();
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.tag.Equals("Enemy"))
            greaterCast(collider.gameObject, spellEffectMod, null);
        else if (collider.gameObject.tag.Equals("Player"))
        {
            if (PVPEnabled && PhotonView.Get(collider).ViewID != CasterPView.ViewID)
            {
                greaterCast(collider.gameObject, spellEffectMod, null);
            }
            else
            {
                lesserCast(collider.gameObject, spellEffectMod, null);
            }
        }
    }
}
