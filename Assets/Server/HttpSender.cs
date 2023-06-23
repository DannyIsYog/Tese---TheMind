using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.Networking;
public class HttpSender : MonoBehaviour
{
    public string url = "http://192.168.88.55:10000";
    public string message = "Hello, server!";

    public List<Command> nextRequest = new List<Command>();

    public enum Command
    {
        LOSE_GAME = 1,
        LOSE_LIFE = 2,
        WIN_GAME = 3,
        WIN_ROUND = 4,
        PLAY_CARD = 5,
        CLOSE_CALL = 6,
        LOOK_AT_OTHERS = 7,
        LOOK_AT_CARD = 8,
    }

    // enum of command to string, no caps. LOSE_GAME -> lose_game, etc.
    public string CommandToString(Command command)
    {
        return command.ToString().ToLower();
    }

    bool disable = false;
    public void addRequest(string request)
    {
        // if theres a command with a higher number in the list, dont add this
        if (nextRequest.Count > 0 && (int)nextRequest[nextRequest.Count - 1] > (int)System.Enum.Parse(typeof(Command), request))
        {
            Debug.Log("Command " + request + " not added");
            return;
        }

        nextRequest.Add((Command)System.Enum.Parse(typeof(Command), request.ToUpper()));
        Debug.Log("Command " + request + " added");
    }

    private void Start()
    {
        // if is master dont send requests
        if (PhotonNetwork.IsMasterClient)
        {
            disable = true;
        }

        // check if component PhotonView is not null
        if (GetComponent<PhotonView>() != null)
        {
            //if not local player, dont send requests
            if (!GetComponent<PhotonView>().IsMine)
            {
                disable = true;
            }
        }
        SendHttpRequest();
    }

    public void SendHttpRequest()
    {
        StartCoroutine(SendRequest("hello"));
    }

    public IEnumerator SendRequest(string message)
    {
        Debug.Log("Sending message " + message);
        if (disable) yield break;
        this.message = message;
        using (UnityWebRequest request = UnityWebRequest.Post(url, message))
        {
            yield return request.SendWebRequest();

            if (request.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError(request.error);
            }
            else
            {
                Debug.Log("HTTP request sent successfully!");
                Debug.Log("Server response: " + request.downloadHandler.text);
            }
        }
    }
}
