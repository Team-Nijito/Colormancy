using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbTrayUIController : MonoBehaviour
{
    List<GameObject> orbs = new List<GameObject>();

    public void AddOrb(Orb orb)
    {
        GameObject gUI = Instantiate(orb.m_UIPrefab, transform.position, Quaternion.identity, transform);
        Vector2 uiPos = gUI.GetComponent<RectTransform>().anchoredPosition;
        orbs.Add(gUI);
        gUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-552 + (138 * (orbs.Count - 1)), -6);
    }
}
