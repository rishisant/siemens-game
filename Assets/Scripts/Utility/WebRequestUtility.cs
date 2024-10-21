using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking; 

public static class WebRequestUtility
{
    public static void SendWebRequest(MonoBehaviour monoBehaviour, string url, string jsonData, System.Action<string> callback)
    {
        monoBehaviour.StartCoroutine(SendWebRequestCoroutine(url, jsonData, callback));
    }

    private static IEnumerator SendWebRequestCoroutine(string url, string jsonData, System.Action<string> callback)
    {
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("success!");
                Debug.Log("invoking callback");
                callback?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
            }
        }
    }
}
