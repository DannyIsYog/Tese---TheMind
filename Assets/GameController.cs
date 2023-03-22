using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class GameController : MonoBehaviourPunCallbacks
{
    // list of hands
    public List<HandController> Hands = new List<HandController>();

    // list of cards in deck
    public List<int> CardsInDeck = new List<int>();

    // list of cards in the middle pile
    public List<int> CardsInMiddlePile = new List<int>();

    // reference to the player who goes first
    private int firstPlayerIndex = -1;

    private bool gameStarted = false;

    public GameObject CardPrefab;

    public GameObject player;

    void Start()
    {
        // create deck
        CreateDeck();

        // shuffle deck
        ShuffleDeck();

        if (PhotonNetwork.IsMasterClient)
        {
            // assign the first player
            firstPlayerIndex = Random.Range(0, PhotonNetwork.PlayerList.Length);
            photonView.RPC("SetFirstPlayer", RpcTarget.AllBuffered, firstPlayerIndex);
        }
    }

    [PunRPC]
    void SetFirstPlayer(int index)
    {
        firstPlayerIndex = index;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }

    void StartGame()
    {
        // check if game has already started
        if (gameStarted)
        {
            return;
        }

        // set gameStarted flag to true
        gameStarted = true;

        // draw 5 cards for each player
        for (int i = 0; i < Hands.Count; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                // get card from deck
                int card = CardsInDeck[0];
                Debug.Log("Player " + i + " drew card " + card);
                // remove card from deck
                CardsInDeck.RemoveAt(0);
                int ID = Hands[i].photonView.ViewID;
                photonView.RPC("DrawCard", RpcTarget.All, ID, card);
            }
        }

        // set active player to the first player
        //photonView.RPC("SetActivePlayer", RpcTarget.AllBuffered, firstPlayerIndex);
    }

    [PunRPC]
    void SetActivePlayer(int index)
    {
        // deactivate all hands
        foreach (HandController hand in Hands)
        {
            hand.gameObject.SetActive(false);
        }

        // activate current player's hand
        Hands[index].gameObject.SetActive(true);

    }

    public void RegisterHand(HandController hand)
    {
        Hands.Add(hand);
    }

    // create deck of cards numbers 1 to 100
    public void CreateDeck()
    {
        // loop through cards
        for (int i = 1; i <= 100; i++)
        {
            // add card to deck
            CardsInDeck.Add(i);
        }
    }

    // shuffle deck
    public void ShuffleDeck()
    {
        // loop through cards in deck
        for (int i = 0; i < CardsInDeck.Count; i++)
        {
            // get random card
            int randomCard = Random.Range(0, CardsInDeck.Count);

            // swap cards
            int temp = CardsInDeck[i];
            CardsInDeck[i] = CardsInDeck[randomCard];
            CardsInDeck[randomCard] = temp;
        }
    }

    [PunRPC]
    public void DrawCard(int playerID, int card)
    {

        // check if deck is empty
        if (CardsInDeck.Count > 0)
        {

            // create card object
            Vector3 cardPosition = transform.position + new Vector3(0, 1.5f, 0); // adjust card position for visibility
            GameObject cardObject = PhotonNetwork.Instantiate(CardPrefab.name, cardPosition, Quaternion.identity);

            // set card text using UI tmp
            cardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.ToString();

            // add card to player hand
            HandController playerHand = GetHand(playerID);
            playerHand.ReceiveCard(cardObject);
        }
    }

    //function that gets hand with ID
    public HandController GetHand(int id)
    {
        foreach (HandController hand in Hands)
        {
            // check if hand has the same ID
            if (hand.photonView.ViewID == id)
            {
                return hand;
            }
        }
        Debug.LogError("Hand with ID " + id + " not found");
        return null;
    }

    public void CardToMiddlePile(GameObject card)
    {
        // get card value
        int cardValue = int.Parse(card.GetComponentInChildren<TMPro.TextMeshProUGUI>().text);

        // add card to middle pile
        CardsInMiddlePile.Add(cardValue);

        // set card position to Game Controller
        card.transform.position = transform.position;
    }
}
