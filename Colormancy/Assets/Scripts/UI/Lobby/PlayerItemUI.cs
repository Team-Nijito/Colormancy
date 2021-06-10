using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class PlayerItemUI : MonoBehaviour
{
    public Player PlayerInfo { get; private set; } // infomation of player associated with this GUI

    [SerializeField]
    private Text m_playerName;

    public void SetPlayerInfo(Player playerInfo)
    {
        PlayerInfo = playerInfo;
        m_playerName.text = playerInfo.NickName;
    }
}
