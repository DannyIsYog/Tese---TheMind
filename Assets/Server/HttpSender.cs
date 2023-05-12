using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class HttpSender : MonoBehaviour
{
    public string url = "http://localhost:8000";
    public string message = "Hello, server!";

    private void Start()
    {
        SendHttpRequest();
    }

    public void SendHttpRequest()
    {
        StartCoroutine(SendRequest());
    }

    IEnumerator SendRequest()
    {
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
