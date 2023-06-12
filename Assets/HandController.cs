using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;

public class HandController : MonoBehaviourPunCallbacks
{
    // list of cards in hand
    public List<GameObject> CardsInHand = new List<GameObject>();
    public int CardsCount = 0;

    // card prefab
    public GameObject CardPrefab;

    public int lifes;

    float MinimumCardSpacing = 0.1f;

    public BoardController boardController;

    public GameObject playArea;

    public GameObject handArea;

    public Camera mainCamera;

    //text of player number
    public TextMeshProUGUI playerNumberText;

    //text of player lifes
    public TextMeshProUGUI playerLifesText;

    //notification text
    public TextMeshProUGUI notificationText;

    public GameObject PlayerReadyButton;

    public GameObject PlayingCardNotification;

    public AI ai;

    public bool ready = false;

    private void Start()
    {
        StartCoroutine(LateStart());
    }

    // late start
    private IEnumerator LateStart()
    {
        // get game controller in scene
        GameController gameController = FindObjectOfType<GameController>();

        //while game controller is not null get the game controller
        while (gameController == null)
        {
            //wait for 1 second
            yield return new WaitForSeconds(1f);
            //get game controller in scene
            gameController = FindObjectOfType<GameController>();
        }

        // register hand with game controller
        gameController.RegisterHand(this);

        if (!photonView.IsMine)
        {
            // disable camera if not owned by local player
            mainCamera.enabled = false;
        }

        // get player number
        int playerNumber = photonView.Owner.ActorNumber;
        playerNumberText.text = "PLayer " + playerNumber.ToString();
        lifes = gameController.lifes;
        UpdateLifeText();

        // set ready button active
        PlayerReadyButton.SetActive(true);
        InvokeRepeating("UpdateCardCountRPC", 0f, 3f);
    }

    // player ready
    public void PlayerReady()
    {
        // get game controller in scene
        GameController gameController = FindObjectOfType<GameController>();

        // if game controller is not null
        if (gameController != null)
        {
            // player ready via RPC
            gameController.photonView.RPC("PlayerReady", RpcTarget.MasterClient, photonView.Owner.ActorNumber);
            PlayerReadyButton.SetActive(false);
            ready = true;
        }
    }

    // recieve card from deck
    public void ReceiveCard(GameObject card)
    {
        // add card to hand
        CardsInHand.Add(card);

        // set card parent to hand
        card.transform.SetParent(transform);

        // get CardDragger component
        CardDragger cardDragger = card.GetComponent<CardDragger>();
        cardDragger.playArea = playArea;
        cardDragger.handController = this;
        cardDragger.boardController = boardController;

        // set card rotation
        card.transform.localRotation = Quaternion.identity;
        card.transform.parent = handArea.transform;
        // sort cards in hand
        SortCardsInHand();

        DispositionCards();
        photonView.RPC("UpdateCardCount", RpcTarget.All, CardsInHand.Count, photonView.ViewID);
    }

    // remove card from hand
    public void RemoveCard(GameObject card)
    {
        // remove card from hand
        CardsInHand.Remove(card);

        SortCardsInHand();

        DispositionCards();
        photonView.RPC("UpdateCardCount", RpcTarget.All, CardsInHand.Count, photonView.ViewID);
    }

    // remove all cards from hand
    public void RemoveAllCards()
    {
        //destroy all cards in hand
        foreach (GameObject card in CardsInHand)
        {
            Destroy(card);
        }
        // loop through cards in hand
        for (int i = CardsInHand.Count - 1; i >= 0; i--)
        {
            // remove card from hand
            RemoveCard(CardsInHand[i]);
        }
        PlayerReadyButton.SetActive(true);
        photonView.RPC("UpdateCardCount", RpcTarget.All, CardsInHand.Count, photonView.ViewID);
    }

    // sort cards in hand by value
    public void SortCardsInHand()
    {
        // loop through cards in hand
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            // loop through cards in hand
            for (int j = 0; j < CardsInHand.Count; j++)
            {
                // check if card value is less than next card value
                // get card value by checking UI tmp text
                if (int.Parse(CardsInHand[i].GetComponentInChildren<TMPro.TextMeshProUGUI>().text) < int.Parse(CardsInHand[j].GetComponentInChildren<TMPro.TextMeshProUGUI>().text))
                {
                    // swap cards
                    GameObject temp = CardsInHand[i];
                    CardsInHand[i] = CardsInHand[j];
                    CardsInHand[j] = temp;
                }

            }

        }

    }

    // disposition cards in hand to fit screen from left to right with minimum spacing
    public void DispositionCards()
    {
        // calculate total width of cards
        float totalWidth = 0f;
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            totalWidth += CardsInHand[i].GetComponent<SpriteRenderer>().bounds.size.x;
        }

        // calculate total spacing between cards
        float totalSpacing = MinimumCardSpacing * (CardsInHand.Count - 1);

        // calculate total width of cards and spacing
        totalWidth += totalSpacing;

        // calculate starting position
        float startingPosition = -totalWidth / 2f;

        // if we have more than 6 cards in hand, the disposition of cards should start on the left to right and not center
        if (CardsInHand.Count > 6)
        {
            startingPosition = -totalWidth / 2f + (((CardsInHand.Count - 6) * CardsInHand[0].GetComponent<SpriteRenderer>().bounds.size.x) / 2f);
        }

        // loop through cards in hand
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            // set card position
            CardsInHand[i].transform.localPosition = new Vector3(startingPosition + (CardsInHand[i].GetComponent<SpriteRenderer>().bounds.size.x / 2f), 0f, 0f);

            // add card width and spacing to starting position
            startingPosition += CardsInHand[i].GetComponent<SpriteRenderer>().bounds.size.x + MinimumCardSpacing;

            // set card sorting order
            CardsInHand[i].GetComponent<SpriteRenderer>().sortingOrder = i;


        }
    }

    public void ChangeCardColours(int card)
    {
        // loop through cards in hand
        for (int i = 0; i < CardsInHand.Count; i++)
        {
            // get card number from text
            int cardNumber = int.Parse(CardsInHand[i].GetComponentInChildren<TextMeshProUGUI>().text);

            // check if card number is equal to card
            if (cardNumber == card)
            {
                // change card colour
                CardsInHand[i].GetComponent<CardDragger>().ChangeCardColours();
            }
        }
    }

    public void UpdateLife(int i)
    {

        // set lifes equal to i make sure lifes is never less than 0
        lifes = Mathf.Max(0, i);
        UpdateLifeText();
    }

    public void UpdateLifeText()
    {
        // repeat the string "♥" equal to the number of lifes on text
        playerLifesText.text = new string('♥', lifes);
    }

    public void UpdateNotification(string text)
    {
        if (notificationText != null)
        {
            //cancel invoke
            CancelInvoke();
        }
        notificationText.text = text;
        Invoke("UpdateNotificationText", 2f);

        ai = FindObjectOfType<AI>();
        if (ai != null)
        {
            ai.UpdateNotification(text);
        }
    }

    public void UpdateNotificationText()
    {
        notificationText.text = "";
    }

    public void ResetHand()
    {
        RemoveAllCards();
        UpdateLifeText();
        // activate ready button
        PlayerReadyButton.SetActive(true);
        photonView.RPC("UpdateCardCount", RpcTarget.All, CardsInHand.Count, photonView.ViewID);
    }

    public void GetReadyButton()
    {
        ready = false;
        // deactivate ready button
        PlayerReadyButton.SetActive(true);
    }

    public void UpdateCardCounterRPC()
    {
        photonView.RPC("UpdateCardCount", RpcTarget.All, CardsInHand.Count, photonView.ViewID);
    }

    [PunRPC]
    public void UpdateCardCount(int count, int ID)
    {
        if (photonView.ViewID == ID)
        {
            CardsCount = count;
        }
    }

    public void PlayingCardNotificationUpdate(bool value, int count)
    {
        bool ours = false;

        if (count == 1)
        {
            foreach (GameObject card in CardsInHand)
            {
                // get CardDragger component
                CardDragger cardDragger = card.GetComponent<CardDragger>();

                // check if card is being played
                if (cardDragger.holding)
                {
                    ours = true;
                }
            }
        }
        // check if any CardsInHand has the variable holding true

        if (ours) { PlayingCardNotification.SetActive(false); }
        else { PlayingCardNotification.SetActive(value); }

    }
}
