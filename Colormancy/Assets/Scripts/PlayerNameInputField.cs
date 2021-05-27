using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;

namespace Com.MyCompany.Name
{
    //Player name input field. Let user input their name, will appear above player in game
    [RequireComponent(typeof(InputField))]
    public class PlayerNameInputField : MonoBehaviour
    {
        #region Private Constants

        // Store PlayerPref
        const string playerNamePrefKey = "PlayerName";

        [SerializeField]
        private Text m_nameDisplayText; // this text will be updated with the character's name as you type

        #endregion

        #region MonoBehaviour Callbacks

        // Start is called before the first frame update
        void Start()
        {
            string defaultName = string.Empty;
            InputField _inputField = GetComponent<InputField>();
            if (_inputField != null)
            {
                if (PlayerPrefs.HasKey(playerNamePrefKey))
                {
                    defaultName = PlayerPrefs.GetString(playerNamePrefKey);
                    _inputField.text = defaultName;
                }
            }

            if (string.IsNullOrEmpty(defaultName))
            {
                // If you don't have a saved name, generate a random name.
                defaultName = "Player" + Random.Range(0, 5000);
            }
            PhotonNetwork.NickName = defaultName;
        }

        #endregion

        #region Public Methods

        //Sets name of player, and save it in PlayerPrefs for future
        public void SetPlayerName(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                // If you have an empty name, just generate a random name
                PhotonNetwork.NickName = "Player" + Random.Range(0, 5000);
            }
            else
            {
                PhotonNetwork.NickName = value;
                PlayerPrefs.SetString(playerNamePrefKey, value);
            }

            if (m_nameDisplayText)
            {
                m_nameDisplayText.text = PhotonNetwork.NickName;
            }
        }

        #endregion
    }
}
