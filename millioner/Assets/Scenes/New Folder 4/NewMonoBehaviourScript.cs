using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class NewMonoBehaviourScript : MonoBehaviour
{
    public AudioClip firstClip;
    public AudioClip secondClip;
    [SerializeField] private Button defaultResetButton;
    [SerializeField] private Button confirmNoColorButton;
    [SerializeField] private Button resetButton;
    public Slider volumeSlider;
    public Toggle muteToggle;
    public Button confirmButton;
    
    private AudioSource audioSource;
    private bool isPlayingFirst = true;
    private float lastVolume;
    private float initialVolume;
    private bool wasManuallyMuted = false;
    private string filePath;
    private bool hasWrittenFive = false;

    private const string VOLUME_KEY = "SavedVolume";
    private const string MUTE_KEY = "IsMuted";
    private const string FIRST_LAUNCH_KEY = "IsFirstLaunch";
    private const float DEFAULT_VOLUME = 0.5f;
    
    private Color defaultButtonColor;
    private Color alternateButtonColor;
    private bool isUpdating = false;
    
    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "birkin.txt");
        
        // Initialize audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Set default colors
        ColorUtility.TryParseHtmlString("#413A69", out defaultButtonColor);
        ColorUtility.TryParseHtmlString("#29325C", out alternateButtonColor);
        confirmButton.GetComponent<Image>().color = defaultButtonColor;

        bool isFirstLaunch = PlayerPrefs.GetInt(FIRST_LAUNCH_KEY, 1) == 1;

        // Check if file exists or is empty and if it's first launch
        if ((!File.Exists(filePath) || string.IsNullOrEmpty(File.ReadAllText(filePath))) && isFirstLaunch)
        {
            // Set default values only on first launch
            volumeSlider.value = DEFAULT_VOLUME;
            lastVolume = DEFAULT_VOLUME;
            initialVolume = DEFAULT_VOLUME;
            muteToggle.isOn = true;
            audioSource.volume = DEFAULT_VOLUME;
            SaveSettings();
            
            // Mark that the first launch has occurred
            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 0);
            PlayerPrefs.Save();
        }
        else
        {
            // Load saved settings
            LoadSettings();
        }

        initialVolume = volumeSlider.value;
        UpdateButtonColor(volumeSlider.value);

        // Setup event listeners
        defaultResetButton.onClick.AddListener(ResetToDefault);
        volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        muteToggle.onValueChanged.AddListener(OnMuteToggled);
        confirmButton.onClick.AddListener(OnConfirmButtonClick);
        confirmNoColorButton.onClick.AddListener(OnConfirmNoColorButtonClick);
        resetButton.onClick.AddListener(OnResetButtonClick);

        // Start playing audio
        PlayFirstClip();
    }

    private void OnConfirmNoColorButtonClick()
    {
        if (volumeSlider.value != initialVolume)
        {
            initialVolume = volumeSlider.value;
            SaveSettings();
            
            string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
            if (fileContent.Contains("5"))
            {
                int index = fileContent.IndexOf('5');
                fileContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, fileContent);
            }
            hasWrittenFive = false;
            
            confirmButton.GetComponent<Image>().color = defaultButtonColor;
        }
    }

    void ResetToDefault()
    {
        File.WriteAllText(filePath, "");
        hasWrittenFive = false;

        volumeSlider.value = DEFAULT_VOLUME;
        lastVolume = DEFAULT_VOLUME;
        initialVolume = DEFAULT_VOLUME;
        
        muteToggle.isOn = true;
        audioSource.volume = DEFAULT_VOLUME;
        
        confirmButton.GetComponent<Image>().color = defaultButtonColor;
        
        SaveSettings();
        
        UpdateButtonColor(DEFAULT_VOLUME);
    }

    private void OnResetButtonClick()
    {
        File.WriteAllText(filePath, "");
        hasWrittenFive = false;

        volumeSlider.value = initialVolume;
        muteToggle.isOn = initialVolume > 0;
        audioSource.volume = initialVolume;
        
        confirmButton.GetComponent<Image>().color = defaultButtonColor;
        
        SaveSettings();
    }

    private void LoadSettings()
    {
        lastVolume = PlayerPrefs.GetFloat(VOLUME_KEY, 0f);
        volumeSlider.value = lastVolume;

        bool isMuted = PlayerPrefs.GetInt(MUTE_KEY, 1) == 0;
        muteToggle.isOn = !isMuted;

        audioSource.volume = isMuted ? 0 : lastVolume;
        if (isMuted)
        {
            volumeSlider.value = 0;
        }

        CheckVolumeForToggle(volumeSlider.value);
    }

    private void OnConfirmButtonClick()
    {
        if (hasWrittenFive)
        {
            string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
            if (fileContent.Contains("5"))
            {
                int index = fileContent.IndexOf('5');
                fileContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, fileContent);
                
                if (fileContent.Length == 0)
                {
                    confirmButton.GetComponent<Image>().color = defaultButtonColor;
                }
            }
            hasWrittenFive = false;
        }
        
        initialVolume = volumeSlider.value;
    }

    void Update()
    {
        if (!audioSource.isPlaying)
        {
            if (isPlayingFirst)
            {
                PlaySecondClip();
            }
            else
            {
                PlayFirstClip();
            }
        }
    }

    private void SaveSettings()
    {
        PlayerPrefs.SetFloat(VOLUME_KEY, lastVolume);
        PlayerPrefs.SetInt(MUTE_KEY, muteToggle.isOn ? 1 : 0);
        PlayerPrefs.Save();
    }
    
    private void PlayFirstClip()
    {
        audioSource.clip = firstClip;
        audioSource.Play();
        isPlayingFirst = true;
    }
    
    private void PlaySecondClip()
    {
        audioSource.clip = secondClip;
        audioSource.Play();
        isPlayingFirst = false;
    }

    private void UpdateButtonColor(float volume)
    {
        string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        
        if (volume != initialVolume)
        {
            if (!hasWrittenFive)
            {
                File.AppendAllText(filePath, "5");
                hasWrittenFive = true;
            }
            confirmButton.GetComponent<Image>().color = alternateButtonColor;
        }
        else
        {
            if (hasWrittenFive)
            {
                if (fileContent.Contains("5"))
                {
                    int index = fileContent.IndexOf('5');
                    string newContent = fileContent.Remove(index, 1);
                    File.WriteAllText(filePath, newContent);
                    
                    if (newContent.Length == 0)
                    {
                        confirmButton.GetComponent<Image>().color = defaultButtonColor;
                    }
                }
                hasWrittenFive = false;
            }
            else if (fileContent.Length == 0)
            {
                confirmButton.GetComponent<Image>().color = defaultButtonColor;
            }
        }
    }

    private void CheckVolumeForToggle(float volume)
    {
        if (isUpdating) return;
        isUpdating = true;

        if (volume <= 0)
        {
            muteToggle.isOn = false;
            audioSource.volume = 0;
            wasManuallyMuted = true;
        }
        else
        {
            muteToggle.isOn = true;
            audioSource.volume = volume;
            wasManuallyMuted = false;
        }

        UpdateButtonColor(volume);
        isUpdating = false;
    }

    private void OnVolumeChanged(float value)
    {
        if (isUpdating) return;
        
        lastVolume = value;
        CheckVolumeForToggle(value);
        UpdateButtonColor(value);
        SaveSettings();
    }

    private void OnMuteToggled(bool isOn)
    {
        if (isUpdating) return;
        isUpdating = true;

        if (isOn)
        {
            float newVolume;
            if (wasManuallyMuted)
            {
                newVolume = lastVolume > 0 ? lastVolume : 0.1f;
            }
            else
            {
                newVolume = lastVolume;
            }
            
            volumeSlider.value = newVolume;
            audioSource.volume = newVolume;
            UpdateButtonColor(newVolume);
        }
        else
        {
            lastVolume = volumeSlider.value;
            volumeSlider.value = 0;
            audioSource.volume = 0;
            UpdateButtonColor(0);
        }

        isUpdating = false;
        SaveSettings();
    }

    private void OnApplicationQuit()
    {
        SaveSettings();
        
        if (hasWrittenFive)
        {
            string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
            if (fileContent.Contains("5"))
            {
                int index = fileContent.IndexOf('5');
                string newContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, newContent);
                
                if (newContent.Length == 0)
                {
                    confirmButton.GetComponent<Image>().color = defaultButtonColor;
                }
            }
            hasWrittenFive = false;
        }
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            SaveSettings();
        }
    }
}