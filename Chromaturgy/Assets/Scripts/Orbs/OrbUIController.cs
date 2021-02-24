using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbUIController : MonoBehaviour
{
    GameObject[] orbs = new GameObject[3];

    public void AddOrb(Orb orb)
    {
        GameObject gUI = Instantiate(orb.UIPrefab, transform.position, Quaternion.identity, transform);
        Vector2 uiPos = gUI.GetComponent<RectTransform>().anchoredPosition;

        if (orbs[2] != null)
        {
            Destroy(orbs[2]);
        }

        orbs[2] = orbs[1];
        orbs[1] = orbs[0];
        orbs[0] = gUI;

        for (int i = 0; i < orbs.Length; i++)
        {
            GameObject game = orbs[i];
            if (game != null)
            {
                game.GetComponent<RectTransform>().anchoredPosition = new Vector2(uiPos.x, 140 - (140 * i));
            }
        }
    }
}
