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
    private HashSet<Transform> savedHits;
    private Material[] savedMats;
    private bool m_debug = false;

    private void Start()
    {
        m_player = GameObject.FindWithTag("Player");
        // savedMats is initialized as an empty list to prevent runtime errors.
        savedMats = new Material[] { };
        savedHits = new HashSet<Transform>();
        m_transparentMat = Resources.Load("Transparent", typeof(Material)) as Material;
    }

    private void FixedUpdate()
    {
        m_currentPlayerDistance = CalculateCameraPlayerDistance();

        if (m_debug)
        {
            Debug.Log("Current Saved Materials: ");
            foreach (Material mat in savedMats)
            {
                if (mat) { Debug.Log(mat); }
            }
        }

        UpdatePastHits();

        savedHits.Clear();

        RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_spherecastRadius, transform.forward, 
                                     m_currentPlayerDistance, ~(1 << 9));
        if (m_debug) { Debug.Log("SpherecastAll() complete"); }

        foreach (RaycastHit hit in hits)
        {
            savedHits.Add(hit.transform);
        }

        UpdateCurrentHits();
    }

    private float CalculateCameraPlayerDistance()
    {
        // Calculate the distance between the camera and the player.
        return Vector3.Distance(m_player.transform.position, transform.position);
    }

    private void UpdatePastHits()
    {
        if (m_debug) { Debug.Log("UpdatePastHits() called"); }
        // Iterate through all the hit colliders from the past update,
        // making them all opaque.
        int i = 0;
        foreach (Transform hit in savedHits)
        {
            if (hit)
            {
                Renderer rend = hit.GetComponent<Renderer>();
                if (rend)
                {
                    // Change the material of all hit colliders
                    // from past update back to opaque.
                    MakeOpaque(rend, i);
                }
            }
            i++;
        }
    }    

    private void UpdateCurrentHits()
    {
        if (m_debug) { Debug.Log("UpdateCurrentHits() called"); }
        // Iterate through all the hit colliders from the current update,
        // making them all transparent.

        savedMats = new Material[savedHits.Count];
        int i = 0;
        foreach (Transform hit in savedHits)
        {
            if (hit)
            {
                if (m_debug) { Debug.Log("Hit: " + hit.gameObject); }
                Renderer rend = hit.GetComponent<Renderer>();
                if (rend)
                {
                    savedMats[i] = rend.material;
                    // Change the material of all hit colliders
                    // to use a transparent shader.
                    MakeTransparent(rend, i);
                }
            }
            i++;
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
