using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RoomItemUI : MonoBehaviourPunCallbacks
{
    #region Private variables

    public RoomInfo RoomInfo { get; private set; } // infomation of the room associated with this GUI

    [SerializeField] private Button m_joinButton;

    [SerializeField] private Text m_roomName;
    [SerializeField] private Text m_playerCount; // amount of players and capacity of players

    #endregion

    #region Photon callbacks

    public override void OnLeftRoom()
    {
        m_joinButton.interactable = true;
    }

    public override void OnJoinedRoom()
    {
        StopAllCoroutines();
        m_joinButton.gameObject.GetComponent<Image>().color = Color.white;
    }

    #endregion

    #region Private functions

    /// <summary>
    /// When you mess up, the join button will be noninteractable, turn red, and tell you why.
    /// </summary>
    /// <param name="buttonToDisplayError">The button that will be noninteractable for the duration</param>
    /// <param name="errorMsg">Error message to display</param>
    /// <param name="durationDisplayError">Duration button is errored.</param>
    /// <returns></returns>
    private IEnumerator ButtonError(string errorMsg = "Error", float durationDisplayError = 1.5f)
    {
        m_joinButton.interactable = false;

        Image buttonImage = m_joinButton.gameObject.GetComponent<Image>(); // image component that is the sibling of the button component
        Text buttonText = m_joinButton.transform.GetComponentInChildren<Text>();

        string originalMessage = buttonText.text;

        buttonImage.color = Color.red;
        buttonText.text = errorMsg;

        yield return new WaitForSecondsRealtime(durationDisplayError);

        buttonImage.color = Color.white;
        buttonText.text = originalMessage;

        m_joinButton.interactable = true;
    }

    /// <summary>
    /// This is a simple 1 second delay wait and check.
    /// This already assumes the button has been pressed and is non interactable and then makes it interactable again when it's done.
    /// </summary>
    private IEnumerator CheckIfYouHaveJoinedTheRoom()
    {
        Text buttonText = m_joinButton.GetComponentInChildren<Text>();
        string originalJoinButtonText = buttonText.text;
        buttonText.text = "Attempting to join...";

        yield return new WaitForSecondsRealtime(1f);
        buttonText.text = originalJoinButtonText; // set the text back to the original before invoking the ButtonErrorWrapper
        m_joinButton.interactable = true;

        if (!PhotonNetwork.InRoom)
        {
            StartCoroutine(ButtonError("Can't join room"));
        }
    }

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
        m_joinButton.interactable = false; // prevent joining the room again when you're already joining the room

        PhotonNetwork.JoinRoom(m_roomName.text);
        CheckIfYouHaveJoinedTheRoom();
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