using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSilverSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastAmt;
    public int lesserCastAmt;
    public float spellEffectMod;

    [Space]

    private GameObject bolt;
    private GameObject contact;
    private GameObject wave;
    private GameObject sparks;

    [SerializeField]
    private AnimationCurve BoltDisplacementSpeed;
    [SerializeField]
    private AnimationCurve BoltDisplacement;
    [SerializeField]
    private AnimationCurve BoltThickness;
    [SerializeField]
    private AnimationCurve BoltWidth;
    [SerializeField]
    private AnimationCurve WaveLerp;
    [SerializeField]
    private AnimationCurve ContactRadius;

    [Space]

    private Material boltMat;
    private Material contactMat;
    private Material waveMat;
    private Light sparkLight;

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

    void Start()
    {
        startTime = Time.time;

        bolt = transform.GetChild(0).gameObject;
        boltMat = bolt.GetComponent<Renderer>().material;

        contact = transform.GetChild(3).gameObject;
        contactMat = contact.GetComponent<Renderer>().material;

        wave = transform.GetChild(1).gameObject;
        waveMat = wave.GetComponent<Renderer>().material;

        sparks = transform.GetChild(2).gameObject;
        sparks.GetComponent<ParticleSystem>().Play();

        sparkLight = GetComponent<Light>();

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius);
    }

    void Update()
    {
        float currentTime = Time.time - startTime;

        if (Time.time - startTime > lifetime && !debug)
            Destroy(gameObject);

        // apply curves to shaders
        boltMat.SetFloat("_ScrollSpeed", BoltDisplacementSpeed.Evaluate(currentTime));
        boltMat.SetFloat("_DisplacementStrength", BoltDisplacement.Evaluate(currentTime));
        boltMat.SetFloat("_Thickness", BoltThickness.Evaluate(currentTime));
        boltMat.SetFloat("_Width", BoltWidth.Evaluate(currentTime));

        waveMat.SetFloat("_Lerp", WaveLerp.Evaluate(currentTime));

        contactMat.SetFloat("_Radius", ContactRadius.Evaluate(currentTime));
        sparkLight.intensity = (ContactRadius.Evaluate(currentTime) + 0.5f) * 5;

        // make spell face the camera
        Vector3 lookPoint = Camera.main.transform.position;
        lookPoint.y = bolt.transform.position.y;
        bolt.transform.LookAt(lookPoint);

        // apply curve to capsule collider
        if (TryGetComponent(out CapsuleCollider collider))
        {
            collider.radius = WaveLerp.Evaluate(currentTime) * 3;

            // remove collider if no wave
            if (currentTime > 0.5f)
                collider.enabled = false;
        }

        // handle replaying
        if (debug && startAnimation)
        {
            startAnimation = false;
            startTime = Time.time;
            sparks.GetComponent<ParticleSystem>().Play();

            collider.enabled = true;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag.Equals("Enemy"))
            greaterCast(collision.gameObject, greaterCastAmt, spellEffectMod);
        else if (collision.gameObject.tag.Equals("Player"))
            lesserCast(collision.gameObject, lesserCastAmt, spellEffectMod);
    }
}
