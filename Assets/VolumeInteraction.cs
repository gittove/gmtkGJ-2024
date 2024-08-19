using UnityEngine;
using UnityEngine.UI;

public class VolumeInteraction : MonoBehaviour
{
    private Slider VolumeSlider;
    private float preMuteMusicVolume;
    private bool musicIsMuted = false;
    
    void Start()
    {
        VolumeSlider = GetComponentInChildren<Slider>();
        if (!PlayerPrefs.HasKey("musicVolume"))
        {
            PlayerPrefs.SetFloat("musicVolume", 0.5f);
            PlayerPrefs.SetFloat("unmutedMusicVolume", 0.5f);
            Load();
        }
        else
        {
            Load();
        }
    }

    public void ChangeVolume()
    {
        AudioListener.volume = VolumeSlider.value;
    }

    private void Load()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("musicVolume");
        preMuteMusicVolume = PlayerPrefs.GetFloat("unmutedMusicVolume");
    }

    private void Save()
    {
        PlayerPrefs.SetFloat("musicVolume", VolumeSlider.value);
        PlayerPrefs.SetFloat("unmutedMusicVolume", preMuteMusicVolume);
    }

    public void ToggleMuteMusic()
    {
        if (!musicIsMuted)
        {
            preMuteMusicVolume = AudioListener.volume;
            VolumeSlider.value = 0f;
        }
        else
        {
            VolumeSlider.value = preMuteMusicVolume;
        }
        Save();
        musicIsMuted = !musicIsMuted;
    }
}
