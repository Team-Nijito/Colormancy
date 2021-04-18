using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcceptButtonHandler : MonoBehaviour
{
    Button button;
    GameManager gameManager;

    // Start is called before the first frame update
    private void Start()
    {
        button = GetComponent<Button>();
        button.onClick.AddListener(GiveOrb);
        gameManager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    public void GiveOrb()
    {
        gameManager.AddCurrentOrb();
    }

    public void RemoveOrb()
    {
        gameManager.RemoveCurrentOrb();
    }
}
