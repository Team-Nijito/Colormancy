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

    // Start is called before the first frame update
    [ExecuteInEditMode]
    void Start()
    {
        baseMaterial = redBase.GetComponent<Renderer>().material;
        edgeMaterial = redEdge.GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    [ExecuteInEditMode]
    void Update()
    {
        baseMaterial.SetFloat("_Lerp", m_lerp);
        edgeMaterial.SetFloat("_Lerp", m_lerp);
    }
}
