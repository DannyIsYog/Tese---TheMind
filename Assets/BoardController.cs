using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    GameController gameController;

    private void Start()
    {
        gameController = FindObjectOfType<GameController>();
    }

    //add card to middle pile
    public void AddCardToMiddlePile(GameObject card)
    {
        gameController.CardToMiddlePile(card);
    }
}
