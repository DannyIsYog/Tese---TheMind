using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class SpawnPlayers : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject HostPrefab;
    public Transform[] SpawnPoints;
    public Transform HostSpawnPoint;

    // Start is called before the first frame update
    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(HostPrefab.name, HostSpawnPoint.position, Quaternion.identity);
        }
        else
        {
            PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
        }
        //PhotonNetwork.Instantiate(PlayerPrefab.name, SpawnPoints[PhotonNetwork.LocalPlayer.ActorNumber - 1].position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
