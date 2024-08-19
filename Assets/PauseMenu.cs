using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Button ResumeButton;
    public Button QuitButton;
    public AudioSource MusicPlayer;
    private Canvas Canvas;
    
    void Start()
    {
        Canvas = GetComponent<Canvas>();
        Canvas.enabled = false;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Canvas.enabled = !Canvas.enabled;
            if (Canvas.enabled)
            {
                Time.timeScale = 0f;
                MusicPlayer.Pause();
            }
            else
            {
                Time.timeScale = 1f;
                MusicPlayer.Play();
            }
        }
    }

    public void Resume()
    {
        Canvas.enabled = false;
        Time.timeScale = 1f;
        MusicPlayer.Play();
    }

    public void Quit()
    {
        Application.Quit();
    }
}
