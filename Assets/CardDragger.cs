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
        int steps = 50;
        for (int i = 0; i < steps; i++)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
            yield return new WaitForSeconds(0.001f);
        }

        handController.RemoveCard(gameObject);
        //add card to middle pile
        boardController.AddCardToMiddlePile(gameObject);
    }

}
