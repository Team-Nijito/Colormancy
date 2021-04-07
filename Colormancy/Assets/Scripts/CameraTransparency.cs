using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class CameraTransparency : MonoBehaviour
{
    public float m_spherecastRadius = 0.5f;
    private Material m_transparentMat;
    private GameObject m_player;
    private float m_currentPlayerDistance;
    private RaycastHit[] hits;
    private Material[] savedMats;

    private void Start()
    {
        m_player = GameObject.FindWithTag("Player");
        // hits is initialized as an empty list to prevent runtime errors.
        hits = new RaycastHit[] { };
        m_transparentMat = Resources.Load("Transparent", typeof(Material)) as Material;
    }

    private void Update()
    {
        m_currentPlayerDistance = CalculateCameraPlayerDistance();

        UpdatePastHits();

        hits = Physics.SphereCastAll(transform.position, m_spherecastRadius, transform.forward, m_currentPlayerDistance);

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
            if (hit.transform)
            {
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (rend)
                {
                    // Change the material of all hit colliders
                    // from past update back to opaque.
                    MakeOpaque(rend, i);
                }
            }
        }
    }    

    private void UpdateCurrentHits()
    {
        // Iterate through all the hit colliders from the current update,
        // making them all transparent.

        savedMats = new Material[hits.Length];
        for (int i = 0; i < hits.Length; i++)
        {
            RaycastHit hit = hits[i];
            if (hit.transform)
            {
                Renderer rend = hit.transform.GetComponent<Renderer>();
                if (rend)
                {
                    savedMats[i] = rend.material;
                }
                if (rend && hit.distance < m_currentPlayerDistance)
                {
                    // Change the material of all hit colliders
                    // to use a transparent shader.
                    MakeTransparent(rend, i);
                }
            }
        }
    }

    private void MakeTransparent(Renderer rend, int index)
    {
        // Make a renderer transparent.
        rend.material = m_transparentMat;
    }

    private void MakeOpaque(Renderer rend, int index)
    {
        // Make a renderer opaque.
        rend.material = savedMats[index];
    }
}
