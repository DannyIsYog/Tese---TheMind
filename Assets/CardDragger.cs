using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    // 2D card dragger
    private Vector3 startPosition;

    public GameObject playArea;

    public HandController handController;

    public BoardController boardController;

    public SpriteRenderer card;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void OnMouseDrag()
    {
        // get mouse position
        Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // set card position
        transform.position = new Vector3(mousePosition.x, mousePosition.y, transform.position.z);
    }

    private void OnMouseUp()
    {
        // check if card is in play area
        if (playArea.GetComponent<Collider2D>().bounds.Contains(transform.position))
        {
            // add card to middle pile
            MoveCardUp();
        }
        else
        {
            transform.position = startPosition;
            Debug.Log("Card returned to hand");
        }
    }

    public void updateStartPosition(Vector3 position)
    {
        startPosition = position;
    }

    // move card up slowly out of screen
    public void MoveCardUp()
    {
        StartCoroutine(MoveCardUpCoroutine());
    }

    IEnumerator MoveCardUpCoroutine()
    {
        int steps = 25;
        for (int i = 0; i < steps; i++)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.4f, transform.position.z);
            yield return new WaitForSeconds(0.001f);
        }

        handController.RemoveCard(gameObject);
        //add card to middle pile
        boardController.AddCardToMiddlePile(gameObject);
    }

    public void ChangeCardColours()
    {
        // change alpha of card to 1
        card.color = new Color(card.color.r, card.color.g, card.color.b, 1);
        StartCoroutine(ChangeCardColourBack());
        MoveCardUp();
    }

    public IEnumerator ChangeCardColourBack()
    {
        // change alpha of card to 0 slowly over 2 seconds
        for (int i = 0; i < 100; i++)
        {
            card.color = new Color(card.color.r, card.color.g, card.color.b, card.color.a - 0.01f);
            yield return new WaitForSeconds(0.02f);
        }
    }

}
