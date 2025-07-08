using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ButtonAnimationController : MonoBehaviour
{
    [SerializeField] private Animator[] buttonAnimators = new Animator[7];
    [SerializeField] private Button[] buttons = new Button[7];
    [SerializeField] private Image[] flashlightImages = new Image[7];
    [SerializeField] private Sprite[] flashlightSprites = new Sprite[7];
    [SerializeField] private Sprite[] initialSprites = new Sprite[7];
    [SerializeField] private Button saveButton;
    private int currentActiveButton = 0;
    private string filePath;
    private bool isInitialized = false;

    void Start()
    {
        if (string.IsNullOrEmpty(Application.persistentDataPath))
        {
            Debug.LogError("persistentDataPath is null or empty");
            return;
        }

        Directory.CreateDirectory(Application.persistentDataPath);
        filePath = Path.Combine(Application.persistentDataPath, "fonariki.txt");
        
        // Always set currentActiveButton to 0 and save it
        currentActiveButton = 0;
        SaveToFile(true);

        // Reset all images to initial state
        for (int i = 0; i < flashlightImages.Length; i++)
        {
            flashlightImages[i].sprite = initialSprites[i];
        }

        // Set up the initial active button
        for (int i = 0; i < buttonAnimators.Length; i++)
        {
            buttonAnimators[i].enabled = (i == currentActiveButton);
            if (i == currentActiveButton)
            {
                flashlightImages[i].sprite = flashlightSprites[i];
            }
        }

        // Set up button listeners
        for (int i = 0; i < buttons.Length; i++)
        {
            int buttonIndex = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(buttonIndex));
        }

        saveButton.onClick.AddListener(() => SaveToFile(true));
        isInitialized = true;
    }

    void LoadSavedState()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string savedIndex = File.ReadAllText(filePath).Trim();
                if (int.TryParse(savedIndex, out int index) && index >= 0 && index < buttonAnimators.Length)
                {
                    currentActiveButton = index;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Error loading saved state: {e.Message}");
        }
    }

    void OnButtonClick(int buttonIndex)
    {
        if (currentActiveButton == buttonIndex)
            return;

        flashlightImages[currentActiveButton].sprite = initialSprites[currentActiveButton];
        buttonAnimators[currentActiveButton].enabled = false;

        buttonAnimators[buttonIndex].enabled = true;
        flashlightImages[buttonIndex].sprite = flashlightSprites[buttonIndex];

        currentActiveButton = buttonIndex;
        SaveToFile(false); // Автоматически сохраняем при каждом клике
    }

    void SaveToFile(bool forceSave = false)
    {
        if (!isInitialized && !forceSave)
            return;

        try
        {
            if (string.IsNullOrEmpty(filePath))
            {
                Debug.LogError("File path is null or empty");
                return;
            }

            string directory = Path.GetDirectoryName(filePath);
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            File.WriteAllText(filePath, currentActiveButton.ToString());
            Debug.Log($"Saved flashlight index {currentActiveButton} to {filePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error saving to file: {e.Message}");
        }
    }

    void OnDisable()
    {
        SaveToFile(true); // Сохраняем состояние при выключении компонента

        for (int i = 0; i < flashlightImages.Length; i++)
        {
            flashlightImages[i].sprite = initialSprites[i];
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }

        saveButton.onClick.RemoveAllListeners();
    }
}