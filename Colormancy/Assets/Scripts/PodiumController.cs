using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PodiumController : MonoBehaviour
{
    public GameObject interactIndicator;
    private SpriteRenderer indicatorSprite;

    [SerializeField]
    private string[] messages = new string[] { "Test Message 1" };
    [SerializeField]
    private string[] returnMessages = new string[] { "bro you want to return the orb?" }; // the user has the orb, and is at the podium where they retrieved the orb at
    [SerializeField]
    private string[] missingMessages = new string[] { "the orb is in another castle, dingus" }; // the orb has been claimed by someone else

    [SerializeField]
    private Sprite[] images;

    private SpellTest playerSpellTest = null;

    public bool InRange { get { return m_inRange; } }
    public bool IsReturning { get { return m_isReturning; } }

    private bool m_inRange = false;
    private bool m_isReturning = false;

    private GameManager manager;

    public enum OrbTypes { None, BlueOrb, BrownOrb, GreenOrb, IndigoOrb, OrangeOrb, QuicksilverOrb, RedOrb, VioletOrb, YellowOrb}

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
            manager.PodiumPopUp(messages, images, orb, playerSpellTest, this);
        }

        if (indicatorSprite.enabled)
        {
            interactIndicator.transform.LookAt(Camera.main.transform.position, Vector3.up);
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
                orbType = new BrownOrb();
                break;
            case OrbTypes.GreenOrb:
                orbType = new GreenOrb();
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
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerView = PhotonView.Get(other.gameObject);
            if (playerView.IsMine)
            {
                object intObject;
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(FetchOrbKey(podiumType), out intObject))
                {
                    int PhotonID = (int)intObject;

                    if (PhotonID == -1)
                    {
                        // nobody owns this
                        OrbTypes OrbOwned = (OrbTypes)PhotonNetwork.LocalPlayer.CustomProperties[GameManager.OrbOwnedInLobbyKey];
                        if (OrbOwned == OrbTypes.None)
                        {
                            // nobody currently "owns" this orb, so present the normal stuff
                            if (playerView.IsMine)
                            {
                                m_inRange = true;
                                indicatorSprite.enabled = true;
                                playerSpellTest = other.gameObject.GetComponent<SpellTest>();
                            }
                        }
                        else
                        {
                            print("You cannot pick up this orb!");
                        }
                    }
                    else if (PhotonID == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        // you own this, are you returning this?

                        print("Are you going to return this?");
                    }
                    else
                    {
                        print("Someone jacked this orb");
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerView = PhotonView.Get(other.gameObject);
            if (playerView.IsMine)
            {
                CloseWindow();
                manager.CloseWindowVisually();
            }
        }
    }

    /// <summary>
    /// Invoke this whenever the player has accepted the orb so that they cannot fetch more than one of the same orbs in the lobby.
    /// </summary>
    public void CloseWindow()
    {
        m_inRange = false;
        indicatorSprite.enabled = false;
        playerSpellTest = null;
    }

    /// <summary>
    /// Map an OrbTypes enum to a GameManager.[Orb]key string in order to utilize both room and player CustomProperties.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string FetchOrbKey(OrbTypes type)
    {
        switch (type)
        {
            case OrbTypes.RedOrb:
                return GameManager.RedOrbKey;
            case OrbTypes.OrangeOrb:
                return GameManager.OrangeOrbKey;
            case OrbTypes.YellowOrb:
                return GameManager.YellowOrbKey;
            case OrbTypes.GreenOrb:
                return GameManager.GreenOrbKey;
            case OrbTypes.BlueOrb:
                return GameManager.BlueOrbKey;
            case OrbTypes.VioletOrb:
                return GameManager.VioletOrbKey;
            case OrbTypes.BrownOrb:
                return GameManager.BrownOrbKey;
            case OrbTypes.QuicksilverOrb:
                return GameManager.QuicksilverOrbKey;
            case OrbTypes.IndigoOrb:
                return GameManager.IndigoOrbKey;
            default:
                return "InvalidKey";
        }
    }

    /// <summary>
    /// Map a GameManager.[Orb]key to a Orb.Element enum in order to utilize both room and player CustomProperties.
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static OrbTypes FetchOrbType(string type)
    {
        switch (type)
        {
            case GameManager.RedOrbKey:
                return OrbTypes.RedOrb;
            case GameManager.OrangeOrbKey:
                return OrbTypes.OrangeOrb;
            case GameManager.YellowOrbKey:
                return OrbTypes.YellowOrb;
            case GameManager.GreenOrbKey:
                return OrbTypes.GreenOrb;
            case GameManager.BlueOrbKey:
                return OrbTypes.BlueOrb;
            case GameManager.VioletOrbKey:
                return OrbTypes.VioletOrb;
            case GameManager.BrownOrbKey:
                return OrbTypes.BrownOrb;
            case GameManager.QuicksilverOrbKey:
                return OrbTypes.QuicksilverOrb;
            case GameManager.IndigoOrbKey:
                return OrbTypes.IndigoOrb;
            default:
                return OrbTypes.IndigoOrb;
        }
    }
}
