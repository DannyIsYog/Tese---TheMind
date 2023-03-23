using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CameraController : MonoBehaviourPunCallbacks
{
    public GameObject Camera;

    public PhotonView hostPhotonView;

    // Start is called before the first frame update
    void Start()
    {
        if (!hostPhotonView.IsMine)
        {
            Camera.SetActive(false);
        }
    }
}
