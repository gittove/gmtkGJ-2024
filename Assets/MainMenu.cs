using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        PlayerPrefs.SetInt("playedBefore", 0);
    }

    public void Play()
    {
        SceneManager.LoadScene("Bootstrap");
    }

    public void Quit()
    {
        Application.Quit();
    }
}
