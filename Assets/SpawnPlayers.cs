using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject HostPrefab;

    public GameObject AIPrefab;
    public Transform[] SpawnPoints;
    public Transform HostSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {   // if its the host spawn the host, if it's the 2nd player spawn the AI, otherwise spawn the player
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(HostPrefab.name, HostSpawnPoint.position, Quaternion.identity);
        }
        else if (PhotonNetwork.CurrentRoom.PlayerCount == 2)
        {
            PhotonNetwork.Instantiate(AIPrefab.name, SpawnPoints[1].position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoints[PhotonNetwork.CurrentRoom.PlayerCount - 1].position, Quaternion.identity);
        }
    }
}
