using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;

public class Settings_UI : MonoBehaviour
{
    [Header("Display settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;

    [Header("Audio Sliders")]
    [SerializeField] private Slider masterSlider;
    [SerializeField] private Slider bgmSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Audio Mixer")]
    [SerializeField] private AudioMixer audioMixer;

    private void Start()
    {
        if (masterSlider != null) masterSlider.value = 1f;
        if (bgmSlider != null) bgmSlider.value = 0.8f;
        if (sfxSlider != null) sfxSlider.value = 0.8f;

        if(resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.AddListener(OnresolutionChange);
        }
    }


    /// <summary>
    /// Changes the resolution of the game based on the dropdown selection.
    /// </summary>
    
    public void OnresolutionChange(int index)
    {
        if (index == 0)
        {
            Screen.SetResolution(1920, 1080, FullScreenMode.Windowed);
            Debug.Log("Resolution set to 1920x1080");
        }
        else if (index == 1)
        {
            Screen.SetResolution(1280, 720, FullScreenMode.Windowed);
            Debug.Log("Resolution set to 1280x720");
        }
    }

    /// <summary>
    /// Adjusts the master volume based on the slider value.
    /// </summary>
    
    public void OnMasterVolumeChange(float value)
    {
        Debug.Log($"Master Volume set to {value}");

        if (audioMixer != null)
        {
            float dB = value <= 0 ? -80f : Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("MasterVolume", dB);
        }
    }

    /// <summary>
    /// Adjusts the BGM volume based on the slider value.
    /// </summary>
    public void OnBGMVolumeChange(float value)
    {        
        Debug.Log($"BGM Volume set to {value}");
        if (audioMixer != null)
        {
            float dB = value <= 0 ? -80f : Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("BGMVolume", dB);
        }
    }

    /// <summary>
    /// Adjusts the SFX volume based on the slider value.
    /// </summary>
    public void OnSFXVolumeChange(float value)
    {
        Debug.Log($"SFX Volume set to {value}");
        if (audioMixer != null)        {
            float dB = value <= 0 ? -80f : Mathf.Log10(value) * 20f;
            audioMixer.SetFloat("SFXVolume", dB);
        }
    }

    /// <summary>
    /// X button clicked.
    /// </summary>
    
    public void OnClickCloseSettings()
    {
        Debug.Log("Closing settings menu");
        gameObject.SetActive(false);
    }
}
