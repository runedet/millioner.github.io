using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class ButtonController213456314 : MonoBehaviour
{
    public Button[] buttons;
    public RectTransform[] objectsToHide;
    public RectTransform[] instantHideObjects;
    public RectTransform showingObject;
    public RectTransform showingObject2;
    
    public Button spendButton;
    public Button hideButton;
    public Button hideButton2;
    
    private float topDisappearBoundary = 900f;
    private float bottomDisappearBoundary = -900f;
    
    private Vector3[] initialPositions;
    private Vector3[] instantInitialPositions;
    private Vector3 initialPosition;
    private Vector3 initialPosition2;
    private float slideDistance = 1000f;
    private float slideDuration = 0.5f;
    private bool isAnimating = false;
    private int currentButtonIndex;

    private int[] buttonValues = { 2000, 10000, 50000, 400000, 3000000, 12000000 };
    private Dictionary<string, HashSet<int>> purchaseStateByUser = new Dictionary<string, HashSet<int>>();
    private string saveFilePath;
    private string currentUser;

    void Start()
    {
        string winFilePath = Path.Combine(Application.dataPath, "win.txt");
        if (!File.Exists(winFilePath))
        {
            File.WriteAllText(winFilePath, "");
        }
        
        saveFilePath = Path.Combine(Application.dataPath, "purchases.txt");
        LoadCurrentUser();
        LoadPurchaseState();

        if (buttons.Length != 6)
        {
            Debug.LogError("Need to assign 6 buttons!");
            return;
        }

        initialPositions = new Vector3[objectsToHide.Length];
        for (int i = 0; i < objectsToHide.Length; i++)
        {
            initialPositions[i] = objectsToHide[i].position;
            if (objectsToHide[i].GetComponent<CanvasGroup>() == null)
                objectsToHide[i].gameObject.AddComponent<CanvasGroup>();
        }

        instantInitialPositions = new Vector3[instantHideObjects.Length];
        for (int i = 0; i < instantHideObjects.Length; i++)
        {
            instantInitialPositions[i] = instantHideObjects[i].position;
        }

        if (showingObject != null && showingObject2 != null)
        {
            initialPosition = showingObject.position;
            initialPosition2 = showingObject2.position;
            showingObject.gameObject.SetActive(false);
            showingObject2.gameObject.SetActive(false);

            if (showingObject.GetComponent<CanvasGroup>() == null)
                showingObject.gameObject.AddComponent<CanvasGroup>();
            if (showingObject2.GetComponent<CanvasGroup>() == null)
                showingObject2.gameObject.AddComponent<CanvasGroup>();
        }

        for (int i = 0; i < buttons.Length; i++)
        {
            int index = i;
            buttons[i].onClick.AddListener(() => OnButtonClick(index));
        }

        if (spendButton != null)
            spendButton.onClick.AddListener(SpendMoneyAndHide);
        
        if (hideButton != null)
            hideButton.onClick.AddListener(() => HideObjectWithAnimation(showingObject, initialPosition));
        
        if (hideButton2 != null)
            hideButton2.onClick.AddListener(() => HideObjectWithAnimation(showingObject2, initialPosition2));

        // Показываем все объекты изначально
        ShowAllObjects();
        // Применяем сохраненное состояние для текущего пользователя
        ApplySavedState();
    }

    private void LoadCurrentUser()
    {
        string imaPath = Path.Combine(Application.dataPath, "ima.txt");
        if (File.Exists(imaPath))
        {
            currentUser = File.ReadAllText(imaPath).Trim();
        }
        else
        {
            currentUser = "default";
            Debug.LogError("ima.txt not found!");
        }
    }

    private void ShowAllObjects()
    {
        foreach (var obj in objectsToHide)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(true);
                var canvasGroup = obj.GetComponent<CanvasGroup>();
                if (canvasGroup != null)
                {
                    canvasGroup.alpha = 1f;
                }
            }
        }

        foreach (var obj in instantHideObjects)
        {
            if (obj != null)
            {
                obj.gameObject.SetActive(true);
            }
        }
    }

    private void LoadPurchaseState()
    {
        purchaseStateByUser.Clear();
        if (File.Exists(saveFilePath))
        {
            string[] lines = File.ReadAllLines(saveFilePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(':');
                if (parts.Length == 2)
                {
                    string username = parts[0];
                    string[] purchaseIndexes = parts[1].Split(',');
                    HashSet<int> purchases = new HashSet<int>();
                    
                    foreach (string indexStr in purchaseIndexes)
                    {
                        if (!string.IsNullOrEmpty(indexStr) && int.TryParse(indexStr, out int index))
                        {
                            purchases.Add(index);
                        }
                    }
                    
                    purchaseStateByUser[username] = purchases;
                }
            }
        }
    }

    private void SavePurchaseState()
    {
        List<string> lines = new List<string>();
        foreach (var kvp in purchaseStateByUser)
        {
            string purchases = string.Join(",", kvp.Value);
            lines.Add($"{kvp.Key}:{purchases}");
        }
        File.WriteAllLines(saveFilePath, lines);
    }

    private void ApplySavedState()
    {
        if (!purchaseStateByUser.ContainsKey(currentUser))
        {
            return;
        }

        HashSet<int> userPurchases = purchaseStateByUser[currentUser];
        foreach (int index in userPurchases)
        {
            if (index < objectsToHide.Length)
            {
                objectsToHide[index].gameObject.SetActive(false);
            }
            if (index < instantHideObjects.Length)
            {
                instantHideObjects[index].gameObject.SetActive(false);
            }
        }
    }

    private void OnButtonClick(int index)
    {
        if (!isAnimating)
        {
            if (purchaseStateByUser.ContainsKey(currentUser) && 
                purchaseStateByUser[currentUser].Contains(index))
            {
                return;
            }

            currentButtonIndex = index;
            CheckMoneyAndAnimate(index);
        }
    }

   private void SpendMoneyAndHide()
{
    if (isAnimating) return;

    try
    {
        string filePath = Path.Combine(Application.persistentDataPath, "money.txt");
        string moneyString = File.ReadAllText(filePath);
        int currentMoney = int.Parse(moneyString);
        
        currentMoney -= buttonValues[currentButtonIndex];
        File.WriteAllText(filePath, currentMoney.ToString());

        // Сохраняем состояние покупки для текущего пользователя
        if (!purchaseStateByUser.ContainsKey(currentUser))
        {
            purchaseStateByUser[currentUser] = new HashSet<int>();
        }
        purchaseStateByUser[currentUser].Add(currentButtonIndex);
        SavePurchaseState();

        if (currentButtonIndex < objectsToHide.Length)
        {
            if (currentButtonIndex == 2 || currentButtonIndex == 5)
            {
                StartCoroutine(SlideUpAndHide(objectsToHide[currentButtonIndex]));
            }
            else
            {
                StartCoroutine(SlideDownAndHide(objectsToHide[currentButtonIndex]));
            }
        }

        if (currentButtonIndex < instantHideObjects.Length)
        {
            InstantHide(instantHideObjects[currentButtonIndex]);
        }

        StartCoroutine(AnimateHide(showingObject, initialPosition));
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error processing money: {e.Message}");
    }
}

private void CheckMoneyAndAnimate(int buttonIndex)
{
    if (isAnimating) return;

    try
    {
        string moneyString = File.ReadAllText(Path.Combine(Application.persistentDataPath, "money.txt"));
        int currentMoney = int.Parse(moneyString);

        if (currentMoney >= buttonValues[buttonIndex])
        {
            StartCoroutine(AnimateShow(showingObject, initialPosition));
        }
        else
        {
            StartCoroutine(AnimateShow(showingObject2, initialPosition2));
        }
    }
    catch (System.Exception e)
    {
        Debug.LogError($"Error reading money.txt: {e.Message}");
    }
}
    private void InstantHide(RectTransform objectToHide)
    {
        if (objectToHide != null)
        {
            objectToHide.gameObject.SetActive(false);
        }
    }

    private IEnumerator SlideDownAndHide(RectTransform objectToHide)
    {
        isAnimating = true;
        Vector3 startPos = objectToHide.position;
        Vector3 endPos = startPos + Vector3.down * 2000f;
        float elapsedTime = 0f;
        float duration = 2.0f;

        CanvasGroup canvasGroup = objectToHide.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            Vector3 newPosition = Vector3.Lerp(startPos, endPos, smoothT);
            objectToHide.position = newPosition;

            if (newPosition.y <= bottomDisappearBoundary)
            {
                float fadeProgress = Mathf.InverseLerp(bottomDisappearBoundary, bottomDisappearBoundary - 100f, newPosition.y);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null;
        }

        objectToHide.gameObject.SetActive(false);
        objectToHide.position = startPos;
        canvasGroup.alpha = 1f;
        isAnimating = false;
    }

    private IEnumerator SlideUpAndHide(RectTransform objectToHide)
    {
        isAnimating = true;
        Vector3 startPos = objectToHide.position;
        Vector3 endPos = startPos + Vector3.up * 2000f;
        float elapsedTime = 0f;
        float duration = 2.0f;

        CanvasGroup canvasGroup = objectToHide.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;
            float smoothT = Mathf.SmoothStep(0, 1, t);
            
            Vector3 newPosition = Vector3.Lerp(startPos, endPos, smoothT);
            objectToHide.position = newPosition;

            if (newPosition.y >= topDisappearBoundary)
            {
                float fadeProgress = Mathf.InverseLerp(topDisappearBoundary, topDisappearBoundary + 100f, newPosition.y);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null;
        }

        objectToHide.gameObject.SetActive(false);
        objectToHide.position = startPos;
        canvasGroup.alpha = 1f;
        isAnimating = false;
    }

   

    private void HideObjectWithAnimation(RectTransform objectToHide, Vector3 startPosition)
    {
        if (!isAnimating)
        {
            StartCoroutine(AnimateHide(objectToHide, startPosition));
        }
    }

    private IEnumerator AnimateShow(RectTransform objectToAnimate, Vector3 targetPosition)
    {
        isAnimating = true;

        showingObject.gameObject.SetActive(false);
        showingObject2.gameObject.SetActive(false);

        objectToAnimate.gameObject.SetActive(true);
        CanvasGroup canvasGroup = objectToAnimate.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0f;

        Vector3 startPosition = targetPosition + Vector3.up * slideDistance;
        objectToAnimate.position = startPosition;

        float elapsedTime = 0f;
        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / slideDuration;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            objectToAnimate.position = newPosition;

            if (newPosition.y <= topDisappearBoundary)
            {
                float fadeProgress = Mathf.InverseLerp(topDisappearBoundary, topDisappearBoundary - 100f, newPosition.y);
                canvasGroup.alpha = Mathf.Lerp(0f, 1f, fadeProgress);
            }

            yield return null;
        }

        objectToAnimate.position = targetPosition;
        canvasGroup.alpha = 1f;
        
        isAnimating = false;
    }

    private IEnumerator AnimateHide(RectTransform objectToHide, Vector3 startPosition)
    {
        isAnimating = true;

        objectToHide.gameObject.SetActive(true);
        CanvasGroup canvasGroup = objectToHide.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1f;

        Vector3 targetPosition = startPosition + Vector3.up * slideDistance;
        float elapsedTime = 0f;

        while (elapsedTime < slideDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / slideDuration;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            Vector3 newPosition = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            objectToHide.position = newPosition;

            if (newPosition.y >= topDisappearBoundary)
            {
                float fadeProgress = Mathf.InverseLerp(topDisappearBoundary, topDisappearBoundary + 100f, newPosition.y);
                canvasGroup.alpha = Mathf.Lerp(1f, 0f, fadeProgress);
            }

            yield return null;
        }

        objectToHide.gameObject.SetActive(false);
        objectToHide.position = startPosition;
        canvasGroup.alpha = 1f;
        
        isAnimating = false;
    }
}