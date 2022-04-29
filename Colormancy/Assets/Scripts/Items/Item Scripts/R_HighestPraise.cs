using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class R_HighestPraise : Item
{

    Orb addedOrb = null;

    public override void AddItemEffects(ItemManager manager)
    {
        List<Orb> availableOrbList = new List<Orb>();        //Get other players GameObjects
        PhotonView[] allPhotonViews = FindObjectsOfType<PhotonView>();
        foreach (PhotonView view in allPhotonViews)
        {
            GameObject viewGo = view.gameObject;
            if (viewGo.CompareTag("Player") && !viewGo.Equals(PlayerPhotonView.gameObject))
            {
                Debug.Log("Found player");
                OrbManager otherPlayerOrbManager = viewGo.GetComponent<OrbManager>();
                foreach(Orb orb in otherPlayerOrbManager.orbs)
                {
                    if (!availableOrbList.Contains(orb))
                        availableOrbList.Add(orb);
                }
            }
        }
        if (availableOrbList.Count > 0)
        {
            int randomChoice = Random.Range(0, availableOrbList.Count);
            addedOrb = availableOrbList[randomChoice];
            PlayerOrbManager.AddSpellOrb(addedOrb, true);
        }else
        {
            Debug.LogError("No allies to get orb from");
        }
    }

    public override void RemoveItemEffects()
    {
        if (addedOrb != null)
            PlayerOrbManager.RemoveSpellOrb(addedOrb, true);
    }
}
