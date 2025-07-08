using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class ResolutionController : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Button confirmButton;
    public Button confirmNoColorButton;
    public Button resetButton;
    public Button defaultResetButton;
    private Resolution[] resolutions;
    private float currentAspectRatio;
    private Vector2 referenceResolution = new Vector2(1920, 1080);
    private Canvas mainCanvas;
    
    private Resolution pendingResolution;
    private bool hasChanges = false;
    private Color confirmActiveColor;
    private Color confirmInactiveColor;
    private string filePath;
    private bool hasWrittenThree = false;
    private int currentResolutionIndex;
    
    private const string RESOLUTION_PREF_WIDTH = "SavedResolutionWidth";
    private const string RESOLUTION_PREF_HEIGHT = "SavedResolutionHeight";
    private const int DEFAULT_WIDTH = 1920;
    private const int DEFAULT_HEIGHT = 1080;
    
    void Start()
    {
        ColorUtility.TryParseHtmlString("#29325c", out confirmActiveColor);
        ColorUtility.TryParseHtmlString("#413A69", out confirmInactiveColor);
        
        filePath = Path.Combine(Application.dataPath, "birkin.txt");
        
        mainCanvas = FindAnyObjectByType<Canvas>();
        if (mainCanvas != null)
        {
            CanvasScaler scaler = mainCanvas.GetComponent<CanvasScaler>();
            if (scaler == null)
                scaler = mainCanvas.gameObject.AddComponent<CanvasScaler>();
                
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;
            scaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
            scaler.matchWidthOrHeight = 0.5f;
        }

        Resolution[] allResolutions = Screen.resolutions;
        resolutions = allResolutions
            .GroupBy(r => new { r.width, r.height })
            .Select(g => g.First())
            .OrderByDescending(r => r.width * r.height)
            .ToArray();

        resolutionDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        
        int savedWidth = PlayerPrefs.GetInt(RESOLUTION_PREF_WIDTH, -1);
        int savedHeight = PlayerPrefs.GetInt(RESOLUTION_PREF_HEIGHT, -1);
    
        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + "/" + resolutions[i].height;
            options.Add(new TMP_Dropdown.OptionData(option));
            
            if (savedWidth != -1 && savedHeight != -1)
            {
                if (resolutions[i].width == savedWidth && resolutions[i].height == savedHeight)
                {
                    currentResolutionIndex = i;
                }
            }
            else if (resolutions[i].width == Screen.width && 
                     resolutions[i].height == Screen.height)
            {
                currentResolutionIndex = i;
            }
        }
        
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        if (savedWidth != -1 && savedHeight != -1)
        {
            Screen.SetResolution(savedWidth, savedHeight, Screen.fullScreen);
        }
        
        currentAspectRatio = (float)Screen.width / Screen.height;
        resolutionDropdown.onValueChanged.AddListener(OnResolutionSelected);
        confirmNoColorButton.onClick.AddListener(ConfirmResolutionChangeNoColor);
        resetButton.onClick.AddListener(ResetSettings);
        defaultResetButton.onClick.AddListener(ResetToDefault);
        
        UpdateConfirmButtonState();
    }

    public void ConfirmResolutionChange()
    {
        if (!hasChanges) return;
        
        Screen.SetResolution(pendingResolution.width, pendingResolution.height, Screen.fullScreen);
        
        PlayerPrefs.SetInt(RESOLUTION_PREF_WIDTH, pendingResolution.width);
        PlayerPrefs.SetInt(RESOLUTION_PREF_HEIGHT, pendingResolution.height);
        PlayerPrefs.Save();
        
        float newAspectRatio = (float)pendingResolution.width / pendingResolution.height;
        if (newAspectRatio != currentAspectRatio)
        {
            AdjustCameraForCurrentResolution();
        }
        
        currentAspectRatio = newAspectRatio;
        hasChanges = false;
        currentResolutionIndex = resolutionDropdown.value;
        
        File.WriteAllText(filePath, "");
        hasWrittenThree = false;
        
        UpdateConfirmButtonState();
    }

    void ResetToDefault()
    {
        int defaultIndex = 0;
        for (int i = 0; i < resolutions.Length; i++)
        {
            if (resolutions[i].width == DEFAULT_WIDTH && resolutions[i].height == DEFAULT_HEIGHT)
            {
                defaultIndex = i;
                break;
            }
        }

        bool needChange = defaultIndex != currentResolutionIndex;
        
        if (needChange)
        {
            resolutionDropdown.value = defaultIndex;
            resolutionDropdown.RefreshShownValue();
            
            Screen.SetResolution(DEFAULT_WIDTH, DEFAULT_HEIGHT, Screen.fullScreen);
            
            PlayerPrefs.SetInt(RESOLUTION_PREF_WIDTH, DEFAULT_WIDTH);
            PlayerPrefs.SetInt(RESOLUTION_PREF_HEIGHT, DEFAULT_HEIGHT);
            PlayerPrefs.Save();
            
            float newAspectRatio = (float)DEFAULT_WIDTH / DEFAULT_HEIGHT;
            if (newAspectRatio != currentAspectRatio)
            {
                AdjustCameraForCurrentResolution();
                currentAspectRatio = newAspectRatio;
            }
            
            currentResolutionIndex = defaultIndex;
            pendingResolution = resolutions[defaultIndex];
            hasChanges = false;
        }
        
        File.WriteAllText(filePath, "");
        hasWrittenThree = false;
        
        confirmButton.GetComponent<Image>().color = confirmInactiveColor;
        
        UpdateConfirmButtonState();
    }

    public void ConfirmResolutionChangeNoColor()
    {
        if (!hasChanges) return;
        
        Screen.SetResolution(pendingResolution.width, pendingResolution.height, Screen.fullScreen);
        
        PlayerPrefs.SetInt(RESOLUTION_PREF_WIDTH, pendingResolution.width);
        PlayerPrefs.SetInt(RESOLUTION_PREF_HEIGHT, pendingResolution.height);
        PlayerPrefs.Save();
        
        float newAspectRatio = (float)pendingResolution.width / pendingResolution.height;
        if (newAspectRatio != currentAspectRatio)
        {
            AdjustCameraForCurrentResolution();
        }
        
        currentAspectRatio = newAspectRatio;
        hasChanges = false;
        currentResolutionIndex = resolutionDropdown.value;
        
        File.WriteAllText(filePath, "");
        hasWrittenThree = false;
        
        confirmButton.GetComponent<Image>().color = confirmInactiveColor;
    }

    void OnResolutionSelected(int index)
    {
        pendingResolution = resolutions[index];
        
        hasChanges = !(pendingResolution.width == Screen.width && 
                      pendingResolution.height == Screen.height);
                      
        UpdateConfirmButtonState();
    }
    
    private void UpdateConfirmButtonState()
    {
        string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        
        if (hasChanges && !hasWrittenThree)
        {
            confirmButton.GetComponent<Image>().color = confirmActiveColor;
            File.AppendAllText(filePath, "3");
            hasWrittenThree = true;
        }
        else if (!hasChanges && hasWrittenThree)
        {
            if (fileContent.Length == 1 && fileContent == "3")
            {
                confirmButton.GetComponent<Image>().color = confirmInactiveColor;
                File.WriteAllText(filePath, "");
            }
            else if (fileContent.Contains("3"))
            {
                int index = fileContent.IndexOf('3');
                fileContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, fileContent);
            }
            hasWrittenThree = false;
        }
        else if (!hasChanges)
        {
            if (fileContent.Length > 0 && (fileContent.Length > 1 || fileContent != "3"))
            {
                return;
            }
            confirmButton.GetComponent<Image>().color = confirmInactiveColor;
        }
    }
    
    void AdjustCameraForCurrentResolution()
    {
        Camera mainCamera = Camera.main;
        if (mainCamera == null) return;

        float targetAspect = referenceResolution.x / referenceResolution.y;
        float currentAspect = (float)Screen.width / Screen.height;
        float orthoSize = referenceResolution.y / 200f;

        if (mainCamera.orthographic)
        {
            if (currentAspect >= targetAspect)
            {
                mainCamera.orthographicSize = orthoSize;
            }
            else
            {
                float differenceInSize = targetAspect / currentAspect;
                mainCamera.orthographicSize = orthoSize * differenceInSize;
            }
        }
        else
        {
            float fov = 2f * Mathf.Atan(Mathf.Tan(mainCamera.fieldOfView * 0.5f * Mathf.Deg2Rad) * 
                (targetAspect / currentAspect)) * Mathf.Rad2Deg;
            
            mainCamera.fieldOfView = Mathf.Clamp(fov, 30f, 100f);
        }
    }

    public void ResetSettings()
    {
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
        
        pendingResolution = resolutions[currentResolutionIndex];
        hasChanges = false;
        
        File.WriteAllText(filePath, "");
        hasWrittenThree = false;
        
        confirmButton.GetComponent<Image>().color = confirmInactiveColor;
        
        UpdateConfirmButtonState();
    }
}