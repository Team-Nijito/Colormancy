using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class OrbUIController : MonoBehaviourPun
{
    [System.Serializable]
    public struct OrbSpritePair
    {
        public Orb.Element element;
        public Color color;
        public Sprite sprite;
    }

    public GameObject[] orbs = new GameObject[3];
    public List<OrbSpritePair> orbSpritePairs;
    public GameObject avatarBackground;
    private List<Sprite> orbSprites;


    public void AddOrb(Orb orb)
    {
        if (orbSpritePairs == null)
        {
            orbSpritePairs = new List<OrbSpritePair>();
        }
        if (orbSprites == null)
            orbSprites = new List<Sprite>();

        // find the orb sprite
        for (int j = 0; j < orbSpritePairs.Count; ++j)
        {
            if (orbSpritePairs[j].element == orb.getElement())
                orbSprites.Add(orbSpritePairs[j].sprite);
        }
        //GameObject gUI = Instantiate(orb.m_UIPrefab, transform.position, Quaternion.identity, transform);
        //Vector2 uiPos = gUI.GetComponent<RectTransform>().anchoredPosition;

        if (orbSprites.Count > 3)
            orbSprites.RemoveAt(0);

        if (orbSprites.Count > 2)
        {
            for (int j = 0; j < orbSpritePairs.Count; ++j)
            {
                if (orbSpritePairs[j].sprite == orbSprites[0])
                    avatarBackground.GetComponent<Image>().color = orbSpritePairs[j].color;
            }
        }
        else
            avatarBackground.GetComponent<Image>().color = Color.white;

        for (int i = 0; i < orbs.Length; i++)
        {
            if (i < orbSprites.Count)
            {
                orbs[i].GetComponent<Image>().sprite = orbSprites[i];
                orbs[i].GetComponent<Image>().color = Color.white;
            }
            else
                orbs[i].GetComponent<Image>().color = Color.clear;
        }
    }
}
