using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class CardDragger : MonoBehaviourPunCallbacks
{
    // 2D card dragger
    private Vector3 startPosition;

    public GameObject playArea;

    public HandController handController;

    public BoardController boardController;

    public SpriteRenderer card;

    public List<TextMeshProUGUI> cardNumber;

    public GameController gameController;

    public bool holding = false;

    private void Start()
    {
        startPosition = transform.position;
        gameController = FindObjectOfType<GameController>();
    }

    public void SetCardNumber(int number)
    {
        foreach (TextMeshProUGUI text in cardNumber)
        {
            text.text = number.ToString();
        }
    }

    private void OnMouseDrag()
    {
        if (holding == false)
        {
            holding = true;
            gameController.photonView.RPC("PlayingCard", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, true);
        }
        // get mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // set card position
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        if (holding == true)
        {
            holding = false;
            gameController.photonView.RPC("PlayingCard", RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, false);
        }
        // check if card is in play area
        if (playArea.GetComponent<Collider2D>().bounds.Contains(transform.position))
        {
            // add card to middle pile
            MoveCardUp();
        }
        else
        {
            transform.position = startPosition;
            Debug.Log("Card returned to hand");
        }
    }

    public void updateStartPosition(Vector3 position)
    {
        startPosition = position;
    }

    // move card up slowly out of screen
    public void MoveCardUp()
    {
        StartCoroutine(MoveCardUpCoroutine());
    }

    IEnumerator MoveCardUpCoroutine()
    {
        int steps = 25;
        for (int i = 0; i < steps; i++)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
            yield return new WaitForSeconds(0.001f);
        }

        handController.RemoveCard(gameObject);
        //add card to middle pile
        boardController.AddCardToMiddlePile(gameObject);
    }

    public void ChangeCardColours()
    {
        // change alpha of card to 1
        card.color = new Color(card.color.r, card.color.g, card.color.b, 1);
        StartCoroutine(ChangeCardColourBack());
        MoveCardUp();
    }

    public IEnumerator ChangeCardColourBack()
    {
        // change alpha of card to 0 slowly over 2 seconds
        for (int i = 0; i < 100; i++)
        {
            card.color = new Color(card.color.r, card.color.g, card.color.b, card.color.a - 0.01f);
            yield return new WaitForSeconds(0.02f);
        }
    }

}
