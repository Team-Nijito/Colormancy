using MyBox;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class RoomListManager : MonoBehaviourPunCallbacks
{
    #region Private variables

    // Room displaying (in the lobby)
    [Separator("Room displaying components")]
    [SerializeField] private RoomItemUI m_roomItemUIPrefab;
    [SerializeField] private Transform m_roomListParent;

    [Separator("LobbyNetworkManager")]
    [SerializeField] private LobbyNetworkManager m_lobbyNetworkManager;

    private List<RoomItemUI> m_roomList = new List<RoomItemUI>();

    #endregion

    #region Photon callbacks

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (RoomInfo info in roomList)
        {
            // Room is being removed from the list
            if (info.RemovedFromList)
            {
                int index = m_roomList.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index != -1)
                {
                    Destroy(m_roomList[index].gameObject);
                    m_roomList.RemoveAt(index);
                }
            }
            // Room is being added to the list
            else
            {
                // Do a preliminary check to see if the room already exists and we're just updating the player count
                int index = m_roomList.FindIndex(x => x.RoomInfo.Name == info.Name);
                if (index == -1 && info.IsVisible)
                {
                    // Doesn't exist, so instantiate a new one
                    RoomItemUI newRoomItem = Instantiate(m_roomItemUIPrefab, m_roomListParent);
                    newRoomItem.SetRoomInfo(info);
                    newRoomItem.CheckMaxCapacity();
                    m_roomList.Add(newRoomItem);
                }
                else
                {
                    // Modify the existing item here with the new information
                    m_roomList[index].SetRoomInfo(info);
                    m_roomList[index].CheckMaxCapacity();
                }
            }
        }
    }

    public override void OnJoinedRoom()
    {
        // Clear the list of rooms whenever you join a room
        foreach (RoomItemUI roomitem in m_roomList)
        {
            Destroy(roomitem.gameObject);
        }
        m_roomList.Clear();
    }

    #endregion
}
