using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpGenericButton : MonoBehaviour
{
    public enum ButtonType { NextButton, CloseButton}

    Button button;
    GameManager gameManager;
    public ButtonType buttonType;

    // Start is called before the first frame update
    void Start()
    {

        button = GetComponent<Button>();
        button.onClick.AddListener(OnButtonPress);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public void OnButtonPress()
    {
        switch (buttonType)
        {
            case ButtonType.NextButton:
                gameManager.NextPage();
                break;
            case ButtonType.CloseButton:
                gameManager.CloseWindow();
                break;
            default:
                Debug.LogWarning("Haven't implemented this AcceptMode! (see AcceptButtonHandler.cs line 46)");
                break;
        }
    }
}
