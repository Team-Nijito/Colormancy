using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedSpellController : MonoBehaviour
{
    public Transform playerTransform;
    private GameObject playerObject;
    private Vector3 startPosition;
    public Vector3 endPosition;

    [SerializeField]
    private float jumpTime;

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

    private bool landed;
    private bool toggleLanding;

    // Start is called before the first frame update
    void Start()
    {
        baseMaterial = redBase.GetComponent<Renderer>().material;
        edgeMaterial = redEdge.GetComponent<Renderer>().material;

        playerObject = playerTransform.gameObject;
        playerObject.GetComponent<PlayerMovement>().enabled = false;
        startPosition = playerTransform.position;

    }

    void OnEnable()
    {
        starttime = Time.time;

        PaintingManager.PaintSphere(paintColor, transform.position, spherePaintRadius);
    }

    // Update is called once per frame
    void Update()
    {
        baseMaterial.SetFloat("_Lerp", m_lerp);
        edgeMaterial.SetFloat("_Lerp", m_lerp);

        if (landed)
        {
            toggleLanding = true;
        }

        if (Time.time - starttime < jumpTime)
        {
            playerObject.GetComponent<CharacterController>().Move(endPosition - startPosition + Vector3.up);
        }
        else
        {
            landed = true;
        }


        //if (Time.time - starttime > lifetime && !debug)
        //Destroy(gameObject);
    }
}
