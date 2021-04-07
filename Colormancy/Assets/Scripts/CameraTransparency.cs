using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CameraTransparency : MonoBehaviour
{

    private GameObject m_player;
    private float m_currentPlayerDistance;
    private RaycastHit[] hits;

    private void Start()
    {
        m_player = GameObject.FindWithTag("Player");
        // hits is initialized as an empty list to prevent runtime errors.
        hits = new RaycastHit[] { };
    }

    private void Update()
    {
        m_currentPlayerDistance = CalculateCameraPlayerDistance();

        UpdatePastHits();

        hits = Physics.RaycastAll(transform.position, transform.forward, 100.0F);

        UpdateCurrentHits();
    }

    private float CalculateCameraPlayerDistance()
    {
        // Calculate the distance between the camera and the player.
        return Vector3.Distance(m_player.transform.position, transform.position);
    }

    private void UpdatePastHits()
    {
        // Iterate through all the hit colliders from the past update,
        // making them all opaque.
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Renderer rend = hit.transform.GetComponent<Renderer>();
            if (rend)
            {
                // Change the material of all hit colliders
                // from past update back to opaque.
                MakeOpaque(rend);
            }
        }
    }    

    private void UpdateCurrentHits()
    {
        // Iterate through all the hit colliders from the current update,
        // making them all transparent.
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            Renderer rend = hit.transform.GetComponent<Renderer>();

            if (rend && hit.distance < m_currentPlayerDistance)
            {
                // Change the material of all hit colliders
                // to use a transparent shader.
                MakeTransparent(rend);
            }
        }
    }

    private void MakeTransparent(Renderer rend)
    {
        // Make a renderer transparent.
        rend.material.shader = Shader.Find("Transparent/Diffuse");
        Color tempColor = rend.material.color;
        tempColor.a = 0.3F;
        rend.material.color = tempColor;
    }

    private void MakeOpaque(Renderer rend)
    {
        // Make a renderer opaque.
        rend.material.shader = Shader.Find("Standard");
        Color tempColor = rend.material.color;
        tempColor.a = 1F;
        rend.material.color = tempColor;
    }
}
