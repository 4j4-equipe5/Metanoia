using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SliderVolume : MonoBehaviour
{

    public Slider VolumeSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (PlayerPrefs.HasKey("soundVolume"))
        {
            LoadVolume();
        }
            
        else
        {
            PlayerPrefs.SetFloat("soundVolume", 1);
            LoadVolume();
        }
    }

    public void SetVolume()
    {
        AudioListener.volume = VolumeSlider.value;
        SaveVolume();
    }

    public void SaveVolume()
    {
        PlayerPrefs.SetFloat("soundVolume", VolumeSlider.value);
    }

    public void LoadVolume()
    {
        VolumeSlider.value = PlayerPrefs.GetFloat("soundVolume");
    }
}
