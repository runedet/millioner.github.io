using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class ScrollingManager1 : MonoBehaviour
{
    public GameObject[] scrollingObjects;
     public Button confirmNoColorButton; // Кнопка 4
    public Button resetButton; // Кнопка 5
    [SerializeField] private Button defaultResetButton; // Кнопка 6
private const bool DEFAULT_ANIMATION_STATE = false; // false = анимация с дугой включена по умолчанию
private Color confirmActiveColor;
    public float scrollSpeed = 5f;
    public float resetPosition = -10f;
    public float startPosition = 10f;
    public float arcHeight = 2f;
    public Button button1;
    public Button button2;
    public Button button3;
    
    private GameObject selectedObject;
    private Vector3 offset;
    private bool isDragging = false;
    private Vector3[] originalPositions;
    private bool isAnimationPaused = false;
    private Color defaultColor;
    private Color whiteColor = Color.white;
    private Color confirmInactiveColor; // Added missing variable
    private bool button1Active = false;
    private bool currentButton1State = false; // Added missing variable
    private string filePath;
    private bool hasWrittenFour = false;

    private const string ANIMATION_STATE_KEY = "ArcAnimationPaused";

    private void Start()
{
    filePath = Path.Combine(Application.dataPath, "birkin.txt");
    
    originalPositions = new Vector3[scrollingObjects.Length];
    
    for (int i = 0; i < scrollingObjects.Length; i++)
    {
        originalPositions[i] = scrollingObjects[i].transform.position;
        
        if (scrollingObjects[i].GetComponent<BoxCollider2D>() == null)
        {
            scrollingObjects[i].AddComponent<BoxCollider2D>();
        }
    }

    ColorUtility.TryParseHtmlString("#413A69", out defaultColor);
    confirmInactiveColor = defaultColor;
    ColorUtility.TryParseHtmlString("#29325c", out confirmActiveColor);
    
    isAnimationPaused = PlayerPrefs.GetInt(ANIMATION_STATE_KEY, 0) == 1;
    button1Active = isAnimationPaused;
    currentButton1State = button1Active;
    
    defaultResetButton.onClick.AddListener(ResetToDefault);
    confirmNoColorButton.onClick.AddListener(ApplyChangesNoColor);
    resetButton.onClick.AddListener(ResetSettings);
    
    // Устанавливаем начальное состояние кнопок
    if (button1Active)
    {
        button1.GetComponent<Image>().color = defaultColor;
        button2.GetComponent<Image>().color = whiteColor;
        button1.interactable = true;
        button2.interactable = false;
    }
    else
    {
        button1.GetComponent<Image>().color = whiteColor;
        button2.GetComponent<Image>().color = defaultColor;
        button1.interactable = false;
        button2.interactable = true;
    }
    
    // Устанавливаем начальное состояние кнопки подтверждения
    string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
    button3.GetComponent<Image>().color = fileContent.Length > 0 ? confirmActiveColor : confirmInactiveColor;
    button3.interactable = true;
}

     public void ApplyChangesNoColor()
    {
        if (button1Active == currentButton1State) return;

        if (!button2.interactable && !isAnimationPaused)
        {
            isAnimationPaused = true;
        }
        else if (!button1.interactable && isAnimationPaused)
        {
            isAnimationPaused = false;
        }
        
        PlayerPrefs.SetInt(ANIMATION_STATE_KEY, isAnimationPaused ? 1 : 0);
        PlayerPrefs.Save();
        
        currentButton1State = button1Active;
        
        // Очищаем файл от "4"
        string fileContent = File.ReadAllText(filePath);
        if (fileContent.Contains("4"))
        {
            int index = fileContent.IndexOf('4');
            fileContent = fileContent.Remove(index, 1);
            File.WriteAllText(filePath, fileContent);
        }
        hasWrittenFour = false;
        
        // Устанавливаем обычный цвет для кнопки 3
        button3.GetComponent<Image>().color = confirmInactiveColor;
    }

    private void ResetToDefault()
{
    // Проверяем, нужно ли менять состояние
    bool needChange = isAnimationPaused != DEFAULT_ANIMATION_STATE;
    
    if (needChange)
    {
        // Устанавливаем состояние по умолчанию
        isAnimationPaused = DEFAULT_ANIMATION_STATE;
        button1Active = DEFAULT_ANIMATION_STATE;
        currentButton1State = DEFAULT_ANIMATION_STATE;
        
        // Обновляем PlayerPrefs
        PlayerPrefs.SetInt(ANIMATION_STATE_KEY, DEFAULT_ANIMATION_STATE ? 1 : 0);
        PlayerPrefs.Save();
        
        // Обновляем состояние кнопок
        if (DEFAULT_ANIMATION_STATE) // Если анимация остановлена
        {
            button1.GetComponent<Image>().color = defaultColor;
            button2.GetComponent<Image>().color = whiteColor;
            button1.interactable = true;
            button2.interactable = false;
        }
        else // Если анимация включена
        {
            button1.GetComponent<Image>().color = whiteColor;
            button2.GetComponent<Image>().color = defaultColor;
            button1.interactable = false;
            button2.interactable = true;
        }
    }
    
    // Сбрасываем позиции объектов к начальным
    ResetPositions();
    
    // Очищаем файл в любом случае
    File.WriteAllText(filePath, "");
    hasWrittenFour = false;
    
    // Сбрасываем цвет кнопки подтверждения
    button3.GetComponent<Image>().color = confirmInactiveColor;
    
    // Обновляем состояние кнопок
    UpdateButtonStates();
}
    public void ResetSettings()
    {
        // Очищаем файл
        File.WriteAllText(filePath, "");
        hasWrittenFour = false;
        
        // Возвращаем начальное состояние анимации
        button1Active = currentButton1State;
        isAnimationPaused = PlayerPrefs.GetInt(ANIMATION_STATE_KEY, 0) == 1;
        
        // Сбрасываем позиции объектов
        ResetPositions();
        
        // Обновляем состояние кнопок
        UpdateButtonStates();
        
        // Устанавливаем обычный цвет для кнопки 3
        button3.GetComponent<Image>().color = confirmInactiveColor;
    }

    private void Update()
    {
        if (!isAnimationPaused)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
                
                if (hit.collider != null)
                {
                    selectedObject = hit.collider.gameObject;
                    isDragging = true;
                    offset = selectedObject.transform.position - Camera.main.ScreenToWorldPoint(Input.mousePosition);
                }
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                isDragging = false;
                selectedObject = null;
            }

            if (isDragging && selectedObject != null)
            {
                Vector3 newPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition) + offset;
                selectedObject.transform.position = new Vector3(newPosition.x, newPosition.y, selectedObject.transform.position.z);
            }
            else
            {
                for (int i = 0; i < scrollingObjects.Length; i++)
                {
                    GameObject obj = scrollingObjects[i];
                    
                    if (obj != selectedObject)
                    {
                        Vector3 position = obj.transform.position;
                        float progress = (startPosition - position.x) / (startPosition - resetPosition);
                        float normalizedProgress = progress * 2 - 1;
                        float heightOffset = arcHeight * (1 - normalizedProgress * normalizedProgress);
                        
                        position.x -= scrollSpeed * Time.deltaTime;
                        position.y = originalPositions[i].y + heightOffset;
                        
                        obj.transform.position = position;

                        if (position.x <= resetPosition)
                        {
                            position.x = startPosition;
                            obj.transform.position = position;
                        }
                    }
                }
            }
        }
    }

    public void ResetPositions()
    {
        for (int i = 0; i < scrollingObjects.Length; i++)
        {
            scrollingObjects[i].transform.position = originalPositions[i];
        }
    }

    private void UpdateButtonStates()
{
    string fileContent = File.Exists(filePath) ? File.ReadAllText(filePath) : "";
    bool hasChanges = button1Active != currentButton1State;
    
    if (button1Active)
    {
        // Записываем 4 только если есть изменения и ещё не записали
        if (hasChanges && !hasWrittenFour)
        {
            File.AppendAllText(filePath, "4");
            hasWrittenFour = true;
        }
        
        button1.GetComponent<Image>().color = defaultColor;
        button2.GetComponent<Image>().color = whiteColor;
        button3.GetComponent<Image>().color = hasChanges ? confirmActiveColor : confirmInactiveColor;
        button1.interactable = true;
        button2.interactable = false;
        button3.interactable = true;
    }
    else
    {
        if (hasWrittenFour)
        {
            if (fileContent.Contains("4"))
            {
                int index = fileContent.IndexOf('4');
                fileContent = fileContent.Remove(index, 1);
                File.WriteAllText(filePath, fileContent);
            }
            hasWrittenFour = false;
        }
        
        button1.GetComponent<Image>().color = whiteColor;
        button2.GetComponent<Image>().color = defaultColor;
        button3.GetComponent<Image>().color = hasChanges ? confirmActiveColor : confirmInactiveColor;
        button1.interactable = false;
        button2.interactable = true;
        button3.interactable = true;
    }
}

    public void ToggleButtonStates()
    {
        button1Active = !button1Active;
        UpdateButtonStates();
    }

    public void Button3Click()
    {
        if (button1Active == currentButton1State) return;

        if (!button2.interactable && !isAnimationPaused)
        {
            isAnimationPaused = true;
        }
        else if (!button1.interactable && isAnimationPaused)
        {
            isAnimationPaused = false;
        }
        
        PlayerPrefs.SetInt(ANIMATION_STATE_KEY, isAnimationPaused ? 1 : 0);
        PlayerPrefs.Save();
        
        currentButton1State = button1Active;
        
        string fileContent = File.ReadAllText(filePath);
        if (fileContent.Contains("4"))
        {
            int index = fileContent.IndexOf('4');
            fileContent = fileContent.Remove(index, 1);
            File.WriteAllText(filePath, fileContent);
            
            if (fileContent.Length == 0)
            {
                button3.GetComponent<Image>().color = confirmInactiveColor;
            }
        }
        hasWrittenFour = false;
    }
}