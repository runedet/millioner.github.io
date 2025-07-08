using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class NewMonoBehaviourScript221351 : MonoBehaviour
{
    public GameObject objectToAnimate;
    public Button showButton;
    public Button hideButton;
    public float animationDuration = 0.5f;

    public TextMeshProUGUI gamesPlayedText;
    public TextMeshProUGUI highScoreText;
    public TextMeshProUGUI correctAnswersText;
    public TextMeshProUGUI hintsUsedText;
    public TextMeshProUGUI maxQuestionLevelText;

    private string gamesPlayedPath;
    private string highScorePath;
    private string correctAnswersPath;
    private string hintsUsedPath;
    private string maxQuestionLevelPath;

    private Vector3 originalPosition;
    private Vector3 hiddenPosition;
    private bool isAnimating = false;
    private float animationTime = 0f;
    private Vector3 startPosition;
    private Vector3 targetPosition;

    private float updateStatsTimer = 0f;
    private const float UPDATE_INTERVAL = 0.016f; // Примерно 60 раз в секунду

    void Start()
    {
        string persistentPath = Application.persistentDataPath;
        gamesPlayedPath = Path.Combine(persistentPath, "games_played_stats.txt");
        highScorePath = Path.Combine(persistentPath, "high_score_stats.txt");
        correctAnswersPath = Path.Combine(persistentPath, "correct_answers_stats.txt");
        hintsUsedPath = Path.Combine(persistentPath, "hints_used_stats.txt");
        maxQuestionLevelPath = Path.Combine(persistentPath, "max_question_level_stats.txt");

        // Вывод путей в консоль
        Debug.Log("Games Played Path: " + gamesPlayedPath);
        Debug.Log("High Score Path: " + highScorePath);
        Debug.Log("Correct Answers Path: " + correctAnswersPath);
        Debug.Log("Hints Used Path: " + hintsUsedPath);
        Debug.Log("Max Question Level Path: " + maxQuestionLevelPath);

        originalPosition = objectToAnimate.transform.position;
        
        float objectWidth = objectToAnimate.GetComponent<RectTransform>().rect.width;
        hiddenPosition = originalPosition + new Vector3(Screen.width + objectWidth, 0, 0);
        
        objectToAnimate.transform.position = hiddenPosition;

        showButton.onClick.AddListener(ShowObject);
        hideButton.onClick.AddListener(HideObject);

        InitializeTextFields();
        UpdateStats();
    }

    void InitializeTextFields()
    {
        gamesPlayedText.text = "0";
        highScoreText.text = "0";
        correctAnswersText.text = "0";
        hintsUsedText.text = "0";
        maxQuestionLevelText.text = "0";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HideObject();
        }

        if (isAnimating)
        {
            animationTime += Time.deltaTime;
            float progress = animationTime / animationDuration;

            if (progress <= 1f)
            {
                objectToAnimate.transform.position = Vector3.Lerp(startPosition, targetPosition, Mathf.SmoothStep(0f, 1f, progress));
            }
            else
            {
                objectToAnimate.transform.position = targetPosition;
                isAnimating = false;
            }
        }

        UpdateStats();
    }

    string ReadStatFile(string path)
    {
        if (!File.Exists(path))
        {
            Debug.Log($"File not found: {path}");
            return "0";
        }

        try
        {
            string content = File.ReadAllText(path).Trim();
            if (string.IsNullOrEmpty(content))
            {
                Debug.Log($"Empty file: {path}");
                return "0";
            }
            return content;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Error reading file {path}: {e.Message}");
            return "0";
        }
    }

    void UpdateStats()
    {
        string gamesPlayed = ReadStatFile(gamesPlayedPath);
        string highScore = ReadStatFile(highScorePath);
        string correctAnswers = ReadStatFile(correctAnswersPath);
        string hintsUsed = ReadStatFile(hintsUsedPath);
        string maxQuestionLevel = ReadStatFile(maxQuestionLevelPath);

        if (gamesPlayedText.text != gamesPlayed) gamesPlayedText.text = gamesPlayed;
        if (highScoreText.text != highScore) highScoreText.text = highScore;
        if (correctAnswersText.text != correctAnswers) correctAnswersText.text = correctAnswers;
        if (hintsUsedText.text != hintsUsed) hintsUsedText.text = hintsUsed;
        if (maxQuestionLevelText.text != maxQuestionLevel) maxQuestionLevelText.text = maxQuestionLevel;
    }

    void ShowObject()
    {
        if (!isAnimating)
        {
            startPosition = hiddenPosition;
            targetPosition = originalPosition;
            StartAnimation();
        }
    }

    void HideObject()
    {
        if (!isAnimating && objectToAnimate.transform.position != hiddenPosition)
        {
            startPosition = objectToAnimate.transform.position;
            targetPosition = hiddenPosition;
            StartAnimation();
        }
    }

    void StartAnimation()
    {
        isAnimating = true;
        animationTime = 0f;
    }
}