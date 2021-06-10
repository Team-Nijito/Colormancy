using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void OnStartGamePressed()
    {
        SceneManager.LoadScene("Lobby");
    }

    public void OnQuitPressed()
    {
        Application.Quit();
    }
}
