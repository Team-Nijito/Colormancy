using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chromaturgy
{
    public class PlayerSpawner : MonoBehaviour
    {
        // Spawns each player once the game begins

        [SerializeField] private GameObject playerPrefab = null;

        // Start is called before the first frame update
        void Start() => PhotonNetwork.Instantiate(playerPrefab.name, Vector3.zero, Quaternion.identity);
    }
}
