using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.IO;
using System.Collections.Generic;

public class NewMonoBehaviourScript90 : MonoBehaviour
{
    [SerializeField] private RectTransform[] images30Deg;
    [SerializeField] private RectTransform[] imagesMinus15Deg;
    [SerializeField] private RectTransform[] images15Deg;
    [SerializeField] private RectTransform[] images0Deg;
    [SerializeField] private RectTransform[] buttons;
    [SerializeField] private RectTransform specialObject;
    [SerializeField] private RectTransform instantHideObject;
    [SerializeField] private RectTransform downwardDisappearingObject;
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private Button hideButton;

    private Vector3[] initialPositions;
    private float dropHeight = 1000f;
    private float swingAmount = 15f;
    private float dropSpeed = 600f;
    private float swingSpeed = 1.5f;
    private float dampingSpeed = 1f;
    private float globalSwingPhase = 0f;
    private bool isAnimatingUp = false;
    private RectTransform[] allElements;
    private string filePath;
    private bool isSpecialObjectVisible = false;
    private Coroutine currentSpecialObjectAnimation;
    private bool shouldShowElements = true;
    private bool isDownwardObjectVisible = true;
    private Coroutine downwardDisappearCoroutine;
    private Coroutine downwardAppearCoroutine;
    private bool isDisappearing = false;
    private int animatingElementsCount;
    private bool isInitialAnimationComplete = false;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "ima.txt");
        CheckFileContent();
        if (inputField != null)
        {
            inputField.characterLimit = 10;
            inputField.onValidateInput += ValidateRussianInput;
        }

        if (specialObject != null)
        {
            specialObject.gameObject.SetActive(false);
        }

        if (hideButton != null)
        {
            hideButton.onClick.AddListener(HideSpecialObject);
        }

        if (instantHideObject != null)
        {
            if (!shouldShowElements)
            {
                instantHideObject.gameObject.SetActive(false);
            }
        }

        if (downwardDisappearingObject != null)
        {
            if (shouldShowElements)
            {
                downwardDisappearingObject.gameObject.SetActive(true);
                StartDownwardAppearAnimation();
            }
            else
            {
                downwardDisappearingObject.gameObject.SetActive(false);
            }
        }

        int totalElements = images30Deg.Length + imagesMinus15Deg.Length + 
                         images15Deg.Length + images0Deg.Length + buttons.Length;
        allElements = new RectTransform[totalElements];
        
        int currentIndex = 0;
        images30Deg.CopyTo(allElements, currentIndex);
        currentIndex += images30Deg.Length;
        
        imagesMinus15Deg.CopyTo(allElements, currentIndex);
        currentIndex += imagesMinus15Deg.Length;
        
        images15Deg.CopyTo(allElements, currentIndex);
        currentIndex += images15Deg.Length;
        
        images0Deg.CopyTo(allElements, currentIndex);
        currentIndex += images0Deg.Length;
        
        buttons.CopyTo(allElements, currentIndex);

        initialPositions = new Vector3[allElements.Length];
        for (int i = 0; i < allElements.Length; i++)
        {
            initialPositions[i] = allElements[i].position;
            if (!shouldShowElements)
            {
                allElements[i].gameObject.SetActive(false);
            }
            else if (shouldShowElements)
            {
                allElements[i].position += Vector3.up * dropHeight;
            }
        }

        if (buttons.Length > 0)
        {
            Button firstButton = buttons[0].GetComponent<Button>();
            if (firstButton != null && shouldShowElements)
            {
                firstButton.interactable = false;
            }
        }

        foreach (RectTransform buttonRect in buttons)
        {
            Button button = buttonRect.GetComponent<Button>();
            if (button != null)
            {
                button.onClick.AddListener(() => HandleButtonClick());
            }
        }

        if (shouldShowElements)
        {
            animatingElementsCount = allElements.Length;
            StartCoroutine(GlobalSwing());

            for (int i = 0; i < allElements.Length; i++)
            {
                StartCoroutine(DropAndSwingElement(allElements[i], initialPositions[i], i, GetFinalAngle(i)));
            }
        }
        else
        {
            isInitialAnimationComplete = true;
            if (buttons.Length > 0)
            {
                Button firstButton = buttons[0].GetComponent<Button>();
                if (firstButton != null)
                {
                    firstButton.interactable = true;
                }
            }
        }
    }

    private char ValidateRussianInput(string text, int charIndex, char addedChar)
    {
        if ((addedChar >= 'а' && addedChar <= 'я') || 
            (addedChar >= 'А' && addedChar <= 'Я') || 
            addedChar == 'ё' || addedChar == 'Ё')
        {
            return addedChar;
        }
        return '\0';
    }

    private void StartDownwardAppearAnimation()
    {
        if (downwardAppearCoroutine != null)
        {
            StopCoroutine(downwardAppearCoroutine);
        }
        downwardAppearCoroutine = StartCoroutine(AnimateDownwardAppear());
    }

    private IEnumerator AnimateDownwardAppear()
    {
        float animationDuration = 1.5f;
        float elapsedTime = 0f;
        
        Vector2 targetPosition = downwardDisappearingObject.anchoredPosition;
        Vector2 startPosition = new Vector2(targetPosition.x, targetPosition.y + 250f);
        
        downwardDisappearingObject.anchoredPosition = startPosition;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            downwardDisappearingObject.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
            yield return null;
        }

        downwardDisappearingObject.anchoredPosition = targetPosition;
    }

    private void CheckFileContent()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string fileContent = File.ReadAllText(filePath);
                shouldShowElements = string.IsNullOrWhiteSpace(fileContent);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error reading file: " + e.Message);
            shouldShowElements = true;
        }
    }

    private void HideSpecialObject()
    {
        if (isSpecialObjectVisible && specialObject != null)
        {
            if (currentSpecialObjectAnimation != null)
            {
                StopCoroutine(currentSpecialObjectAnimation);
            }
            currentSpecialObjectAnimation = StartCoroutine(AnimateSpecialObjectHide());
        }
    }

    private IEnumerator AnimateSpecialObjectHide()
    {
        float animationDuration = 0.5f;
        float elapsedTime = 0f;
        Vector2 startPosition = specialObject.anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, Screen.height + 100f);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            specialObject.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
            yield return null;
        }

        specialObject.gameObject.SetActive(false);
        isSpecialObjectVisible = false;
    }

    private void HandleButtonClick()
    {
        if (isDisappearing) return;

        if (inputField != null)
        {
            string inputText = inputField.text;
            
            Button clickedButton = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
            int clickedButtonIndex = System.Array.IndexOf(buttons, clickedButton.GetComponent<RectTransform>());
            
            if (clickedButtonIndex == 1)
            {
                StartSequentialDisappearAnimation();
                return;
            }
            
            if (string.IsNullOrEmpty(inputText))
            {
                if (!isSpecialObjectVisible)
                {
                    StartSpecialObjectAnimation();
                }
            }
            else
            {
                SaveTextToFile(inputText);
                inputField.text = "";
                StartAllElementsDisappearAnimation();
                if (instantHideObject != null)
                {
                    instantHideObject.gameObject.SetActive(false);
                }
                if (downwardDisappearingObject != null && isDownwardObjectVisible)
                {
                    StartDownwardDisappearAnimation();
                }
            }
        }
    }

    private void StartSequentialDisappearAnimation()
    {
        if (isDisappearing) return;
        isDisappearing = true;
        StartCoroutine(AnimateSequentialDisappear());
    }

    private IEnumerator AnimateElementDisappear(RectTransform element, float duration)
    {
        if (element == null) yield break;

        Vector3 startPosition = element.position;
        Vector3 targetPosition = startPosition + Vector3.up * (Screen.height * 1.2f);
        
        float startTime = Time.unscaledTime;
        
        while (Time.unscaledTime - startTime < duration)
        {
            if (element == null) yield break;
            
            float progress = (Time.unscaledTime - startTime) / duration;
            float easedProgress = progress * progress;
            
            element.position = Vector3.Lerp(startPosition, targetPosition, easedProgress);
            
            yield return null;
        }

        if (element != null)
        {
            element.gameObject.SetActive(false);
            element.position = startPosition;
        }
    }

    private IEnumerator AnimateSequentialDisappear()
    {
        if (isDisappearing) yield break;
        isDisappearing = true;

        List<RectTransform> elementsToAnimate = new List<RectTransform>();
        elementsToAnimate.AddRange(images0Deg);
        elementsToAnimate.AddRange(images15Deg);
        elementsToAnimate.AddRange(buttons);

        float delayBetweenElements = 0.05f;
        float animationDuration = 0.7f;

        List<Coroutine> runningCoroutines = new List<Coroutine>();

        foreach (RectTransform element in elementsToAnimate)
        {
            if (element != null && element.gameObject.activeInHierarchy)
            {
                var coroutine = StartCoroutine(AnimateElementDisappear(element, animationDuration));
                runningCoroutines.Add(coroutine);
                yield return new WaitForSecondsRealtime(delayBetweenElements);
            }
        }

        yield return new WaitForSecondsRealtime(animationDuration);
        
        isDisappearing = false;
    }

    private void StartDownwardDisappearAnimation()
    {
        if (downwardDisappearCoroutine != null)
        {
            StopCoroutine(downwardDisappearCoroutine);
        }
        downwardDisappearCoroutine = StartCoroutine(AnimateDownwardDisappear());
    }

    private IEnumerator AnimateDownwardDisappear()
    {
        float animationDuration = 1.5f;
        float elapsedTime = 0f;
        Vector2 startPosition = downwardDisappearingObject.anchoredPosition;
        Vector2 targetPosition = new Vector2(startPosition.x, -Screen.height - 100f);

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            downwardDisappearingObject.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
            yield return null;
        }

        downwardDisappearingObject.gameObject.SetActive(false);
        isDownwardObjectVisible = false;
    }

    private void StartAllElementsDisappearAnimation()
    {
        foreach (RectTransform element in allElements)
        {
            StartCoroutine(AnimateElementDisappear(element, 1.5f));
        }
    }

    private void StartSpecialObjectAnimation()
    {
        if (specialObject != null && !isSpecialObjectVisible)
        {
            isSpecialObjectVisible = true;
            specialObject.gameObject.SetActive(true);
            if (currentSpecialObjectAnimation != null)
            {
                StopCoroutine(currentSpecialObjectAnimation);
            }
            currentSpecialObjectAnimation = StartCoroutine(AnimateSpecialObjectShow());
        }
    }

    private IEnumerator AnimateSpecialObjectShow()
    {
        float animationDuration = 0.5f;
        float elapsedTime = 0f;

        specialObject.anchoredPosition = new Vector2(specialObject.anchoredPosition.x, Screen.height + 100f);
        Vector2 startPosition = specialObject.anchoredPosition;
        Vector2 targetPosition = Vector2.zero;

        while (elapsedTime < animationDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / animationDuration;
            float easedProgress = Mathf.SmoothStep(0, 1, progress);

            specialObject.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, easedProgress);
            yield return null;
        }

        specialObject.anchoredPosition = targetPosition;
    }

    private void SaveTextToFile(string text)
    {
        try
        {
            File.AppendAllText(filePath, text + System.Environment.NewLine);
            Debug.Log("Text saved to file: " + filePath);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Error saving to file: " + e.Message);
        }
    }

    private float GetFinalAngle(int index)
    {
        int previousArraysLength = images30Deg.Length + imagesMinus15Deg.Length + 
                                 images15Deg.Length + images0Deg.Length;

        if (index < images30Deg.Length)
        {
            return 30f;
        }
        else if (index < images30Deg.Length + imagesMinus15Deg.Length)
        {
            return -15f;
        }
        else if (index < images30Deg.Length + imagesMinus15Deg.Length + images15Deg.Length)
        {
            return 15f;
        }
        else if (index < images30Deg.Length + imagesMinus15Deg.Length + images15Deg.Length + images0Deg.Length)
        {
            return 0f;
        }
        else
        {
            int buttonIndex = index - previousArraysLength;
            return buttonIndex == 0 ? 0f : 0f;
        }
    }

    private IEnumerator GlobalSwing()
    {
        while (!isAnimatingUp)
        {
            globalSwingPhase += Time.deltaTime * swingSpeed;
            yield return null;
        }
    }

    private IEnumerator DropAndSwingElement(RectTransform element, Vector3 targetPosition, int index, float finalAngle)
    {
        float elapsedTime = 0f;
        Vector3 startPosition = element.position;
        float currentSwingAmount = swingAmount;

        float baseAngle = finalAngle;
        int direction = finalAngle >= 0 ? 1 : -1;

        element.rotation = Quaternion.Euler(0f, 0f, baseAngle);

        while (elapsedTime < 3f && !isAnimatingUp)
        {
            elapsedTime += Time.deltaTime;

            float t = Mathf.Min(1, (elapsedTime * dropSpeed) / dropHeight);
            float currentY = Mathf.Lerp(startPosition.y, targetPosition.y, t);

            float swing = baseAngle + (Mathf.Sin(globalSwingPhase) * currentSwingAmount * direction);

            currentSwingAmount = Mathf.Lerp(currentSwingAmount, 0f, Time.deltaTime * dampingSpeed);

            element.position = new Vector3(targetPosition.x, currentY, targetPosition.z);
            element.rotation = Quaternion.Euler(0f, 0f, swing);

            yield return null;
        }

        if (!isAnimatingUp)
        {
            element.position = targetPosition;
            element.rotation = Quaternion.Euler(0f, 0f, baseAngle);
            
            animatingElementsCount--;
            if (animatingElementsCount <= 0 && !isInitialAnimationComplete)
            {
                isInitialAnimationComplete = true;
                if (buttons.Length > 0)
                {
                    Button firstButton = buttons[0].GetComponent<Button>();
                    if (firstButton != null)
                    {
                        firstButton.interactable = true;
                    }
                }
            }
        }
    }
}