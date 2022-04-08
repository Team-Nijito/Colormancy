using Photon.Pun;
using PhotonHashtable = ExitGames.Client.Photon.Hashtable; // to use with Photon's CustomProperties
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemPodium : Podium
{
    //[SerializeField]
    //private string[] returnMessage = new string[] { "Do you want to return your orb?" }; // the user has the orb, and is at the podium where they retrieved the orb at
    //[SerializeField]
    //private string[] maxCapMessage = new string[] { "You cannnot pick up any more orbs!" }; // the orb has been claimed by someone else
    //[SerializeField]
    //private string[] missingMessage = new string[] { "Somebody else currently has this orb" }; // the orb has been claimed by someone else
    //[SerializeField]
    //private string[] waitingMessage = new string[] { "Somebody else is currently browsing this orb" }; // somebody else is currently looking at the orb

    [SerializeField]
    private ItemSO m_item;
    private ItemManager m_itemManager = null;

    //public enum OrbTypes { None, BlueOrb, BrownOrb, GreenOrb, IndigoOrb, OrangeOrb, QuicksilverOrb, RedOrb, VioletOrb, YellowOrb }

    //public OrbTypes podiumType;

    public override void CloseWindow()
    {
        base.CloseWindow();
        m_itemManager = null;
    }

    protected override void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            messages[0] = m_item.itemName;
            //Uses name of ItemSO to pass into GameManager, we can change this to a string within the SO but this works for now
            manager.PopUpItem(messages, images, m_item.itemScriptName, m_itemManager, this);
            manager.ChangeGUIMode(AcceptButtonHandler.AcceptMode.GiveItem);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PhotonView playerView = PhotonView.Get(other.gameObject);
            if (playerView.IsMine)
            {
                //if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(FetchOrbKey(podiumType), out object obj))
                //{
                //    int[] orbInfo = (int[])obj; // first element, ID that is perusing orb, second element, ID that has obtained and now owns the orb,
                //    if (orbInfo[0] == -1)
                //    {
                //        // nobody is currently browsing the orb
                //        orbInfo[0] = PhotonNetwork.LocalPlayer.ActorNumber;

                //        // update the room properties to show that you're perusing this orb podium
                //        PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                //        roomProperties[FetchOrbKey(podiumType)] = orbInfo;
                //        PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GameManager.OrbsNeededKey, out object num);

                //        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);

                //        if (orbInfo[1] == -1)
                //        {
                //            // nobody owns this
                //            //OrbTypes OrbOwned = (OrbTypes)PhotonNetwork.LocalPlayer.CustomProperties[GameManager.OrbOwnedInLobbyKey];

                //            int playerOrbCount = ((GameObject)PhotonNetwork.LocalPlayer.TagObject).GetComponent<OrbManager>().orbs.Count;

                //            if (playerOrbCount < (int)num) //OrbOwned == OrbTypes.None
                //            {
                //                // nobody currently "owns" this orb, so present the normal stuff
                //                podiumStatus = PodiumStatus.Available;
                //            }
                //            else
                //            {
                //                // you can only have one orb
                //                podiumStatus = PodiumStatus.AlreadyHaveOrb;
                //            }
                //        }
                //        else if (orbInfo[1] == PhotonNetwork.LocalPlayer.ActorNumber)
                //        {
                //            // you own this, are you returning this?
                //            podiumStatus = PodiumStatus.Returnable;
                //        }
                //        else
                //        {
                //            // somebody else has this orb
                //            podiumStatus = PodiumStatus.OutOfStock;
                //        }
                //    }
                //    else
                //    {
                //        // somebody else is currently looking at the orb
                //        podiumStatus = PodiumStatus.Waiting;
                //    }
                //}

                InRange = true;
                indicatorSprite.enabled = true;
                m_itemManager = other.gameObject.GetComponent<ItemManager>();
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
                //if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(FetchOrbKey(podiumType), out object obj))
                //{
                //    int[] orbInfo = (int[])obj; // first element, ID that is perusing orb, second element, ID that has obtained and now owns the orb

                //    if (orbInfo[0] == PhotonNetwork.LocalPlayer.ActorNumber)
                //    {
                //        // we're no longer perusing the orb
                //        orbInfo[0] = -1;

                //        // update the room properties to show that you're no longer perusing this orb podium
                //        PhotonHashtable roomProperties = PhotonNetwork.CurrentRoom.CustomProperties;
                //        roomProperties[FetchOrbKey(podiumType)] = orbInfo;

                //        PhotonNetwork.CurrentRoom.SetCustomProperties(roomProperties);
                //    }
                //}
                CloseWindow();
                manager.CloseWindowVisually();
            }
        }
    }
}
