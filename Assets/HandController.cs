using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    // list of cards in hand
    public List<GameObject> CardsInHand = new List<GameObject>();

    // card prefab
    public GameObject CardPrefab;

    float MinimumCardSpacing = 0.5f;

    // card spacing

    // recieve card from deck
    public void ReceiveCard(GameObject card)
    {
        // add card to hand
        CardsInHand.Add(card);

        // set card parent to hand
        card.transform.SetParent(transform);

        // set card rotation
        card.transform.localRotation = Quaternion.identity;

        // sort cards in hand
        SortCardsInHand();

        DispositionCards();
    }

    // remove card from hand
    public void RemoveCard(GameObject card)
    {
        // remove card from hand
        CardsInHand.Remove(card);

        SortCardsInHand();

        DispositionCards();
    }

    // remove all cards from hand
    public void RemoveAllCards()
    {
        // loop through cards in hand
        for (int i = CardsInHand.Count - 1; i >= 0; i--)
        {
            // remove card from hand
            RemoveCard(CardsInHand[i]);
        }
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

    // get card spacing
    public float GetCardSpacing()
    {
        // calculate card spacing
        float cardSpacing = (HandWidth - CardWidth) / (CardsInHand.Count - 1);

        // check if card spacing is less than minimum spacing
        if (cardSpacing < MinimumCardSpacing)
        {
            // set card spacing to minimum spacing
            cardSpacing = MinimumCardSpacing;
        }

        // return card spacing
        return cardSpacing;
    }

    // get hand width
    public float HandWidth
    {
        get
        {
            // get hand width
            return GetComponent<RectTransform>().rect.width;
        }
    }

    // get card width
    public float CardWidth
    {
        get
        {
            // get card width
            return CardPrefab.GetComponent<RectTransform>().rect.width;
        }
    }
}
