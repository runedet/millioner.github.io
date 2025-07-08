using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ButtonController1 : MonoBehaviour
{
    [SerializeField] private Button hardwareMouseButton;
    [SerializeField] private Button gameMouseButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button confirmNoColorButton;
    [SerializeField] private Button resetButton;
    [SerializeField] private Button defaultResetButton; // Новая кнопка
    [SerializeField] private Texture2D gameCursorTexture;
    
    private Color activeColor;
    private Color inactiveColor = Color.white;
    private Color confirmActiveColor;
    private Color confirmInactiveColor;
    private bool isGameMouseSelected = false;
    private bool currentIsGameMouse;
    private string filePath;
    private bool hasWrittenOne = false;
    
    private const string CURSOR_PREF_KEY = "IsGameCursor";
    private const bool DEFAULT_CURSOR_MODE = true; // true = игровой курсор по умолчанию
    
    void Start()
    {
        ColorUtility.TryParseHtmlString("#413A69", out activeColor);
        ColorUtility.TryParseHtmlString("#29325c", out confirmActiveColor);
        ColorUtility.TryParseHtmlString("#413A69", out confirmInactiveColor);
        
        filePath = Path.Combine(Application.dataPath, "birkin.txt");
        
        currentIsGameMouse = PlayerPrefs.GetInt(CURSOR_PREF_KEY, 1) == 1;
        isGameMouseSelected = currentIsGameMouse;
        
        if (currentIsGameMouse)
        {
            Cursor.SetCursor(gameCursorTexture, Vector2.zero, CursorMode.Auto);
            SetGameMouseState();
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
            SetHardwareMouseState();
        }
        
        hardwareMouseButton.onClick.AddListener(SelectHardwareMouse);
        gameMouseButton.onClick.AddListener(SelectGameMouse);
        confirmButton.onClick.AddListener(ApplyChanges);
        confirmNoColorButton.onClick.AddListener(ApplyChangesNoColor);
        resetButton.onClick.AddListener(ResetSettings);
        defaultResetButton.onClick.AddListener(ResetToDefault); // Новый слушатель
        
        UpdateConfirmButtonState();
    }
    
    
    void SetGameMouseState()
    {
        hardwareMouseButton.image.color = activeColor;
        gameMouseButton.image.color = inactiveColor;
        hardwareMouseButton.interactable = true;
        gameMouseButton.interactable = false;
    }
    
    void SetHardwareMouseState()
    {
        hardwareMouseButton.image.color = inactiveColor;
        gameMouseButton.image.color = activeColor;
        hardwareMouseButton.interactable = false;
        gameMouseButton.interactable = true;
    }
     void ResetToDefault()
    {
        // Проверяем, нужно ли менять состояние
        bool needChange = currentIsGameMouse != DEFAULT_CURSOR_MODE;
        
        if (needChange)
        {
            // Устанавливаем выбранное состояние
            isGameMouseSelected = DEFAULT_CURSOR_MODE;
            
            // Применяем курсор
            if (DEFAULT_CURSOR_MODE)
            {
                Cursor.SetCursor(gameCursorTexture, Vector2.zero, CursorMode.Auto);
                SetGameMouseState();
            }
            else
            {
                Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
                SetHardwareMouseState();
            }
            
            // Обновляем текущее состояние
            currentIsGameMouse = DEFAULT_CURSOR_MODE;
            
            // Сохраняем настройки
            PlayerPrefs.SetInt(CURSOR_PREF_KEY, DEFAULT_CURSOR_MODE ? 1 : 0);
            PlayerPrefs.Save();
        }
        
        // Очищаем файл в любом случае
        File.WriteAllText(filePath, "");
        hasWrittenOne = false;
        
        confirmButton.image.color = confirmInactiveColor;
        UpdateConfirmButtonState();
    }
    
    void SelectHardwareMouse()
    {
        if (!hardwareMouseButton.interactable) return;
        
        isGameMouseSelected = false;
        SetHardwareMouseState();
        UpdateConfirmButtonState();
    }
    
    void SelectGameMouse()
    {
        if (!gameMouseButton.interactable) return;
        
        isGameMouseSelected = true;
        SetGameMouseState();
        UpdateConfirmButtonState();
    }
    
    void UpdateConfirmButtonState()
    {
        bool hasChanges = isGameMouseSelected != currentIsGameMouse;
        string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
        
        if (hasChanges && !hasWrittenOne)
        {
            confirmButton.image.color = confirmActiveColor;
            File.AppendAllText(filePath, "1");
            hasWrittenOne = true;
        }
        else if (!hasChanges && hasWrittenOne)
        {
            if (fileContent.Length == 1 && fileContent == "1")
            {
                confirmButton.image.color = confirmInactiveColor;
                File.WriteAllText(filePath, "");
            }
            else if (fileContent.Contains("1"))
            {
                int index = fileContent.IndexOf('1');
                fileContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, fileContent);
            }
            hasWrittenOne = false;
        }
        else if (!hasChanges)
        {
            if (fileContent.Length > 0 && (fileContent.Length > 1 || fileContent != "1"))
            {
                return;
            }
            confirmButton.image.color = confirmInactiveColor;
        }
    }
    
    void ApplyChanges()
    {
        if (isGameMouseSelected == currentIsGameMouse) return;
        
        if (isGameMouseSelected)
        {
            Cursor.SetCursor(gameCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        
        currentIsGameMouse = isGameMouseSelected;
        
        PlayerPrefs.SetInt(CURSOR_PREF_KEY, currentIsGameMouse ? 1 : 0);
        PlayerPrefs.Save();
        
        File.WriteAllText(filePath, "");
        hasWrittenOne = false;
        
        UpdateConfirmButtonState();
    }
     void ApplyChangesNoColor()
    {
        if (isGameMouseSelected == currentIsGameMouse) return;
        
        if (isGameMouseSelected)
        {
            Cursor.SetCursor(gameCursorTexture, Vector2.zero, CursorMode.Auto);
        }
        else
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
        }
        
        currentIsGameMouse = isGameMouseSelected;
        
        PlayerPrefs.SetInt(CURSOR_PREF_KEY, currentIsGameMouse ? 1 : 0);
        PlayerPrefs.Save();
        
        File.WriteAllText(filePath, "");
        hasWrittenOne = false;
        
        // Устанавливаем обычный цвет для кнопки подтверждения
        confirmButton.image.color = confirmInactiveColor;
    }

    void ResetSettings()
    {
        File.WriteAllText(filePath, "");
        hasWrittenOne = false;
        
        isGameMouseSelected = currentIsGameMouse;
        
        if (currentIsGameMouse)
        {
            SetGameMouseState();
        }
        else
        {
            SetHardwareMouseState();
        }
        
        // Устанавливаем обычный цвет для кнопки подтверждения
        confirmButton.image.color = confirmInactiveColor;
        
        UpdateConfirmButtonState();
    }
}