using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PodiumController : MonoBehaviour
{
    SpellTest playerSpellTest = null;

    bool InRange = false;

    public enum OrbTypes { BlueOrb, BrownOrb, GreenOrb, IndigoOrb, OrangeOrb, RedOrb, VioletOrb, YellowOrb, QuicksilverOrb}

    public OrbTypes podiumType;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            AddCurrentSpellOrb();
        }
    }

    void AddCurrentSpellOrb()
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

        if (orbType != null)
            playerSpellTest.AddSpellOrb(orbType);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = true;
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
                playerSpellTest = null;
            }
        }
    }
}
