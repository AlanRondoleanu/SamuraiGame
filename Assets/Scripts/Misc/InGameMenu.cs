using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameMenu : MonoBehaviour
{
    public GameObject menu;

    void Update()
    {
        if (Input.GetKeyDown("escape"))
        {
            if (menu.activeInHierarchy == false)
            {
                menu.SetActive(true);
                GameManager.instance.Pause();
            }
            else
            {
                menu.SetActive(false);
                GameManager.instance.Resume();
            }
        }
    }

    public void Restart()
    {
        GameManager.instance.ManualRestart();
        GameManager.instance.Resume();
    }

    public void Resume()
    {
        menu.SetActive(false);
        GameManager.instance.Resume();
    }
}
