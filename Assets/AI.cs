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

    bool far = false;

    private void Start()
    {
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

        // start playing
        checkReady();
    }

    void checkReady()
    {
        if (!handController.ready)
        {
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
        playedCards = new Dictionary<int, bool>();
        count();
    }

    void count()
    {
        // add 1 to counter
        counter++;
        counterText.text = counter.ToString();
        checkCards();

        if (handController.CardsInHand.Count == 0)
        {
            CancelInvoke("count");
            WaitForReady();
        }
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

    // check if all cards are 10 or more away from counter, return bool
    void isCardFar()
    {
        if (far) return;
        far = true;
        // check all cards in hand, if the number is the same as the counter, play the card
        foreach (GameObject card in handController.CardsInHand)
        {
            int numberCard = int.Parse(card.GetComponentInChildren<TextMeshProUGUI>().text);

            // if the card is ten or more than the counter
            if (numberCard <= counter + 10)
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
        switch (text)
        {
            case "Round ended":
                command("win_round");
                break;
            case "Game ended":
                command("win_game");
                break;
        }
    }

    public void command(string message)
    {
        StartCoroutine(httpSender.SendRequest(message));
    }


}
