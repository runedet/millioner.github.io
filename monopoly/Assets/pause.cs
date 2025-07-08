using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    public bool PauseGame;
    public GameObject pauseGameMenu;
    public GameObject canvasElement;
    
    // Добавляем ссылку на компонент Image панели
    public Image panelImage;
    
    // Параметры для изменения цвета
    private float hue = 0f;
    private float saturation = 0.7f;
    private float brightness = 0.7f;
    private float colorChangeSpeed = 0.3f;
    private bool isChangingColor = false;

    void Start()
    {
        PauseGame = false;
        pauseGameMenu.SetActive(false);
        canvasElement.SetActive(false);
        Time.timeScale = 1f;
        
        // Получаем компонент Image, если он не назначен
        if (panelImage == null && pauseGameMenu != null)
        {
            panelImage = pauseGameMenu.GetComponent<Image>();
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (PauseGame)
            {
                Resume();
            }
            else
            {
                Pause1();
            }
        }

        // Обновляем цвет панели, если она активна
        if (isChangingColor && panelImage != null)
        {
            UpdatePanelColor();
        }
    }

    void UpdatePanelColor()
    {
        // Увеличиваем значение цветового оттенка
        hue += colorChangeSpeed * Time.unscaledDeltaTime;
        
        // Сбрасываем значение, когда достигаем полного круга
        if (hue > 1f)
        {
            hue -= 1f;
        }

        // Конвертируем HSV в RGB
        Color newColor = Color.HSVToRGB(hue, saturation, brightness);
        
        // Сохраняем текущую прозрачность
        newColor.a = panelImage.color.a;
        
        // Применяем новый цвет
        panelImage.color = newColor;
    }

    public void Resume()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
        isChangingColor = false;
    }

    public void Pause1()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
        isChangingColor = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void ShowCanvas()
    {
        canvasElement.SetActive(true);
    }

    public void HideCanvas()
    {
        canvasElement.SetActive(false);
    }

    // Методы для настройки параметров изменения цвета
    public void SetColorChangeSpeed(float speed)
    {
        colorChangeSpeed = speed;
    }

    public void SetSaturation(float sat)
    {
        saturation = Mathf.Clamp01(sat);
    }

    public void SetBrightness(float bright)
    {
        brightness = Mathf.Clamp01(bright);
    }
}