using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class BoardController : MonoBehaviourPunCallbacks
{
    GameController gameController;

    private void Start()
    {
        StartCoroutine(LateStart());
    }
    private IEnumerator LateStart()
    {
        yield return new WaitForSeconds(2f);
        gameController = FindObjectOfType<GameController>();
    }

    //add card to middle pile
    public void AddCardToMiddlePile(GameObject card)
    {
        //gameController.CardToMiddlePile(card);
        //rpc call
        // get card number from text
        int cardNumber = int.Parse(card.GetComponentInChildren<TextMeshProUGUI>().text);

        Destroy(card);

        //gameController.photonView.RPC("CardToMiddlePile", RpcTarget.All, cardNumber);
        gameController.CardToMiddlePile(cardNumber);
    }
}
