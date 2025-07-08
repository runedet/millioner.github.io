using UnityEngine;
using UnityEngine.UI;

public class NewMonoBehaviourScript52345634 : MonoBehaviour
{
    public Button showButton;
    public Button hideButton;
    public GameObject targetObject;
    
    private Vector2 originalPosition;
    private Vector2 hiddenPosition;
    private RectTransform targetRectTransform;
    
    [SerializeField]
    private float animationDuration = 1f;
    [SerializeField]
    private float offsetDistance = 1000f;
    
    private bool isAnimating = false;
    private bool isShowing = false;
    private float animationProgress = 0f;
    
    void Start()
    {
        if (targetObject == null)
        {
            Debug.LogError("Target Object is not assigned!");
            return;
        }
        if (showButton == null)
        {
            Debug.LogError("Show Button is not assigned!");
            return;
        }
        if (hideButton == null)
        {
            Debug.LogError("Hide Button is not assigned!");
            return;
        }

        targetRectTransform = targetObject.GetComponent<RectTransform>();
        if (targetRectTransform == null)
        {
            Debug.LogError("Target Object must be a UI element with RectTransform!");
            return;
        }

        showButton.onClick.AddListener(Show);
        hideButton.onClick.AddListener(Hide);
        
        // Сохраняем исходную позицию как Vector2
        originalPosition = targetRectTransform.anchoredPosition;
        
        // Вычисляем позицию скрытия
        hiddenPosition = originalPosition + new Vector2(0, offsetDistance);
        
        // Изначально прячем объект
        targetRectTransform.anchoredPosition = hiddenPosition;
        isShowing = false;
    }

    void Update()
    {
        if (isAnimating)
        {
            animationProgress += Time.deltaTime / animationDuration;
            
            if (animationProgress >= 1f)
            {
                animationProgress = 1f;
                isAnimating = false;
            }

            // Используем разные начальные и конечные позиции в зависимости от направления анимации
            Vector2 startPos = isShowing ? hiddenPosition : originalPosition;
            Vector2 endPos = isShowing ? originalPosition : hiddenPosition;

            float smoothProgress = Mathf.Sin(animationProgress * Mathf.PI * 0.5f);
            targetRectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, smoothProgress);
        }
    }

    public void Show()
    {
        if (!isShowing)
        {
            isShowing = true;
            isAnimating = true;
            animationProgress = 0f;
        }
    }

    public void Hide()
    {
        if (isShowing)
        {
            isShowing = false;
            isAnimating = true;
            animationProgress = 0f;
        }
    }

    void OnDestroy()
    {
        if (showButton != null)
            showButton.onClick.RemoveListener(Show);
        if (hideButton != null)
            hideButton.onClick.RemoveListener(Hide);
    }
}