using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
    public int startingLevel = 4;

    public void StartGame()
    {
        SceneManager.LoadScene(startingLevel);
        GameManager.instance.PrepareNewGame(startingLevel);
    }
}
