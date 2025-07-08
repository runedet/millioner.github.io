using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript1123523466 : MonoBehaviour
{
    public Button[] buttons; // Массив кнопок
    private float[] buttonBlockTimers; // Таймеры блокировки для каждой кнопки
    private const float BLOCK_DURATION = 5f; // Длительность блокировки в секундах

    void Start()
    {
        // Инициализируем массив таймеров
        buttonBlockTimers = new float[buttons.Length];
    }

    void Update()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            // Проверяем видимость кнопки
            if (IsButtonVisible(buttons[i]))
            {
                // Если кнопка видима и не заблокирована
                if (buttonBlockTimers[i] <= 0)
                {
                    // Блокируем кнопку на 5 секунд
                    buttonBlockTimers[i] = BLOCK_DURATION;
                    buttons[i].interactable = false;
                }
            }

            // Обновляем таймер блокировки
            if (buttonBlockTimers[i] > 0)
            {
                buttonBlockTimers[i] -= Time.deltaTime;
                
                // Если таймер истёк, разблокируем кнопку
                if (buttonBlockTimers[i] <= 0)
                {
                    buttons[i].interactable = true;
                }
            }
        }
    }

    // Функция проверки видимости кнопки
    private bool IsButtonVisible(Button button)
    {
        // Проверяем, что кнопка существует и активна
        if (button == null || !button.gameObject.activeInHierarchy)
            return false;

        // Получаем RectTransform кнопки
        RectTransform rectTransform = button.GetComponent<RectTransform>();
        if (rectTransform == null)
            return false;

        // Проверяем, находится ли кнопка в пределах экрана
        Vector3[] corners = new Vector3[4];
        rectTransform.GetWorldCorners(corners);
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        
        foreach (Vector3 corner in corners)
        {
            Vector3 screenPoint = Camera.main.WorldToScreenPoint(corner);
            if (screenRect.Contains(new Vector2(screenPoint.x, screenPoint.y)))
                return true;
        }

        return false;
    }
}