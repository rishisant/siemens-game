using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Networking; // Required namespace for UnityWebRequest

public class API_Testing_Script : MonoBehaviour
{
    [SerializeField] private TMP_Text text_Type;
    [SerializeField] private TMP_Text text_Result;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetAPI());
    }

    // Get the API using UnityWebRequest
    IEnumerator GetAPI()
    {
        string url = "https://g7fh351dz2.execute-api.us-east-1.amazonaws.com/default/GetAllItems";

        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            // Send the request and wait until it completes
            yield return webRequest.SendWebRequest();

            // Check for errors
            if (webRequest.result == UnityWebRequest.Result.ConnectionError || 
                webRequest.result == UnityWebRequest.Result.ProtocolError)
            {
                // Display the error
                Debug.Log("Error: " + webRequest.error);
                text_Type.SetText("GET");
                text_Result.SetText(webRequest.error);
            }
            else
            {
                // Successful request, display the result
                Debug.Log(webRequest.downloadHandler.text);
                text_Type.SetText("GET");
                text_Result.SetText(webRequest.downloadHandler.text);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}