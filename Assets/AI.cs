using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AI : MonoBehaviour
{
    // AI that will play the game

    public int counter;

    public HandController handController;

    public BoardController boardController;

    public TextMeshProUGUI counterText;

    public HttpSender httpSender;

    public Dictionary<int, bool> playedCards = new Dictionary<int, bool>();

    public int cardsInHands = -1;

    bool far = false;

    bool roundEnded = false;

    int lifes = 3;

    string lastMessage = "";

    int lastCardPlayed = -10;

    private void Start()
    {
        lifes = handController.lifes;
        command("start AI");
    }

    public void WaitForReady()
    {
        // wait for ready
        if (handController.PlayerReadyButton.activeSelf == true)
        {
            StartCheck();
        }
        else
        {
            Invoke("WaitForReady", 1f);
        }
    }

    public void StartCheck()
    {
        // get counter
        counter = 0;
        roundEnded = false;

        // start playing
        checkReady();
    }

    void checkReady()
    {
        if (!handController.ready)
        {
            cardsInHands = -1;
            handController.PlayerReady();
            WaitForGameStart();
        }
    }

    // wait for game start, the game starts when the handcontroller has cards in hand
    void WaitForGameStart()
    {
        // if the hand controller has cards in hand
        if (handController.CardsInHand.Count > 0)
        {
            // start playing
            StartGame();
        }
        else
        {
            // wait for 1 second
            Invoke("WaitForGameStart", 1f);
        }
    }

    void StartGame()
    {
        counter = 0;
        playedCards = new Dictionary<int, bool>();
        command("start");
        count();
    }

    void count()
    {
        if (handController.CardsInHand.Count == 0)
        {
            counter = 0;
            CancelInvoke();
            WaitForReady();
        }
        // add 1 to counter
        counter++;
        counterText.text = counter.ToString();
        checkCards();


    }

    void checkCards()
    {

        isCardFar();
        bool anyClose = false;
        bool played = false;
        // check all cards in hand, if the number is the same as the counter, play the card
        foreach (GameObject card in handController.CardsInHand)
        {
            if (card == null) return;
            int numberCard = int.Parse(card.GetComponentInChildren<TextMeshProUGUI>().text);

            // if the card is two or one less than the counter
            if (numberCard == counter + 1 || numberCard == counter + 2)
            {
                if (playedCards.ContainsKey(numberCard)) continue;
                playedCards.Add(numberCard, true);
                // play the card
                anyClose = true;
                card.GetComponent<CardDragger>().PlayingCardNotification(true);
                command("play_card");
            }
            if (numberCard == counter)
            {
                // play the card
                card.GetComponent<CardDragger>().PlayingCardNotification(false);
                card.GetComponent<CardDragger>().MoveCardUp();
                far = false;
                played = true;
            }
            if (!anyClose)
            {
                card.GetComponent<CardDragger>().PlayingCardNotification(false);
            }
        }
        if (played) Invoke("count", 2f);
        else Invoke("count", 1f);
    }

    void playAllCards()
    {
        // play all cards
        foreach (GameObject card in handController.CardsInHand)
        {
            if (card == null) return;
            card.GetComponent<CardDragger>().PlayingCardNotification(false);
            card.GetComponent<CardDragger>().MoveCardUp();
        }
    }

    // check if all cards are 10 or more away from counter, return bool
    void isCardFar()
    {
        if (far) return;
        if (handController.CardsInHand.Count == 0)
        {
            far = true;
            return;
        }
        far = true;
        // check all cards in hand, if the number is the same as the counter, play the card
        foreach (GameObject card in handController.CardsInHand)
        {
            int numberCard = int.Parse(card.GetComponentInChildren<TextMeshProUGUI>().text);

            // if the card is ten or more than the counter
            if (numberCard <= counter + 30)
            {
                far = false;
            }
        }

        if (far)
        {
            // play the card
            command("look_at_others");
        }
    }

    public void UpdateNotification(string text)
    {
        if (text.Equals(lastMessage))
        {
            return;
        }
        lastMessage = text;
        // check if text is in the format "Card " + cardValue + " was played"
        if (text.Contains("Card"))
        {
            int cardValue = int.Parse(text.Split(' ')[1]);

            // get the number of the player that played the card
            int playerNumber = int.Parse(text.Split(' ')[5]);

            Debug.Log("Card " + cardValue + " was played by player " + playerNumber);
            // if the card played is very close to the last card played, command hello
            if (cardValue == lastCardPlayed + 1 || cardValue == lastCardPlayed + 2)
            {
                command("close_call");
            }
            else if (playerNumber == 3)
            {
                Debug.Log("look right");
                command("look_right");
            }
            else if (playerNumber == 4)
            {
                Debug.Log("look left");
                command("look_left");
            }
            lastCardPlayed = cardValue;
            //set counter to card value
            counter = cardValue;
            cardsInHands--;
        }

        if (roundEnded) return;

        if (text.Contains("Game ended"))
        {
            roundEnded = true;
            command("win_game");
        }
        else if (text.Contains("Round ended"))
        {
            roundEnded = true;
            command("win_round");
        }
    }

    private void Update()
    {
        // check handcontroller for changes in life
        if (handController.lifes < lifes)
        {
            lifes = handController.lifes;
            command("lose_life");
        }
    }

    public void command(string message)
    {
        StartCoroutine(httpSender.SendRequest(message));
    }


}
