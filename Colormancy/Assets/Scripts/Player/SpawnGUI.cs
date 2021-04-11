using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class SpawnGUI : MonoBehaviourPunCallbacks
{
    [SerializeField]
    GameObject healthManaPrefab;

    GameObject BarPanel = null;

    [SerializeField]
    GameObject SpellUI;

    private void Start()
    {
        ResetUIAfterSceneLoad();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        //Add new players health bar when they join the game
        int GUIPosNumber = PhotonNetwork.CurrentRoom.PlayerCount - 1;
        GameObject newBar = Instantiate(healthManaPrefab, new Vector2(0, GUIPosNumber * 150), Quaternion.identity);
        newBar.transform.SetParent(BarPanel.transform);
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in photonViews)
        {
            Player player = view.Owner;
            if (player == newPlayer)
            {
                newBar.GetComponent<PlayerGUI>().SetTarget(view.gameObject);
            }
        }
        //print("OnPlayerEnteredRoom::created remote GUI");
    }

    void SpawnExistingPlayersHealthBars()
    {
        GameObject localPlayer = null;
        //Check if there are any characters in the room already if so add their health bars
        PhotonView[] photonViews = FindObjectsOfType<PhotonView>();
        //print("There are " + PhotonNetwork.CurrentRoom.PlayerCount + " players in the room."); commented this print statement -w
        //print("Found " + photonViews.Length + " photon views on joining");
        int remoteIndex = 1;
        foreach (PhotonView view in photonViews)
        {
            Player player = view.Owner;
            if (player != null)
            {
                //print("Looking to add GUI for player " + player.NickName);
                if (view.IsMine)
                {
                    localPlayer = view.gameObject;
                }
                else
                {
                    GameObject remoteBar = Instantiate(healthManaPrefab, new Vector2(0, remoteIndex * 150), Quaternion.identity);
                    remoteBar.transform.SetParent(BarPanel.transform);
                    remoteBar.GetComponent<PlayerGUI>().SetTarget(view.gameObject);
                    //print("Created remote health bar for player " + remoteIndex++);
                }
            }
        }

        //Create local players health bar at bottom left of screen
        GameObject bar = Instantiate(healthManaPrefab, new Vector2(0, 0), Quaternion.identity);
        bar.transform.SetParent(BarPanel.transform);
        bar.GetComponent<PlayerGUI>().SetTarget(localPlayer);
        //print("Created local health bar");
    }

    public void ResetUIAfterSceneLoad()
    {
        BarPanel = GameObject.Find("BarPanel");

        if (BarPanel && photonView.IsMine)
        {
            Transform canvas = GameObject.Find("Canvas").transform;
            GameObject g_SpellUI = Instantiate(SpellUI, canvas.transform.position, Quaternion.identity, canvas);
            Vector2 uiPos = g_SpellUI.GetComponent<RectTransform>().anchoredPosition;
            g_SpellUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(uiPos.x + 85, 0);

            SpawnExistingPlayersHealthBars();
        }
    }
}
