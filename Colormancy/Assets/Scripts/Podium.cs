using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Podium : MonoBehaviour
{
    protected SpriteRenderer indicatorSprite;

    [SerializeField]
    protected string[] messages = new string[] { "Test Message 1" };

    [SerializeField]
    protected Sprite[] images;

    protected GameManager manager;

    protected bool InRange = false;

    public virtual void CloseWindow()
    {
        InRange = false;
        indicatorSprite.enabled = false;
    }

    protected virtual void Start()
    {
        manager = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>();
        indicatorSprite = GetComponentInChildren<SpriteRenderer>();
    }

    protected virtual void Update()
    {
        if (Input.GetMouseButtonDown(0) && InRange)
        {
            manager.PopUp(messages, images);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = true;
                indicatorSprite.enabled = true;
            }
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (other.gameObject.GetComponent<PhotonView>().IsMine)
            {
                InRange = false;
                indicatorSprite.enabled = false;
                manager.CloseWindowVisually();
            }
        }
    }
}
