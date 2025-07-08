using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class NewMonoBehaviourScript1273809 : MonoBehaviour
{
    public Button moveButton;
    public Button moveDownButton;
    public Button showTopObjectButton;
    private string imaFilePath = "ima.txt";  // File in the project root folder
    private string imaContent = "";
    public Button hideTopObjectButton;
    public Button[] animationButtons = new Button[0];
    public GameObject yuu;
    public GameObject darkenPanel; // Новая панель затемнения
    private bool isYuuVisible = false;
    
    public GameObject[] objects = new GameObject[4];
    public GameObject panel;
    public GameObject[] topSlideInObjects = new GameObject[10]; 
    public GameObject moveDownPanel;
    public Text uiText;
    private string[] messages;
    private int currentIndex = 0;
    private bool isFirstTextChange = true;
    
    public GameObject simpleObject;
    public GameObject bottomSlideObject;
    public GameObject[] leftSlideObjects = new GameObject[7];
    public GameObject topSlideObject;
    
    private string filePath;
    private int activeLeftObjectIndex = 0;
    private bool shouldAnimateFromRight = false;
    private bool shouldAnimate = true;
    
    public float animationDuration = 5.0f;
    public float leftObjectDuration = 10.0f;
    public float additionalAnimationDuration = 1.0f;
    private bool isMoving = false;
    private string garantFilePath;
    private readonly int[] amounts = new int[] 
    { 
        500, 1000, 2000, 3000, 5000, 10000, 15000, 25000, 
        50000, 100000, 200000, 400000, 800000, 1500000, 3000000 
    };
    private float heightMultiplier = 10.83f;
    private float moveDistance;

   void Start()
{
    messages = new string[] 
    {
        $"{imaContent} выберите несгораемую сумму, нажав на старый обьект на цепях.",
        "",
        "",
        "",
        ""
    };

    Debug.Log($"First message: {messages[0]}"); // Для отладки

        if (darkenPanel != null)
        {
            darkenPanel.SetActive(false);
        }

        if (darkenPanel != null)
        {
            darkenPanel.SetActive(false);
        }

        filePath = Path.Combine(Application.persistentDataPath, "fonariki.txt");

        moveButton.onClick.AddListener(StartMoveUp);
        moveDownButton.onClick.AddListener(StartMoveDown);
        showTopObjectButton.onClick.AddListener(ShowTopObject);
        hideTopObjectButton.onClick.AddListener(HideTopObject);

        // Модифицированная инициализация кнопок анимации
        // Modified animation buttons initialization
        for (int i = 0; i < animationButtons.Length; i++)
        {
            int buttonIndex = i;
            if (animationButtons[i] != null)
            {
                animationButtons[i].onClick.AddListener(() =>
                {
                    OnAnimationButtonClick(buttonIndex);
                    UpdateGarantFile(buttonIndex);
                    HideAllTopSlideInObjects();
                });
            }
        }


        if (simpleObject) simpleObject.SetActive(false);
        if (bottomSlideObject) bottomSlideObject.SetActive(false);
        foreach (var obj in leftSlideObjects)
        {
            if (obj) obj.SetActive(false);
        }
        foreach (var obj in topSlideInObjects)
        {
            if (obj) obj.SetActive(false);
        }
        if (topSlideObject) topSlideObject.SetActive(false);
        if (moveDownPanel) moveDownPanel.SetActive(false);

        foreach (GameObject obj in objects)
        {
            if (obj != null)
            {
                Animator animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false;
                }

                if (obj.transform is RectTransform)
                {
                    Canvas.ForceUpdateCanvases();
                }
            }
        }

        moveDistance = objects[0].GetComponent<RectTransform>().rect.height * heightMultiplier;
        for (int i = 0; i < animationButtons.Length; i++)
        {
            int buttonIndex = i;
            if (animationButtons[i] != null)
            {
                animationButtons[i].onClick.AddListener(() => OnAnimationButtonClick(buttonIndex));
            }
        }

        if (yuu != null)
        {
            yuu.SetActive(false);
        }
        garantFilePath = Path.Combine(Application.persistentDataPath, "garant.txt");

        for (int i = 0; i < animationButtons.Length; i++)
        {
            int buttonIndex = i;
            if (animationButtons[i] != null)
            {
                animationButtons[i].onClick.AddListener(() => {
                    OnAnimationButtonClick(buttonIndex);
                    UpdateGarantFile(buttonIndex);
                });
            }
        }
    }

    private void UpdateGarantFile(int buttonIndex)
    {
        try
        {
            if (buttonIndex >= 0 && buttonIndex < amounts.Length)
            {
                // Write value to file
                File.WriteAllText(garantFilePath, amounts[buttonIndex].ToString());
                Debug.Log($"Successfully wrote {amounts[buttonIndex]} to garant.txt");
            }
            else
            {
                Debug.LogError($"Button index {buttonIndex} is out of range");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error writing to garant.txt: {e.Message}");
        }
    }
    private void ReadFileAndSetupAnimation()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string indexStr = File.ReadAllText(filePath);
                activeLeftObjectIndex = int.Parse(indexStr);

                if (activeLeftObjectIndex == 0 || activeLeftObjectIndex == 2)
                {
                    shouldAnimateFromRight = true;
                    shouldAnimate = true;
                }
                else if (activeLeftObjectIndex >= 1 && activeLeftObjectIndex <= 5)
                {
                    shouldAnimateFromRight = false;
                    shouldAnimate = true;
                }
                else if (activeLeftObjectIndex == 6)
                {
                    shouldAnimate = false;
                }
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading file: {e.Message}");
        }
    }

    void StartMoveUp()
    {
        if (!isMoving)
        {
            StopAllCoroutines();
            StartCoroutine(MoveUpAnimation());
            
            if (simpleObject) simpleObject.SetActive(false);
            if (bottomSlideObject) bottomSlideObject.SetActive(false);
            foreach(var obj in leftSlideObjects)
            {
                if(obj) obj.SetActive(false);
            }
            if (topSlideObject) topSlideObject.SetActive(false);
            if (moveDownPanel) moveDownPanel.SetActive(false);
        }
    }

    private void OnAnimationButtonClick(int buttonIndex)
    {
        if (!isMoving)
        {
            StopAllCoroutines();
            StartCoroutine(HandleButtonAnimation(buttonIndex));
        }
    }

    private IEnumerator HandleButtonAnimation(int buttonIndex)
    {
        isMoving = true;

        foreach (GameObject obj in topSlideInObjects)
        {
            if (obj != null && obj.activeSelf)
            {
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                Vector2 startPosition = rectTransform.anchoredPosition;
                Vector2 targetPosition = startPosition + new Vector2(0, 700);

                float elapsedTime = 0f;
                while (elapsedTime < additionalAnimationDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float progress = elapsedTime / additionalAnimationDuration;
                    float smoothProgress = Mathf.SmoothStep(0, 1, progress);

                    rectTransform.anchoredPosition = Vector2.Lerp(
                        startPosition,
                        targetPosition,
                        smoothProgress
                    );

                    yield return null;
                }

                obj.SetActive(false);
                rectTransform.anchoredPosition = startPosition;
            }
        }

        if (yuu != null)
        {
            yuu.SetActive(true);
            RectTransform yuuRect = yuu.GetComponent<RectTransform>();
            Vector2 finalPosition = yuuRect.anchoredPosition;
            yuuRect.anchoredPosition = finalPosition + new Vector2(0, 700);

            float elapsedTime = 0f;
            while (elapsedTime < additionalAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / additionalAnimationDuration;
                float smoothProgress = Mathf.SmoothStep(0, 1, progress);

                yuuRect.anchoredPosition = Vector2.Lerp(
                    finalPosition + new Vector2(0, 700),
                    finalPosition,
                    smoothProgress
                );

                yield return null;
            }

            yuuRect.anchoredPosition = finalPosition;
            isYuuVisible = true;
        }

        isMoving = false;
    }
  private void HideAllTopSlideInObjects()
    {
        StartCoroutine(HideAllTopSlideInObjectsCoroutine());
    }

    private IEnumerator HideAllTopSlideInObjectsCoroutine()
{
    float animationDuration = additionalAnimationDuration;
    
    // Собираем все активные объекты и их начальные позиции
    List<(GameObject obj, Vector2 startPos)> activeObjects = new List<(GameObject, Vector2)>();
    
    foreach (GameObject obj in topSlideInObjects)
    {
        if (obj != null && obj.activeSelf)
        {
            RectTransform rectTransform = obj.GetComponent<RectTransform>();
            activeObjects.Add((obj, rectTransform.anchoredPosition));
        }
    }

    // Анимируем все объекты одновременно
    float elapsedTime = 0f;
    while (elapsedTime < animationDuration)
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / animationDuration;
        float smoothProgress = Mathf.SmoothStep(0, 1, progress);

        foreach (var (obj, startPos) in activeObjects)
        {
            Vector2 targetPosition = startPos + new Vector2(0, 700);
            obj.GetComponent<RectTransform>().anchoredPosition = Vector2.Lerp(
                startPos,
                targetPosition,
                smoothProgress
            );
        }

        yield return null;
    }

    // Деактивируем все объекты после завершения анимации
    foreach (var (obj, _) in activeObjects)
    {
        obj.SetActive(false);
    }
}
    void StartMoveDown()
    {
        if (!isMoving)
        {
            StopAllCoroutines();
            
            if (moveDownPanel != null)
            {
                moveDownPanel.SetActive(true);
                CanvasGroup canvasGroup = moveDownPanel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = moveDownPanel.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 1f;
            }

            if (panel != null)
            {
                panel.SetActive(true);
                CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
                if (canvasGroup == null)
                {
                    canvasGroup = panel.AddComponent<CanvasGroup>();
                }
                canvasGroup.alpha = 1f;
            }

            if (topSlideObject != null && topSlideObject.activeSelf)
            {
                StartCoroutine(HideTopObjectWithMoveDown());
            }
            else
            {
                StartCoroutine(MoveDownAnimation());
            }
        }
    }

    void ShowTopObject()
    {
        if (!isMoving && topSlideObject != null)
        {
            StopAllCoroutines();
            StartCoroutine(ShowTopObjectAnimation());
        }
    }

    void HideTopObject()
    {
        if (!isMoving && topSlideObject != null && topSlideObject.activeSelf)
        {
            StopAllCoroutines();
            StartCoroutine(HideTopObjectAnimation());
        }
    }

    IEnumerator ShowAdditionalObjects()
    {
        if (simpleObject)
        {
            simpleObject.SetActive(true);
        }

        if (bottomSlideObject)
        {
            bottomSlideObject.SetActive(true);
            RectTransform rectTransform = bottomSlideObject.GetComponent<RectTransform>();
            Vector2 finalPosition = rectTransform.anchoredPosition;
            
            rectTransform.anchoredPosition = finalPosition - new Vector2(0, 500);
            
            float elapsedTime = 0f;
            while (elapsedTime < additionalAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / additionalAnimationDuration;
                float smoothProgress = Mathf.SmoothStep(0, 1, progress);
                
                rectTransform.anchoredPosition = Vector2.Lerp(
                    finalPosition - new Vector2(0, 500),
                    finalPosition,
                    smoothProgress
                );
                
                yield return null;
            }
            
            rectTransform.anchoredPosition = finalPosition;
        }

        if (activeLeftObjectIndex >= 0 && activeLeftObjectIndex < leftSlideObjects.Length && 
            leftSlideObjects[activeLeftObjectIndex] != null)
        {
            GameObject activeLeftObject = leftSlideObjects[activeLeftObjectIndex];
            activeLeftObject.SetActive(true);
            RectTransform rectTransform = activeLeftObject.GetComponent<RectTransform>();
            Vector2 finalPosition = rectTransform.anchoredPosition;

            if (activeLeftObjectIndex == 6)
            {
                rectTransform.anchoredPosition = finalPosition - new Vector2(0, 700);
                
                float elapsedTime = 0f;
                while (elapsedTime < leftObjectDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float progress = elapsedTime / leftObjectDuration;
                    float smoothProgress = Mathf.SmoothStep(0, 1, progress);
                    
                    rectTransform.anchoredPosition = Vector2.Lerp(
                        finalPosition - new Vector2(0, 700),
                        finalPosition,
                        smoothProgress
                    );
                    
                    yield return null;
                }
                
                rectTransform.anchoredPosition = finalPosition;
            }
            else if (shouldAnimate)
            {
                Vector2 startOffset = shouldAnimateFromRight ? new Vector2(700, 0) : new Vector2(-700, 0);
                rectTransform.anchoredPosition = finalPosition + startOffset;
                
                float elapsedTime = 0f;
                while (elapsedTime < leftObjectDuration)
                {
                    elapsedTime += Time.deltaTime;
                    float progress = elapsedTime / leftObjectDuration;
                    float smoothProgress = Mathf.SmoothStep(0, 1, progress);
                    
                    rectTransform.anchoredPosition = Vector2.Lerp(
                        finalPosition + startOffset,
                        finalPosition,
                        smoothProgress
                    );
                    
                    yield return null;
                }
                
                rectTransform.anchoredPosition = finalPosition;
            }
            else
            {
                rectTransform.anchoredPosition = finalPosition;
            }
        }
    }

    IEnumerator ShowTopObjectAnimation()
    {
        isMoving = true;
        topSlideObject.SetActive(true);
        
        RectTransform rectTransform = topSlideObject.GetComponent<RectTransform>();
        Vector2 finalPosition = rectTransform.anchoredPosition;
        
        rectTransform.anchoredPosition = finalPosition + new Vector2(0, 500);
        
        float elapsedTime = 0f;
        while (elapsedTime < additionalAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / additionalAnimationDuration;
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);
            
            rectTransform.anchoredPosition = Vector2.Lerp(
                finalPosition + new Vector2(0, 500),
                finalPosition,
                smoothProgress
            );
            
            yield return null;
        }
        
        rectTransform.anchoredPosition = finalPosition;
        isMoving = false;
    }

    IEnumerator HideTopObjectAnimation()
    {
        isMoving = true;
        
        RectTransform rectTransform = topSlideObject.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = startPosition + new Vector2(0, 500);
        
        float elapsedTime = 0f;
        while (elapsedTime < additionalAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / additionalAnimationDuration;
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);
            
            rectTransform.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );
            
            yield return null;
        }
        
        topSlideObject.SetActive(false);
        rectTransform.anchoredPosition = startPosition;
        isMoving = false;
    }

    IEnumerator HideTopObjectWithMoveDown()
    {
        isMoving = true;
        
        RectTransform rectTransform = topSlideObject.GetComponent<RectTransform>();
        Vector2 startPosition = rectTransform.anchoredPosition;
        Vector2 targetPosition = startPosition + new Vector2(0, 500);
        
        float elapsedTime = 0f;
        while (elapsedTime < additionalAnimationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / additionalAnimationDuration;
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);
            
            rectTransform.anchoredPosition = Vector2.Lerp(
                startPosition,
                targetPosition,
                smoothProgress
            );
            
            yield return null;
        }
        
        topSlideObject.SetActive(false);
        rectTransform.anchoredPosition = startPosition;
        
        StartCoroutine(MoveDownAnimation());
    }

    IEnumerator MoveUpAnimation()
    {
        isMoving = true;
        float elapsedTime = 0f;
        
        Vector3[] startPositions = new Vector3[objects.Length];
        Vector3[] targetPositions = new Vector3[objects.Length];
        
        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                Animator animator = objects[i].GetComponent<Animator>();
                if (animator != null)
                {
                    animator.enabled = false;
                }
                
                startPositions[i] = objects[i].transform.position;
                targetPositions[i] = startPositions[i] + new Vector3(0, moveDistance, 0);
                
                RectTransform rectTransform = objects[i].GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    rectTransform.localScale = Vector3.one;
                    rectTransform.rotation = Quaternion.identity;
                }
            }
        }

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float smoothProgress = Mathf.SmoothStep(0, 1, progress);

            for (int i = 0; i < objects.Length; i++)
            {
                if (objects[i] != null)
                {
                    objects[i].transform.position = Vector3.Lerp(
                        startPositions[i], 
                        targetPositions[i], 
                        smoothProgress
                    );
                }
            }

            yield return null;
        }

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].transform.position = targetPositions[i];
            }
        }

        StartCoroutine(FadeOutPanel());

        isMoving = false;
    }

    public void ChangeToNextText()
{
    if (uiText != null)
    {
        currentIndex = (currentIndex + 1) % messages.Length;
        
        // If we're returning to the first message, make sure to include the ima text
        if (currentIndex == 0)
        {
            uiText.text = $"{imaContent} выберите несгораемую сумму, нажав на старый обьект на цепях.";
        }
        else
        {
            uiText.text = messages[currentIndex];
        }
    }
}

 private void ReadImaFileAndUpdateText()
{
    try
    {
        string primaryPath = Path.Combine(Application.dataPath, "ima.txt");
        Debug.Log($"Looking for ima.txt at path: {primaryPath}");
        
        if (File.Exists(primaryPath))
        {
            imaContent = File.ReadAllText(primaryPath).Trim();
            Debug.Log($"Successfully found and read ima.txt at: {primaryPath}");
            Debug.Log($"Content: {imaContent}");
        }
        else
        {
            Debug.LogWarning($"ima.txt file not found at path: {primaryPath}");
        }

        // Update the first message with the new ima content
        messages[0] = $"{imaContent} выберите несгораемую сумму, нажав на старый обьект на цепях.";
        
        // Update UI text if it exists
        if (uiText != null)
        {
            uiText.text = messages[0];
        }

        Debug.Log($"Updated first message: {messages[0]}");
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error reading ima.txt: {e.Message}");
    }
}

IEnumerator MoveDownAnimation()
{
    isMoving = true;
    float elapsedTime = 0f;
    
    Vector3[] startPositions = new Vector3[objects.Length];
    Vector3[] targetPositions = new Vector3[objects.Length];
    
    for (int i = 0; i < objects.Length; i++)
    {
        if (objects[i] != null)
        {
            Animator animator = objects[i].GetComponent<Animator>();
            if (animator != null)
            {
                animator.enabled = false;
            }
            
            startPositions[i] = objects[i].transform.position;
            targetPositions[i] = startPositions[i] - new Vector3(0, moveDistance, 0);
            
            RectTransform rectTransform = objects[i].GetComponent<RectTransform>();
            if (rectTransform != null)
            {
                rectTransform.localScale = Vector3.one;
                rectTransform.rotation = Quaternion.identity;
            }
        }
    }

    while (elapsedTime < animationDuration)
    {
        elapsedTime += Time.deltaTime;
        float progress = elapsedTime / animationDuration;
        float smoothProgress = Mathf.SmoothStep(0, 1, progress);

        for (int i = 0; i < objects.Length; i++)
        {
            if (objects[i] != null)
            {
                objects[i].transform.position = Vector3.Lerp(
                    startPositions[i], 
                    targetPositions[i], 
                    smoothProgress
                );
            }
        }

        if (progress > 0.8f)
        {
            if (panel != null)
            {
                StartCoroutine(FadeOutPanel());
            }
            if (moveDownPanel != null)
            {
                StartCoroutine(FadeOutMoveDownPanel());
            }
        }

        yield return null;
    }

    for (int i = 0; i < objects.Length; i++)
    {
        if (objects[i] != null)
        {
            objects[i].transform.position = targetPositions[i];
        }
    }

    // Read ima.txt and update text after animation completes
    ReadImaFileAndUpdateText();

    ReadFileAndSetupAnimation();
    StartCoroutine(ShowAdditionalObjects());
    StartCoroutine(ShowTopSlideInObjects());

    isMoving = false;
    yield break;
}

    IEnumerator ShowTopSlideInObjects()
    {
        // Показываем и настраиваем панель затемнения
        if (darkenPanel != null)
        {
            darkenPanel.SetActive(true);
            CanvasGroup panelCanvasGroup = darkenPanel.GetComponent<CanvasGroup>();
            if (panelCanvasGroup == null)
            {
                panelCanvasGroup = darkenPanel.AddComponent<CanvasGroup>();
            }
            panelCanvasGroup.alpha = 1f;
        }

        foreach (GameObject obj in topSlideInObjects)
        {
            if (obj != null)
            {
                obj.SetActive(true);
                RectTransform rectTransform = obj.GetComponent<RectTransform>();
                if (rectTransform != null)
                {
                    Vector2 finalPosition = rectTransform.anchoredPosition;
                    rectTransform.anchoredPosition = finalPosition + new Vector2(0, 700);
                    
                    float elapsedTime = 0f;
                    while (elapsedTime < additionalAnimationDuration)
                    {
                        elapsedTime += Time.deltaTime;
                        float progress = elapsedTime / additionalAnimationDuration;
                        float smoothProgress = Mathf.SmoothStep(0, 1, progress);
                        
                        rectTransform.anchoredPosition = Vector2.Lerp(
                            finalPosition + new Vector2(0, 700),
                            finalPosition,
                            smoothProgress
                        );
                        
                        yield return null;
                    }
                    
                    rectTransform.anchoredPosition = finalPosition;
                }
            }
            yield return new WaitForSeconds(0.2f);
        }

        // Плавно скрываем панель затемнения
        if (darkenPanel != null)
        {
            CanvasGroup panelCanvasGroup = darkenPanel.GetComponent<CanvasGroup>();
            float fadeDuration = 0.5f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / fadeDuration;
                panelCanvasGroup.alpha = Mathf.Lerp(1f, 0f, progress);
                yield return null;
            }

            darkenPanel.SetActive(false);
        }
    }

    IEnumerator FadeOutPanel()
    {
        if (panel != null)
        {
            CanvasGroup canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = panel.AddComponent<CanvasGroup>();
            }

            float fadeDuration = 1.0f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }

            panel.SetActive(false);
        }
    }

    IEnumerator FadeOutMoveDownPanel()
    {
        if (moveDownPanel != null)
        {
            CanvasGroup canvasGroup = moveDownPanel.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = moveDownPanel.AddComponent<CanvasGroup>();
            }

            float fadeDuration = 1.0f;
            float elapsedTime = 0f;

            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.deltaTime;
                float alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
                canvasGroup.alpha = alpha;
                yield return null;
            }

            moveDownPanel.SetActive(false);
        }
    }
}