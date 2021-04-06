using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PodiumController : MonoBehaviour
{
    public GameObject interactIndicator;
    SpriteRenderer indicatorSprite;

    [SerializeField]
    string[] messages = new string[] { "Test Message 1" };

    [SerializeField]
    Sprite[] images;

    SpellTest playerSpellTest = null;

    bool InRange = false;

    GameManager manager;

    public enum OrbTypes { BlueOrb, BrownOrb, GreenOrb, IndigoOrb, OrangeOrb, RedOrb, VioletOrb, YellowOrb, QuicksilverOrb}

    public OrbTypes podiumType;

    // Start is called before the first frame update
    void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        indicatorSprite = interactIndicator.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            Orb orb = GetCurrentOrb();
            manager.PodiumPopUp(messages, images, orb, playerSpellTest);
        }
    }

    Orb GetCurrentOrb()
    {
        Orb orbType = null;
        switch(podiumType)
        {
            case OrbTypes.BlueOrb:
                orbType = new BlueOrb();
                break;
            case OrbTypes.BrownOrb:
                break;
            case OrbTypes.GreenOrb:
                break;
            case OrbTypes.QuicksilverOrb:
                orbType = new QuickSilverOrb();
                break;
            case OrbTypes.IndigoOrb:
                orbType = new IndigoOrb();
                break;
            case OrbTypes.OrangeOrb:
                orbType = new OrangeOrb();
                break;
            case OrbTypes.RedOrb:
                orbType = new RedOrb();
                break;
            case OrbTypes.VioletOrb:
                orbType = new VioletOrb();
                break;
            case OrbTypes.YellowOrb:
                orbType = new YellowOrb();
                break;
        }

        return orbType;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = true;
                indicatorSprite.enabled = true;
                playerSpellTest = other.gameObject.GetComponent<SpellTest>();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = false;
                indicatorSprite.enabled = false;
                playerSpellTest = null;
            }
        }
    }
}
