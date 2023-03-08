using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    private void Start()
    {

    }

    //add card to middle pile
    public void AddCardToMiddlePile(GameObject card)
    {
        // get card value
        int cardValue = int.Parse(card.GetComponentInChildren<TMPro.TextMeshProUGUI>().text);
    }
}
