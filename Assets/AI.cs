using TMPro;
using TMPro;
using UnityEngine;

public class AI : MonoBehaviour
{
    // AI that will play the game

    public int counter;

    public HandController handController;

    public BoardController boardController;

    public TextMeshProUGUI counterText;

    private void Start()
    {

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
        // check all cards in hand, if the number is the same as the counter, play the card
        foreach (GameObject card in handController.CardsInHand)
        {
            if (card == null) return;
            int numberCard = int.Parse(card.GetComponentInChildren<TextMeshProUGUI>().text);

            // if the card is two or one less than the counter
            if (numberCard == counter || numberCard == counter + 1 || numberCard == counter + 2 || numberCard == counter + 3 || numberCard == counter + 4)
            {
                // play the card
                card.GetComponent<CardDragger>().PlayingCardNotification(true);
            }
            if (numberCard == counter)
            {
                // play the card
                card.GetComponent<CardDragger>().PlayingCardNotification(false);
                card.GetComponent<CardDragger>().MoveCardUp();
                Invoke("count", 2f);
                return;
            }
        }
        Invoke("count", 1f);
    }


}
