using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

/**
 * @class WebRequestUtility
 * @brief This is a wrapper around the UnityWebRequest to make sending web
 * requests easier
 * @details A MonoBehavior child is needed to be passed in to this class, since
 * WebRequests in Unity must be Coroutines. There is also a callback for
 * success and failure callback. This is done since the return type of these
 * functions must be `IEnumerator` type, making it a Coroutine, so we can't
 * collect the response from the API call how we normally would.
 * Using this WebRequestUtility is done like so
 * ```
 *   string url = "url";
 *   string jsonData = System.String.Format(@"{{
 *       ""user_id"": {0},
 *       ""game_id"": {1},
 *       ""score"": {2}
 *   }}", data1, data2, data3);
 *   WebRequestUtility.SendWebRequest(this, url, jsonData, successCallbackFunction, failCallbackFunction);
 * ```
 */
public static class WebRequestUtility
{
    public static void SendWebRequest(MonoBehaviour monoBehaviour, string url, string jsonData, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        if (LocalPlaytest.IsActive) { failCallback?.Invoke("Local playtest: online services are disabled."); return; }
        monoBehaviour.StartCoroutine(SendWebRequestCoroutine(url, jsonData, successCallback, failCallback));
    }

    public static void SendGetWebRequest(MonoBehaviour monobehavior, string url, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        if (LocalPlaytest.IsActive) { failCallback?.Invoke("Local playtest: online services are disabled."); return; }
        monobehavior.StartCoroutine(SendGetWebRequestCoroutine(url, successCallback, failCallback));
    }

    private static IEnumerator SendWebRequestCoroutine(string url, string jsonData, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        byte[] jsonToSend = System.Text.Encoding.UTF8.GetBytes(jsonData);

        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            webRequest.uploadHandler = new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = 12;

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("success!");
                Debug.Log("invoking callback");
                successCallback?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                // Pass the server's response body along so callers can see
                // why the request failed, not just that it did
                string body = webRequest.downloadHandler?.text;
                failCallback?.Invoke(string.IsNullOrEmpty(body) ? webRequest.error : body);
            }
        }
    }

    private static IEnumerator SendGetWebRequestCoroutine(string url, System.Action<string> successCallback, System.Action<string> failCallback)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "GET"))
        {
            webRequest.downloadHandler = new DownloadHandlerBuffer();
            webRequest.timeout = 12;

            webRequest.SetRequestHeader("Content-Type", "application/json");

            yield return webRequest.SendWebRequest();

            if (webRequest.result == UnityWebRequest.Result.Success)
            {
                Debug.Log("success!");
                Debug.Log("invoking callback");
                successCallback?.Invoke(webRequest.downloadHandler.text);
            }
            else
            {
                Debug.LogError("Error: " + webRequest.error);
                // Pass the server's response body along so callers can see
                // why the request failed, not just that it did
                string body = webRequest.downloadHandler?.text;
                failCallback?.Invoke(string.IsNullOrEmpty(body) ? webRequest.error : body);
            }
        }
    }
}
