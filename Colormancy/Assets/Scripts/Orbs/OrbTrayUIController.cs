using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrbTrayUIController : MonoBehaviour
{
    [System.Serializable]
    public struct OrbTrayUIElementColorPair
    {
        public Orb.Element element;
        public Color color;
    }

    public List<OrbTrayUIElementColorPair> elementColorPairs;

    public List<GameObject> splashes;
    private List<Color> addedElementColors;

    //List<GameObject> orbs = new List<GameObject>();

    public void AddOrb(Orb orb)
    {
        /*
        GameObject gUI = Instantiate(orb.m_UIPrefab, transform.position, Quaternion.identity, transform);
        gUI.name = orb.getElement().ToString() + "OrbGui";
        //Vector2 uiPos = gUI.GetComponent<RectTransform>().anchoredPosition;
        orbs.Add(gUI);
        gUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(-552 + (138 * (orbs.Count - 1)), -6);
        */

        if (addedElementColors == null)
        {
            foreach (GameObject g in splashes)
            {
                g.GetComponent<Image>().color = Color.clear;
            }
            addedElementColors = new List<Color>();
        }

        for (int i = 0; i < elementColorPairs.Count; ++i)
        {
            if (elementColorPairs[i].element == orb.getElement())
                addedElementColors.Add(elementColorPairs[i].color);
        }

        RefreshOrbs();
    }

    public void RemoveOrb(Orb orb)
    {
        /*
        string markedForRemoval = orb.getElement().ToString() + "OrbGui";
        GameObject orbToBeRemoved = orbs.Find(x => x.name == markedForRemoval);
        orbs.Remove(orbToBeRemoved);

        Destroy(orbToBeRemoved);
        */

        for (int i = 0; i < elementColorPairs.Count; ++i)
        {
            if (elementColorPairs[i].element == orb.getElement())
                addedElementColors.Remove(elementColorPairs[i].color);
        }

        RefreshOrbs();
    }

    private void RefreshOrbs()
    {
        for (int i = 0; i < splashes.Count; ++i)
        {
            if (i < addedElementColors.Count)
                splashes[i].GetComponent<Image>().color = addedElementColors[i];
            else
                splashes[i].GetComponent<Image>().color = Color.clear;
        }
    }
}
