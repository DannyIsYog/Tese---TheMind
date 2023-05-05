using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;

public class GameController : MonoBehaviourPunCallbacks
{
    // list of hands
    public List<HandController> Hands = new List<HandController>();

    public Dictionary<int, TextMeshProUGUI> cardCountEachPlayer = new Dictionary<int, TextMeshProUGUI>();

    public Dictionary<int, bool> playingCard = new Dictionary<int, bool>();

    public List<TextMeshProUGUI> cardCountEachPlayerText = new List<TextMeshProUGUI>();

    // list of cards in deck
    public List<int> CardsInDeck = new List<int>();

    //list of players and if they are ready
    public Dictionary<int, bool> PlayersReady = new Dictionary<int, bool>();

    public List<int> CardsInHands = new List<int>();

    // list of cards in the middle pile
    public List<int> CardsInMiddlePile = new List<int>();

    public int lifes = 3;

    public GameObject CardShowing;

    // reference to the player who goes first
    private int firstPlayerIndex = -1;

    private bool gameStarted = false;

    public GameObject CardPrefab;

    public GameObject StartButton;

    int level = 8;

    public TextMeshProUGUI PlayerReadyText;

    bool endRoundRunning = false;
    object lockObjectEndRound = new object();

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

    void Update()
    {
        UpdateCardCountEachPlayer();
    }

    [PunRPC]
    void SetFirstPlayer(int index)
    {
        firstPlayerIndex = index;
    }

    public void StartGame()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }
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
            for (int j = 0; j < level; j++)
            {
                // get card from deck
                int card = CardsInDeck[0];
                Debug.Log("Player " + i + " drew card " + card);
                CardsInHands.Add(card);
                // remove card from deck
                CardsInDeck.RemoveAt(0);
                int ID = Hands[i].photonView.ViewID;
                photonView.RPC("DrawCard", RpcTarget.Others, ID, card);
            }
        }
        UpdateCardCountEachPlayer();
        photonView.RPC("UpdateLifes", RpcTarget.Others, lifes);
    }

    public void RegisterHand(HandController hand)
    {
        Hands.Add(hand);
        PlayersReady.Add(hand.photonView.Owner.ActorNumber, false);
        playingCard.Add(hand.photonView.Owner.ActorNumber, false);
        cardCountEachPlayer.Add(hand.photonView.ViewID, cardCountEachPlayerText[Hands.Count - 1]);
        UpdatePlayerReadyText();
    }

    public void UpdatePlayerReadyText()
    {
        // text in form "PLayers Ready 5/6"
        PlayerReadyText.text = "Players Ready " + GetPlayersReady() + "/" + PlayersReady.Count;
    }

    public int GetPlayersReady()
    {
        int count = 0;
        foreach (var player in PlayersReady)
        {
            if (player.Value)
            {
                count++;
            }
        }
        return count;
    }

    [PunRPC]
    public void PlayerReady(int playerID)
    {
        PlayersReady[playerID] = true;
        bool allReady = true;
        foreach (var player in PlayersReady)
        {
            if (!player.Value)
            {
                allReady = false;
                break;
            }
        }
        if (allReady)
        {
            StartGame();
        }
        UpdatePlayerReadyText();
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
        // if is master return
        if (PhotonNetwork.IsMasterClient)
        {
            return;
        }

        // check if deck is empty
        if (CardsInDeck.Count > 0)
        {

            // create card object
            Vector3 cardPosition = transform.position + new Vector3(0, 1.5f, 0); // adjust card position for visibility
            GameObject cardObject = Instantiate(CardPrefab, cardPosition, Quaternion.identity);

            // set card text using UI tmp
            cardObject.GetComponentInChildren<CardDragger>().SetCardNumber(card);

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

    public void CardToMiddlePile(int card)
    {
        //rpc call
        photonView.RPC("CardToMiddlePileRPC", RpcTarget.MasterClient, card);
    }

    [PunRPC]
    public void CardToMiddlePileRPC(int cardValue)
    {
        if (!gameStarted) return;
        // check if card played has higher value than the current card showing
        if (CardsInMiddlePile.Count > 0)
        {
            if (cardValue <= CardsInMiddlePile[CardsInMiddlePile.Count - 1])
            {
                Debug.Log("Card played is lower than the current card showing");
                return;
            }
        }
        Destroy(CardShowing);

        // instantiate card object
        Vector3 cardPosition = transform.position; // adjust card position for visibility
        GameObject cardObject = Instantiate(CardPrefab, cardPosition, Quaternion.identity);
        cardObject.GetComponentInChildren<CardDragger>().SetCardNumber(cardValue);
        CardsInHands.Remove(cardValue);
        if (!IsLowestCard(cardValue))
        {
            LoseLifes(cardValue);
        }

        // set cardShowing to the card object
        CardShowing = cardObject;
        SendNotificationRPC("Card " + cardValue + " was played");
        CheckHands();
    }

    public void LoseLifes(int cardValue)
    {
        List<int> cardsLower = GetCardsLowerThan(cardValue);
        lifes -= 1;

        foreach (int card in cardsLower)
        {
            photonView.RPC("CardsLoseColour", RpcTarget.Others, card);
            photonView.RPC("UpdateLifes", RpcTarget.Others, lifes);
            CheckLife();
        }
    }

    [PunRPC]
    public void CardsLoseColour(int card)
    {
        foreach (HandController hand in Hands)
        {
            hand.ChangeCardColours(card);
        }
    }

    [PunRPC]
    public void UpdateLifes(int lifes)
    {
        foreach (HandController hand in Hands)
        {
            hand.UpdateLife(lifes);
        }
    }

    // check if the card is the lowest card in the hands of the players
    public bool IsLowestCard(int card)
    {
        // loop through cards in hands
        foreach (int cardInHand in CardsInHands)
        {
            // check if card is lower or equal than the card in hand
            if (cardInHand <= card)
            {
                return false;
            }
        }
        return true;
    }

    // function that gets all cards lower than the card given
    public List<int> GetCardsLowerThan(int card)
    {
        List<int> cards = new List<int>();

        // loop through cards in hands
        foreach (int cardInHand in CardsInHands)
        {
            // check if card is lower than the card in hand
            if (cardInHand < card)
            {
                cards.Add(cardInHand);
            }
        }
        return cards;
    }

    // check if all hands are empty, if so end round and increase level
    public void CheckHands()
    {
        UpdateCardCountEachPlayer();
        if (CardsInHands.Count > 0) return;

        lock (lockObjectEndRound)
        {
            if (!endRoundRunning) StartCoroutine(EndRound());
        }
    }

    public IEnumerator EndRound()
    {
        lock (lockObjectEndRound)
        {
            if (endRoundRunning) yield break;
            endRoundRunning = true;
        }
        if (gameStarted)
        {
            gameStarted = false;
            level++;
        }
        // send notification to all players that the round has ended
        SendNotificationRPC("Round ended");

        // wait 5 seconds
        yield return new WaitForSeconds(5);

        // reset gameStarted flag
        gameStarted = false;

        // reset cards in hands
        CardsInHands.Clear();

        // reset cards in middle pile
        CardsInMiddlePile.Clear();

        // reset card showing
        Destroy(CardShowing);

        // reset deck
        CreateDeck();
        ShuffleDeck();
        ResetPlayersReady();
        // get ready button via RPC
        photonView.RPC("GetReadyButton", RpcTarget.Others);
        lock (lockObjectEndRound)
        {
            endRoundRunning = false;
        }
    }

    public void CheckLife()
    {
        if (lifes < 0)
        {
            StartCoroutine(EndGame());
        }
        Debug.Log("CheckLife");
    }

    public IEnumerator EndGame()
    {
        SendNotificationRPC("Game Over");

        // wait 5 seconds
        yield return new WaitForSeconds(5);

        Debug.Log("EndGame");
        // reset gameStarted flag
        gameStarted = false;

        // reset cards in hands
        CardsInHands.Clear();

        // reset cards in middle pile
        CardsInMiddlePile.Clear();

        // reset deck
        CreateDeck();
        ShuffleDeck();

        // reset start button

        // reset lifes
        lifes = 3;

        // reset level
        level = 1;

        // reset hands
        ResetHandsRPC();

        // update lifes
        photonView.RPC("UpdateLifes", RpcTarget.Others, lifes);

        // reset card showing
        Destroy(CardShowing);
        ResetPlayersReady();
    }

    public void ResetPlayersReady()
    {
        // create copy of playersReady
        Dictionary<int, bool> playersReadyCopy = new Dictionary<int, bool>(PlayersReady);
        // reset all the values of the dictionary to false

        foreach (KeyValuePair<int, bool> entry in playersReadyCopy)
        {
            PlayersReady[entry.Key] = false;
        }
    }

    public void ResetHandsRPC()
    {
        photonView.RPC("ResetHands", RpcTarget.Others);
    }

    [PunRPC]
    public void ResetHands()
    {
        foreach (HandController hand in Hands)
        {
            hand.ResetHand();
        }
    }

    //send notification
    public void SendNotificationRPC(string message)
    {
        photonView.RPC("SendNotification", RpcTarget.All, message);
    }

    [PunRPC]
    public void SendNotification(string message)
    {
        foreach (HandController hand in Hands)
        {
            hand.UpdateNotification(message);
        }
    }

    [PunRPC]
    public void GetReadyButton()
    {
        foreach (HandController hand in Hands)
        {
            hand.GetReadyButton();
        }
    }

    public void UpdateCardCountEachPlayer()
    {
        //iterate the dictionary cardCountEachPlayer and show how many cards each player has
        foreach (KeyValuePair<int, TextMeshProUGUI> entry in cardCountEachPlayer)
        {
            int count = GetHand(entry.Key).CardsCount;
            entry.Value.text = count.ToString();
        }
    }

    [PunRPC]
    public void PlayingCard(int ID, bool isPlaying)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        playingCard[ID] = isPlaying;

        // if at least one player is playing, send notification, else remove notification
        // get how many true values are in the dictionary
        int count = 0;
        foreach (KeyValuePair<int, bool> entry in playingCard)
        {
            if (entry.Value) count++;
        }
        if (count > 0)
        {
            photonView.RPC("UpdatePlayingCard", RpcTarget.All, true, count);
        }
        else
        {
            photonView.RPC("UpdatePlayingCard", RpcTarget.All, false, 0);
        }
    }

    [PunRPC]
    public void UpdatePlayingCard(bool isPlaying, int count)
    {

        foreach (HandController hand in Hands)
        {
            hand.PlayingCardNotificationUpdate(isPlaying, count);
        }
    }
}
