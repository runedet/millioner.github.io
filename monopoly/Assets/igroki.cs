using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.IO;

public class ImageChanger : MonoBehaviour
{
    public Texture2D[] images;
    private int[] currentImageIndex;
    public Image[] displayImages;
    public Button[] buttons;
    private bool[] isButtonBlocked;
    private Dictionary<int, int> buttonRestrictedImages = new Dictionary<int, int>();
    public Button startGameButton; // Reference to the Start Game button

    void Start()
    {
        currentImageIndex = new int[buttons.Length];
        isButtonBlocked = new bool[buttons.Length];

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));

            // Randomly assign a different image to each button
            currentImageIndex[i] = Random.Range(0, 4); // Assuming you have at least 4 images
            UpdateDisplayImage(i);
        }

        // Add listener for Start Game button
        startGameButton.onClick.AddListener(OnStartGameClicked);
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                if (RectTransformUtility.RectangleContainsScreenPoint((RectTransform)buttons[i].transform, Input.mousePosition))
                {
                    ToggleButtonLock(i);
                    break;
                }
            }
        }
    }

    private void OnButtonClick(int buttonIndex)
    {
        if (!isButtonBlocked[buttonIndex])
        {
            ChangeImage(buttonIndex);
        }
    }

    private void ChangeImage(int buttonIndex)
    {
        int nextIndex = currentImageIndex[buttonIndex];
        bool found = false;
        int attempts = 0;

        while (!found && attempts < 4)
        {
            nextIndex = (nextIndex + 1) % 4;
            found = true;

            // Check all blocked buttons for restricted images
            foreach (var restrictedImage in buttonRestrictedImages)
            {
                if (restrictedImage.Value == nextIndex)
                {
                    found = false;
                    break;
                }
            }
            attempts++;
        }

        currentImageIndex[buttonIndex] = nextIndex;
        UpdateDisplayImage(buttonIndex);
    }

    private void UpdateDisplayImage(int buttonIndex)
    {
        int imageIndex = buttonIndex * 4 + currentImageIndex[buttonIndex];
        if (imageIndex < images.Length)
        {
            displayImages[buttonIndex].sprite = Sprite.Create(
                images[imageIndex],
                new Rect(0, 0, images[imageIndex].width, images[imageIndex].height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    private void ToggleButtonLock(int buttonIndex)
    {
        isButtonBlocked[buttonIndex] = !isButtonBlocked[buttonIndex];
        buttons[buttonIndex].interactable = !isButtonBlocked[buttonIndex];

        // Handle restricted images
        if (isButtonBlocked[buttonIndex])
        {
            buttonRestrictedImages[buttonIndex] = currentImageIndex[buttonIndex];
            UpdateOtherButtons(buttonIndex);
        }
        else
        {
            if (buttonRestrictedImages.ContainsKey(buttonIndex))
            {
                buttonRestrictedImages.Remove(buttonIndex);
            }
        }

        Debug.Log(isButtonBlocked[buttonIndex] 
            ? $"Кнопка {buttonIndex} заблокирована. Запрещенное изображение: {currentImageIndex[buttonIndex]}" 
            : $"Кнопка {buttonIndex} разблокирована.");
    }

    private void UpdateOtherButtons(int excludeButtonIndex)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (i != excludeButtonIndex && !isButtonBlocked[i])
            {
                if (currentImageIndex[i] == currentImageIndex[excludeButtonIndex])
                {
                    ChangeToNextAvailableImage(i);
                }
            }
        }
    }

    private void ChangeToNextAvailableImage(int buttonIndex)
    {
        int nextIndex = currentImageIndex[buttonIndex];
        bool found = false;
        int attempts = 0;

        while (!found && attempts < 4)
        {
            nextIndex = (nextIndex + 1) % 4;
            found = true;

            foreach (var restrictedImage in buttonRestrictedImages)
            {
                if (restrictedImage.Value == nextIndex)
                {
                    found = false;
                    break;
                }
            }
            attempts++;
        }

        if (found)
        {
            currentImageIndex[buttonIndex] = nextIndex;
            UpdateDisplayImage(buttonIndex);
        }
    }

    private void OnStartGameClicked()
    {
        List<int> blockedIndices = new List<int>();

        for (int i = 0; i < buttons.Length; i++)
        {
            if (isButtonBlocked[i])
            {
                blockedIndices.Add(currentImageIndex[i]);
            }
        }

        SaveBlockedIndicesToFile(blockedIndices);
    }

    private void SaveBlockedIndicesToFile(List<int> indices)
    {
        string path = Path.Combine(Application.persistentDataPath, "kraski.txt");

        using (StreamWriter writer = new StreamWriter(path))
        {
            foreach (var index in indices)
            {
                writer.WriteLine(index);
            }
        }

        Debug.Log($"Сохраненные индексы заблокированных изображений в файл: {path}");
    }
}
