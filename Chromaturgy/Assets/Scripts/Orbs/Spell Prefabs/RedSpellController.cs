using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpellController : MonoBehaviour
{
    [SerializeField]
    [Range(0, 1)]
    private float m_lerp;

    [SerializeField]
    private GameObject redBase;
    [SerializeField]
    private GameObject redEdge;
    private Material baseMaterial;
    private Material edgeMaterial;

    private float starttime;
    [SerializeField]
    private float lifetime;

    [SerializeField]
    private float spherePaintRadius;
    [SerializeField]
    private Color paintColor;

    [SerializeField]
    private bool debug;

    // Start is called before the first frame update
    void Start()
    {
        baseMaterial = redBase.GetComponent<Renderer>().material;
        edgeMaterial = redEdge.GetComponent<Renderer>().material;
    }

    void OnEnable()
    {
        starttime = Time.time;

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius, 0.8f);
    }

    // Update is called once per frame
    void Update()
    {
        baseMaterial.SetFloat("_Lerp", m_lerp);
        edgeMaterial.SetFloat("_Lerp", m_lerp);

        if (Time.time - starttime > lifetime && !debug)
            Destroy(gameObject);
    }
}
