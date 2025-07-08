using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public AudioMixer audioMixer;
    public Dropdown resolutionDropdown;
    public Slider volumeSlider;
    public Toggle muteToggle;
    public Toggle fullscreenToggle;

    private Resolution[] resolutions;
    private float previousVolume;
    private float mutedVolume;
    private bool isMuted = false;
    private float currentVolume;

    private const float MIN_VOLUME = -20f;
    private const float MAX_VOLUME = 20f;
    private const float MUTED_VOLUME = -80f; // Added constant for muted volume
    private const string VOLUME_FILE_PATH = "merrycraisler.txt";

    void Start()
    {
        volumeSlider.minValue = 0f;
        volumeSlider.maxValue = 1f;

        float savedSliderValue = LoadVolumeFromFile();
        volumeSlider.value = savedSliderValue;

        SetVolume(savedSliderValue);

        volumeSlider.onValueChanged.AddListener(OnSliderValueChanged);

        InitializeResolutions();
        UpdateMuteToggleState(currentVolume);

        int fullscreenPref = PlayerPrefs.GetInt("Fullscreen", 1);
        bool isFullscreen = fullscreenPref == 1;
        SetFullscreen(isFullscreen);
        fullscreenToggle.isOn = isFullscreen;
    }

    private void InitializeResolutions()
    {
        resolutions = Screen.resolutions;
        resolutionDropdown.ClearOptions();
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].width == Screen.currentResolution.width && 
                resolutions[i].height == Screen.currentResolution.height)
            {
                currentResolutionIndex = i;
            }
        }

        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private float NormalizeVolume(float volume)
    {
        if (volume <= MUTED_VOLUME) return 0f;
        return (volume - MIN_VOLUME) / (MAX_VOLUME - MIN_VOLUME);
    }

    private float DenormalizeVolume(float normalizedVolume)
    {
        if (normalizedVolume == 0f) return MUTED_VOLUME;
        return Mathf.Lerp(MIN_VOLUME, MAX_VOLUME, normalizedVolume);
    }

    private void UpdateMuteToggleState(float volume)
    {
        if (muteToggle != null)
        {
            muteToggle.isOn = volume > MUTED_VOLUME;
        }
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void SetVolume(float normalizedVolume)
    {
        float volume = DenormalizeVolume(normalizedVolume);
        audioMixer.SetFloat("volume", volume);
        currentVolume = volume;

        SaveVolumeToFile(normalizedVolume);
        
        UpdateMuteToggleState(volume);
        
        if (volume <= MUTED_VOLUME)
        {
            muteToggle.isOn = false;
            isMuted = true;
        }
        else
        {
            isMuted = false;
            muteToggle.isOn = true;
        }
    }

    public void OnSliderValueChanged(float value)
    {
        SetVolume(value);
    }

    public void ToggleVolume()
    {
        if (isMuted)
        {
            if(mutedVolume==-80){
            mutedVolume=20;
            audioMixer.SetFloat("volume", mutedVolume);
            volumeSlider.value = NormalizeVolume(mutedVolume);
            isMuted = false;
            UpdateMuteToggleState(mutedVolume);
            
            SaveVolumeToFile(volumeSlider.value);
            }
            else{
                audioMixer.SetFloat("volume", mutedVolume);
            volumeSlider.value = NormalizeVolume(mutedVolume);
            isMuted = false;
            UpdateMuteToggleState(mutedVolume);
            
            SaveVolumeToFile(volumeSlider.value);
            }
        }
        else
        {
            audioMixer.GetFloat("volume", out mutedVolume);
            audioMixer.SetFloat("volume", MUTED_VOLUME);
            volumeSlider.value = 0f;
            isMuted = true;
            UpdateMuteToggleState(MUTED_VOLUME);
            
            SaveVolumeToFile(0f);
        }
    }

    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
        PlayerPrefs.SetInt("Fullscreen", isFullscreen ? 1 : 0);
    }

    public void SettingsClose()
    {
        audioMixer.GetFloat("volume", out currentVolume);
        
        PlayerPrefs.SetInt("Fullscreen", fullscreenToggle.isOn ? 1 : 0);
        
        PlayerPrefs.Save();
        
        SceneManager.LoadScene(0);
    }

    public void OpenSettings()
    {
       float savedSliderValue = LoadVolumeFromFile();
       
       volumeSlider.value = savedSliderValue;       
       SetVolume(savedSliderValue);

       UpdateMuteToggleState(currentVolume);

       int fullscreenPref = PlayerPrefs.GetInt("Fullscreen", 1);       
       bool isFullscreen = fullscreenPref == 1;        
       
       fullscreenToggle.isOn = isFullscreen;        
       
       if(fullscreenPref == 0)        
       {            
           fullscreenToggle.isOn = true;            
           SetFullscreen(true);        
       }    
   }

   private void SaveVolumeToFile(float normalizedVolume)
   {
       string path = Path.Combine(Application.persistentDataPath, VOLUME_FILE_PATH);
       File.WriteAllText(path, normalizedVolume.ToString());
   }

   private float LoadVolumeFromFile()
   {
       string path = Path.Combine(Application.persistentDataPath, VOLUME_FILE_PATH);
       
       if (File.Exists(path))
       {
           string volumeString = File.ReadAllText(path);
           float normalizedVolume;

           if (float.TryParse(volumeString, out normalizedVolume))
           {
               return normalizedVolume;
           }
       }
       return NormalizeVolume(MUTED_VOLUME);
   }
}