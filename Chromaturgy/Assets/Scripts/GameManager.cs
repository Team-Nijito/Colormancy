using UnityEngine;
using UnityEngine.SceneManagement;

using Photon.Pun;
using Photon.Realtime;

public class GameManager : MonoBehaviourPunCallbacks

{
    #region Public Fields

    [Tooltip("The prefab to use for representing the player")]
    public GameObject playerPrefab;

    [Tooltip("The prefab for the Health and Mana GUI")]
    public GameObject healthManaBarPrefab;

    #endregion

    #region Private Fields
    #endregion

    #region Photon Methods

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(0);
    }

    #endregion

    #region Public Methods

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    #endregion

    #region Private Methods

    private void Start()
    {
        if (playerPrefab == null)
        {
            Debug.LogError("<Color=Red>Missing player prefab");
        }
        else
        {
            if (PlayerController.LocalPlayerInstance == null)
            {
                if (HealthScript.LocalPlayerInstance == null)
                {
                    GameObject player = PhotonNetwork.Instantiate(this.playerPrefab.name, new Vector3(0f, 3f, 0f), Quaternion.identity, 0);

                    // Instantiate the health/mana GUI after instantiating the player
                    //GameObject playerUI = Instantiate(healthManaBarPrefab);
                    //playerUI.GetComponent<PlayerGUI>().SetTarget(player);
                }
                else
                {
                    Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
                }
            }
            else
            {
                Debug.LogFormat("Ignoring scene load for {0}", SceneManagerHelper.ActiveSceneName);
            }
        }
    }

    void LoadArena()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("PhotonNetwork::Trying to load level but not master");
        }
        PhotonNetwork.LoadLevel("SampleScene");
    }

    #endregion
}
