using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class DialogueController : MonoBehaviour
{
    [SerializeField]
    string[] messages = new string[] { "Test Message 1" };

    [SerializeField]
    Sprite[] images;
    
    GameManager manager;

    bool InRange = false;

    private void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            manager.PopUp(messages, images);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = false;
                manager.CloseWindow();
            }
        }
    }
}
