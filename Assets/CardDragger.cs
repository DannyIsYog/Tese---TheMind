using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDragger : MonoBehaviour
{
    // 2D card dragger
    private Vector3 startPosition;

    private GameObject playArea;

    private HandController handController;

    private BoardController boardController;

    private void Start()
    {
        startPosition = transform.position;
        playArea = GameObject.Find("PlayArea");
        handController = GameObject.Find("Hand").GetComponent<HandController>();
        boardController = GameObject.Find("Board").GetComponent<BoardController>();
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
        if (transform.position.y >= playArea.transform.position.y - playArea.transform.position.y / 2)
        {
            //transform.position = new Vector3(playArea.transform.position.x, playArea.transform.position.y, playArea.transform.position.z);
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
        int steps = 100;
        for (int i = 0; i < steps; i++)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y + 0.02f, transform.position.z);
            yield return new WaitForSeconds(0.001f);
        }

        handController.RemoveCard(gameObject);
        //add card to middle pile
        boardController.AddCardToMiddlePile(gameObject);
    }

}
