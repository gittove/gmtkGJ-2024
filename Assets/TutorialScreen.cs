using System;
using UnityEngine;

public class TutorialScreen : MonoBehaviour
{
    private HUD PlayerHUD;
    public AudioSource MusicPlayer;
    public bool Deactivated;

    private void Start()
    {
        Time.timeScale = 0f;
        PlayerHUD = FindFirstObjectByType<HUD>();
        PlayerHUD.GetComponent<Canvas>().enabled = false;
        MusicPlayer.Stop();
        if(PlayerPrefs.GetInt("playedBefore") == 1)
            Resume();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !Deactivated)
        {
            Resume();
        }
    }

    public void Resume()
    {
        PlayerPrefs.SetInt("playedBefore", 1);
        PlayerHUD.GetComponent<Canvas>().enabled = true;
        Deactivated = true;
        GetComponent<Canvas>().enabled = false;
        Time.timeScale = 1f;
        MusicPlayer.Play();
    }
}
