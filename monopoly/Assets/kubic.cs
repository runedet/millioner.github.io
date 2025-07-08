using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Events;


public class DiceRoller : MonoBehaviour {
    // Console panel components
    public Text consolePanelText;
    public ScrollRect consoleScrollRect;
    private Queue<string> consoleMessages = new Queue<string>();
    private const int MAX_CONSOLE_MESSAGES = 5;
    
    // Массив купи меня
    public Text[] Cena;
    public Image[] predpriiatia;
    // облигация игрока
    public Image[] viigral2;
    public Text[] textComponents;
    public Button[] buton;
    public Image[] igri;
    
    public Sprite[] predpriiatiakupil0;
    public Sprite[] predpriiatiakupilpovernuli0;

    // Массив компонентов Image для фишек
    public Sprite[] predpriiatiakupil;
    public Sprite[] predpriiatiakupilpovernuli;
    public Button rollButton;
    public Image dieImage1;
    public Button[] buttons1;
    public Image dieImage2;
    public Sprite[] dieSprites;
    public Transform[] positions;
    public Image[] playerTokens;
    public Sprite[] playerTokenSprites;
    public Sprite[] buttonImages;
private float gameTimer = 0f;
private float rentMultiplier = 1f;
private const float RENT_INCREASE_INTERVAL = 100f; // 5 minutes in seconds
    private bool isAnimating = false;
    private bool g=true;
    private bool d=true;
    private bool f=true;
    private bool j=true;
    private bool isTimerPaused = false;
    private bool k=true;
    private List<Image> activePlayerTokens = new List<Image>();
    private int currentPlayerIndex = 0;
    private List<int> buttonImageIndices = new List<int>();
    private int currentButtonImageIndex = 0;
    private Dictionary<int, List<Image>> tokenPositions = new Dictionary<int, List<Image>>();
    private Dictionary<int, int> propertyOwnership = new Dictionary<int, int>();
    private Dictionary<int, bool> skipNextTurn = new Dictionary<int, bool>();

private Dictionary<int, bool> moveBackwardsNextTurn = new Dictionary<int, bool>();
    private int die1;
    private int die2;
    private int bankruptPlayerCount = 0;
    private int lastMovedTokenIndex = -1;

    private void AddConsoleMessage(string message) {
        consoleMessages.Enqueue($"* {message}");

        while (consoleMessages.Count > MAX_CONSOLE_MESSAGES) {
            consoleMessages.Dequeue();
        }

        consolePanelText.text = string.Join("\n", consoleMessages.ToArray());
        Canvas.ForceUpdateCanvases();

        float scrollAmount = 1000f;
        float newScrollPosition = consoleScrollRect.verticalNormalizedPosition - (scrollAmount / consoleScrollRect.content.rect.height);
        consoleScrollRect.verticalNormalizedPosition = Mathf.Clamp(newScrollPosition, 0f, 1f);

        if (consoleScrollRect.verticalNormalizedPosition <= 0.01f) {
            consoleScrollRect.verticalNormalizedPosition = 0f;
        }
    }

    private void Start() {
        for (int i = 0; i < playerTokens.Length; i++) {
        moveBackwardsNextTurn[i] = false;
    }
        // Initialize skipNextTurn dictionary
        for (int i = 0; i < playerTokens.Length; i++) {
            skipNextTurn[i] = false;
        }

        foreach (Text textComponent in textComponents) {
            textComponent.text = "2.0м";
        }
        
        for (int i = 0; i < positions.Length; i++) {
            tokenPositions[i] = new List<Image>();
        }
        
        foreach (Button button in buttons1) {
            button.gameObject.SetActive(false);
        }
        for (int i = 0; i < buttons1.Length; i++) {
            int index = i;
            buttons1[i].onClick.AddListener(() => OnButtonClicked(index));
        }
         foreach (Image element in viigral2) {
        element.gameObject.SetActive(false);
    }
        rollButton.gameObject.SetActive(false);
    StartCoroutine(ShowRollButtonAnimation());
        rollButton.onClick.AddListener(OnRollButtonClicked);
        dieImage1.color = new Color(1, 1, 1, 0);
        dieImage2.color = new Color(1, 1, 1, 0);
        LoadPlayerTokenSprites("kraski.txt");
        rentMultiplier = 1f;
        if (buttonImageIndices.Count > 0) {
            UpdateButtonImage();
        }

        AddConsoleMessage("игра началась)");
        StartCoroutine(CheckBankruptcy());
}
private System.Collections.IEnumerator AnimateElement0() {
    Image element = viigral2[0];
    
    element.transform.localScale = Vector3.one;
    yield return new WaitForSeconds(0.8f);
    
    // Только вращение
    while (true) {
        element.transform.Rotate(0, 0, -180 * Time.deltaTime);
        yield return null;
    }
}

private System.Collections.IEnumerator AnimateElement1() {
    Image element = viigral2[1];
    float duration = 1f;
    float elapsedTime = 0f;
    
    Vector3 endPos = element.transform.position;
    // Начальная позиция - за левым краем экрана
    Vector3 startPos = new Vector3(-500, endPos.y, endPos.z);
    // Перемещаем элемент на стартовую позицию
    element.transform.position = startPos;
    
    // Анимация движения
    while (elapsedTime < duration) {
        float t = elapsedTime / duration;
        float smoothT = Mathf.SmoothStep(0, 1, t);
        element.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    element.transform.position = endPos;
}

private System.Collections.IEnumerator AnimateElement2() {
    Image element = viigral2[2];
    float duration = 1f;
    float elapsedTime = 0f;
    
    // Сохраняем исходную позицию как конечную
    Vector3 endPos = element.transform.position;
    // Устанавливаем начальную позицию слева
    Vector3 startPos = new Vector3(2500, endPos.y, endPos.z); // Изменено с -1080 на 1080
    // Перемещаем элемент на стартовую позицию
    element.transform.position = startPos;
    
    // Анимация движения
    while (elapsedTime < duration) {
        float t = elapsedTime / duration;
        float smoothT = Mathf.SmoothStep(0, 1, t);
        element.transform.position = Vector3.Lerp(startPos, endPos, smoothT);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    // Убеждаемся, что элемент точно встал на конечную позицию
    element.transform.position = endPos;
}
// Удалить из Start():
// StartCoroutine(HandleVictory());

private void Update() {
    if (!isTimerPaused) {
        gameTimer += Time.deltaTime;
        
        if (gameTimer >= RENT_INCREASE_INTERVAL) {
            UpdateAllRents();
            gameTimer = 0f;
        }
    }
    
    // Добавить проверку на победу
    StartCoroutine(HandleVictory());
}

private System.Collections.IEnumerator HandleVictory() {
    if (isAnimating) yield break; // Прерываем если идет анимация
    
    int activeTokenCount = 0;
    int winnerIndex = -1;
    
    for (int i = 0; i < playerTokens.Length; i++) {
        if (playerTokens[i].gameObject.activeInHierarchy) {
            activeTokenCount++;
            winnerIndex = i;
        }
    }
//activeTokenCount == 1 && !isAnimating &&
    if (activeTokenCount == 1 && !isAnimating &&f) {
        isAnimating = true; // Предотвращаем повторный запуск
        
        // Скрываем все кнопки
        foreach (Button button in buttons1) {
            if (button.gameObject.activeInHierarchy) {
                StartCoroutine(HideButtonAnimation(button));
            }
        }

        // Скрываем кнопку броска кубиков
        rollButton.gameObject.SetActive(false);

        yield return new WaitForSeconds(0.5f);
        // Анимируем перемещение фишки победителя
        Image winnerToken = playerTokens[winnerIndex];
        Vector3 startPos = winnerToken.transform.position;
        Vector3 endPos = viigral2[0].transform.position;
        float duration = 1f;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float yOffset = Mathf.Sin(t * Mathf.PI) * 100f;
            Vector3 currentPos = Vector3.Lerp(startPos, endPos, t);
            currentPos.y += yOffset;
            winnerToken.transform.position = currentPos;
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        winnerToken.transform.position = endPos;
foreach (Image element in viigral2) {
            element.gameObject.SetActive(true);
        }
        // Запускаем анимации элементов победы
        StartCoroutine(AnimateElement0());
        StartCoroutine(AnimateElement1());
        StartCoroutine(AnimateElement2());

        AddConsoleMessage($"игрок {winnerIndex + 1} побеждает в игре)))");
        f = false;
        isTimerPaused = true;
    }
}
private void ResetPropertyPrice(int propertyIndex) {
    Dictionary<int, float> specialBaseRents = new Dictionary<int, float>() {
        {6, 250f},
        {14, 250f},
        {25, 250f},
        {33, 250f}
    };
Dictionary<int, float> propertySetBaseRents = new Dictionary<int, float>() {
    {1, 500f}, {3, 500f}, {4, 500f},  // Первая группа
    {7, 500f}, {8, 500f}, {10, 500f},  // Вторая группа
    {11, 300f}, {12, 300f},  // Третья группа
    {15, 700f}, {16, 700f}, {18, 700f},  // Четвертая группа
    {20, 900f}, {22, 900f}, {23, 900f},  // Пятая группа
    {26, 900f}, {27, 900f}, {29, 900f},  // Шестая группа
    {30, 600f}, {31, 600f},  // Седьмая группа
    {34, 1200f}, {35, 1200f}, {37, 1200f}  // Восьмая группа
};
Dictionary<int, float> propertySetBaseRents2 = new Dictionary<int, float>() {
    {1, 100f}, {3, 100f}, {4, 100f},  // Первая группа
    {7, 130f}, {8, 130f}, {10, 130f},  // Вторая группа
    {11, 150f}, {12, 150f},  // Третья группа
    {15, 260f}, {16, 260f}, {18, 260f},  // Четвертая группа
    {20, 320f}, {22, 320f}, {23, 320f},  // Пятая группа
    {26, 350f}, {27, 350f}, {29, 350f},  // Шестая группа
    {30, 400f}, {31, 400f},  // Седьмая группа
    {34, 550f}, {35, 550f}, {37, 550f}  // Восьмая группа
};
    // Словарь групп предприятий
    Dictionary<int, List<int>> propertyGroups = new Dictionary<int, List<int>>() {
        {1, new List<int> {1, 3, 4}},        // Первая группа
        {2, new List<int> {7, 8, 10}},       // Вторая группа
        {3, new List<int> {11, 12}},         // Третья группа
        {4, new List<int> {15, 16, 18}},     // Четвертая группа
        {5, new List<int> {20, 22, 23}},     // Пятая группа
        {6, new List<int> {26, 27, 29}},     // Шестая группа
        {7, new List<int> {30, 31}},         // Седьмая группа
        {8, new List<int> {34, 35, 37}}      // Восьмая группа
    };

    float baseRent;
    
    // Для специальных предприятий
    if (specialBaseRents.ContainsKey(propertyIndex)) {
        baseRent = specialBaseRents[propertyIndex];
    }
    // Для предприятий из групп
    else if (propertySetBaseRents2.ContainsKey(propertyIndex)) {
        // Находим группу, к которой принадлежит предприятие
        var group = propertyGroups.FirstOrDefault(g => g.Value.Contains(propertyIndex));
        if (!group.Equals(default(KeyValuePair<int, List<int>>))) {
            // Проверяем, владеет ли один игрок всеми предприятиями в группе
            int owner = -1;
            bool hasFullGroup = true;
            
            foreach (int propIndex in group.Value) {
                if (!propertyOwnership.ContainsKey(propIndex)) {
                    hasFullGroup = false;
                    break;
                }
                
                if (owner == -1) {
                    owner = propertyOwnership[propIndex];
                } else if (owner != propertyOwnership[propIndex]) {
                    hasFullGroup = false;
                    break;
                }
            }

            baseRent = hasFullGroup ? 
                propertySetBaseRents2[propertyIndex]: 
                propertySetBaseRents2[propertyIndex];
        } else {
            baseRent = propertySetBaseRents2[propertyIndex];
        }
    }
    // Для остальных предприятий
    else {
        string basePrice = Cena[propertyIndex].text.Trim();
        bool isInThousands = basePrice.EndsWith("к");
        basePrice = basePrice.Replace("м", "").Replace("к", "").Trim();
        baseRent = float.Parse(basePrice, System.Globalization.CultureInfo.InvariantCulture);

        if (!isInThousands) {
            baseRent /= 1000f;
        }
    }

    // Обновляем отображение цены
    if (baseRent >= 1000) {
        Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
    } else {
        Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
    }
}
private System.Collections.IEnumerator HidePlayerToken(int playerIndex)
{
    // Check if the player index is valid 
    if (playerIndex < playerTokens.Length && playerTokens[playerIndex] != null)
    {
        // Get RectTransforms for animation
        RectTransform tokenTransform = playerTokens[playerIndex].GetComponent<RectTransform>();
        RectTransform buttonTransform = buton[playerIndex].GetComponent<RectTransform>();
        RectTransform textTransform = textComponents[playerIndex].GetComponent<RectTransform>();
        RectTransform imageTransform = igri[playerIndex].GetComponent<RectTransform>();

        float duration = 0.3f; // Duration of the animation
        float elapsedTime = 0f;
        Vector3 startScale = tokenTransform.localScale; // Current scale
        Vector3 endScale = Vector3.zero; // Target scale

        // Store initial positions
        Vector3 buttonStartPos = buttonTransform.position;
        Vector3 textStartPos = textTransform.position;  
        Vector3 tokenStartPos = tokenTransform.position;
        Vector3 targetPos = imageTransform.position; // All elements will move to igri position

        // Animate shrinking and movement
        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Easing function for smoother animation
            
            // Token moves to its own position and scales
            tokenTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            tokenTransform.position = Vector3.Lerp(tokenStartPos, tokenStartPos, t); // Stays in place

            // Other elements scale and move to igri position
            buttonTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            textTransform.localScale = Vector3.Lerp(startScale, endScale, t);
            imageTransform.localScale = Vector3.Lerp(startScale, endScale, t);

            buttonTransform.position = Vector3.Lerp(buttonStartPos, targetPos, t);
            textTransform.position = Vector3.Lerp(textStartPos, targetPos, t);

            elapsedTime += Time.deltaTime;
            StartCoroutine(HandleVictory());
    
    yield return null;
        }

        // Ensure the final scale is zero and position is at target
        tokenTransform.localScale = endScale;
        tokenTransform.position = tokenStartPos;
        
        buttonTransform.localScale = endScale;
        textTransform.localScale = endScale;
        imageTransform.localScale = endScale;

        buttonTransform.position = targetPos;
        textTransform.position = targetPos;
        imageTransform.position = targetPos;
        // Hide the components visually after animation
        playerTokens[playerIndex].gameObject.SetActive(false);
        buton[playerIndex].gameObject.SetActive(false);
        igri[playerIndex].gameObject.SetActive(false);
    }
}

private System.Collections.IEnumerator CheckBankruptcy() {
    while (true) {
        List<int> bankruptPlayers = new List<int>();
        
        // Сначала находим всех обанкротившихся игроков
        for (int i = 0; i < textComponents.Length; i++) {
            if (textComponents[i] != null) {
                try {
                    string moneyText = textComponents[i].text.Trim();
                    bool isInThousands = moneyText.EndsWith("к");
                    moneyText = moneyText.Replace("м", "").Replace("к", "").Trim();
                    float money = float.Parse(moneyText, System.Globalization.CultureInfo.InvariantCulture);

                    if (!isInThousands) {
                        money *= 1000f;
                    }

                    if (money <= 0 && !bankruptPlayers.Contains(i)) {
                        bankruptPlayers.Add(i);
                        skipNextTurn[i] = true;
                        
                        if (i == currentPlayerIndex) {
                            currentPlayerIndex = (currentPlayerIndex + 1) % activePlayerTokens.Count;
                            currentButtonImageIndex = currentPlayerIndex;
                        }

                        if (!string.IsNullOrEmpty(textComponents[i].text) && textComponents[i].text != "-1000м"&&g==true) {
                            textComponents[i].gameObject.SetActive(false);
                             StartCoroutine(HidePlayerToken(i));
                            if(i==0){
                            bankruptPlayerCount++;
                            AddConsoleMessage($"игрок {i + 1} обанкротился(");
                            textComponents[i].text = "-1000м";
                            g=false;
                            }
                        }
                        if (!string.IsNullOrEmpty(textComponents[i].text) && textComponents[i].text != "-1000м"&&d==true) {
                            textComponents[i].gameObject.SetActive(false);
                             StartCoroutine(HidePlayerToken(i));
                            if(i==1){
                            bankruptPlayerCount++;
                            AddConsoleMessage($"игрок {i + 1} обанкротился(");
                            textComponents[i].text = "-1000м";
                            d=false;
                            }
                        }
                        if (!string.IsNullOrEmpty(textComponents[i].text) && textComponents[i].text != "-1000м"&&j==true) {
                            textComponents[i].gameObject.SetActive(false);
                             StartCoroutine(HidePlayerToken(i));
                            if(i==2){
                            bankruptPlayerCount++;
                            AddConsoleMessage($"игрок {i + 1} обанкротился(");
                            textComponents[i].text = "-1000м";
                            j=false;
                            }
                        }
                        if (!string.IsNullOrEmpty(textComponents[i].text) && textComponents[i].text != "-1000м"&&k==true) {
                            textComponents[i].gameObject.SetActive(false);
                             StartCoroutine(HidePlayerToken(i));
                            if(i==3){
                            bankruptPlayerCount++;
                            AddConsoleMessage($"игрок {i + 1} обанкротился(");
                            textComponents[i].text = "-1000м";
                            k=false;
                            }
                        }
                    }
                }
                catch (System.Exception e) {
                    Debug.LogError($"Error checking bankruptcy for player {i}: {e.Message}");
                }
            }
        }

        // Обрабатываем собственность обанкротившихся игроков
        if (bankruptPlayers.Count > 0) {
            foreach (var propertyPair in propertyOwnership.ToList()) {
                int propertyIndex = propertyPair.Key;
                int ownerIndex = propertyPair.Value;

                if (bankruptPlayers.Contains(ownerIndex)) {
                    // Возвращаем спрайт к исходному состоянию
                    if ((propertyIndex >= 14 && propertyIndex <= 18) || 
                        (propertyIndex >= 33 && propertyIndex <= 37)) {
                        predpriiatia[propertyIndex].sprite = predpriiatiakupilpovernuli0[0];
                    } else {
                        predpriiatia[propertyIndex].sprite = predpriiatiakupil0[0];
                    }

                    // Сбрасываем цену в зависимости от типа предприятия
                    ResetPropertyPrice(propertyIndex);

                    // Удаляем запись о владении
                    propertyOwnership.Remove(propertyIndex);
                }
            }
        }

        yield return new WaitForSeconds(0.1f);
    }
}
private void UpdateAllRents() {
    rentMultiplier *= 2f;
    AddConsoleMessage($"аренда всех предприятий увеличена в 2 раза)");

    foreach (var property in propertyOwnership) {
        int positionIndex = property.Key;
        int ownerIndex = property.Value;

        try {
            // Get current rent value with suffix
            string currentRent = Cena[positionIndex].text.Trim();
            float baseRent;

            // Handle different suffixes
            if (currentRent.EndsWith("м")) {
                currentRent = currentRent.Replace("м", "").Trim();
                baseRent = float.Parse(currentRent, System.Globalization.CultureInfo.InvariantCulture) * 1000f;
            } else {
                currentRent = currentRent.Replace("к", "").Trim();
                baseRent = float.Parse(currentRent, System.Globalization.CultureInfo.InvariantCulture);
            }

            // Remove previous multiplier to get base rent
            baseRent = baseRent / (rentMultiplier / 2f);

            // Apply new multiplier
            float newRent = baseRent * rentMultiplier;

            // Update display with appropriate suffix
            if (newRent >= 1000) {
                Cena[positionIndex].text = (newRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
            } else {
                Cena[positionIndex].text = newRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"Error updating rent for position {positionIndex}: {e.Message}");
            AddConsoleMessage($"Ошибка при обновлении аренды для позиции {positionIndex}");
        }
    }
}
private System.Collections.IEnumerator ShowButtonAnimation(Button button) {
    isTimerPaused = true;  // Останавливаем таймер при появлении кнопок
    button.gameObject.SetActive(true);
    RectTransform rectTransform = button.GetComponent<RectTransform>();
    float duration = 0.3f;
    float elapsedTime = 0f;
    Vector3 startScale = Vector3.zero;
    Vector3 endScale = Vector3.one;
    
    rectTransform.localScale = startScale;
    
    while (elapsedTime < duration) {
        float t = elapsedTime / duration;
        t = Mathf.Sin(t * Mathf.PI * 0.5f); // Easing function for smoother animation
        rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    rectTransform.localScale = endScale;
}

private System.Collections.IEnumerator HideButtonAnimation(Button button) {
    RectTransform rectTransform = button.GetComponent<RectTransform>();
    float duration = 0.3f;
    float elapsedTime = 0f;
    Vector3 startScale = rectTransform.localScale;
    Vector3 endScale = Vector3.zero;
    
    while (elapsedTime < duration) {
        float t = elapsedTime / duration;
        t = Mathf.Sin(t * Mathf.PI * 0.5f); // Easing function for smoother animation
        rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    button.gameObject.SetActive(false);
    rectTransform.localScale = Vector3.one; // Reset scale for next time

    // Проверяем, все ли кнопки скрыты
    bool allButtonsHidden = true;
    foreach (Button btn in buttons1) {
        if (btn.gameObject.activeInHierarchy) {
            allButtonsHidden = false;
            break;
        }
    }
    
    // Возобновляем таймер только если все кнопки скрыты
    if (allButtonsHidden) {
        isTimerPaused = false;
    }
}

private void OnRollButtonClicked() {
    if (isAnimating || activePlayerTokens.Count == 0) return;

    StartCoroutine(HideRollButtonAnimation());
    
    currentButtonImageIndex = currentPlayerIndex;

    while (skipNextTurn[currentPlayerIndex]) {
        skipNextTurn[currentPlayerIndex] = false;
        currentPlayerIndex = (currentPlayerIndex + 1) % activePlayerTokens.Count;
        currentButtonImageIndex = currentPlayerIndex;
        UpdateButtonImage();
        AddConsoleMessage($"игрок {currentPlayerIndex + 1} пропускает ход(");
    }

    die1 = Random.Range(1, 7);
    die2 = Random.Range(1, 7);
    dieImage1.sprite = dieSprites[die1 - 1];
    dieImage2.sprite = dieSprites[die2 - 1];
    WriteToFile(die1, die2);
    StartCoroutine(AnimateDice());
    int totalSteps = die1 + die2;
    if (moveBackwardsNextTurn[currentPlayerIndex]) {
        totalSteps = -totalSteps;
        moveBackwardsNextTurn[currentPlayerIndex] = false;
        AddConsoleMessage($"игрок {currentPlayerIndex + 1} двигается назад на {Mathf.Abs(totalSteps)} {(Mathf.Abs(totalSteps) >= 2 && Mathf.Abs(totalSteps) <= 4 ? "шага" : "шагов")})");
    } else {
        AddConsoleMessage($"игрок {currentPlayerIndex + 1} двигается вперед на {totalSteps} {(totalSteps >= 2 && totalSteps <= 4 ? "шага" : "шагов")})");
    }
    
    lastMovedTokenIndex = currentPlayerIndex;
    MovePlayerToken(activePlayerTokens[currentPlayerIndex], totalSteps);
    
    if (die1 != die2) {
        currentPlayerIndex = (currentPlayerIndex + 1) % activePlayerTokens.Count;
        currentButtonImageIndex = currentPlayerIndex;
    }
}
// Add these methods to the DiceRoller class:

private void UpdateSpecialPropertiesRent(int playerIndex) {
    int[] specialProperties = { 6, 14, 25, 33 };
    int ownedCount = 0;
    
    foreach (int propertyIndex in specialProperties) {
        if (propertyOwnership.ContainsKey(propertyIndex) && propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount++;
        }
    }
    
    if (ownedCount >= 2) {
        float baseRent = ownedCount * 100f;
        float newRent = baseRent * rentMultiplier;
        
        foreach (int propertyIndex in specialProperties) {
            if (propertyOwnership.ContainsKey(propertyIndex) && propertyOwnership[propertyIndex] == playerIndex) {
                if (newRent >= 1000) {
                    Cena[propertyIndex].text = (newRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = newRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        
        AddConsoleMessage($"игрок {playerIndex + 1} владеет {ownedCount} специальными объектами, теперь у них новая аренда)");
    }
}

// Modify the ProcessPropertyPurchase method to include the special properties check:
private void UpdatePropertySetRent(int playerIndex) {
    // First property set (1, 3, 4)
    int[] propertySet1 = { 1, 3, 4 };
    // Second property set (7, 8, 10)
    int[] propertySet2 = { 7, 8, 10 };
    // Third property set (11, 12)
    int[] propertySet3 = { 11, 12 };
    int[] propertySet4 = { 15, 16, 18 };
    int[] propertySet5 = { 20, 22, 23 };
    int[] propertySet6 = { 26, 27, 29 };
    // New property set (30, 31)
    int[] propertySet7 = { 30, 31 };
    int[] propertySet8 = { 34, 35, 37 };
    
    // Check first property set
    int ownedCount1 = 0;
    foreach (int propertyIndex in propertySet1) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount1++;
        }
    }
    
    // Check second property set
    int ownedCount2 = 0;
    foreach (int propertyIndex in propertySet2) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount2++;
        }
    }
    
    // Check third property set
    int ownedCount3 = 0;
    foreach (int propertyIndex in propertySet3) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount3++;
        }
    }
    int ownedCount4 = 0;
    foreach (int propertyIndex in propertySet4) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount4++;
        }
    }
    int ownedCount5 = 0;
    foreach (int propertyIndex in propertySet5) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount5++;
        }
    }
    int ownedCount6 = 0;
    foreach (int propertyIndex in propertySet6) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount6++;
        }
    }
    // Check new property set (30, 31)
    int ownedCount7 = 0;
    foreach (int propertyIndex in propertySet7) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount7++;
        }
    }
    int ownedCount8 = 0;
    foreach (int propertyIndex in propertySet8) {
        if (propertyOwnership.ContainsKey(propertyIndex) && 
            propertyOwnership[propertyIndex] == playerIndex) {
            ownedCount8++;
        }
    }
    
    // Update rent for first set if player owns all properties
    if (ownedCount1 == propertySet1.Length) {
        foreach (int propertyIndex in propertySet1) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 500f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами первой группы, теперь у них новая аренда)");
    }
    
    // Update rent for second set if player owns all properties
    if (ownedCount2 == propertySet2.Length) {
        foreach (int propertyIndex in propertySet2) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 500f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами второй группы, теперь у них новая аренда)");
    }
    
    // Update rent for third set if player owns all properties
    if (ownedCount3 == propertySet3.Length) {
        foreach (int propertyIndex in propertySet3) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 300f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами третьей группы, теперь у них новая аренда)");
    }
    if (ownedCount4 == propertySet4.Length) {
        foreach (int propertyIndex in propertySet4) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 700f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами четвертой группы, теперь у них новая аренда)");
    }
    if (ownedCount5 == propertySet5.Length) {
        foreach (int propertyIndex in propertySet5) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 900f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами пятой группы, теперь у них новая аренда)");
    }
    if (ownedCount6 == propertySet6.Length) {
        foreach (int propertyIndex in propertySet6) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 900f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами шестой группы, теперь у них новая аренда)");
    }
    // Update rent for new property set (30, 31)
    if (ownedCount7 == propertySet7.Length) {
        foreach (int propertyIndex in propertySet7) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 600f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами седьмой группы, новая аренда теперь у них новая аренда)");
    }
    if (ownedCount8 == propertySet8.Length) {
        foreach (int propertyIndex in propertySet8) {
            if (propertyOwnership.ContainsKey(propertyIndex) && 
                propertyOwnership[propertyIndex] == playerIndex) {
                float baseRent = 1200f * rentMultiplier;
                if (baseRent >= 1000) {
                    Cena[propertyIndex].text = (baseRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
                } else {
                    Cena[propertyIndex].text = baseRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
                }
            }
        }
        AddConsoleMessage($"игрок {playerIndex + 1} владеет всеми объектами восьмой группы, новая аренда теперь у них новая аренда)");
    }
}
private void ProcessPropertyPurchase(int positionIndex, int imageIndex, bool isSpecialProperty) {
    if (positionIndex < predpriiatia.Length) {
        try {
            string currentPrice = Cena[positionIndex].text.Trim();
            currentPrice = currentPrice.Replace("к", "").Trim();
            float price = float.Parse(currentPrice, System.Globalization.CultureInfo.InvariantCulture);

            string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
            bool isInThousands = playerMoneyText.EndsWith("к");
            playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
            float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

            if (!isInThousands) {
                playerMoney *= 1000f;
            }

            if (playerMoney - price < 0) {
                buttons1[0].interactable = false;
                AddConsoleMessage($"у игрока {lastMovedTokenIndex + 1} недостаточно денег для покупки(");
                return;
            }

            predpriiatia[positionIndex].sprite = isSpecialProperty ? 
                predpriiatiakupilpovernuli[imageIndex] : 
                predpriiatiakupil[imageIndex];

            propertyOwnership[positionIndex] = lastMovedTokenIndex;

            // Calculate base rent (without multiplier)
            float baseRent = positionIndex == 6 || positionIndex == 14 || 
                            positionIndex == 25 || positionIndex == 33 ? 
                            price * 0.4f : price * 0.1f;

            // Apply current rent multiplier
            float newRent = baseRent * rentMultiplier;

            // Update the rent display
            if (newRent >= 1000) {
                Cena[positionIndex].text = (newRent / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
            } else {
                Cena[positionIndex].text = newRent.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
            }

            playerMoney -= price;

            if (playerMoney >= 1000) {
                textComponents[lastMovedTokenIndex].text = (playerMoney / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
            } else {
                textComponents[lastMovedTokenIndex].text = playerMoney.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
            }

            // Check and update special properties rent after purchase
            if (positionIndex == 6 || positionIndex == 14 || positionIndex == 25 || positionIndex == 33) {
                UpdateSpecialPropertiesRent(lastMovedTokenIndex);
            }
            UpdatePropertySetRent(lastMovedTokenIndex);
        } catch (System.Exception e) {
            Debug.LogError($"Error parsing numbers: {e.Message}");
            AddConsoleMessage("Ошибка при покупке собственности");
        }
    }
}    private void LoadPlayerTokenSprites(string fileName) {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        if (File.Exists(path)) {
            string[] lines = File.ReadAllLines(path);
            activePlayerTokens.Clear();
            buttonImageIndices.Clear();
            foreach (var position in tokenPositions) {
                position.Value.Clear();
            }
            for (int i = 0; i < playerTokens.Length; i++) {
                if (i < lines.Length && int.TryParse(lines[i], out int index) && index >= 0 && index < playerTokenSprites.Length) {
                    playerTokens[i].sprite = playerTokenSprites[index];
                    playerTokens[i].gameObject.SetActive(true);
                    activePlayerTokens.Add(playerTokens[i]);
                    buttonImageIndices.Add(index);
                    if (tokenPositions.ContainsKey(0)) {
                        tokenPositions[0].Add(playerTokens[i]);
                        playerTokens[i].transform.position = GetStackedPosition(0, tokenPositions[0].Count - 1);
                    }
                } else {
                    playerTokens[i].gameObject.SetActive(false);
                }
            }
        } else {
            Debug.LogError($"Файл не найден: {path}");
            AddConsoleMessage("Ошибка загрузки файла игроков");
        }
    }
    private System.Collections.IEnumerator ShowRollButtonAnimation() {
    rollButton.gameObject.SetActive(true);
    RectTransform rectTransform = rollButton.GetComponent<RectTransform>();
    float duration = 0.3f;
    float elapsedTime = 0f;
    Vector3 startScale = Vector3.zero;
    Vector3 endScale = Vector3.one;
    
    rectTransform.localScale = startScale;
    
    while (elapsedTime < duration) {
        float t = elapsedTime / duration;
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
        rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    rectTransform.localScale = endScale;
}

private System.Collections.IEnumerator HideRollButtonAnimation() {
    RectTransform rectTransform = rollButton.GetComponent<RectTransform>();
    float duration = 0.3f;
    float elapsedTime = 0f;
    Vector3 startScale = rectTransform.localScale;
    Vector3 endScale = Vector3.zero;
    
    while (elapsedTime < duration) {
        float t = elapsedTime / duration;
        t = Mathf.Sin(t * Mathf.PI * 0.5f);
        rectTransform.localScale = Vector3.Lerp(startScale, endScale, t);
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    rollButton.gameObject.SetActive(false);
    rectTransform.localScale = Vector3.one;
}
private void UpdateButtonImage() {
    if (buttonImageIndices.Count > 0) {
        while (skipNextTurn[currentButtonImageIndex]) {
            skipNextTurn[currentButtonImageIndex] = false;
            currentButtonImageIndex = (currentButtonImageIndex + 1) % buttonImageIndices.Count;
            currentPlayerIndex = currentButtonImageIndex;
        }
        
        rollButton.image.sprite = buttonImages[buttonImageIndices[currentButtonImageIndex]];
        StartCoroutine(ShowRollButtonAnimation());
    }
}

    private void WriteToFile(int die1, int die2) {
        int sum = die1 + die2;
        string path = Path.Combine(Application.persistentDataPath, "dice_results.txt");
        using (StreamWriter writer = new StreamWriter(path, false)) {
            writer.WriteLine(sum);
        }
        Debug.Log($"Сохраненная сумма бросков в файл: {path}");
    }

    private void MovePlayerToken(Image token, int steps) {
    int currentIndex = GetCurrentPositionIndex(token);
    
    if (currentIndex >= 0 && currentIndex < positions.Length) {
        tokenPositions[currentIndex].Remove(token);
        RearrangeTokens(currentIndex);
    }

    int targetIndex;
    if (steps < 0) {
        targetIndex = (currentIndex + steps);
        if (targetIndex < 0) targetIndex = positions.Length + targetIndex;
    } else {
        targetIndex = (currentIndex + steps) % positions.Length;
    }
    
    SetTokenOnTop(token);
    StartCoroutine(JumpThroughPositions(token, currentIndex + (steps < 0 ? -1 : 1), targetIndex));
}

    private Vector3 GetStackedPosition(int positionIndex, int stackIndex) {
    Vector3 basePosition = positions[positionIndex].position;
    
    // Positions 14-18: Stack horizontally to the left
    if (positionIndex >= 14 && positionIndex <= 18) {
        return new Vector3(basePosition.x - (stackIndex * 15f), basePosition.y, basePosition.z);
    }
    // Positions 19-32: Stack vertically upward
    else if (positionIndex >= 19 && positionIndex <= 32) {
        return new Vector3(basePosition.x, basePosition.y - (stackIndex * 15f), basePosition.z);
    }
    // Positions 33-37: Stack horizontally to the right
    else if (positionIndex >= 33 && positionIndex <= 37) {
        return new Vector3(basePosition.x + (stackIndex * 15f), basePosition.y, basePosition.z);
    }
    // All other positions: Stack vertically upward (default behavior)
    else {
        return new Vector3(basePosition.x, basePosition.y + (stackIndex * 15f), basePosition.z);
    }
}

    private void RearrangeTokens(int positionIndex) {
        List<Image> tokens = tokenPositions[positionIndex];
        for (int i = 0; i < tokens.Count; i++) {
            tokens[i].transform.position = GetStackedPosition(positionIndex, i);
        }
    }

    private int GetCurrentPositionIndex(Image token) {
    for (int i = 0; i < positions.Length; i++) {
        Vector3 basePosition = positions[i].position;
        
        if (i >= 14 && i <= 18) {
            // Для позиций 14-18 проверяем горизонтальное смещение влево
            float minX = basePosition.x - (tokenPositions[i].Count * 15f);
            float maxX = basePosition.x;
            if (token.transform.position.y == basePosition.y && 
                token.transform.position.x >= minX && 
                token.transform.position.x <= maxX) {
                return i;
            }
        }
        else if (i >= 19 && i <= 32) {
            // Для позиций 19-32 проверяем вертикальное смещение вниз
            float minY = basePosition.y - (tokenPositions[i].Count * 15f);
            float maxY = basePosition.y;
            if (token.transform.position.x == basePosition.x && 
                token.transform.position.y >= minY && 
                token.transform.position.y <= maxY) {
                return i;
            }
        }
        else if (i >= 33 && i <= 37) {
            // Для позиций 33-37 проверяем горизонтальное смещение вправо
            float minX = basePosition.x;
            float maxX = basePosition.x + (tokenPositions[i].Count * 15f);
            if (token.transform.position.y == basePosition.y && 
                token.transform.position.x >= minX && 
                token.transform.position.x <= maxX) {
                return i;
            }
        }
        else {
            // Для остальных позиций проверяем вертикальное смещение вверх
            float minY = basePosition.y;
            float maxY = basePosition.y + (tokenPositions[i].Count * 15f);
            if (token.transform.position.x == basePosition.x && 
                token.transform.position.y >= minY && 
                token.transform.position.y <= maxY) {
                return i;
            }
        }
    }
    return 0;
}
private System.Collections.IEnumerator GiveBonusNextTurn(int playerIndex) {
    // Wait until the next turn and the roll button is clicked
    yield return new WaitUntil(() => currentPlayerIndex == playerIndex && !isAnimating);
    
    // Wait for roll button click
    bool rollButtonClicked = false;
    UnityAction originalAction = null;
    
    if (rollButton != null) {
        originalAction = new UnityAction(() => {
            rollButtonClicked = true;
        });
        rollButton.onClick.AddListener(originalAction);
    }
    
    yield return new WaitUntil(() => rollButtonClicked);
    
    // Remove the temporary listener
    if (rollButton != null && originalAction != null) {
        rollButton.onClick.RemoveListener(originalAction);
    }
    
    // Now wait for 2 seconds after the button click
    yield return new WaitForSeconds(0.5f);
    
    try {
        string playerMoneyText = textComponents[playerIndex].text.Trim();
        bool isInThousands = playerMoneyText.EndsWith("к");
        playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
        float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

        if (!isInThousands) {
            playerMoney *= 1000f;
        }

        playerMoney += 200f;
        UpdatePlayerMoney(playerIndex, playerMoney);
        AddConsoleMessage($"игрок {playerIndex + 1} получил 200к за проход круга)");
    } catch (System.Exception e) {
        Debug.LogError($"Error processing next turn bonus: {e.Message}");
        AddConsoleMessage("Ошибка при начислении бонуса");
    }
}
    private System.Collections.IEnumerator JumpThroughPositions(Image token, int startIndex, int endIndex) {
    if (startIndex >= positions.Length) startIndex = 0;
    if (endIndex >= positions.Length) endIndex = 0;

    isAnimating = true;
    rollButton.interactable = false;

    // Check if we're moving backwards
    if (endIndex < startIndex && startIndex - endIndex < positions.Length / 2) {
        // Moving directly backwards
        for (int i = startIndex; i > endIndex; i--) {
            Vector3 targetPosition = positions[i].position;
            SetTokenOnTop(token);
            yield return JumpToPosition(token, targetPosition);
        }
    } else {
        // Moving forwards or across the board
        if (startIndex > endIndex) {
            // Moving past position 0
            for (int i = startIndex; i < positions.Length; i++) {
                Vector3 targetPosition = positions[i].position;
                SetTokenOnTop(token);
                yield return JumpToPosition(token, targetPosition);
            }
            startIndex = 0;
            
            // Add 200k bonus for passing position 0
            if (lastMovedTokenIndex >= 0 && !moveBackwardsNextTurn[lastMovedTokenIndex]) {
                try {
                    string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
                    bool isInThousands = playerMoneyText.EndsWith("к");
                    playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
                    float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

                    if (!isInThousands) {
                        playerMoney *= 1000f;
                    }

                    playerMoney += 200f;
                    UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);
                    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} получил 200к за проход круга)");
                } catch (System.Exception e) {
                    Debug.LogError($"Error processing circle completion bonus: {e.Message}");
                    AddConsoleMessage("Ошибка при начислении бонуса за круг");
                }
            }
        }

        for (int i = startIndex; i != endIndex; i = (i + 1) % positions.Length) {
            Vector3 targetPosition = positions[i].position;
            SetTokenOnTop(token);
            yield return JumpToPosition(token, targetPosition);
            
            // Check if passing position 0 during normal movement
            if (i == positions.Length - 1 && !moveBackwardsNextTurn[lastMovedTokenIndex]) {
                try {
                    string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
                    bool isInThousands = playerMoneyText.EndsWith("к");
                    playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
                    float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

                    if (!isInThousands) {
                        playerMoney *= 1000f;
                    }

                    playerMoney += 200f;
                    UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);
                    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} получил 200к за проход круга)");
                } catch (System.Exception e) {
                    Debug.LogError($"Error processing circle completion bonus: {e.Message}");
                    AddConsoleMessage("Ошибка при начислении бонуса за круг");
                }
            }
        }
    }

    tokenPositions[endIndex].Add(token);
    Vector3 finalPosition = GetStackedPosition(endIndex, tokenPositions[endIndex].Count - 1);

    SetTokenOnTop(token);
    yield return JumpToPosition(token, finalPosition);
    if (endIndex == 37 && lastMovedTokenIndex >= 0) {
    StartCoroutine(GiveBonusNextTurn(lastMovedTokenIndex));
}
// Inside JumpThroughPositions method, modify the teleportation check:
if (endIndex == 13) {
    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} отправляется в отпуск)))");
    tokenPositions[endIndex].Remove(token);
    RearrangeTokens(endIndex);
    tokenPositions[32].Add(token);
    Vector3 teleportPosition = GetStackedPosition(32, tokenPositions[32].Count - 1);
    SetTokenOnTop(token);
    yield return JumpToPosition(token, teleportPosition);
    endIndex = 32;
} else if (endIndex == 32) {
    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} возвращается из отпуска)");
    tokenPositions[endIndex].Remove(token);
    RearrangeTokens(endIndex);
    tokenPositions[13].Add(token);
    Vector3 teleportPosition = GetStackedPosition(13, tokenPositions[13].Count - 1);
    SetTokenOnTop(token);
    yield return JumpToPosition(token, teleportPosition);
    endIndex = 13;
    skipNextTurn[lastMovedTokenIndex] = true;
    if (die1 != die2) {
        
    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} пропускает следующий ход(");
    }
    if (die1 == die2) {
        AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} теряет дополнительный ход за дубль(");
        die1 = -1; // Сбрасываем значения, чтобы дубль не сработал
        die2 = -2;
    }
}

if (endIndex == 24 && lastMovedTokenIndex >= 0) {
    moveBackwardsNextTurn[lastMovedTokenIndex] = true;
    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} будет двигаться назад в следующем ходу)");
}
    // Добавляем проверку для позиции 5
    if (endIndex == 5 && lastMovedTokenIndex >= 0) {
    skipNextTurn[lastMovedTokenIndex] = true;
    if (die1 != die2) {
    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} пропускает следующий ход(");
    }
    if (die1 == die2) {
        AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} теряет дополнительный ход за дубль(");
        die1 = -1; // Сбрасываем значения, чтобы дубль не сработал
        die2 = -2;
    }
}

    // Tax deduction for positions 2, 21, and 36
    if ((endIndex == 2 || endIndex == 21 || endIndex == 36) && lastMovedTokenIndex >= 0) {
        try {
            string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
            bool isInThousands = playerMoneyText.EndsWith("к");
            playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
            float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

            if (!isInThousands) {
                playerMoney *= 1000f;
            }

            float taxAmount = playerMoney * 0.15f;
            playerMoney -= taxAmount;

            UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);

            if (taxAmount >= 1000) {
                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} заплатил налог {(taxAmount / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture)}м)");
            } else {
                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} заплатил налог {taxAmount.ToString("F0", System.Globalization.CultureInfo.InvariantCulture)}к)");
            }
        } catch (System.Exception e) {
            Debug.LogError($"Error processing tax deduction: {e.Message}");
            AddConsoleMessage("Ошибка при расчете налога");
        }
    }
    // Chance positions 9, 17, and 28
    else if ((endIndex == 9 || endIndex == 17 || endIndex == 28) && lastMovedTokenIndex >= 0) {
        try {
            bool isLucky = Random.value >= 0.5f;

            if (isLucky) {
                // Receive 100k from each player
                float totalReceived = 0f;
                for (int i = 0; i < activePlayerTokens.Count; i++) {
                    if (i != lastMovedTokenIndex) {
                        string playerMoneyText = textComponents[i].text.Trim();
                        bool isInThousands = playerMoneyText.EndsWith("к");
                        playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
                        float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

                        if (!isInThousands) {
                            playerMoney *= 1000f;
                        }

                        playerMoney -= 100f;
                        UpdatePlayerMoney(i, playerMoney);
                        totalReceived += 100f;
                    }
                }
                
                totalReceived -= 100f*bankruptPlayerCount;

                // Add received money to current player
                string currentPlayerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
                bool currentIsInThousands = currentPlayerMoneyText.EndsWith("к");
                currentPlayerMoneyText = currentPlayerMoneyText.Replace("м", "").Replace("к", "").Trim();
                float currentPlayerMoney = float.Parse(currentPlayerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

                if (!currentIsInThousands) {
                    currentPlayerMoney *= 1000f;
                }

                currentPlayerMoney += totalReceived;
                UpdatePlayerMoney(lastMovedTokenIndex, currentPlayerMoney);

                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} получил по 100к от каждого игрока)");
            } else {
                // Pay 150k fine
                string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
                bool isInThousands = playerMoneyText.EndsWith("к");
                playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
                float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

                if (!isInThousands) {
                    playerMoney *= 1000f;
                }

                playerMoney -= 150f;
                UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);

                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} заплатил штраф 150к(");
            }
        } catch (System.Exception e) {
            Debug.LogError($"Error processing chance event: {e.Message}");
            AddConsoleMessage("Ошибка при обработке события");
        }
    }
    if (endIndex == 19 && lastMovedTokenIndex >= 0) {
    try {
        string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
        bool isInThousands = playerMoneyText.EndsWith("к");
        playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
        float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

        if (!isInThousands) {
            playerMoney *= 1000f;
        }

        playerMoney += 350f;
        UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);

        AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} сорвал джек пот 350к)");
    } catch (System.Exception e) {
        Debug.LogError($"Error processing bonus: {e.Message}");
        AddConsoleMessage("Ошибка при начислении бонуса");
    }
    }

    if (endIndex == 0 && lastMovedTokenIndex >= 0) {
    try {
        string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
        bool isInThousands = playerMoneyText.EndsWith("к");
        playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
        float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

        if (!isInThousands) {
            playerMoney *= 1000f;
        }

        // Add the bonus of 400k
        playerMoney += 400f;

        UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);

        AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} получил бонус 400к)");
    } catch (System.Exception e) {
        Debug.LogError($"Error processing bonus: {e.Message}");
        AddConsoleMessage("Ошибка при начислении бонуса");
    }
}

     if (!IsRestrictedPosition(endIndex)) {
    ShowButtons();
}
else {
    rollButton.interactable = true;
}

isAnimating = false;

// If it's doubles (die1 == die2), don't hide the roll button and don't update the button image
if (die1 == die2 && endIndex != 5) {
    StartCoroutine(ShowRollButtonAnimation());
    AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} может бросить кубики еще раз)");
} else if (skipNextTurn[currentPlayerIndex]) {
    UpdateButtonImage();
}

// Обновляем изображение кнопки только если это не дубль или если игрок должен пропустить ход
if (die1 != die2 || skipNextTurn[currentPlayerIndex]) {
    UpdateButtonImage();
}
}

    private bool IsRestrictedPosition(int positionIndex) {
        int[] restrictedPositions = { 0, 2, 5, 9, 13, 17, 19, 21, 24, 28, 32, 36 };
        foreach (int pos in restrictedPositions) {
            if (pos == positionIndex) {
                return true;
            }
        }
        return false;
    }

    private void OnButtonClicked(int buttonIndex) {
    if (buttonIndex == 0 && lastMovedTokenIndex >= 0) {
        int positionIndex = GetCurrentPositionIndex(activePlayerTokens[lastMovedTokenIndex]);
        AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} приобрел собственность)");
        
        if (positionIndex >= 0) {
            int imageIndex = buttonImageIndices[lastMovedTokenIndex];

            if (positionIndex == 14 || positionIndex == 15 || positionIndex == 16 || 
                positionIndex == 18 || positionIndex == 33 || positionIndex == 34 || 
                positionIndex == 35 || positionIndex == 37) {
                ProcessPropertyPurchase(positionIndex, imageIndex, true);
            } else {
                ProcessPropertyPurchase(positionIndex, imageIndex, false);
            }
        }
    }

    foreach (Button button in buttons1) {
        StartCoroutine(HideButtonAnimation(button));
    }
    rollButton.interactable = true;
}


    private void PayRent(int positionIndex) {
    try {
        if (propertyOwnership.TryGetValue(positionIndex, out int ownerIndex)) {
            string rentText = Cena[positionIndex].text.Trim();
            float rentAmount;

            if (rentText.EndsWith("м")) {
                rentText = rentText.Replace("м", "").Trim();
                rentAmount = float.Parse(rentText, System.Globalization.CultureInfo.InvariantCulture) * 1000f;
            } else {
                rentText = rentText.Replace("к", "").Trim();
                rentAmount = float.Parse(rentText, System.Globalization.CultureInfo.InvariantCulture);
            }

            if (rentAmount >= 1000) {
                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} платит аренду {rentAmount/1000f:F1}м игроку {ownerIndex + 1})");
            } else {
                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} платит аренду {rentAmount:F0}к игроку {ownerIndex + 1})");
            }

            string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
            bool isInThousands = playerMoneyText.EndsWith("к");
            playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
            float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

            if (!isInThousands) {
                playerMoney *= 1000f;
            }

            // Проверяем, хватает ли денег на оплату
            if (playerMoney < rentAmount) {
                textComponents[lastMovedTokenIndex].gameObject.SetActive(false);
                // Если выпал дубль, сбрасываем значения кубиков
                if (die1 == die2) {
                    die1 = -1;
                    die2 = -2;
                }
            }

            ProcessRentPayment(ownerIndex, rentAmount);
        }

        buttons1[2].gameObject.SetActive(false);
        rollButton.interactable = true;
    }
    catch (System.Exception e) {
        Debug.LogError($"Error processing rent payment: {e.Message}");
        AddConsoleMessage("Ошибка при оплате аренды");
        rollButton.interactable = true;
    }
}

    private void ProcessRentPayment(int ownerIndex, float rentAmount) {
        string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
        bool isInThousands = playerMoneyText.EndsWith("к");
        playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
        float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

        if (!isInThousands) {
            playerMoney *= 1000f;
        }

        string ownerMoneyText = textComponents[ownerIndex].text.Trim();
        bool ownerIsInThousands = ownerMoneyText.EndsWith("к");
        ownerMoneyText = ownerMoneyText.Replace("м", "").Replace("к", "").Trim();
        float ownerMoney = float.Parse(ownerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

        if (!ownerIsInThousands) {
            ownerMoney *= 1000f;
        }

        playerMoney -= rentAmount;
        ownerMoney += rentAmount;

        UpdatePlayerMoney(lastMovedTokenIndex, playerMoney);
        UpdatePlayerMoney(ownerIndex, ownerMoney);
    }

    private void UpdatePlayerMoney(int playerIndex, float amount) {
        if (amount >= 1000) {
            textComponents[playerIndex].text = (amount / 1000f).ToString("F1", System.Globalization.CultureInfo.InvariantCulture) + "м";
        } else {
            textComponents[playerIndex].text = amount.ToString("F0", System.Globalization.CultureInfo.InvariantCulture) + "к";
        }
    }

    private void ShowButtons() {
    int positionIndex = GetCurrentPositionIndex(activePlayerTokens[lastMovedTokenIndex]);
    
    // Проверяем, является ли позиция специальной (где нельзя покупать собственность)
    if (IsRestrictedPosition(positionIndex)) {
        foreach (Button button in buttons1) {
            StartCoroutine(HideButtonAnimation(button));
        }
        rollButton.interactable = true;
        return;
    }
    
    // Проверяем, входит ли позиция в допустимый диапазон
    if (positionIndex >= 0 && positionIndex < predpriiatia.Length && predpriiatia[positionIndex] != null) {
        bool isOwned = false;
        int ownerIndex = -1;
        
        Sprite currentSprite = predpriiatia[positionIndex].sprite;
        for (int i = 0; i < predpriiatiakupil.Length; i++) {
            if (currentSprite == predpriiatiakupil[i] || currentSprite == predpriiatiakupilpovernuli[i]) {
                isOwned = true;
                ownerIndex = i;
                break;
            }
        }

        if (isOwned) {
            if (ownerIndex != buttonImageIndices[lastMovedTokenIndex]) {
                StartCoroutine(HideButtonAnimation(buttons1[0]));
                StartCoroutine(HideButtonAnimation(buttons1[1]));
                StartCoroutine(ShowButtonAnimation(buttons1[2]));
                buttons1[2].onClick.RemoveAllListeners();
                buttons1[2].onClick.AddListener(() => PayRent(positionIndex));
                AddConsoleMessage($"игрок {lastMovedTokenIndex + 1} должен оплатить аренду(");
            } else {
                foreach (Button button in buttons1) {
                    StartCoroutine(HideButtonAnimation(button));
                }
                rollButton.interactable = true;
            }
        } else {
            foreach (Button button in buttons1) {
                StartCoroutine(ShowButtonAnimation(button));
            }
            CheckAffordability(positionIndex);
        }
    } else {
        foreach (Button button in buttons1) {
            StartCoroutine(HideButtonAnimation(button));
        }
        rollButton.interactable = true;
    }
}

    private void CheckAffordability(int positionIndex) {
        try {
            string currentPrice = Cena[positionIndex].text.Trim();
            currentPrice = currentPrice.Replace("к", "").Trim();
            float price = float.Parse(currentPrice, System.Globalization.CultureInfo.InvariantCulture);

            string playerMoneyText = textComponents[lastMovedTokenIndex].text.Trim();
            bool isInThousands = playerMoneyText.EndsWith("к");
            playerMoneyText = playerMoneyText.Replace("м", "").Replace("к", "").Trim();
            float playerMoney = float.Parse(playerMoneyText, System.Globalization.CultureInfo.InvariantCulture);

            if (!isInThousands) {
                playerMoney *= 1000f;
            }

            buttons1[0].interactable = (playerMoney - price >= 0);
            buttons1[2].gameObject.SetActive(false);

            if (!buttons1[0].interactable) {
                AddConsoleMessage($"у игрока {lastMovedTokenIndex + 1} недостаточно денег для покупки собственности(");
            }
        }
        catch (System.Exception e) {
            Debug.LogError($"Error checking affordability: {e.Message}");
            AddConsoleMessage("Ошибка при проверке возможности покупки");
            buttons1[0].interactable = false;
        }
    }

    private void SetTokenOnTop(Image token) {
        token.transform.SetAsLastSibling();
    }
    

    private System.Collections.IEnumerator JumpToPosition(Image token, Vector3 targetPosition) {
        float jumpHeight = 50f;
        float duration = 0.5f;
        float elapsedTime = 0f;
        Vector3 startPosition = token.transform.position;

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            float yOffset = Mathf.Sin(t * Mathf.PI) * jumpHeight;
            token.transform.position = Vector3.Lerp(startPosition, targetPosition, t) + new Vector3(0f, yOffset, 0f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        token.transform.position = targetPosition;
    }

    private System.Collections.IEnumerator AnimateDice() {
        Vector3 startPositionDie1 = new Vector3(Screen.width / 2 - 20, -100, 0);
        Vector3 startPositionDie2 = new Vector3(Screen.width / 2 + 20, -100, 0);
        Vector3 endPositionDie1 = new Vector3(Screen.width / 2 + 10, Screen.height / 2 + 10, 0);
        Vector3 endPositionDie2 = new Vector3(Screen.width / 2 - 80, Screen.height / 2 - 80, 0);

        dieImage1.transform.position = startPositionDie1;
        dieImage2.transform.position = startPositionDie2;

        float duration = 0.5f;
        float elapsedTime = 0f;

        dieImage1.color = new Color(1f, 1f, 1f, 1f);
        dieImage2.color = new Color(1f, 1f, 1f, 1f);

        while (elapsedTime < duration) {
            float t = elapsedTime / duration;
            dieImage1.transform.position = Vector3.Lerp(startPositionDie1, endPositionDie1, t);
            dieImage2.transform.position = Vector3.Lerp(startPositionDie2, endPositionDie2, t);
            dieImage1.transform.Rotate(new Vector3(0, 0, 360) * Time.deltaTime * (1 / duration));
            dieImage2.transform.Rotate(new Vector3(0, 0, -360) * Time.deltaTime * (1 / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        dieImage1.transform.position = endPositionDie1;
        dieImage2.transform.position = endPositionDie2;
    }
}