
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptButtonHandler : MonoBehaviour
{
    Button button;
    GameManager gameManager;
    Text textObj;

    public enum AcceptMode { GiveOrb, RemoveOrb, CloseWindow, GiveItem }

    public AcceptMode CurrentMode { get { return m_currentMode; } private set { m_currentMode = value; } }
    private AcceptMode m_currentMode = AcceptMode.GiveOrb;

    // Start is called before the first frame update
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPress);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public void OnButtonPress()
    {
        switch (m_currentMode) 
        {
            case AcceptMode.GiveOrb:
                gameManager.AddCurrentOrb();
                break;
            case AcceptMode.RemoveOrb:
                gameManager.RemoveCurrentOrb();
                break;
            case AcceptMode.CloseWindow:
                gameManager.CloseWindow();
                break;
            case AcceptMode.GiveItem:
                gameManager.AddCurrentItem();
                break;
            default:
                Debug.LogWarning("Haven't implemented this AcceptMode! (see AcceptButtonHandler.cs line 46)");
                break;
        }
    }

    public void ChangeCurrentMode(AcceptMode newMode)
    {
        m_currentMode = newMode;
        if (textObj == null)
        {
            textObj = transform.Find("Text").GetComponent<Text>();
        }
        switch (m_currentMode)
        {
            case AcceptMode.GiveItem:
            case AcceptMode.GiveOrb:
                textObj.text = "Accept";
                break;
            case AcceptMode.RemoveOrb:
                textObj.text = "Return orb";
                break;
            case AcceptMode.CloseWindow:
                textObj.text = "Close";
                break;
            default:
                Debug.LogWarning("Haven't implemented this AcceptMode! (see AcceptButtonHandler.cs line 46)");
                break;
        }
    }
}
