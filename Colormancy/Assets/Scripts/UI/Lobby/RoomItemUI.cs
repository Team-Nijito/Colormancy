using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviour
{
    #region Private variables

    public RoomInfo RoomInfo { get; private set; } // infomation of the room associated with this GUI

    [SerializeField] private Button m_joinButton;

    [SerializeField] private Text m_roomName;
    [SerializeField] private Text m_playerCount; // amount of players and capacity of players

    #endregion

    #region Public functions

    /// <summary>
    /// Check if the room is full, if it is, disable the JoinButton!
    /// </summary>
    public void CheckMaxCapacity()
    {
        if (RoomInfo.PlayerCount == RoomInfo.MaxPlayers)
        {
            m_joinButton.interactable = false;
        }
        else
        {
            m_joinButton.interactable = true;
        }
    }

    /// <summary>
    /// If you press the join button associated with the GUI, you join the room
    /// </summary>
    public void OnJoinPressed()
    {
        PhotonNetwork.JoinRoom(m_roomName.text);
    }

    /// <summary>
    /// Invoked whenever the room information is updated.
    /// </summary>
    /// <param name="newRoomInfo">The new room info</param>
    public void SetRoomInfo(RoomInfo newRoomInfo)
    {
        RoomInfo = newRoomInfo;

        m_roomName.text = newRoomInfo.Name;
        
        // Update text fields
        m_playerCount.text = newRoomInfo.PlayerCount + "/" + newRoomInfo.MaxPlayers;
    }

    #endregion
}