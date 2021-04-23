using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastLevel;
    public int lesserCastLevel;
    public float spellEffectMod;

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

    [Space]

    private float startTime;
    [SerializeField]
    private float lifetime;

    [Space]

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

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

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius);
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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
            greaterCast(collision.gameObject, greaterCastLevel, spellEffectMod);
        else if (collision.gameObject.tag.Equals("Player"))
            lesserCast(collision.gameObject, lesserCastLevel, spellEffectMod);
    }
}
