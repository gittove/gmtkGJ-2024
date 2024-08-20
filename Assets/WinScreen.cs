using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WinScreen : MonoBehaviour
{
    public HUD PlayerHUD;
    public AudioSource GameMusic;
    public AudioSource WinScreenMusic;
    private Canvas Canvas;
    public TMP_Text ScoreText;
    public TMP_Text CompletedText;
    public TMP_Text FailedText;
    
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

        ScoreText.text = $"{PlayerHUD.Score}";
        CompletedText.text = $"{PlayerHUD.CompletedDeliveries}";
        FailedText.text = $"{PlayerHUD.FailedDeliveries}";
    }

    public void Replay()
    {
        SceneManager.LoadScene(1);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
