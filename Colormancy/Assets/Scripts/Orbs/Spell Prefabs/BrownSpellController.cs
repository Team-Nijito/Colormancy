using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrownSpellController : MonoBehaviour
{
    public Orb.GreaterCast greaterCast;
    public Orb.LesserCast lesserCast;
    public int greaterCastAmt;
    public int lesserCastAmt;
    public float spellEffectMod;

    private float startTime;
    [SerializeField]
    private float lifetime;

    [SerializeField]
    private AnimationCurve ImpactLerp;
    [SerializeField]
    private AnimationCurve WaveLerp;

    private GameObject impact;
    private GameObject wave;
    private GameObject rocks;

    private Material impactMat;
    private Material waveMat;

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
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time - startTime;

        impactMat.SetFloat("_Lerp", WaveLerp.Evaluate(currentTime));

        waveMat.SetFloat("_Lerp", WaveLerp.Evaluate(currentTime));

        if (debug && startAnimation)
        {
            startAnimation = false;
            startTime = Time.time;
            rocks.GetComponent<ParticleSystem>().Play();
        }
    }
}
