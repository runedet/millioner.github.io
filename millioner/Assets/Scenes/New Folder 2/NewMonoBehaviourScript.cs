using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ButtonController : MonoBehaviour
{
    [SerializeField] private Button windowedButton;
    [SerializeField] private Button fullscreenButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button confirmNoColorButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button defaultResetButton;
    
    private Color activeColor;
    private Color inactiveColor = Color.white;
    private Color confirmActiveColor;
    private Color confirmInactiveColor;
    private bool isWindowedSelected = false;
    private bool currentIsWindowed;
    private string filePath;
    private bool hasWrittenTwo = false;
    
    private const string WINDOW_MODE_PREF_KEY = "IsWindowedMode";
    private const bool DEFAULT_WINDOW_MODE = false; // false = fullscreen по умолчанию
    
    void Start()
    {
        ColorUtility.TryParseHtmlString("#413A69", out activeColor);
        ColorUtility.TryParseHtmlString("#29325c", out confirmActiveColor);
        ColorUtility.TryParseHtmlString("#413A69", out confirmInactiveColor);
        
        filePath = Path.Combine(Application.dataPath, "birkin.txt");
        
        bool savedIsWindowed = PlayerPrefs.GetInt(WINDOW_MODE_PREF_KEY, 0) == 1;
        Screen.fullScreen = !savedIsWindowed;
        
        currentIsWindowed = !Screen.fullScreen;
        isWindowedSelected = currentIsWindowed;
        
        SetInitialState();
        
        windowedButton.onClick.AddListener(SelectWindowedMode);
        fullscreenButton.onClick.AddListener(SelectFullscreenMode);
        confirmButton.onClick.AddListener(ApplyChanges);
        confirmNoColorButton.onClick.AddListener(ApplyChangesNoColor);
        resetButton.onClick.AddListener(ResetSettings);
        defaultResetButton.onClick.AddListener(ResetToDefault);
        
        UpdateConfirmButtonState();
    }
    
    void SetInitialState()
    {
        if (Screen.fullScreen)
        {
            windowedButton.image.color = activeColor;
            fullscreenButton.image.color = inactiveColor;
            windowedButton.interactable = true;
            fullscreenButton.interactable = false;
        }
        else
        {
            windowedButton.image.color = inactiveColor;
            fullscreenButton.image.color = activeColor;
            windowedButton.interactable = false;
            fullscreenButton.interactable = true;
        }
    }
    
    void SelectWindowedMode()
    {
        if (!windowedButton.interactable) return;
        
        isWindowedSelected = true;
        
        windowedButton.image.color = inactiveColor;
        fullscreenButton.image.color = activeColor;
        windowedButton.interactable = false;
        fullscreenButton.interactable = true;
        
        UpdateConfirmButtonState();
    }
    
    void SelectFullscreenMode()
    {
        if (!fullscreenButton.interactable) return;
        
        isWindowedSelected = false;
        
        windowedButton.image.color = activeColor;
        fullscreenButton.image.color = inactiveColor;
        windowedButton.interactable = true;
        fullscreenButton.interactable = false;
        
        UpdateConfirmButtonState();
    }
    
    void UpdateConfirmButtonState()
    {
        bool hasChanges = isWindowedSelected != currentIsWindowed;
        string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        
        if (hasChanges && !hasWrittenTwo)
        {
            confirmButton.image.color = confirmActiveColor;
            File.AppendAllText(filePath, "2");
            hasWrittenTwo = true;
        }
        else if (!hasChanges && hasWrittenTwo)
        {
            if (fileContent.Length == 1 && fileContent == "2")
            {
                confirmButton.image.color = confirmInactiveColor;
                File.WriteAllText(filePath, "");
            }
            else if (fileContent.Contains("2"))
            {
                int index = fileContent.IndexOf('2');
                fileContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, fileContent);
            }
            hasWrittenTwo = false;
        }
        else if (!hasChanges)
        {
            if (fileContent.Length > 0 && (fileContent.Length > 1 || fileContent != "2"))
            {
                return;
            }
            confirmButton.image.color = confirmInactiveColor;
        }
    }
    
    void ApplyChanges()
    {
        if (isWindowedSelected == currentIsWindowed) return;
        
        Screen.fullScreen = !isWindowedSelected;
        currentIsWindowed = isWindowedSelected;
        
        PlayerPrefs.SetInt(WINDOW_MODE_PREF_KEY, currentIsWindowed ? 1 : 0);
        PlayerPrefs.Save();
        
        File.WriteAllText(filePath, "");
        hasWrittenTwo = false;
        
        UpdateConfirmButtonState();
    }

    void ApplyChangesNoColor()
    {
        if (isWindowedSelected == currentIsWindowed) return;
        
        Screen.fullScreen = !isWindowedSelected;
        currentIsWindowed = isWindowedSelected;
        
        PlayerPrefs.SetInt(WINDOW_MODE_PREF_KEY, currentIsWindowed ? 1 : 0);
        PlayerPrefs.Save();
        
        File.WriteAllText(filePath, "");
        hasWrittenTwo = false;
        
        confirmButton.image.color = confirmInactiveColor;
    }

    void ResetSettings()
    {
        File.WriteAllText(filePath, "");
        hasWrittenTwo = false;
        
        isWindowedSelected = currentIsWindowed;
        SetInitialState();
        
        confirmButton.image.color = confirmInactiveColor;
        
        UpdateConfirmButtonState();
    }

    void ResetToDefault()
    {
        // Сначала определяем, нужно ли менять состояние
        bool needChange = currentIsWindowed != DEFAULT_WINDOW_MODE;
        
        if (needChange)
        {
            // Устанавливаем выбранное состояние
            isWindowedSelected = DEFAULT_WINDOW_MODE;
            
            // Обновляем цвета и интерактивность кнопок
            if (DEFAULT_WINDOW_MODE) // Если оконный режим
            {
                windowedButton.image.color = inactiveColor;
                fullscreenButton.image.color = activeColor;
                windowedButton.interactable = false;
                fullscreenButton.interactable = true;
            }
            else // Если полноэкранный режим
            {
                windowedButton.image.color = activeColor;
                fullscreenButton.image.color = inactiveColor;
                windowedButton.interactable = true;
                fullscreenButton.interactable = false;
            }
            
            // Применяем настройки экрана
            Screen.fullScreen = !DEFAULT_WINDOW_MODE;
            currentIsWindowed = DEFAULT_WINDOW_MODE;
            
            // Сохраняем настройки
            PlayerPrefs.SetInt(WINDOW_MODE_PREF_KEY, DEFAULT_WINDOW_MODE ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        // Очищаем файл в любом случае
        File.WriteAllText(filePath, "");
        hasWrittenTwo = false;
        
        confirmButton.image.color = confirmInactiveColor;
        UpdateConfirmButtonState();
    }
}