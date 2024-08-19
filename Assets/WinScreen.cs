using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public HUD PlayerHUD;
    public AudioSource GameMusic;
    public AudioSource WinScreenMusic;
    private Canvas Canvas;
    
    void Start()
    {
        PlayerHUD = FindFirstObjectByType<HUD>(FindObjectsInactive.Include);
        Canvas = GetComponent<Canvas>();
        Canvas.enabled = false;
    }

    void Update()
    {
        
    }

    public void Show()
    {
        Canvas.enabled = true;
        PlayerHUD.GetComponent<Canvas>().enabled = false;
        
        GameMusic.Stop();
        GameMusic.time = 0f;
        WinScreenMusic.Play();
        WinScreenMusic.time = 0f;
        Time.timeScale = 0f;
    }

    public void Replay()
    {
        SceneManager.LoadScene("Bootstrap", LoadSceneMode.Single);
        //Time.timeScale = 1f;
    }

    public void Quit()
    {
        Application.Quit(0);
    }
}
