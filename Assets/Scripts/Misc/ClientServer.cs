using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;
using UnityEngine.Networking;


public class ClientServer : MonoBehaviour
{
    public static IEnumerator PostData(string jsonData)
    {
        // URL to Post the data.
        string url1 = "https://flowery-unusual-army.anvil.app/_/api/time/" +GameManager.instance.data.time;
        string url2 = "https://flowery-unusual-army.anvil.app/_/api/slain/" + GameManager.instance.data.enemies_killed;
        string url3 = "https://flowery-unusual-army.anvil.app/_/api/melee/" + GameManager.instance.data.melee_use;
        string url4 = "https://flowery-unusual-army.anvil.app/_/api/ranged/" + GameManager.instance.data.ranged_use;

        string[] urls = { url1, url2, url3, url4 };

        //using (UnityWebRequest request = UnityWebRequest.Put(url1, jsonData))
        //{
        //    request.method = UnityWebRequest.kHttpVerbPOST;
        //    request.SetRequestHeader("Content-Type", "application/json");
        //    request.SetRequestHeader("Accept", "application/json");

        //    yield return request.SendWebRequest();

        //    if (!request.isNetworkError && request.responseCode == (int)HttpStatusCode.OK)
        //        Debug.Log("Data successfully sent to the server");

        //    else
        //        Debug.Log("Error sending data to the server: Error " + request.responseCode);
        //}

        foreach (string url in urls)
        {
            using (UnityWebRequest request = UnityWebRequest.Put(url, jsonData))
            {
                request.method = UnityWebRequest.kHttpVerbPOST;
                request.SetRequestHeader("Content-Type", "application/json");
                request.SetRequestHeader("Accept", "application/json");

                yield return request.SendWebRequest();

                if (!request.isNetworkError && request.responseCode == (int)HttpStatusCode.OK)
                    Debug.Log("Data successfully sent to the server");
                else
                    Debug.Log("Error sending data to the server: Error " + request.responseCode);
            }
        }
    }
}

