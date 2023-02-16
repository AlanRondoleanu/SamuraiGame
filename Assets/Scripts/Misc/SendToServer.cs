using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SendToServer : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(SendDataToServer());
    }

    IEnumerator SendDataToServer()
    {
        yield return new WaitForSeconds(2);

        string jsonData = JsonUtility.ToJson(GameManager.instance.data);
        StartCoroutine(ClientServer.PostData(jsonData));

        StartCoroutine(SendDataToServer());
    }
}
