using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; // to use with Photon's CustomProperties

public class OrbPodium : Podium
{
    [SerializeField]
    private string[] returnMessage = new string[] { "Do you want to return your orb?" }; // the user has the orb, and is at the podium where they retrieved the orb at
    [SerializeField]
    private string[] maxCapMessage = new string[] { "You cannnot pick up any more orbs!" }; // the orb has been claimed by someone else
    [SerializeField]
    private string[] missingMessage = new string[] { "Somebody else currently has this orb" }; // the orb has been claimed by someone else
    [SerializeField]
    private string[] waitingMessage = new string[] { "Somebody else is currently browsing this orb" }; // somebody else is currently looking at the orb

    private OrbManager m_orbManager = null;

    public enum OrbTypes { None, BlueOrb, BrownOrb, GreenOrb, IndigoOrb, OrangeOrb, QuicksilverOrb, RedOrb, VioletOrb, YellowOrb }

    public OrbTypes orbType;

    // Update is called once per frame
    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            Orb orb = GetCurrentOrb();
            if (podiumStatus == PodiumStatus.Available)
            {
                manager.PopUpOrb(messages, images, orb, m_orbManager, this);
                manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.GiveOrb);
            }
            else if (podiumStatus == PodiumStatus.Waiting)
            {
                manager.PopUpOrb(waitingMessage, new Sprite[] { }, orb, m_orbManager, this);
                manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.CloseWindow);
            }
            else if (podiumStatus == PodiumStatus.OutOfStock)
            {
                manager.PopUpOrb(missingMessage, new Sprite[] { }, orb, m_orbManager, this);
                manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.CloseWindow);
            }
            else if (podiumStatus == PodiumStatus.Returnable)
            {
                manager.PopUpOrb(returnMessage, new Sprite[] { }, orb, m_orbManager, this);
                manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.RemoveOrb);
            }
            else if (podiumStatus == PodiumStatus.AlreadyHaveOrb)
            {
                manager.PopUpOrb(maxCapMessage, new Sprite[] { }, orb, m_orbManager, this);
                manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.CloseWindow);
            }
        }

        if (indicatorSprite.enabled && Camera.main)
        {
            indicatorSprite.transform.LookAt(Camera.main.transform.position, Vector3.up);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerView = PhotonView.Get(other.gameObject);
            if (playerView.IsMine)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(FetchOrbKey(orbType), out object obj))
                {
                    int[] orbInfo = (int[])obj; // first element, ID that is perusing orb, second element, ID that has obtained and now owns the orb, third element is number of orbs the player currently 
                    if (orbInfo[0] == -1)
                    {
                        // nobody is currently browsing the orb
                        orbInfo[0] = PhotonNetwork.LocalPlayer.ActorNumber;

                        // update the room properties to show that you're perusing this orb podium
                        PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                        roomProperties[FetchOrbKey(orbType)] = orbInfo;
                        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameManager.OrbsNeededKey, out object num);

                        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

                        if (orbInfo[1] == -1)
                        {
                            // nobody owns this
                            //OrbTypes OrbOwned = (OrbTypes)PhotonNetwork.LocalPlayer.CustomProperties[GameManager.OrbOwnedInLobbyKey];

                            int playerOrbCount = ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<OrbManager>().orbs.Count;
                            
                            if (playerOrbCount < (int)num) //OrbOwned == OrbTypes.None
                            {
                                // nobody currently "owns" this orb, so present the normal stuff
                                podiumStatus = PodiumStatus.Available;
                            }
                            else
                            {
                                // you can only have one orb
                                podiumStatus = PodiumStatus.AlreadyHaveOrb;
                            }
                        }
                        else if (orbInfo[1] == PhotonNetwork.LocalPlayer.ActorNumber)
                        {
                            // you own this, are you returning this?
                            podiumStatus = PodiumStatus.Returnable;
                        }
                        else
                        {
                            // somebody else has this orb
                            podiumStatus = PodiumStatus.OutOfStock;
                        }
                    }
                    else
                    {
                        // somebody else is currently looking at the orb
                        podiumStatus = PodiumStatus.Waiting;
                    }

                    InRange = true;
                    indicatorSprite.enabled = true;
                    m_orbManager = other.gameObject.GetComponent<OrbManager>();
                }
            }
        }
    }

    protected override void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerView = PhotonView.Get(other.gameObject);
            if (playerView.IsMine)
            {
                if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(FetchOrbKey(orbType), out object obj))
                {
                    int[] orbInfo = (int[])obj; // first element, ID that is perusing orb, second element, ID that has obtained and now owns the orb

                    if (orbInfo[0] == PhotonNetwork.LocalPlayer.ActorNumber)
                    {
                        // we're no longer perusing the orb
                        orbInfo[0] = -1;

                        // update the room properties to show that you're no longer perusing this orb podium
                        PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                        roomProperties[FetchOrbKey(orbType)] = orbInfo;

                        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                    }
                }
                CloseWindow();
                manager.CloseWindowVisually();
            }
        }
    }

    /// <summary>
    /// Invoke this whenever the player has accepted the orb so that they cannot fetch more than one of the same orbs in the lobby.
    /// </summary>
    public override void CloseWindow()
    {
        base.CloseWindow();
        m_orbManager = null;
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

    Orb GetCurrentOrb()
    {
        Orb orbType = null;
        switch (this.orbType)
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
}
