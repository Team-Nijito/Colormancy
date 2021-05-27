using MyBox;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class PlayerListManager : MonoBehaviourPunCallbacks
{
    #region Private variables

    // Player displaying (in the room)
    [Separator("Player displaying components")]
    [SerializeField] private PlayerItemUI m_playerItemUIPrefab;
    [SerializeField] private Transform m_playerListParent;

    [Separator("LobbyNetworkManager")]
    [SerializeField] private LobbyNetworkManager m_lobbyNetworkManager;

    private List<PlayerItemUI> m_playerList = new List<PlayerItemUI>();

    #endregion

    #region Private functions

    // instantiate new player GUI to denote that
    // player is in the room
    private void AddNewPlayerItem(Player newPlayer)
    {
        // Check if this player already exists as a GUI before instantiating a new GUI
        int index = m_playerList.FindIndex(x => x.PlayerInfo == newPlayer);
        if (index != -1)
        {
            m_playerList[index].SetPlayerInfo(newPlayer);
        }
        else
        {
            PlayerItemUI newPlayerItem = Instantiate(m_playerItemUIPrefab, m_playerListParent);
            if (newPlayerItem)
            {
                newPlayerItem.SetPlayerInfo(newPlayer);
                m_playerList.Add(newPlayerItem);
            }
        }
    }

    private void GetCurrentRoomPlayers()
    {
        foreach (KeyValuePair<int, Player> pair in PhotonNetwork.CurrentRoom.Players)
        {
            AddNewPlayerItem(pair.Value);
        }
    }

    #endregion

    #region Photon callbacks

    public override void OnJoinedRoom()
    {
        // Update the current player List whenever you join
        GetCurrentRoomPlayers();
    }

    public override void OnLeftRoom()
    {
        // Clear the current player List whenever you leave
        foreach (PlayerItemUI playerItem in m_playerList)
        {
            Destroy(playerItem.gameObject);
        }
        m_playerList.Clear();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        // Update the current player List whenever others join
        AddNewPlayerItem(newPlayer);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        // Whenever an other player exits the room, remove the GUI associated with that player
        int index = m_playerList.FindIndex(x => x.PlayerInfo == otherPlayer);
        if (index != -1)
        {
            Destroy(m_playerList[index].gameObject);
            m_playerList.RemoveAt(index);
        }

        // Check if we're now the master client, so we can wield the power to start the game
        if (PhotonNetwork.IsMasterClient)
        {
            m_lobbyNetworkManager.ActivateStartButton();
        }
    }

    #endregion
}