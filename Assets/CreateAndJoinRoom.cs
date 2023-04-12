using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class CreateAndJoinRoom : MonoBehaviourPunCallbacks
{
    public TMP_InputField RoomNameInputField;

    // Start is called before the first frame update
    void Start()
    {
        // set player name
        PhotonNetwork.NickName = "Player " + Random.Range(0, 1000).ToString("0000");
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreateRoom()
    {

        // create room
        PhotonNetwork.CreateRoom(RoomNameInputField.text);
    }

    public void JoinRoom()
    {
        // join room
        PhotonNetwork.JoinRoom(RoomNameInputField.text);
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();

        // load game scene
        PhotonNetwork.LoadLevel("Main");
    }
}
