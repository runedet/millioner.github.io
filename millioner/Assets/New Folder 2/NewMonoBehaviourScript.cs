using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using PUSHKA.MySQL;
using System.IO;
using System.Linq;

public class NewMonoBehaviourScript1234 : MonoBehaviour
{
    // ... другие переменные ...
    public Image topPauseImages1;
private const string GAMES_PLAYED_FILE = "games_played_stats.txt";
private const string HIGH_SCORE_FILE = "high_score_stats.txt";
private const string CORRECT_ANSWERS_FILE = "correct_answers_stats.txt";
private const string HINTS_USED_FILE = "hints_used_stats.txt";
private const string MAX_QUESTION_LEVEL_FILE = "max_question_level_stats.txt";
// ... остальные переменные ...
    public TMP_Text endGameMessageText; // Назначьте этот компонент в инспекторе Unity
private bool isGameOver = false;    // Флаг для обозначения конца игры
private Vector3 originalEndGameMessageScale; // Для хранения исходного размера текста сообщения конца игры
    private float audioTimeWhenPaused = 0f;
private string lastLegacyText = "";
private bool wasAudioPlaying = false;
    public GameObject escBlocker; // Object that blocks ESC from closing pause menu when active
    // Add these at the top with other public variables
    public GameObject slideObject1; // First object to slide
public GameObject slideObject2; // Second object to slide
public Button showObject1Button; // Button to show first object
public Button hideObject1Button; // Button to hide first object 
public Button showObject2Button; // Button to show second object
public Button hideObject2Button; // Button to hide second object
private Vector3 slideObject1OriginalPos; // Original position of first object
private Vector3 slideObject2OriginalPos; // Original position of second object
    public GameObject lolo; // Reference to the lolo object
    public Button resumeButton; // Button to resume game
    public Button showLoloButton; // Button to show lolo object
    public Button exitButton; // Button to exit to scene 0
    public Button restartButton; // Button to restart game
    public Image[] topPauseImages; // Images that will appear from top
public Image[] bottomPauseImages; // Images that will appear from bottom
private Vector2[] originalTopImagePositions;
private Vector2[] originalBottomImagePositions;
private const float PAUSE_ANIMATION_SPEED = 0.5f;
public GameObject pausePanel; // Reference to the pause menu panel
private bool isPaused = false;
private Vector3 originalPausePanelScale;
private const float PAUSE_ANIMATION_DURATION = 0.3f;
public TMP_Text[] audienceAnswerTexts; // Array of 4 texts for answer variants
    // Add these new fields at the class level
    public Slider[] audienceSliders; // Array of 4 sliders
public TMP_Text[] audiencePercentTexts; // Array of 4 percentage texts
private float[] targetPercentages; // To store target percentages
private bool isAnimatingPercentages = false;
private float percentageAnimationDuration = 3f; // Duration of percentage animation
public GameObject help; // Добавьте это поле в список полей класса
public GameObject panel1; // Add this line at the class level
    public AudioClip endCallSound; // Added end call sound
    private const float SLIDE_ANIMATION_DURATION = 0.8f; // Duration in seconds for slide animations
private const float SLIDE_ANIMATION_DISTANCE = 100f; // Distance above screen for slide animations
    public GameObject olePanel; // Panel that appears with Ole
public AudioSource dialToneAudioSource; // Separate audio source for dial tone sound
    public GameObject ole; // Reference to the ole GameObject
    public Button confirmFriendNameButton; // Кнопка подтверждения имени друга
public TMP_InputField friendNameInput; // Input field for friend's name
private Vector3 oleOriginalPosition; // Store original position of ole object
    public TMP_Text timerText;
    private float currentTime = 90f;
private float maxTime = 90f;
    private Coroutine timerCoroutine;
    private bool isQuestionText = false;
    private AudioSource audioSource;
    public TMP_Text textElement;
    public Text legacyText;
    public Button[] moneyButtons;
    public Button[] answerButtons;
    public Button[] additionalButtons;
    public Button[] hideButtonsSet1;
    public Button[] hideButtonsSet2;
    public GameObject[] associatedObjects;
    private class CoroutineState
    {
        public IEnumerator Coroutine;
        public bool IsRunning;
    }
private List<CoroutineState> activeCoroutines = new List<CoroutineState>();
    private Vector3[] objectOriginalPositions;
    private bool hasSecondChance = false;
    private GameObject currentlyShownObject;
    public TMP_Text[] answerTexts;
    public TMP_Text scoreText;
    public TMP_Text garantText;
    private const string IMA_FILE = "ima.txt";
private string lastImaValue = "";
private float imaCheckInterval = 1f; // Проверять каждую секунду
private Coroutine imaCheckCoroutine;
    public GameObject questionContainer;
    public GameObject panel;
    private Coroutine panelCoroutine;
    private bool isLoadingGarant = false;
    private SqlDataBase dataBase;
    private List<int> usedQuestionIds = new List<int>();
    private float typingSpeed = 0.05f;
    private Vector3 originalQuestionContainerPosition;
    private int currentQuestionIndex = -1;
    private DataTable cachedQuestions;
    private string garantContent;
    private int[] moneyAmounts = new int[] { 500, 1000, 2000, 3000, 5000, 10000, 15000, 25000, 50000, 100000, 200000, 400000, 800000, 1500000, 3000000 };
    private int currentMoneyIndex = 0;
    private bool isFirstQuestion = true;
    private Vector2[] originalButtonPositions;
    private Vector2[] originalAnswerTextPositions;
    private Vector3[] additionalButtonOriginalPositions;
    public Image timerImage; // Reference to the timer image component
public Sprite activeTimerSprite; // Sprite for active timer
public Sprite inactiveTimerSprite; // Sprite for inactive timer
    private Vector3 originalTimerScale;
public AudioClip friendCallSound; // Sound to play when calling friend
    private bool isPlayingFriendCallSound = false;

    void Start()
{
    IncrementGamesPlayed();
    imaCheckCoroutine = StartCoroutine(CheckImaFile());
    audioSource = GetComponent<AudioSource>();
    if (audioSource == null)
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }
    if (friendNameInput != null)
    {
        friendNameInput.characterLimit = 10;
        friendNameInput.onValidateInput += ValidateRussianInput;
    }
    originalButtonPositions = new Vector2[answerButtons.Length];
    originalAnswerTextPositions = new Vector2[answerTexts.Length];

    // Сохраняем оригинальные позиции
    for (int i = 0; i < answerButtons.Length; i++)
    {
        originalButtonPositions[i] = answerButtons[i].GetComponent<RectTransform>().anchoredPosition;
        originalAnswerTextPositions[i] = answerTexts[i].GetComponent<RectTransform>().anchoredPosition;
    }
     if (topPauseImages != null && bottomPauseImages != null)
    {
        originalTopImagePositions = new Vector2[topPauseImages.Length];
        originalBottomImagePositions = new Vector2[bottomPauseImages.Length];

        // Store original positions
        for (int i = 0; i < topPauseImages.Length; i++)
        {
            originalTopImagePositions[i] = topPauseImages[i].rectTransform.anchoredPosition;
            topPauseImages[i].gameObject.SetActive(false);
        }
          if (resumeButton != null)
        {
            // resumeButton.onClick.AddListener(() => TogglePause()); // Listener будет добавлен/изменен позже
        }

        if (showLoloButton != null)
        {
            showLoloButton.onClick.AddListener(() => ToggleLolo());
        }
          if (slideObject1 != null)
    {
        slideObject1OriginalPos = slideObject1.transform.position;
        slideObject1.SetActive(false);
    }
    
    if (slideObject2 != null)
    {
        slideObject2OriginalPos = slideObject2.transform.position;
        slideObject2.SetActive(false);
    }
    
    if (showObject1Button != null)
        showObject1Button.onClick.AddListener(ShowSlideObject1);
        
    if (hideObject1Button != null)
        hideObject1Button.onClick.AddListener(HideSlideObject1);
        
    if (showObject2Button != null)
        showObject2Button.onClick.AddListener(ShowSlideObject2);
        
    if (hideObject2Button != null)
        hideObject2Button.onClick.AddListener(HideSlideObject2);
        
    // Modify existing RestartGame and ExitToMainMenu methods to include hide animations
    if (restartButton != null)
        restartButton.onClick.RemoveAllListeners();
        restartButton.onClick.AddListener(OnRestartButtonClick);
        
    if (exitButton != null)
        exitButton.onClick.RemoveAllListeners();
        exitButton.onClick.AddListener(OnExitButtonClick);

        // Удаляем старые слушатели, если они были, чтобы избежать дублирования
        if (resumeButton != null) resumeButton.onClick.RemoveAllListeners();
        if (resumeButton != null) resumeButton.onClick.AddListener(TogglePause); // Добавляем слушатель здесь

        // if (exitButton != null) // Уже обрабатывается OnExitButtonClick
        // {
        //     exitButton.onClick.AddListener(() => ExitToMainMenu());
        // }

        // if (restartButton != null) // Уже обрабатывается OnRestartButtonClick
        // {
        //     restartButton.onClick.AddListener(() => RestartGame());
        // }


        if (lolo != null)
        {
            lolo.SetActive(false);
        }

        for (int i = 0; i < bottomPauseImages.Length; i++)
            {
                originalBottomImagePositions[i] = bottomPauseImages[i].rectTransform.anchoredPosition;
                bottomPauseImages[i].gameObject.SetActive(false);
            }
    }

    if (panel != null)
    {
        panel.SetActive(false);
    }
    if (panel1 != null)
    {
        panel1.SetActive(false);
    }
    if (help != null)
    {
        help.SetActive(false);
    }
    if (timerImage != null)
    {
        originalTimerScale = timerText.transform.localScale; // Убедитесь, что originalTimerScale инициализируется до endGameMessageText
        timerImage.sprite = isQuestionText ? activeTimerSprite : inactiveTimerSprite;
    }

    // Инициализация endGameMessageText
    if (endGameMessageText != null)
    {
        originalEndGameMessageScale = endGameMessageText.transform.localScale;
        endGameMessageText.gameObject.SetActive(false);
    }

    additionalButtonOriginalPositions = new Vector3[additionalButtons.Length];
    objectOriginalPositions = new Vector3[associatedObjects.Length];
    
    for (int i = 0; i < additionalButtons.Length; i++)
    {
        additionalButtonOriginalPositions[i] = additionalButtons[i].transform.position;
        additionalButtons[i].gameObject.SetActive(false);
    }
    if (pausePanel != null)
    {
        originalPausePanelScale = pausePanel.transform.localScale;
        pausePanel.SetActive(false);
    }

    for (int i = 0; i < associatedObjects.Length; i++)
    {
        objectOriginalPositions[i] = associatedObjects[i].transform.position;
        associatedObjects[i].SetActive(false);
    }

    timerCoroutine = StartCoroutine(StartTimer());
    StartCoroutine(Initialize());

    // Setup hide buttons
    for (int i = 0; i < hideButtonsSet1.Length; i++)
    {
        int buttonIndex = i; // Захват переменной для лямбда-выражения
        hideButtonsSet1[i].onClick.AddListener(() => HideAssociatedObject(buttonIndex));
    }
    if (ole != null)
    {
        oleOriginalPosition = ole.transform.position;
        ole.SetActive(false);
        
        if (confirmFriendNameButton != null)
        {
            confirmFriendNameButton.onClick.AddListener(ConfirmFriendName);
        }
    }

    for (int i = 0; i < hideButtonsSet2.Length; i++)
    {
        int buttonIndex = i; // Захват переменной
        hideButtonsSet2[i].onClick.AddListener(() => HideAssociatedObject(buttonIndex));
    }
}

    void Update()
{
    bool shouldBlockButtons = audioSource.isPlaying || currentlyShownObject != null;

    // Блокировка кнопок ответов и подсказок во время проигрывания аудио или показа объекта,
    // или если игра окончена.
    bool blockGameActions = shouldBlockButtons || isGameOver;

    foreach (var button in answerButtons)
    {
        button.interactable = !blockGameActions;
    }

    foreach (var button in additionalButtons)
    {
        button.interactable = !blockGameActions;
    }

    if (Input.GetKeyDown(KeyCode.Escape))
    {
        // Если игра окончена и меню паузы уже показано, TogglePause ничего не сделает.
        // Если игра окончена и меню паузы не показано (например, во время 3-секундной задержки),
        // TogglePause покажет его, и resumeButton будет неактивен.
        // Если игра не окончена, TogglePause работает как обычно.
        TogglePause();
    }

    // Кнопки скрытия всегда активны (если это требование)
    // Если они тоже должны блокироваться при isGameOver, добавьте проверку
    foreach (var button in hideButtonsSet1)
    {
        button.interactable = !isGameOver; // Пример: блокируем, если игра окончена
    }

    foreach (var button in hideButtonsSet2)
    {
        button.interactable = !isGameOver; // Пример: блокируем, если игра окончена
    }
}
    private char ValidateRussianInput(string text, int charIndex, char addedChar)
    {
        // Check if the character is a Russian letter
        if ((addedChar >= 'а' && addedChar <= 'я') || 
            (addedChar >= 'А' && addedChar <= 'Я') || 
            addedChar == 'ё' || addedChar == 'Ё')
        {
            return addedChar;
        }
        
        // Return empty char if not Russian
        return '\0';
    }
    private IEnumerator HandleEndOfGame(string message, bool isWinOutcome, bool isFullGameWinContext = false)
{
    isGameOver = true;
    isQuestionText = false; 

    // Скрываем resumeButton и topPauseImages1 перед показом меню паузы
    if (resumeButton != null)
    {
        resumeButton.gameObject.SetActive(false);
    }
    if (topPauseImages1 != null)
    {
        topPauseImages1.gameObject.SetActive(false);
    }

    int gameScore = 0; // Сумма для обновления рекорда в этой игре

    if (isWinOutcome)
    {
        if (isFullGameWinContext) 
        {
            gameScore = moneyAmounts[moneyAmounts.Length - 1];
            AddToMoneyFile(gameScore); 
            WriteToWinFile(true);
        }
        else 
        {
            gameScore = currentMoneyIndex > 0 ? moneyAmounts[currentMoneyIndex - 1] : 0;
            AddToMoneyFile(gameScore);
            WriteToWinFile(true); 
        }
    }
    else 
    {
        gameScore = GetCurrentGarantAmount(); // В случае проигрыша, рекорд может быть несгораемой суммой
        AddToMoneyFile(gameScore); 
        WriteToWinFile(false);
    }
    UpdateHighScore(gameScore); 
    UpdateMaxQuestionLevelReached(currentMoneyIndex + 1);
    
    if (questionContainer != null && questionContainer.activeSelf)
    {
        float elapsedTime = 0f;
        Vector3 startPos = questionContainer.transform.position;
        Vector3 endPos = startPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);

        while (elapsedTime < SLIDE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime; // Используем deltaTime, т.к. Time.timeScale еще не 0
            float t = elapsedTime / SLIDE_ANIMATION_DURATION;
            float smoothT = t * t * (3f - 2f * t);
            questionContainer.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
        questionContainer.SetActive(false);
    }
    else if (questionContainer != null)
    {
        questionContainer.SetActive(false);
    }

    yield return StartCoroutine(HideAllAdditionalButtonsSimultaneously());

    if (endGameMessageText != null)
    {
        endGameMessageText.text = message;
        endGameMessageText.gameObject.SetActive(true);
        SetLegacyTextAndRestartTimer(message, false); // Озвучка сообщения о конце игры
        yield return StartCoroutine(EndGameMessagePulseAndShrinkAnimation()); // Используем новую анимацию
    }
    else
    {
        SetLegacyTextAndRestartTimer(message, false); // Озвучка, даже если текста нет
    }

    if (!isPaused)
    {
        // Вызываем TogglePause с флагом gameOver
        StartCoroutine(ShowPauseMenuAfterGameOver());
    }
}

private IEnumerator ShowPauseMenuAfterGameOver()
{
    // Убеждаемся, что resumeButton и topPauseImages1 скрыты
    if (resumeButton != null) resumeButton.gameObject.SetActive(false);
    if (topPauseImages1 != null) topPauseImages1.gameObject.SetActive(false);

    // Устанавливаем флаг паузы
    isPaused = true;
    Time.timeScale = 0f;

    // Показываем панель паузы
    if (pausePanel != null)
    {
        pausePanel.SetActive(true);
        yield return StartCoroutine(AnimatePauseImages(true));
    }

    // Делаем кнопку возобновления неактивной (хотя она уже скрыта)
    if (resumeButton != null) resumeButton.interactable = false;
}

// В методе TogglePause добавляем проверку на isGameOver
private void TogglePause()
{
    if (isPaused) // Попытка снять с паузы
    {
        if (isGameOver) // Нельзя снять с паузы, если игра окончена
        {
            if (resumeButton != null) resumeButton.interactable = false;
            return;
        }
        
        // Если пытаемся снять паузу (не конец игры), но блокировщик активен
        if (escBlocker != null && escBlocker.activeSelf)
        {
            return;
        }

        // Обычное снятие с паузы
        isPaused = false;
        Time.timeScale = 1f;

        if (pausePanel != null)
        {
            StartCoroutine(AnimatePauseImages(false)); // Это уже обрабатывает pausePanel.SetActive(false)
        }

        // Восстанавливаем корутины
        foreach (var state in activeCoroutines)
        {
            if (state.IsRunning)
            {
                StartCoroutine(state.Coroutine);
            }
        }
        activeCoroutines.Clear();

        // Восстанавливаем аудио
        if (wasAudioPlaying && audioSource != null)
        {
            StartCoroutine(ResumeAudioState());
        }

        // Возобновляем dial tone
        if (dialToneAudioSource != null && dialToneAudioSource.clip != null)
        {
            dialToneAudioSource.UnPause();
        }
    }
    else // Попытка поставить на паузу
    {
        // Если игра уже окончена, используем специальный метод показа паузы
        if (isGameOver)
        {
            StartCoroutine(ShowPauseMenuAfterGameOver());
            return;
        }

        // Обычная пауза во время игры
        isPaused = true;

        // Сохраняем состояние аудио
        if (audioSource != null)
        {
            wasAudioPlaying = audioSource.isPlaying;
            if (wasAudioPlaying)
            {
                audioTimeWhenPaused = audioSource.time;
                lastLegacyText = legacyText.text; // Сохраняем текст, который был во время аудио
                audioSource.Stop();
            }
        }
        
        // Сохраняем активные корутины
        activeCoroutines.Clear();
        var allCoroutines = GetCoroutinesList(); 
        foreach (var coroutine in allCoroutines)
        {
            activeCoroutines.Add(new CoroutineState { Coroutine = coroutine, IsRunning = true });
        }

        // Пауза для dial tone
        if (dialToneAudioSource != null)
        {
            dialToneAudioSource.Pause();
        }
        
        // Скрываем панели
        if (panel != null) panel.SetActive(false);
        if (panel1 != null) panel1.SetActive(false);

        Time.timeScale = 0f; // Ставим игру на паузу

        if (pausePanel != null)
        {
            pausePanel.SetActive(true);
            StartCoroutine(AnimatePauseImages(true));
        }

        if (resumeButton != null)
        {
            resumeButton.interactable = true;
        }
    }
}

    private IEnumerator EndGameMessagePulseAndShrinkAnimation()
    {
        if (endGameMessageText == null) yield break;

        Vector3 originalScale = originalEndGameMessageScale;
        Vector3 maxScale = originalScale * 1.5f;
        float pulseDuration = 9.0f;
        int pulseCount = 3;

        endGameMessageText.color = new Color32(209, 209, 209, 255);
        endGameMessageText.transform.localScale = originalScale;

        for (int i = 0; i < pulseCount; i++)
        {
            float elapsedTime = 0f;
            float singlePulsePhaseDuration = pulseDuration / (pulseCount * 2f);

            while (elapsedTime < singlePulsePhaseDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / singlePulsePhaseDuration);
                float smoothT = t * t * (3f - 2f * t);
                endGameMessageText.transform.localScale = Vector3.Lerp(originalScale, maxScale, smoothT);
                yield return null;
            }
            endGameMessageText.transform.localScale = maxScale;

            elapsedTime = 0f;
            while (elapsedTime < singlePulsePhaseDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = Mathf.Clamp01(elapsedTime / singlePulsePhaseDuration);
                float smoothT = t * t * (3f - 2f * t);
                endGameMessageText.transform.localScale = Vector3.Lerp(maxScale, originalScale, smoothT);
                yield return null;
            }
            endGameMessageText.transform.localScale = originalScale;
        }

        // --- Начало новой части: Анимация уменьшения ---
        float shrinkDuration = 0.7f; // Длительность уменьшения
        float elapsedShrinkTime = 0f;
        Vector3 currentScale = endGameMessageText.transform.localScale; // Должен быть originalScale

        while (elapsedShrinkTime < shrinkDuration)
        {
            elapsedShrinkTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedShrinkTime / shrinkDuration);
            float smoothT = t * t; // Простое замедление (ease-in) для уменьшения
            endGameMessageText.transform.localScale = Vector3.Lerp(currentScale, Vector3.zero, smoothT);
            yield return null;
        }
        // --- Конец новой части ---

        endGameMessageText.gameObject.SetActive(false); // Скрываем объект после анимации
        endGameMessageText.transform.localScale = originalScale; // Восстанавливаем исходный размер на случай повторного использования
    }

    private IEnumerator CheckImaFile()
    {
        while (true)
        {
            yield return new WaitForSecondsRealtime(imaCheckInterval);

            string filePath = Path.Combine(Application.persistentDataPath, IMA_FILE);
            if (File.Exists(filePath))
            {
                string currentValue = File.ReadAllText(filePath).Trim();
                if (currentValue != lastImaValue)
                {
                    lastImaValue = currentValue;
                }
            }
        }
    }
    private IEnumerator AnimateQuestionContainerOut()
    {
        float elapsedTime = 0f;
        Vector3 startPos = questionContainer.transform.position;
        Vector3 endPos = startPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);

        while (elapsedTime < SLIDE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / SLIDE_ANIMATION_DURATION;
            float smoothT = t * t * (3f - 2f * t); // Smooth easing
            questionContainer.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        questionContainer.SetActive(false);

        // Показываем промежуточный выигрыш только если игра не окончена
        if (!isGameOver)
        {
            ShowWinningAmount(); // Это для пульсации счета между вопросами
        }
    }
    private void WriteToMoneyFile(int amount)
{
    string filePath = Path.Combine(Application.persistentDataPath, "money.txt");
    int currentMoney = 0;

    // Read current money if file exists
    if (File.Exists(filePath))
    {
        string content = File.ReadAllText(filePath);
        int.TryParse(content, out currentMoney);
    }

    // Add new amount
    currentMoney += amount;

    // Write updated amount
    File.WriteAllText(filePath, currentMoney.ToString());
}

private void WriteToWinFile(bool isWin)
{
    string filePath = Path.Combine(Application.persistentDataPath, "win.txt");
    File.WriteAllText(filePath, isWin ? "1" : "0");
}
    private void ToggleLolo()
    {
        if (lolo != null)
        {
            lolo.SetActive(!lolo.activeSelf);
        }
    }
private void ShowSlideObject1()
{
    StartCoroutine(AnimateSlideObjectShow(slideObject1, slideObject1OriginalPos));
}

private void HideSlideObject1()
{
    StartCoroutine(AnimateSlideObjectHide(slideObject1, slideObject1OriginalPos));
}

private void ShowSlideObject2()
{
    StartCoroutine(AnimateSlideObjectShow(slideObject2, slideObject2OriginalPos));
}

private void HideSlideObject2()
{
    StartCoroutine(AnimateSlideObjectHide(slideObject2, slideObject2OriginalPos));
}

private IEnumerator AnimateSlideObjectShow(GameObject obj, Vector3 originalPos)
{
    if (obj == null) yield break;

    obj.transform.position = originalPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);
    obj.SetActive(true);

    float elapsedTime = 0f;
    Vector3 startPos = obj.transform.position;
    Vector3 endPos = originalPos;

    while (elapsedTime < SLIDE_ANIMATION_DURATION)
    {
        elapsedTime += Time.unscaledDeltaTime; // Changed from deltaTime to unscaledDeltaTime
        float t = elapsedTime / SLIDE_ANIMATION_DURATION;
        float smoothT = t * t * (3f - 2f * t); // Smooth easing
        obj.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        yield return null;
    }

    obj.transform.position = endPos;
}

private IEnumerator AnimateSlideObjectHide(GameObject obj, Vector3 originalPos)
{
    if (obj == null || !obj.activeSelf) yield break;

    float elapsedTime = 0f;
    Vector3 startPos = obj.transform.position;
    Vector3 endPos = startPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);

    while (elapsedTime < SLIDE_ANIMATION_DURATION)
    {
        elapsedTime += Time.unscaledDeltaTime; // Changed from deltaTime to unscaledDeltaTime
        float t = elapsedTime / SLIDE_ANIMATION_DURATION;
        float smoothT = t * t * (3f - 2f * t); // Smooth easing
        obj.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        yield return null;
    }

    obj.SetActive(false);
    obj.transform.position = originalPos;
}
  
    
private IEnumerator RestartGameWithAnimation()
{
    if (slideObject1 != null && slideObject1.activeSelf)
    {
        yield return StartCoroutine(AnimateSlideObjectHide(slideObject1, slideObject1OriginalPos));
    }
    
    if (slideObject2 != null && slideObject2.activeSelf)
    {
        yield return StartCoroutine(AnimateSlideObjectHide(slideObject2, slideObject2OriginalPos));
    }
    
    yield return new WaitForSecondsRealtime(0.1f); // Use WaitForSecondsRealtime instead of WaitForSeconds
    Time.timeScale = 1f;
    UnityEngine.SceneManagement.SceneManager.LoadScene(
        UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
    );
}

private IEnumerator ExitToMainMenuWithAnimation()
{
    if (slideObject1 != null && slideObject1.activeSelf)
    {
        yield return StartCoroutine(AnimateSlideObjectHide(slideObject1, slideObject1OriginalPos));
    }
    
    if (slideObject2 != null && slideObject2.activeSelf)
    {
        yield return StartCoroutine(AnimateSlideObjectHide(slideObject2, slideObject2OriginalPos));
    }
    
    yield return new WaitForSecondsRealtime(0.1f); // Use WaitForSecondsRealtime instead of WaitForSeconds
    Time.timeScale = 1f;
    UnityEngine.SceneManagement.SceneManager.LoadScene(0);
}
private void OnRestartButtonClick()
{
    StartCoroutine(RestartGameWithAnimation());
}

private void OnExitButtonClick()
{
    StartCoroutine(ExitToMainMenuWithAnimation());
}

    private void ExitToMainMenu()
    {
        Time.timeScale = 1f; // Reset time scale before loading new scene
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    private void RestartGame()
    {
        Time.timeScale = 1f; // Reset time scale before restarting
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex
        );
    }


    // Модифицированный TogglePause() без скрытия кнопок


    private IEnumerator AnimatePauseImages(bool showing)
    {
        float screenHeight = Screen.height;

        if (showing)
        {
            // Activate all images but position them off-screen
            foreach (var img in topPauseImages)
            {
                img.gameObject.SetActive(true);
                img.rectTransform.anchoredPosition = new Vector2(
                    img.rectTransform.anchoredPosition.x,
                    screenHeight + 100
                );
            }

            foreach (var img in bottomPauseImages)
            {
                img.gameObject.SetActive(true);
                img.rectTransform.anchoredPosition = new Vector2(
                    img.rectTransform.anchoredPosition.x,
                    -screenHeight - 100
                );
            }

            // Animate them to their original positions
            float elapsedTime = 0f;
            while (elapsedTime < PAUSE_ANIMATION_SPEED)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / PAUSE_ANIMATION_SPEED;
                float smoothT = t * t * (3f - 2f * t); // Smooth easing

                for (int i = 0; i < topPauseImages.Length; i++)
                {
                    topPauseImages[i].rectTransform.anchoredPosition = Vector2.Lerp(
                        new Vector2(originalTopImagePositions[i].x, screenHeight + 100),
                        originalTopImagePositions[i],
                        smoothT
                    );
                }

                for (int i = 0; i < bottomPauseImages.Length; i++)
                {
                    bottomPauseImages[i].rectTransform.anchoredPosition = Vector2.Lerp(
                        new Vector2(originalBottomImagePositions[i].x, -screenHeight - 100),
                        originalBottomImagePositions[i],
                        smoothT
                    );
                }

                yield return null;
            }
        }
        else
        {
            // Animate images off-screen
            float elapsedTime = 0f;
            while (elapsedTime < PAUSE_ANIMATION_SPEED)
            {
                elapsedTime += Time.unscaledDeltaTime;
                float t = elapsedTime / PAUSE_ANIMATION_SPEED;
                float smoothT = t * t * (3f - 2f * t); // Smooth easing

                for (int i = 0; i < topPauseImages.Length; i++)
                {
                    topPauseImages[i].rectTransform.anchoredPosition = Vector2.Lerp(
                        originalTopImagePositions[i],
                        new Vector2(originalTopImagePositions[i].x, screenHeight + 100),
                        smoothT
                    );
                }

                for (int i = 0; i < bottomPauseImages.Length; i++)
                {
                    bottomPauseImages[i].rectTransform.anchoredPosition = Vector2.Lerp(
                        originalBottomImagePositions[i],
                        new Vector2(originalBottomImagePositions[i].x, -screenHeight - 100),
                        smoothT
                    );
                }

                yield return null;
            }

            // Deactivate all images
            foreach (var img in topPauseImages)
            {
                img.gameObject.SetActive(false);
            }
            foreach (var img in bottomPauseImages)
            {
                img.gameObject.SetActive(false);
            }

            pausePanel.SetActive(false);
        }
    }
    
private IEnumerator StartTimer()
    {
        while (true)
        {
            if (isQuestionText && currentlyShownObject == null && !isPaused) // Add isPaused check
            {
                if (timerImage != null && timerImage.sprite != activeTimerSprite)
                {
                    timerImage.sprite = activeTimerSprite;
                }

                // Используем Time.unscaledDeltaTime для независимого от частоты кадров отсчета
                currentTime -= Time.unscaledDeltaTime;

                if (currentTime <= 0)
                {
                    HandleTimerExpiration();
                    currentTime = maxTime;
                }

                int newTime = Mathf.CeilToInt(currentTime);
                if (timerText.text != newTime.ToString())
                {
                    StartCoroutine(AnimateTimerText(newTime.ToString()));
                }
            }
            else
            {
                if (timerImage != null && timerImage.sprite != inactiveTimerSprite)
                {
                    timerImage.sprite = inactiveTimerSprite;
                }
            }
            yield return null;
        }
    }



// Метод для получения всех активных корутин
private List<IEnumerator> GetCoroutinesList()
{
    // Здесь нужно вручную перечислить все корутины, которые могут быть активны
    var list = new List<IEnumerator>();
    
    
    // Добавьте другие корутины по аналогии
    
    return list;
}
private void DisableAllButtons()
{
    foreach (var button in moneyButtons)
    {
        if (button != null) button.interactable = false;
    }
    foreach (var button in answerButtons)
    {
        if (button != null) button.interactable = false;
    }
    foreach (var button in additionalButtons)
    {
        if (button != null) button.interactable = false;
    }
    foreach (var button in hideButtonsSet1)
    {
        if (button != null) button.interactable = false;
    }
    foreach (var button in hideButtonsSet2)
    {
        if (button != null) button.interactable = false;
    }
}

private void EnableAllButtons()
{
    foreach (var button in moneyButtons)
    {
        if (button != null) button.interactable = true;
    }
    foreach (var button in answerButtons)
    {
        if (button != null) button.interactable = true;
    }
    foreach (var button in additionalButtons)
    {
        if (button != null) button.interactable = true;
    }
    foreach (var button in hideButtonsSet1)
    {
        if (button != null) button.interactable = true;
    }
    foreach (var button in hideButtonsSet2)
    {
        if (button != null) button.interactable = true;
    }
}

private void RestartCoroutines()
{
    // Restart necessary coroutines
    timerCoroutine = StartCoroutine(StartTimer());
    if (isQuestionText)
    {
        currentTime = maxTime;
    }
}


private IEnumerator AnimateQuestionContainerIn()
    {
        float elapsedTime = 0f;
        Vector3 startPos = questionContainer.transform.position;
        Vector3 endPos = originalQuestionContainerPosition;

        while (elapsedTime < SLIDE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / SLIDE_ANIMATION_DURATION;
            float smoothT = t * t * (3f - 2f * t); // Smooth easing
            questionContainer.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        questionContainer.transform.position = endPos;
    }
private int ReadIntFromFile(string fileName, int defaultValue = 0)
{
    string filePath = Path.Combine(Application.persistentDataPath, fileName);
    if (File.Exists(filePath))
    {
        if (int.TryParse(File.ReadAllText(filePath), out int value))
        {
            return value;
        }
    }
    return defaultValue;
}

private void WriteIntToFile(string fileName, int value)
{
    string filePath = Path.Combine(Application.persistentDataPath, fileName);
    File.WriteAllText(filePath, value.ToString());
}

// --- Методы для обновления статистики ---

private void IncrementGamesPlayed()
{
    int gamesPlayed = ReadIntFromFile(GAMES_PLAYED_FILE);
    gamesPlayed++;
    WriteIntToFile(GAMES_PLAYED_FILE, gamesPlayed);
}

private void UpdateHighScore(int currentScore)
{
    int highScore = ReadIntFromFile(HIGH_SCORE_FILE);
    if (currentScore > highScore)
    {
        WriteIntToFile(HIGH_SCORE_FILE, currentScore);
    }
}

private void IncrementCorrectAnswers()
{
    int correctAnswers = ReadIntFromFile(CORRECT_ANSWERS_FILE);
    correctAnswers++;
    WriteIntToFile(CORRECT_ANSWERS_FILE, correctAnswers);
}

private void IncrementHintsUsed()
{
    int hintsUsed = ReadIntFromFile(HINTS_USED_FILE);
    hintsUsed++;
    WriteIntToFile(HINTS_USED_FILE, hintsUsed);
}

private void UpdateMaxQuestionLevelReached(int level) // Уровень от 1 до 15
{
    int maxLevel = ReadIntFromFile(MAX_QUESTION_LEVEL_FILE);
    if (level > maxLevel)
    {
        WriteIntToFile(MAX_QUESTION_LEVEL_FILE, level);
    }
}
private IEnumerator ShowObject(GameObject obj)
{
    obj.transform.position = objectOriginalPositions[System.Array.IndexOf(associatedObjects, obj)] + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);
    obj.SetActive(true);

    float elapsedTime = 0f;
    Vector3 startPos = obj.transform.position;
    Vector3 endPos = objectOriginalPositions[System.Array.IndexOf(associatedObjects, obj)];

    while (elapsedTime < SLIDE_ANIMATION_DURATION)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / SLIDE_ANIMATION_DURATION;
        float smoothT = t * t * (3f - 2f * t); // Smooth easing
        obj.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        yield return null;
    }

    obj.transform.position = endPos;
}

private IEnumerator HideObject(GameObject obj)
{
    float elapsedTime = 0f;
    Vector3 startPos = obj.transform.position;
    Vector3 endPos = startPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);

    while (elapsedTime < SLIDE_ANIMATION_DURATION)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / SLIDE_ANIMATION_DURATION;
        float smoothT = t * t * (3f - 2f * t); // Smooth easing
        obj.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        yield return null;
    }

    obj.SetActive(false);
    if (currentlyShownObject == obj)
    {
        currentlyShownObject = null;
    }
}

private IEnumerator AnimateAdditionalButtons()
{
    for (int i = 0; i < additionalButtons.Length; i++)
    {
        additionalButtons[i].transform.position = additionalButtonOriginalPositions[i] + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);
        additionalButtons[i].gameObject.SetActive(false);
    }
    
    for (int i = 0; i < additionalButtons.Length; i++)
    {
        additionalButtons[i].gameObject.SetActive(true);
        
        float elapsedTime = 0f;
        Vector3 startPos = additionalButtons[i].transform.position;
        Vector3 endPos = additionalButtonOriginalPositions[i];
        
        while (elapsedTime < SLIDE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / SLIDE_ANIMATION_DURATION;
            float smoothT = t * t * (3f - 2f * t); // Smooth easing
            additionalButtons[i].transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }
        
        additionalButtons[i].transform.position = additionalButtonOriginalPositions[i];
        yield return new WaitForSeconds(0.05f);
    }
}

private IEnumerator ShowOleWithAnimation()
{
    if (ole != null)
    {
        if (olePanel != null)
        {
            olePanel.SetActive(true);
        }

        ole.transform.position = oleOriginalPosition + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);
        ole.SetActive(true);

        float elapsedTime = 0f;
        Vector3 startPos = ole.transform.position;
        Vector3 endPos = oleOriginalPosition;

        while (elapsedTime < SLIDE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / SLIDE_ANIMATION_DURATION;
            float smoothT = t * t * (3f - 2f * t); // Smooth easing
            ole.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        ole.transform.position = endPos;

        if (friendNameInput != null)
        {
            friendNameInput.gameObject.SetActive(true);
            friendNameInput.text = "";
            friendNameInput.Select();
            friendNameInput.ActivateInputField();

            StartCoroutine(HideOlePanelAfterDelay());

            if (confirmFriendNameButton != null)
            {
                confirmFriendNameButton.gameObject.SetActive(true);
            }
        }

        SetLegacyTextAndRestartTimer("Вы использовали подсказку звонок другу, введите пожалуйста имя своего друга", false);
    }
}

private IEnumerator HideOleWithAnimation()
{
    if (ole != null)
    {
        float elapsedTime = 0f;
        Vector3 startPos = ole.transform.position;
        Vector3 endPos = startPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);

        while (elapsedTime < SLIDE_ANIMATION_DURATION)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / SLIDE_ANIMATION_DURATION;
            float smoothT = t * t * (3f - 2f * t); // Smooth easing
            ole.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
            yield return null;
        }

        ole.SetActive(false);
        ole.transform.position = oleOriginalPosition;
    }
}

private void ChangeQuestion()
{
    if (currentQuestionIndex == -1 || cachedQuestions == null) return;

    string currentTable = currentMoneyIndex == 0 ? "b" : $"b{currentMoneyIndex}";

    var validRows = cachedQuestions.AsEnumerable()
        .Where(row => row.Field<string>("table_name") == currentTable && 
                     !usedQuestionIds.Contains(cachedQuestions.Rows.IndexOf(row)))
        .ToList();

    if (validRows.Count == 0)
    {
        Debug.LogWarning("No more unused questions available in this level");
        return;
    }

    int randomIndex = UnityEngine.Random.Range(0, validRows.Count);
    DataRow newRow = validRows[randomIndex];

    currentQuestionIndex = cachedQuestions.Rows.IndexOf(newRow);
    usedQuestionIds.Add(currentQuestionIndex);

    string questionText = newRow["question"].ToString();
    SetLegacyTextAndRestartTimer("Вы использовали подсказку смена вопроса,", false);
    
    // Создаем список индексов скрытых кнопок
    List<int> hiddenButtonIndices = new List<int>();
    for (int i = 0; i < answerButtons.Length; i++)
    {
        if (!answerButtons[i].gameObject.activeSelf)
        {
            hiddenButtonIndices.Add(i);
        }
    }

    // Анимируем появление только скрытых кнопок
    StartCoroutine(RestoreHiddenButtonsWithAnimation(hiddenButtonIndices));
    
    // Затем запускаем анимацию смены вопроса
    StartCoroutine(AnimateQuestionChange(newRow, questionText));
}

private IEnumerator RestoreHiddenButtonsWithAnimation(List<int> hiddenIndices)
{
    float duration = 0.5f;
    
    // Активируем и анимируем только скрытые кнопки
    foreach (int i in hiddenIndices)
    {
        answerButtons[i].gameObject.SetActive(true);
        answerButtons[i].interactable = true;
        
        RectTransform buttonRect = answerButtons[i].GetComponent<RectTransform>();
        RectTransform textRect = answerTexts[i].GetComponent<RectTransform>();
        
        // Устанавливаем начальную позицию над экраном
        Vector2 startPosButton = originalButtonPositions[i] + new Vector2(0, Screen.height + 100);
        Vector2 startPosText = originalAnswerTextPositions[i] + new Vector2(0, Screen.height + 100);
        
        buttonRect.anchoredPosition = startPosButton;
        textRect.anchoredPosition = startPosText;
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float smoothT = t * t * (3f - 2f * t); // Плавное движение
            
            buttonRect.anchoredPosition = Vector2.Lerp(startPosButton, originalButtonPositions[i], smoothT);
            textRect.anchoredPosition = Vector2.Lerp(startPosText, originalAnswerTextPositions[i], smoothT);
            
            yield return null;
        }
        
        // Убеждаемся, что кнопки встали точно на свои места
        buttonRect.anchoredPosition = originalButtonPositions[i];
        textRect.anchoredPosition = originalAnswerTextPositions[i];
        
        yield return new WaitForSeconds(0.1f);
    }
}

private IEnumerator AnimateQuestionChange(DataRow newRow, string questionText)
{
    // Стирание текущего вопроса побуквенно
    string currentQuestion = textElement.text;
    while (currentQuestion.Length > 0)
    {
        currentQuestion = currentQuestion.Substring(0, currentQuestion.Length - 1);
        textElement.text = currentQuestion;
        yield return new WaitForSeconds(0.02f);
    }

    // Стирание только видимых вариантов ответа побуквенно
    List<(int index, string text)> visibleAnswers = new List<(int index, string text)>();
    for (int i = 0; i < answerTexts.Length; i++)
    {
        if (answerButtons[i].gameObject.activeSelf)
        {
            visibleAnswers.Add((i, answerTexts[i].text));
        }
    }

    bool answersRemaining = true;
    while (answersRemaining)
    {
        answersRemaining = false;
        foreach (var (index, _) in visibleAnswers)
        {
            if (answerTexts[index].text.Length > 0)
            {
                answerTexts[index].text = answerTexts[index].text.Substring(0, answerTexts[index].text.Length - 1);
                answersRemaining = true;
            }
        }
        yield return new WaitForSeconds(0.01f);
    }

    yield return new WaitForSeconds(0.5f);

    // Запись нового вопроса побуквенно
    string newQuestionText = "";
    foreach (char c in questionText)
    {
        newQuestionText += c;
        textElement.text = newQuestionText;
        yield return new WaitForSeconds(0.05f);
    }

    // Подготовка новых ответов
    string[] newAnswers = new string[] 
    { 
        newRow["v1"].ToString(),
        newRow["v2"].ToString(),
        newRow["v3"].ToString(),
        newRow["v4"].ToString()
    };

    // Активация всех неактивных кнопок с анимацией
    List<int> inactiveButtons = new List<int>();
    for (int i = 0; i < answerButtons.Length; i++)
    {
        if (!answerButtons[i].gameObject.activeSelf)
        {
            inactiveButtons.Add(i);
            answerButtons[i].gameObject.SetActive(true);
            answerButtons[i].interactable = true;

            // Установка начальной позиции над экраном
            RectTransform buttonRect = answerButtons[i].GetComponent<RectTransform>();
            RectTransform textRect = answerTexts[i].GetComponent<RectTransform>();
            
            buttonRect.anchoredPosition = originalButtonPositions[i] + new Vector2(0, Screen.height + 100);
            textRect.anchoredPosition = originalAnswerTextPositions[i] + new Vector2(0, Screen.height + 100);
        }
    }

    // Анимация появления неактивных кнопок
    float duration = 0.5f;
    foreach (int i in inactiveButtons)
    {
        RectTransform buttonRect = answerButtons[i].GetComponent<RectTransform>();
        RectTransform textRect = answerTexts[i].GetComponent<RectTransform>();
        
        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float smoothT = t * t * (3f - 2f * t); // Плавное движение
            
            buttonRect.anchoredPosition = Vector2.Lerp(
                originalButtonPositions[i] + new Vector2(0, Screen.height + 100),
                originalButtonPositions[i],
                smoothT
            );
            textRect.anchoredPosition = Vector2.Lerp(
                originalAnswerTextPositions[i] + new Vector2(0, Screen.height + 100),
                originalAnswerTextPositions[i],
                smoothT
            );
            
            yield return null;
        }
        
        // Убеждаемся, что кнопки встали точно на свои места
        buttonRect.anchoredPosition = originalButtonPositions[i];
        textRect.anchoredPosition = originalAnswerTextPositions[i];
        
        yield return new WaitForSeconds(0.1f);
    }

    // Запись текста ответов побуквенно
    bool answersComplete = false;
    int[] currentLengths = new int[4] { 0, 0, 0, 0 };

    while (!answersComplete)
    {
        answersComplete = true;
        for (int i = 0; i < answerTexts.Length; i++)
        {
            if (currentLengths[i] < newAnswers[i].Length)
            {
                currentLengths[i]++;
                answerTexts[i].text = newAnswers[i].Substring(0, currentLengths[i]);
                answersComplete = false;
            }
        }
        yield return new WaitForSeconds(0.02f);
    }

    string newMessageText = $"Итак, вопрос:\n\n{questionText}\n\nА вот и варианты ответа:\n\n" +
        $"1. {newRow["v1"]}\n\n2. {newRow["v2"]}\n\n3. {newRow["v3"]}\n\n4. {newRow["v4"]}\n";
    SetLegacyTextAndRestartTimer(newMessageText, true);
}

private void ShowAssociatedObject(int index)
{
    if (index >= associatedObjects.Length) return;


    if (currentlyShownObject != null)
    {
        StartCoroutine(HideObject(currentlyShownObject));
    }

    GameObject objectToShow = associatedObjects[index];
    currentlyShownObject = objectToShow;
    StartCoroutine(ShowObject(objectToShow));
}

private IEnumerator RemoveTwoWrongAnswers()
{
    if (currentQuestionIndex == -1) yield break;

    DataRow currentQuestion = cachedQuestions.Rows[currentQuestionIndex];
    string correctAnswer = currentQuestion["right"].ToString();
    
    // Find index of correct answer
    int correctIndex = -1;
    List<int> activeWrongIndices = new List<int>();

    // Get all active wrong answer indices
    for (int i = 0; i < answerTexts.Length; i++)
    {
        if (answerButtons[i].gameObject.activeSelf) // Check if the button is active
        {
            if (answerTexts[i].text == correctAnswer)
            {
                correctIndex = i;
            }
            else
            {
                activeWrongIndices.Add(i);
            }
        }
    }

    if (correctIndex == -1 || activeWrongIndices.Count == 0) yield break;

    // Randomly select wrong answers to remove from active wrong answers
    int removeCount = Mathf.Min(2, activeWrongIndices.Count);
    System.Random rnd = new System.Random();
    var indicesToRemove = activeWrongIndices
        .OrderBy(x => rnd.Next())
        .Take(removeCount)
        .ToList();

    // Animate removal of selected wrong answers
    foreach (int wrongIndex in indicesToRemove)
    {
        StartCoroutine(AnimateAnswerRemoval(wrongIndex));
        answerButtons[wrongIndex].interactable = false;
    }

    SetLegacyTextAndRestartTimer("Вы использовали подсказку 50 на 50, убраны два неверных варианта ответа", false);
}
// No actual change needed to its logic as it already adds.
// Just for clarity, the name implies writing, but the logic is "read, add, write".
private void AddToMoneyFile(int amountToAdd)
{
    string filePath = Path.Combine(Application.persistentDataPath, "money.txt");
    int currentMoney = 0;

    // Read current money if file exists
    if (File.Exists(filePath))
    {
        string content = File.ReadAllText(filePath);
        int.TryParse(content, out currentMoney);
    }

    // Add new amount
    currentMoney += amountToAdd;

    // Write updated amount
    File.WriteAllText(filePath, currentMoney.ToString());
}
private IEnumerator AnimateAnswerRemoval(int answerIndex)
{
    TMP_Text answerText = answerTexts[answerIndex];
    Button answerButton = answerButtons[answerIndex];
    
    RectTransform textRectTransform = answerText.GetComponent<RectTransform>();
    RectTransform buttonRectTransform = answerButton.GetComponent<RectTransform>();
    
    Vector2 textStartPosition = textRectTransform.anchoredPosition;
    Vector2 buttonStartPosition = buttonRectTransform.anchoredPosition;
    
    Vector2 textEndPosition = textStartPosition + new Vector2(0, Screen.height + 100);
    Vector2 buttonEndPosition = buttonStartPosition + new Vector2(0, Screen.height + 100);
    
    float duration = 0.5f;
    float elapsedTime = 0f;

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        float smoothT = t * t * (3f - 2f * t); // Smooth easing
        
        // Анимируем текст и кнопку одновременно
        textRectTransform.anchoredPosition = Vector2.Lerp(textStartPosition, textEndPosition, smoothT);
        buttonRectTransform.anchoredPosition = Vector2.Lerp(buttonStartPosition, buttonEndPosition, smoothT);
        
        yield return null;
    }

    // Очищаем текст и возвращаем элементы на исходные позиции
    answerText.text = "";
    textRectTransform.anchoredPosition = textStartPosition;
    buttonRectTransform.anchoredPosition = buttonStartPosition;
    
    // Деактивируем кнопку
    answerButton.gameObject.SetActive(false);
}


    private void HideAssociatedObject(int index)
{
    if (index >= associatedObjects.Length) return;

    GameObject objectToHide = associatedObjects[index];
    Button selectedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject?.GetComponent<Button>();


    // For hideButtonsSet1[1] (take money button) - это не подсказка
    if (index == 1 && hideButtonsSet1.Contains(selectedButton))
    {
        StartCoroutine(HideObjectWithAnimation(objectToHide));
        StartCoroutine(ShrinkAdditionalButton(1));
        StartCoroutine(TakeMoneyAndExit());
        return;
    }

    if (index == 0 && hideButtonsSet1.Contains(selectedButton)) // Смена вопроса
    {
        IncrementHintsUsed(); // <--- ДОБАВЛЕНО
        StartCoroutine(HideObjectWithAnimation(objectToHide));
        StartCoroutine(ShrinkAdditionalButton(0));
        ChangeQuestion(); // ChangeQuestion сама по себе не должна вызывать IncrementHintsUsed, если вызов идет отсюда
        return;
    }

    if (index == 2 && hideButtonsSet1.Contains(selectedButton)) // 50/50
    {
        IncrementHintsUsed(); // <--- ДОБАВЛЕНО
        StartCoroutine(RemoveTwoWrongAnswers()); // RemoveTwoWrongAnswers сама по себе не должна вызывать IncrementHintsUsed
        StartCoroutine(HideObjectWithAnimation(objectToHide));
        StartCoroutine(ShrinkAdditionalButton(2));
        return;
    }

    if (index == 3 && hideButtonsSet1.Contains(selectedButton)) // Право на ошибку
    {
        IncrementHintsUsed(); // <--- ДОБАВЛЕНО
        hasSecondChance = true;
        StartCoroutine(HideObjectWithAnimation(objectToHide));
        StartCoroutine(ShrinkAdditionalButton(3));
        SetLegacyTextAndRestartTimer("Вы использовали подсказку право на ошибку, теперь у вас есть возможность ошибиться один раз", false);
        return;
    }

    if (index == 4 && hideButtonsSet1.Contains(selectedButton)) // Помощь зала
    {
        IncrementHintsUsed(); // <--- ДОБАВЛЕНО
        StartCoroutine(ShowHelpWithAnimation()); // ShowHelpWithAnimation сама по себе не должна вызывать IncrementHintsUsed
        StartCoroutine(HideObjectWithAnimation(objectToHide));
        StartCoroutine(ShrinkAdditionalButton(4));
        return;
    }

    if (index == 5 && hideButtonsSet1.Contains(selectedButton)) // Звонок другу
    {
        IncrementHintsUsed(); // <--- ДОБАВЛЕНО
        StartCoroutine(ShowOleWithAnimation()); // ShowOleWithAnimation сама по себе не должна вызывать IncrementHintsUsed
        StartCoroutine(HideObjectWithAnimation(objectToHide));
        StartCoroutine(ShrinkAdditionalButton(5));
        return;
    }

    if (objectToHide.activeSelf)
    {
        StartCoroutine(HideObjectWithAnimation(objectToHide));
    }
}
private string NumberToWords(int number)
{
    if (number == 0) return "ноль";

    string[] units = { "", "один", "два", "три", "четыре", "пять", "шесть", "семь", "восемь", "девять", "десять",
                      "одиннадцать", "двенадцать", "тринадцать", "четырнадцать", "пятнадцать", "шестнадцать",
                      "семнадцать", "восемнадцать", "девятнадцать" };
    string[] tens = { "", "", "двадцать", "тридцать", "сорок", "пятьдесят", "шестьдесят", "семьдесят", "восемьдесят", "девяносто" };
    string[] hundreds = { "", "сто", "двести", "триста", "четыреста", "пятьсот", "шестьсот", "семьсот", "восемьсот", "девятьсот" };
    string[] thousands = { "", "тысяча", "тысячи", "тысяч" };
    string[] millions = { "", "миллион", "миллиона", "миллионов" };

    string GetNumeral(int n, string form1, string form2, string form5)
    {
        n = System.Math.Abs(n) % 100;
        if (n > 10 && n < 20) return form5;
        n = n % 10;
        if (n == 1) return form1;
        if (n > 1 && n < 5) return form2;
        return form5;
    }

    List<string> parts = new List<string>();
    
    if (number < 0)
    {
        parts.Add("минус");
        number = System.Math.Abs(number);
    }

    int millions_part = number / 1000000;
    int thousands_part = (number % 1000000) / 1000;
    int remaining = number % 1000;

    if (millions_part > 0)
    {
        int hundreds_part = millions_part / 100;
        int tens_part = (millions_part % 100) / 10;
        int units_part = millions_part % 10;

        if (hundreds_part > 0)
            parts.Add(hundreds[hundreds_part]);

        if (tens_part >= 2)
        {
            parts.Add(tens[tens_part]);
            if (units_part > 0)
                parts.Add(units[units_part]);
        }
        else
        {
            if (tens_part * 10 + units_part > 0)
                parts.Add(units[tens_part * 10 + units_part]);
        }

        parts.Add(GetNumeral(millions_part, millions[1], millions[2], millions[3]));
    }

    if (thousands_part > 0)
    {
        int hundreds_part = thousands_part / 100;
        int tens_part = (thousands_part % 100) / 10;
        int units_part = thousands_part % 10;

        if (hundreds_part > 0)
            parts.Add(hundreds[hundreds_part]);

        if (tens_part >= 2)
        {
            parts.Add(tens[tens_part]);
            if (units_part > 0)
                parts.Add(units_part == 1 ? "одна" : units_part == 2 ? "две" : units[units_part]);
        }
        else
        {
            if (tens_part * 10 + units_part > 0)
                parts.Add(tens_part * 10 + units_part == 1 ? "одна" : 
                         tens_part * 10 + units_part == 2 ? "две" : 
                         units[tens_part * 10 + units_part]);
        }

        parts.Add(GetNumeral(thousands_part, thousands[1], thousands[2], thousands[3]));
    }

    if (remaining > 0 || parts.Count == 0)
    {
        int hundreds_part = remaining / 100;
        int tens_part = (remaining % 100) / 10;
        int units_part = remaining % 10;

        if (hundreds_part > 0)
            parts.Add(hundreds[hundreds_part]);

        if (tens_part >= 2)
        {
            parts.Add(tens[tens_part]);
            if (units_part > 0)
                parts.Add(units[units_part]);
        }
        else
        {
            if (tens_part * 10 + units_part > 0)
                parts.Add(units[tens_part * 10 + units_part]);
        }
    }

    return string.Join(" ", parts);
}
void OnDestroy()
{
    // Остановка корутины при уничтожении объекта
    if (imaCheckCoroutine != null)
    {
        StopCoroutine(imaCheckCoroutine);
    }
}
private IEnumerator TakeMoneyAndExit()
{
    if (isGameOver) yield break;
    int currentBank = currentMoneyIndex > 0 ? moneyAmounts[currentMoneyIndex - 1] : 0; 
    string moneyInWords = NumberToWords(currentBank);
    string messageText = $"вы забрали текущий банк {moneyInWords} спасибо за участие";
    
    // Обновляем максимальный уровень до того, как HandleEndOfGame это сделает,
    // так как currentMoneyIndex здесь отражает последний *отвеченный* вопрос.
    // Если игрок забирает деньги, он дошел до currentMoneyIndex + 1 вопроса, но не ответил на него.
    // Однако, для статистики "максимальный вопрос до которого доходили" логичнее считать тот, который был показан.
    // currentMoneyIndex - это индекс *текущего* вопроса (0-14). Если он ответил на него и забирает деньги,
    // то он дошел до уровня currentMoneyIndex + 1.
    // Если он еще не ответил (находится на вопросе currentMoneyIndex), то уровень currentMoneyIndex + 1.
    UpdateMaxQuestionLevelReached(currentMoneyIndex + 1); // <--- ДОБАВЛЕНО

    yield return StartCoroutine(HandleEndOfGame(messageText, true, false)); 
}

    private float[] GenerateRandomPercentages()
    {
        float[] percentages = new float[4];

        if (currentQuestionIndex != -1 && cachedQuestions != null)
        {
            // Get list of active answer indices
            List<int> activeAnswerIndices = new List<int>();
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i].gameObject.activeSelf)
                {
                    activeAnswerIndices.Add(i);
                }
            }

            if (activeAnswerIndices.Count == 0) return percentages;

            DataRow currentQuestion = cachedQuestions.Rows[currentQuestionIndex];
            string correctAnswer = currentQuestion["right"].ToString();

            // Find the index of the correct answer among active answers
            int correctIndex = -1;
            for (int i = 0; i < activeAnswerIndices.Count; i++)
            {
                if (answerTexts[activeAnswerIndices[i]].text == correctAnswer)
                {
                    correctIndex = activeAnswerIndices[i];
                    break;
                }
            }

            if (correctIndex != -1)
            {
                // 30% chance that audience will favor a wrong answer
                if (Random.value < 0.3f)
                {
                    // Choose random wrong answer from active answers
                    List<int> wrongIndices = activeAnswerIndices.FindAll(x => x != correctIndex);

                    if (wrongIndices.Count > 0)
                    {
                        int misleadingIndex = wrongIndices[Random.Range(0, wrongIndices.Count)];
                        percentages[misleadingIndex] = Random.Range(55f, 75f);
                        percentages[correctIndex] = 100f - percentages[misleadingIndex];
                    }
                }
                else
                {
                    // Correct answer gets higher percentage
                    percentages[correctIndex] = Random.Range(55f, 75f);

                    // Distribute remaining percentage among other active answers
                    float remaining = 100f - percentages[correctIndex];
                    List<int> otherIndices = activeAnswerIndices.FindAll(x => x != correctIndex);

                    while (otherIndices.Count > 1)
                    {
                        int idx = otherIndices[0];
                        float maxAllowed = remaining - (otherIndices.Count - 1);
                        percentages[idx] = Random.Range(1f, Mathf.Min(maxAllowed, remaining / 2));
                        remaining -= percentages[idx];
                        otherIndices.RemoveAt(0);
                    }

                    if (otherIndices.Count > 0)
                    {
                        percentages[otherIndices[0]] = remaining;
                    }
                }
            }
        }

        return percentages;
    }

    private IEnumerator ShowHelpWithAnimation()
    {
        if (help != null)
        {
            // Get list of active answer buttons
            List<int> activeAnswerIndices = new List<int>();
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i].gameObject.activeSelf)
                {
                    activeAnswerIndices.Add(i);
                }
            }

            targetPercentages = GenerateRandomPercentages();

            // Hide all audience elements initially
            for (int i = 0; i < 4; i++)
            {
                if (audienceSliders != null && audienceSliders.Length > i)
                {
                    audienceSliders[i].gameObject.SetActive(false);
                    audienceSliders[i].value = 0;
                }
                if (audiencePercentTexts != null && audiencePercentTexts.Length > i)
                {
                    audiencePercentTexts[i].gameObject.SetActive(false);
                    audiencePercentTexts[i].text = "0%";
                }
                if (audienceAnswerTexts != null && audienceAnswerTexts.Length > i)
                {
                    audienceAnswerTexts[i].gameObject.SetActive(false);
                    audienceAnswerTexts[i].text = "";
                }
            }

            // Show only the active answers' audience elements
            for (int i = 0; i < activeAnswerIndices.Count; i++)
            {
                int originalIndex = activeAnswerIndices[i];

                if (audienceSliders != null && audienceSliders.Length > i)
                {
                    audienceSliders[i].gameObject.SetActive(true);
                }
                if (audiencePercentTexts != null && audiencePercentTexts.Length > i)
                {
                    audiencePercentTexts[i].gameObject.SetActive(true);
                }
                if (audienceAnswerTexts != null && audienceAnswerTexts.Length > i)
                {
                    audienceAnswerTexts[i].gameObject.SetActive(true);
                    audienceAnswerTexts[i].text = answerTexts[originalIndex].text;
                }
            }

            Vector3 originalPosition = help.transform.position;
            help.transform.position = originalPosition + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);
            help.SetActive(true);

            SetLegacyTextAndRestartTimer("Вы использовали подсказку помощь зала", false);

            float elapsedTime = 0f;
            Vector3 startPos = help.transform.position;
            Vector3 endPos = originalPosition;

            while (elapsedTime < SLIDE_ANIMATION_DURATION)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / SLIDE_ANIMATION_DURATION;
                float smoothT = t * t * (3f - 2f * t);
                help.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
                yield return null;
            }

            help.transform.position = endPos;

            // Type in the text character by character
            for (int i = 0; i < activeAnswerIndices.Count; i++)
            {
                if (audienceAnswerTexts != null && audienceAnswerTexts.Length > i)
                {
                    string fullText = answerTexts[activeAnswerIndices[i]].text;
                    audienceAnswerTexts[i].text = "";

                    for (int charIndex = 0; charIndex < fullText.Length; charIndex++)
                    {
                        audienceAnswerTexts[i].text += fullText[charIndex];
                        yield return new WaitForSeconds(0.02f);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);

            // Animate percentages only for active answers
            elapsedTime = 0f;
            while (elapsedTime < percentageAnimationDuration)
            {
                elapsedTime += Time.deltaTime;
                float progress = elapsedTime / percentageAnimationDuration;
                float smoothProgress = progress * progress * (3f - 2f * progress);

                for (int i = 0; i < activeAnswerIndices.Count; i++)
                {
                    int originalIndex = activeAnswerIndices[i];
                    float currentPercentage = targetPercentages[originalIndex] * smoothProgress;

                    if (audienceSliders != null && audienceSliders.Length > i)
                    {
                        audienceSliders[i].value = currentPercentage / 100f;
                    }

                    if (audiencePercentTexts != null && audiencePercentTexts.Length > i)
                    {
                        audiencePercentTexts[i].text = $"{Mathf.RoundToInt(currentPercentage)}%";
                    }
                }

                yield return null;
            }

            yield return new WaitForSeconds(5f);

            // Directly hide the help object without animating sliders and text
            elapsedTime = 0f;
            startPos = help.transform.position;
            endPos = startPos + Vector3.up * (Screen.height + SLIDE_ANIMATION_DISTANCE);

            while (elapsedTime < SLIDE_ANIMATION_DURATION)
            {
                elapsedTime += Time.deltaTime;
                float t = elapsedTime / SLIDE_ANIMATION_DURATION;
                float smoothT = t * t * (3f - 2f * t);
                help.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
                yield return null;
            }

            help.SetActive(false);
            help.transform.position = originalPosition;

            // Reset all audience elements instantly
            for (int i = 0; i < 4; i++)
            {
                if (audienceAnswerTexts != null && audienceAnswerTexts.Length > i)
                {
                    audienceAnswerTexts[i].text = "";
                }
                if (audienceSliders != null && audienceSliders.Length > i)
                {
                    audienceSliders[i].value = 0;
                }
                if (audiencePercentTexts != null && audiencePercentTexts.Length > i)
                {
                    audiencePercentTexts[i].text = "0%";
                }
            }
        }
    }

  private IEnumerator HideOlePanelAfterDelay()
{
    yield return new WaitForSeconds(6f);
    if (olePanel != null)
    {
        olePanel.SetActive(false);
    }
}
private void ConfirmFriendName()
{
    if (friendNameInput != null && !string.IsNullOrEmpty(friendNameInput.text) && !isPlayingFriendCallSound)
    {
        StartCoroutine(HideOleWithAnimation());
        StartCoroutine(PlayFriendCallSequence());
    }
}
    private IEnumerator AnimateSingleButtonShrink(Button button, float duration)
    {
        // Capture scale at the start of this specific animation
        Vector3 initialScale = button.transform.localScale;

        float elapsedTime = 0f;
        while (elapsedTime < duration)
        {
            // Using unscaledDeltaTime for UI animations to ensure they run smoothly
            // even if Time.timeScale is modified (though not expected here before pause).
            elapsedTime += Time.unscaledDeltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration); // Ensure t is between 0 and 1
            float scaleValue = 1f - t; // Linear shrink from 100% to 0% of initialScale
            button.transform.localScale = initialScale * scaleValue;
            yield return null;
        }
        button.gameObject.SetActive(false);
        // Reset scale to what it was before this shrink animation,
        // so if it's reactivated, it has its proper scale.
        button.transform.localScale = initialScale;
    }

private IEnumerator PlayFriendCallSequence()
{
    if (friendCallSound != null && dialToneAudioSource != null)
    {
        isPlayingFriendCallSound = true;
        
        if (panel1 != null)
        {
            panel1.SetActive(true);
        }

        dialToneAudioSource.clip = friendCallSound;
        dialToneAudioSource.Play();

        yield return new WaitForSeconds(10f);

        dialToneAudioSource.Stop();
        isPlayingFriendCallSound = false;

        string filePath = Path.Combine(Application.persistentDataPath, "voice.txt");

        File.WriteAllText(filePath, "ru-ru-ruslan-medium");
        string firstMessage = $"Здравствуйте, это {friendNameInput.text}?";
        SetLegacyTextAndRestartTimer(firstMessage, false);

        yield return new WaitForSeconds(4f);

        string name = friendNameInput.text.ToLower();
        bool isFemale = name.EndsWith("а") || name.EndsWith("я");
        string voice = isFemale ? "ru-ru-irina-medium" : "ru-ru-denis-medium";
yield return new WaitForSeconds(1f);
        File.WriteAllText(filePath, voice);
        string secondMessage = $"ну тип да, это я {friendNameInput.text}, а че ты хочешь?";
        SetLegacyTextAndRestartTimer(secondMessage, false);
        yield return new WaitForSeconds(3f);

        File.WriteAllText(filePath, "ru-ru-ruslan-medium");
        string thirdMessage = "участник выбрал вас что бы помочь ему с вопросом, вы можете помочь в этом?";
        SetLegacyTextAndRestartTimer(thirdMessage, false);
        yield return new WaitForSeconds(4f);

        if (UnityEngine.Random.value < 0.5f)
        {
            File.WriteAllText(filePath, voice);
            string busyText = isFemale ? "занята" : "занят";
            string rejectMessage = $"извини, но я сейчас очень {busyText}, не могу помочь. может в другой раз";
            SetLegacyTextAndRestartTimer(rejectMessage, false);
            yield return new WaitForSeconds(3f);

            File.WriteAllText(filePath, "ru-ru-ruslan-medium");
            string goodbyeMessage = "понимаю, извините за беспокойство, досвидания";
            SetLegacyTextAndRestartTimer(goodbyeMessage, false);
            yield return new WaitForSeconds(8f);
            
            if (endCallSound != null)
            {
                dialToneAudioSource.clip = endCallSound;
                dialToneAudioSource.Play();
                yield return new WaitForSeconds(3f);
                dialToneAudioSource.Stop();
            }
            
            if (panel1 != null)
            {
                panel1.SetActive(false);
            }
            
            yield break;
        }

        File.WriteAllText(filePath, voice);
        string fourthMessage = "ну да, че нет то, говори уже вопрос бырее, у вас же там типо все на время.";
        SetLegacyTextAndRestartTimer(fourthMessage, false);
        yield return new WaitForSeconds(3f);

        File.WriteAllText(filePath, "ru-ru-ruslan-medium");
        if (currentQuestionIndex != -1 && cachedQuestions?.Rows != null)
        {
            DataRow currentQuestion = cachedQuestions.Rows[currentQuestionIndex];
            
            List<(string number, string answer)> activeAnswers = new List<(string, string)>();
            for (int i = 0; i < answerButtons.Length; i++)
            {
                if (answerButtons[i].gameObject.activeSelf)
                {
                    activeAnswers.Add(((i + 1).ToString(), currentQuestion[$"v{i + 1}"].ToString()));
                }
            }

            string activeAnswersText = string.Join(", ", 
                activeAnswers.Select(a => $"{a.number}. {a.answer}"));

            string questionAndAnswers = $"Внимательно слушайте вопрос: {currentQuestion["question"]} " + $"варианты ответа.{activeAnswersText}";

            SetLegacyTextAndRestartTimer(questionAndAnswers, false);
            yield return new WaitForSeconds(14f);

            File.WriteAllText(filePath, voice);
            string correctAnswer = currentQuestion["right"].ToString();
            
            if (UnityEngine.Random.value < 0.5f)
            {
                var wrongAnswers = activeAnswers
                    .Where(a => a.answer != correctAnswer)
                    .Select(a => a.answer)
                    .ToList();

                if (wrongAnswers.Any())
                {
                    string wrongAnswer = wrongAnswers[UnityEngine.Random.Range(0, wrongAnswers.Count)];
                    string friendResponse = $"так, дай подумать... эээээээ, я думаю, что правильный ответ это {wrongAnswer}";
                    SetLegacyTextAndRestartTimer(friendResponse, false);
                }
            }
            else
            {
                string friendResponse = $"так, дай подумать... эээээээ, я думаю, что правильный ответ это {correctAnswer}";
                SetLegacyTextAndRestartTimer(friendResponse, false);
            }
            
            yield return new WaitForSeconds(3f);

            File.WriteAllText(filePath, "ru-ru-ruslan-medium");
            string goodbyeMessage = $"спасибо {friendNameInput.text} за ваш ответ, участник вам благодарен, досвидания";
            SetLegacyTextAndRestartTimer(goodbyeMessage, false);

            yield return new WaitForSeconds(9f);
            if (endCallSound != null)
            {
                dialToneAudioSource.clip = endCallSound;
                dialToneAudioSource.Play();
                yield return new WaitForSeconds(3f);
                dialToneAudioSource.Stop();
            }

            if (panel1 != null)
            {
                panel1.SetActive(false);
            }
        }
    }
}

   void CheckAnswer(int buttonIndex)
{
    if (currentQuestionIndex == -1 || isGameOver) return;

    DataRow currentQuestion = cachedQuestions.Rows[currentQuestionIndex];
    bool hadSecondChanceBeforeCheck = hasSecondChance;
    // hasSecondChance = false; // Сбрасывается ниже по логике

    if (answerTexts[buttonIndex].text == currentQuestion["right"].ToString())
    {
        IncrementCorrectAnswers(); // <--- ДОБАВЛЕНО
        UpdateMaxQuestionLevelReached(currentMoneyIndex + 1); // <--- ДОБАВЛЕНО (текущий уровень + 1)

        hasSecondChance = false; 
        if (currentMoneyIndex == moneyAmounts.Length - 1) 
        {
            StartCoroutine(HandleEndOfGame("поздравляем вы выиграли полный банк", true, true));
        }
        else // Правильный ответ, переход к следующему вопросу
        {
            string messageText = $"Вы ответили верно на {currentMoneyIndex + 1} вопрос и ваш банк повышается";
            if (hadSecondChanceBeforeCheck)
            {
                messageText = $"Вы ответили верно на {currentMoneyIndex + 1} вопрос, использовав право на ошибку. Ваш банк повышается";
            }
            bool isGarantAmount = garantContent.Split('\n')
                .Select(line => line.Trim())
                .Any(line => line == moneyAmounts[currentMoneyIndex].ToString());
            if (isGarantAmount)
            {
                messageText = $"Вы ответили верно на {currentMoneyIndex + 1} вопрос и ваш банк повышается, кстати говоря вы добрались до несгораемой суммы поздравляю";
            }
            SetLegacyTextAndRestartTimer(messageText);
            isQuestionText = false;
            StartCoroutine(AnimateQuestionContainerOut());
        }
    }
    else // Неправильный ответ
    {
         UpdateMaxQuestionLevelReached(currentMoneyIndex + 1);
        if (hadSecondChanceBeforeCheck)
            {
                hasSecondChance = false;
                SetLegacyTextAndRestartTimer("Ответ неверный, но у вас было право на ошибку. Оно использовано. Попробуйте еще раз!", false);
                StartCoroutine(AnimateWrongAnswerButton(buttonIndex));
                return;
            }

        int garantAmount = GetCurrentGarantAmount();
        string loseMessage = $"к сожалению вы проиграли правильный ответ был {currentQuestion["right"]}";
        if (garantAmount > 0)
        {
            loseMessage += $"\nно вы имеете несгораемый осадок";
            // WriteToMoneyFile(garantAmount); // Перенесено в HandleEndOfGame
        }
        // else
        // {
        //     WriteToMoneyFile(0); // Перенесено в HandleEndOfGame
        // }
        // WriteToWinFile(false); // Перенесено в HandleEndOfGame
        StartCoroutine(HandleEndOfGame(loseMessage, false, false));
    }
}

    private void HandleTimerExpiration()
{
    if (isGameOver) return;

    int garantAmount = GetCurrentGarantAmount();
    string message = "к сожалению, время вышло, вы проиграли";

    if (garantAmount > 0)
    {
        message += $"\nно у вас есть несгораемый остаток";
        // WriteToMoneyFile(garantAmount); // Перенесено в HandleEndOfGame
    }
    // else
    // {
    //     WriteToMoneyFile(0); // Перенесено в HandleEndOfGame
    // }
    // WriteToWinFile(false); // Перенесено в HandleEndOfGame
    StartCoroutine(HandleEndOfGame(message, false, false));
}
private IEnumerator ShowPauseMenuAfterDelay()
{
    yield return new WaitForSeconds(5f);
    TogglePause();
}

private IEnumerator AnimateWrongAnswerButton(int buttonIndex)
{
    Button wrongButton = answerButtons[buttonIndex];
    TMP_Text wrongAnswerText = answerTexts[buttonIndex];
    
    RectTransform buttonRect = wrongButton.GetComponent<RectTransform>();
    RectTransform textRect = wrongAnswerText.GetComponent<RectTransform>();
    
    Vector2 buttonStartPos = buttonRect.anchoredPosition;
    Vector2 textStartPos = textRect.anchoredPosition;
    
    Vector2 endPos = new Vector2(buttonStartPos.x, Screen.height + 100);
    
    float duration = 0.5f;
    float elapsedTime = 0f;
    
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        float smoothT = t * t * (3f - 2f * t);
        
        buttonRect.anchoredPosition = Vector2.Lerp(buttonStartPos, endPos, smoothT);
        textRect.anchoredPosition = Vector2.Lerp(textStartPos, endPos, smoothT);
        
        yield return null;
    }
    
    wrongButton.gameObject.SetActive(false);
    wrongAnswerText.text = "";
    
    // Reset positions for future use
    buttonRect.anchoredPosition = buttonStartPos;
    textRect.anchoredPosition = textStartPos;
}

private IEnumerator ShrinkAdditionalButton(int buttonIndex)
{
    if (buttonIndex >= additionalButtons.Length) yield break;

    Button button = additionalButtons[buttonIndex];
    float duration = 0.5f;
    Vector3 originalScale = button.transform.localScale;
    
    float elapsedTime = 0f;
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        float scale = 1 - t; // Линейное уменьшение от 1 до 0

        button.transform.localScale = originalScale * scale;
        yield return null;
    }

    // Деактивируем кнопку и восстанавливаем её исходный размер
    button.gameObject.SetActive(false);
    button.transform.localScale = originalScale;

    // Если это кнопка 50/50 (индекс 2), также уменьшаем и скрываем соответствующую additional кнопку
    if (buttonIndex == 2 && additionalButtons.Length > 2)
    {
        Button additionalButton = additionalButtons[2];
        elapsedTime = 0f;
        originalScale = additionalButton.transform.localScale;
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float scale = 1 - t;

            additionalButton.transform.localScale = originalScale * scale;
            yield return null;
        }

        additionalButton.gameObject.SetActive(false);
        additionalButton.transform.localScale = originalScale;
    }
}

private IEnumerator HideObjectWithAnimation(GameObject obj)
{
    if (!obj.activeSelf) yield break;

    float duration = 0.5f;
    float elapsedTime = 0f;
    Vector3 startPos = obj.transform.position;
    Vector3 endPos = startPos + Vector3.up * (Screen.height + 100);

    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        float smoothT = t * t * (3f - 2f * t); // Плавное движение
        obj.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        yield return null;
    }

    obj.SetActive(false);
    obj.transform.position = objectOriginalPositions[System.Array.IndexOf(associatedObjects, obj)];

    if (currentlyShownObject == obj)
    {
        currentlyShownObject = null;
    }
}

    IEnumerator Initialize()
    {
        dataBase = new SqlDataBase("185.105.91.114;SslMode=None", "a", "a", "1");
        yield return StartCoroutine(LoadQuestions());
        SetupButtons();
        scoreText.gameObject.SetActive(false);
        if (questionContainer != null)
        {
            originalQuestionContainerPosition = questionContainer.transform.position;
        }
    }

    private int GetCurrentGarantAmount()
    {
        int lastGarantAmount = 0;
        for (int i = 0; i < currentMoneyIndex; i++)
        {
            if (garantContent.Split('\n')
                .Select(line => line.Trim())
                .Any(line => line == moneyAmounts[i].ToString()))
            {
                lastGarantAmount = moneyAmounts[i];
            }
        }
        return lastGarantAmount;
    }

   
// Add this new method for timer text animation
private IEnumerator AnimateTimerText(string newValue)
{
    // Scale down with fade out
    float duration = 0.15f;
    float elapsedTime = 0f;
    Color originalColor = timerText.color;
    
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        timerText.transform.localScale = Vector3.Lerp(originalTimerScale, Vector3.zero, t);
        timerText.color = new Color(originalColor.r, originalColor.g, originalColor.b, 1 - t);
        yield return null;
    }
    
    // Update text
    timerText.text = newValue;
    
    // Scale up with fade in
    elapsedTime = 0f;
    while (elapsedTime < duration)
    {
        elapsedTime += Time.deltaTime;
        float t = elapsedTime / duration;
        timerText.transform.localScale = Vector3.Lerp(Vector3.zero, originalTimerScale, t);
        timerText.color = new Color(originalColor.r, originalColor.g, originalColor.b, t);
        yield return null;
    }
    
    // Ensure final state is correct
    timerText.transform.localScale = originalTimerScale;
    timerText.color = originalColor;
}
private IEnumerator HideAllAdditionalButtonsSimultaneously()
{
    List<Coroutine> shrinkCoroutines = new List<Coroutine>();
    float commonShrinkDuration = 0.5f; // Duration for each button's shrink animation

    foreach (Button button in additionalButtons)
    {
        if (button != null && button.gameObject.activeSelf)
        {
            // Start a shrink coroutine for each active button.
            // AnimateSingleButtonShrink will handle capturing the button's current scale.
            shrinkCoroutines.Add(StartCoroutine(AnimateSingleButtonShrink(button, commonShrinkDuration)));
        }
    }

    // Wait for all started shrink animations to complete
    foreach (Coroutine coroutine in shrinkCoroutines)
    {
        yield return coroutine;
    }
}
private IEnumerator ResumeAudioState()
{
    // Restore the text first
    legacyText.text = lastLegacyText;

    // Ensure the buttons are disabled while audio is playing
    foreach (var button in answerButtons)
    {
        if (button != null) button.interactable = false;
    }
    foreach (var button in additionalButtons)
    {
        if (button != null) button.interactable = false;
    }

    // Get the remaining duration
    float remainingDuration = audioSource.clip.length - audioTimeWhenPaused;

    // Play the audio from where it was paused
    audioSource.time = audioTimeWhenPaused;
    audioSource.Play();

    // Wait for the remaining duration
    yield return new WaitForSeconds(remainingDuration);

    // Re-enable buttons after audio finishes
    foreach (var button in answerButtons)
    {
        if (button != null) button.interactable = true;
    }
    foreach (var button in additionalButtons)
    {
        if (button != null) button.interactable = true;
    }

    // Reset the stored audio state
    wasAudioPlaying = false;
    audioTimeWhenPaused = 0f;
}

    // Modify your SetLegacyTextAndRestartTimer method to store the text

    private void SetLegacyTextAndRestartTimer(string text, bool isQuestion = false)
    {
        legacyText.text = text;
        lastLegacyText = text; // Store the text
        if (isQuestion)
        {
            currentTime = maxTime;
            isQuestionText = true;
        }
    }

    IEnumerator LoadQuestions()
    {
        cachedQuestions = new DataTable();
        cachedQuestions.Columns.Add("question");
        cachedQuestions.Columns.Add("v1");
        cachedQuestions.Columns.Add("v2");
        cachedQuestions.Columns.Add("v3");
        cachedQuestions.Columns.Add("v4");
        cachedQuestions.Columns.Add("right");
        cachedQuestions.Columns.Add("table_name");

        for (int i = 0; i <= 14; i++)
        {
            string tableName = i == 0 ? "b" : $"b{i}";
            DataTable tempTable;
            dataBase.SelectQuery($"SELECT * FROM a.{tableName};", out tempTable);

            if (tempTable != null && tempTable.Rows.Count > 0)
            {
                foreach (DataRow row in tempTable.Rows)
                {
                    DataRow newRow = cachedQuestions.NewRow();
                    newRow["question"] = row["question"];
                    newRow["v1"] = row["v1"];
                    newRow["v2"] = row["v2"];
                    newRow["v3"] = row["v3"];
                    newRow["v4"] = row["v4"];
                    newRow["right"] = row["right"];
                    newRow["table_name"] = tableName;
                    cachedQuestions.Rows.Add(newRow);
                }
            }
        }
        yield return null;
    }

    void SetupButtons()
    {
        foreach (var button in moneyButtons)
        {
            button.interactable = true;
        }
        foreach (var button in answerButtons)
        {
            button.interactable = true;
        }

        for (int i = 0; i < moneyButtons.Length; i++)
        {
            int buttonIndex = i;
            moneyButtons[i].onClick.AddListener(() => OnMoneyButtonClick(buttonIndex));
        }

        for (int i = 0; i < answerButtons.Length; i++)
        {
            int buttonIndex = i;
            answerButtons[i].onClick.AddListener(() => CheckAnswer(buttonIndex));
        }

        for (int i = 0; i < additionalButtons.Length; i++)
        {
            int buttonIndex = i;
            additionalButtons[i].onClick.AddListener(() => ShowAssociatedObject(buttonIndex));
        }
    }

    void OnMoneyButtonClick(int buttonIndex)
    {
        ShowPanelTemporarily();
        StartCoroutine(AnimateAdditionalButtons());

        if (cachedQuestions == null || cachedQuestions.Rows.Count == 0) return;

        moneyButtons[buttonIndex].interactable = false;
        StartCoroutine(LoadGarantWithDelay(buttonIndex));

        UpdateMaxQuestionLevelReached(currentMoneyIndex + 1); // <--- ДОБАВЛЕНО

    string currentTable = currentMoneyIndex == 0 ? "b" : $"b{currentMoneyIndex}";

        var validRows = cachedQuestions.AsEnumerable()
            .Where(row => row.Field<string>("table_name") == currentTable)
            .ToList();

        if (validRows.Count == 0)
        {
            Debug.LogError($"No questions found in table {currentTable}");
            return;
        }

        int randomIndex = Random.Range(0, validRows.Count);
        DataRow selectedRow = validRows[randomIndex];

        currentQuestionIndex = cachedQuestions.Rows.IndexOf(selectedRow);
        usedQuestionIds.Add(currentQuestionIndex);

        string questionText = selectedRow["question"].ToString();

        answerTexts[0].text = selectedRow["v1"].ToString();
        answerTexts[1].text = selectedRow["v2"].ToString();
        answerTexts[2].text = selectedRow["v3"].ToString();
        answerTexts[3].text = selectedRow["v4"].ToString();

        string introText = $"Итак, была выбрана несгораемая сумма,перед вами так же представлены подсказки которыми вы можете воспользоваться, теперь озвучим первый вопрос:\n\n";
        string answersIntro = "\nА вот и варианты ответа:\n\n";
        string answers = $"1. {selectedRow["v1"]}\n\n2. {selectedRow["v2"]}\n\n3. {selectedRow["v3"]}\n\n4. {selectedRow["v4"]}\n";
        SetLegacyTextAndRestartTimer(introText + questionText + answersIntro + answers, true);

        if (isFirstQuestion)
        {
            isFirstQuestion = false;
        }
        else
        {
            questionContainer.transform.position = originalQuestionContainerPosition + Vector3.up * (Screen.height + questionContainer.GetComponent<RectTransform>().rect.height);
            questionContainer.SetActive(true);
            StartCoroutine(AnimateQuestionContainerIn());
        }

        StartCoroutine(TypeText(questionText));
    }

    private void ShowPanelTemporarily()
    {
        if (panelCoroutine != null)
        {
            StopCoroutine(panelCoroutine);
        }
        panelCoroutine = StartCoroutine(ShowPanelCoroutine());
    }

    private IEnumerator ShowPanelCoroutine()
    {
        if (panel != null)
        {
            panel.SetActive(true);
            yield return new WaitForSeconds(13f);
            panel.SetActive(false);
        }
    }

    IEnumerator LoadGarantWithDelay(int buttonIndex)
    {
        if (isLoadingGarant) yield break;

        isLoadingGarant = true;
        yield return new WaitForSeconds(2f);

        try
        {
            string content = "";
            bool fileFound = false;

            string persistentPath = Path.Combine(Application.persistentDataPath, "garant.txt");
            if (File.Exists(persistentPath))
            {
                content = File.ReadAllText(persistentPath);
                fileFound = true;
            }

            if (!fileFound)
            {
                string dataPath = Path.Combine(Application.dataPath, "garant.txt");
                if (File.Exists(dataPath))
                {
                    content = File.ReadAllText(dataPath);
                    fileFound = true;
                }
            }

            if (!fileFound)
            {
                TextAsset textAsset = Resources.Load<TextAsset>("garant");
                if (textAsset != null)
                {
                    content = textAsset.text;
                    fileFound = true;
                }
            }

            if (!fileFound)
            {
                string currentDirPath = Path.Combine(Directory.GetCurrentDirectory(), "garant.txt");
                if (File.Exists(currentDirPath))
                {
                    content = File.ReadAllText(currentDirPath);
                    fileFound = true;
                }
            }

            if (!fileFound)
            {
                Debug.LogWarning("garant.txt not found in any location. Setting default content.");
                content = "Несгораемая сумма:";
            }

            garantContent = content;
            if (garantText != null)
            {
                garantText.text = "";
                StartCoroutine(TypeGarantText(content));
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error loading garant.txt: {e.Message}");
            garantContent = "Несгораемая сумма:";
            if (garantText != null)
            {
                garantText.text = "";
                StartCoroutine(TypeGarantText(garantContent));
            }
        }

        isLoadingGarant = false;
        StartCoroutine(ReactivateMoneyButton(buttonIndex));
    }

    IEnumerator TypeGarantText(string text)
    {
        garantText.text = "";
        foreach (char letter in text)
        {
            garantText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    IEnumerator ReactivateMoneyButton(int buttonIndex)
    {
        yield return new WaitForSeconds(0.5f);
        moneyButtons[buttonIndex].interactable = true;
    }

    void ShowRandomQuestion()
    {
        usedQuestionIds.Clear();
        currentQuestionIndex = -1;
    }

    IEnumerator TypeText(string text)
    {
        textElement.text = "";
        foreach (char letter in text)
        {
            textElement.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }

    void ShowWinningAmount()
{
    scoreText.gameObject.SetActive(true);
    
    // Only show amount if not game over
    if (currentMoneyIndex < moneyAmounts.Length)
    {
        scoreText.text = moneyAmounts[currentMoneyIndex].ToString();
    }
    else 
    {
        // Show "Вы проиграли" or "Вы выиграли" instead of amount
        scoreText.text = (currentMoneyIndex == moneyAmounts.Length) ? "Вы проиграли" : "Вы выиграли";
    }
    
    StartCoroutine(BigPulseAnimation());
}

    private IEnumerator BigPulseAnimation()
{
    ShowPanelTemporarily();

    Vector3 originalScale = Vector3.one;
    Vector3 maxScale = Vector3.one * 3f;
    float pulseDuration = 3f;
    float pulseCount = 3;
    float shrinkDuration = 0.5f;

    bool isGarantAmount = garantContent.Split('\n')
        .Select(line => line.Trim())
        .Any(line => line == moneyAmounts[Mathf.Min(currentMoneyIndex, moneyAmounts.Length - 1)].ToString());

    if (isGarantAmount)
    {
        scoreText.color = new Color32(103, 38, 38, 255);
    }
    else
    {
        scoreText.color = new Color32(180, 180, 180, 255);
    }

    for (int i = 0; i < pulseCount; i++)
    {
        float elapsedTime = 0f;
        while (elapsedTime < pulseDuration / (pulseCount * 2))
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (pulseDuration / (pulseCount * 2));
            float smoothT = t * t * (3f - 2f * t);
            scoreText.transform.localScale = Vector3.Lerp(originalScale, maxScale, smoothT);
            yield return null;
        }

        elapsedTime = 0f;
        while (elapsedTime < pulseDuration / (pulseCount * 2))
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / (pulseDuration / (pulseCount * 2));
            float smoothT = t * t * (3f - 2f * t);
            scoreText.transform.localScale = Vector3.Lerp(maxScale, originalScale, smoothT);
            yield return null;
        }
    }

    yield return new WaitForSeconds(0.5f);

    float shrinkElapsedTime = 0f;
    while (shrinkElapsedTime < shrinkDuration)
    {
        shrinkElapsedTime += Time.deltaTime;
        float t = shrinkElapsedTime / shrinkDuration;
        float scale = Mathf.Lerp(1f, 0f, Mathf.Pow(t, 2));
        scoreText.transform.localScale = Vector3.one * scale;
        yield return null;
    }

    scoreText.transform.localScale = originalScale;
    scoreText.gameObject.SetActive(false);
    scoreText.color = Color.white;

    if (currentMoneyIndex >= moneyAmounts.Length)
    {
        yield break;
    }

    // Восстанавливаем все кнопки перед следующим вопросом
    ResetAllButtons();

    currentMoneyIndex++; // Индекс следующего вопроса
    // После инкремента currentMoneyIndex, обновляем максимальный достигнутый уровень
    UpdateMaxQuestionLevelReached(currentMoneyIndex + 1); 
    if (currentMoneyIndex < moneyAmounts.Length)
    {
        questionContainer.transform.position = originalQuestionContainerPosition + Vector3.up * (Screen.height + questionContainer.GetComponent<RectTransform>().rect.height);
        questionContainer.SetActive(true);

        StartCoroutine(AnimateQuestionContainerIn());

        string nextTable = $"b{currentMoneyIndex}";

        var validRows = cachedQuestions.AsEnumerable()
            .Where(row => row.Field<string>("table_name") == nextTable)
            .ToList();

        if (validRows.Count == 0)
        {
            Debug.LogError($"No questions found in table {nextTable}");
            yield break;
        }

        int randomIndex = Random.Range(0, validRows.Count);
        DataRow nextRow = validRows[randomIndex];

        currentQuestionIndex = cachedQuestions.Rows.IndexOf(nextRow);
        usedQuestionIds.Add(currentQuestionIndex);

        string questionText = nextRow["question"].ToString();
        textElement.text = "";

        answerTexts[0].text = nextRow["v1"].ToString();
        answerTexts[1].text = nextRow["v2"].ToString();
        answerTexts[2].text = nextRow["v3"].ToString();
        answerTexts[3].text = nextRow["v4"].ToString();

        string newMessageText = $"Итак, вопрос:\n\n{questionText}\n\nА вот и варианты ответа:\n\n1. {nextRow["v1"]}\n\n2. {nextRow["v2"]}\n\n3. {nextRow["v3"]}\n\n4. {nextRow["v4"]}\n";
        SetLegacyTextAndRestartTimer(newMessageText, true);

        StartCoroutine(TypeText(questionText));
    }
}

private void ResetAllButtons()
{
    // Восстанавливаем только те кнопки ответов, которые были скрыты подсказкой 50/50
    for (int i = 0; i < answerButtons.Length; i++)
    {
        if (!answerButtons[i].gameObject.activeSelf)
        {
            answerButtons[i].gameObject.SetActive(true);
            answerButtons[i].interactable = true;
        }
    }

    // Восстанавливаем только hideButtonsSet1[2] (кнопка 50/50) и additionalButtons[2]
    if (hideButtonsSet1.Length > 2)
    {
        hideButtonsSet1[2].gameObject.SetActive(true);
        hideButtonsSet1[2].transform.localScale = Vector3.one;
    }


    // Восстанавливаем позиции текстов ответов
    for (int i = 0; i < answerTexts.Length; i++)
    {
        RectTransform textRect = answerTexts[i].GetComponent<RectTransform>();
        if (textRect != null)
        {
            textRect.anchoredPosition = Vector2.zero; // или сохраненная исходная позиция
        }
    }
}
}