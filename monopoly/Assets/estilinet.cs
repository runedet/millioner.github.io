using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Estilinet : MonoBehaviour
{
    public Texture2D[] buttonImages; // Массив изображений для кнопок
    public Texture2D[] imageDisplay; // Массив изображений для отображения
    public Button[] buttons; // Массив кнопок для отображения изображений
    public Image[] displayImages; // Массив компонентов Image для отображения дополнительных изображений

    void Start()
    {
        LoadIndicesFromFile();
    }

    private void LoadIndicesFromFile()
    {
        string path = Path.Combine(Application.persistentDataPath, "kraski.txt");

        if (File.Exists(path))
        {
            string[] lines = File.ReadAllLines(path);
            for (int i = 0; i < buttons.Length; i++)
            {
                // Рассчитываем базовый индекс для каждой кнопки
                int baseIndex = i * 4;

                // Проверяем, достаточно ли строк для этой кнопки
                if (i < lines.Length)
                {
                    // Пытаемся разобрать индекс из файла
                    if (int.TryParse(lines[i], out int index))
                    {
                        // Рассчитываем фактический индекс изображения на основе базового индекса
                        int imageIndex = baseIndex + index;

                        // Проверяем, находится ли индекс изображения в пределах массива buttonImages
                        if (imageIndex >= 0 && imageIndex < buttonImages.Length)
                        {
                            // Устанавливаем изображение кнопки
                            SetButtonImage(buttons[i], imageIndex);
                            buttons[i].gameObject.SetActive(true); // Делаем кнопку видимой
                        }
                        else
                        {
                            buttons[i].gameObject.SetActive(false); // Скрываем кнопку, если индекс недействителен
                        }

                        // Теперь назначаем изображение для displayImages на основе того же индекса
                        int displayImageIndex = index; // Используем тот же индекс для displayImages

                        if (displayImageIndex >= 0 && displayImageIndex < imageDisplay.Length)
                        {
                            SetDisplayImage(displayImages[i], displayImageIndex);
                            displayImages[i].gameObject.SetActive(true); // Делаем изображение видимым
                        }
                        else
                        {
                            displayImages[i].gameObject.SetActive(false); // Скрываем изображение, если индекс недействителен
                        }
                    }
                    else
                    {
                        buttons[i].gameObject.SetActive(false); // Скрываем кнопку, если разбор не удался
                        displayImages[i].gameObject.SetActive(false); // Скрываем изображение, если разбор не удался
                    }
                }
                else
                {
                    buttons[i].gameObject.SetActive(false); // Скрываем кнопку, если нет индекса
                    displayImages[i].gameObject.SetActive(false); // Скрываем изображение, если нет индекса
                }
            }
        }
        else
        {
            Debug.LogWarning("Файл kraski.txt не найден.");
        }
    }

    private void SetButtonImage(Button button, int imageIndex)
    {
        Image buttonImage = button.GetComponent<Image>();
        if (buttonImage != null && imageIndex < buttonImages.Length)
        {
            buttonImage.sprite = Sprite.Create(
                buttonImages[imageIndex],
                new Rect(0, 0, buttonImages[imageIndex].width, buttonImages[imageIndex].height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }

    private void SetDisplayImage(Image displayImage, int imageIndex)
    {
        if (displayImage != null && imageIndex < imageDisplay.Length)
        {
            displayImage.sprite = Sprite.Create(
                imageDisplay[imageIndex],
                new Rect(0, 0, imageDisplay[imageIndex].width, imageDisplay[imageIndex].height),
                new Vector2(0.5f, 0.5f)
            );
        }
    }
}
