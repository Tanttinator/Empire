using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuUI : MonoBehaviour
{
    static PauseMenuUI instance;

    public void ExitGame()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public static void Show()
    {
        instance.gameObject.SetActive(true);
    }

    private void Awake()
    {
        instance = this;
        gameObject.SetActive(false);
    }
}
