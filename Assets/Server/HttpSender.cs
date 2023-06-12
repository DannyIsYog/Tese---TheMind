using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpSender : MonoBehaviour
{
    public string url = "http://10.42.0.1:10000";
    public string message = "Hello, server!";

    bool disable = false;

    private void Start()
    {
        SendHttpRequest();
    }

    public void SendHttpRequest()
    {
        StartCoroutine(SendRequest("Hello, server!"));
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
