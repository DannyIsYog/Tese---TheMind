using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour
{
    // list of hands
    public List<HandController> Hands = new List<HandController>();

    // list of cards in deck
    public List<int> CardsInDeck = new List<int>();

    // list of cards in the middle pile
    public List<int> CardsInMiddlePile = new List<int>();
    // Start is called before the first frame update

    public GameObject CardPrefab;
    void Start()
    {
        // create deck
        CreateDeck();

        // shuffle deck
        ShuffleDeck();

        StartGame();
    }

    void StartGame()
    {
        // draw 5 cards for each player
        for (int i = 0; i < 5; i++)
        {
            foreach (HandController hand in Hands)
            {
                DrawCard(hand);
            }
        }
    }

    // create deck of cards numberes 1 to 100
    public void CreateDeck()
    {
        // loop through cards
        for (int i = 1; i <= 100; i++)
        {
            // add card to deck
            CardsInDeck.Add(i);
        }
        // shuffle deck
        ShuffleDeck();
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

    public void DrawCard(HandController hand)
    {
        // check if deck is empty
        if (CardsInDeck.Count > 0)
        {
            // get card from deck
            int card = CardsInDeck[0];

            // remove card from deck
            CardsInDeck.RemoveAt(0);

            // create card object
            GameObject cardObject = Instantiate(CardPrefab, transform);

            // set card text using UI tmp
            cardObject.GetComponentInChildren<TMPro.TextMeshProUGUI>().text = card.ToString();

            // add card to hand
            hand.ReceiveCard(cardObject);
        }
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
