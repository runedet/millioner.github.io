using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class ButtonManager : MonoBehaviour
{
    public Button[] buttons;
    public Image[] fog;
    public Image cloud;
    public Image background;
    public Sprite newBackgroundSprite;
    private bool hasMovedCloud = false;

    // Массив для хранения корутин анимации тумана
    private Coroutine[] fogAnimationCoroutines;

    void Start()
    {
        fogAnimationCoroutines = new Coroutine[buttons.Length];
        
        // Изначально скрываем весь туман
        foreach (Image fogImage in fog)
        {
            Color color = fogImage.color;
            color.a = 0;
            fogImage.color = color;
        }

        // Добавляем слушатели для всех кнопок
        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i; // Сохраняем индекс для использования в лямбда-выражении
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
            
            // Добавляем слушатель изменения состояния интерактивности
            MonitorButtonInteractable(index);
        }
    }

    private void MonitorButtonInteractable(int index)
    {
        StartCoroutine(CheckButtonState(index));
    }

    private IEnumerator CheckButtonState(int index)
    {
        bool previousState = buttons[index].interactable;
        
        while (true)
        {
            if (previousState != buttons[index].interactable)
            {
                previousState = buttons[index].interactable;
                
                // Если кнопка заблокирована, показываем туман
                if (!buttons[index].interactable)
                {
                    if (fogAnimationCoroutines[index] != null)
                        StopCoroutine(fogAnimationCoroutines[index]);
                    fogAnimationCoroutines[index] = StartCoroutine(AnimateFog(index, true));
                }
                // Если кнопка разблокирована, скрываем туман
                else
                {
                    if (fogAnimationCoroutines[index] != null)
                        StopCoroutine(fogAnimationCoroutines[index]);
                    fogAnimationCoroutines[index] = StartCoroutine(AnimateFog(index, false));
                }
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator AnimateFog(int index, bool appear)
    {
        if (index >= fog.Length) yield break;

        float duration = 0.5f;
        float currentTime = 0f;
        
        float startAlpha = fog[index].color.a;
        float targetAlpha = appear ? 1f : 0f;

        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float alpha = Mathf.Lerp(startAlpha, targetAlpha, currentTime / duration);
            
            Color newColor = fog[index].color;
            newColor.a = alpha;
            fog[index].color = newColor;
            
            yield return null;
        }

        // Устанавливаем конечное значение прозрачности
        Color finalColor = fog[index].color;
        finalColor.a = targetAlpha;
        fog[index].color = finalColor;
    }

    public void StartGame()
    {
        bool isButton0Blocked = !buttons[0].interactable;
        bool isButton1Blocked = !buttons[1].interactable;
        bool isButton2Blocked = buttons.Length > 2 && !buttons[2].interactable;
        bool isButton3Blocked = buttons.Length > 3 && !buttons[3].interactable;

        if (isButton0Blocked && isButton1Blocked && 
            (isButton2Blocked || !isButton2Blocked && !isButton3Blocked))
        {
            SceneManager.LoadScene(3);
        }
        else
        {
            background.sprite = newBackgroundSprite;

            if (!hasMovedCloud)
            {
                StartCoroutine(MoveCloud());
                hasMovedCloud = true;
            }
        }
    }

    private void OnButtonClick(int index)
    {
        // Обработка клика по кнопке
        Debug.Log($"Button {index} clicked");
    }

    private IEnumerator MoveCloud()
    {
        Vector3 targetPosition = new Vector3(cloud.transform.position.x - 100, 
            cloud.transform.position.y, cloud.transform.position.z);
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startingPosition = cloud.transform.position;

        while (elapsedTime < duration)
        {
            cloud.transform.position = Vector3.Lerp(startingPosition, 
                targetPosition, (elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cloud.transform.position = targetPosition;
    }
}