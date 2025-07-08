using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class RevealAndShrink : MonoBehaviour
{
    public Image hiddenImage;
    public Image[] overlayImages;
    public GameObject objectToHide;
    public float fadeDuration = 0.05f;
    public float shrinkDuration = 1f;

    private bool isAnimating = false;
    private Vector3[] originalScales;

    void Start()
    {
        hiddenImage.gameObject.SetActive(false);
        SetImageAlpha(0.5f);
        hiddenImage.transform.localScale = Vector3.one;

        originalScales = new Vector3[overlayImages.Length];
        for (int i = 0; i < overlayImages.Length; i++)
        {
            if (overlayImages[i] != null)
            {
                originalScales[i] = overlayImages[i].transform.localScale;
            }
        }
    }

    public void OnButtonClick()
    {
        if (!isAnimating)
        {
            if (objectToHide != null)
            {
                objectToHide.SetActive(false);
            }
            StartCoroutine(RevealAndShrinkCoroutine());
        }
    }

    private System.Collections.IEnumerator RevealAndShrinkCoroutine()
    {
        isAnimating = true;

        foreach (var img in overlayImages)
        {
            if (img != null)
                img.transform.SetSiblingIndex(hiddenImage.transform.GetSiblingIndex() + 1);
        }

        hiddenImage.gameObject.SetActive(true);

        float timer = 0f;
        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;
            SetImageAlpha(t);
            yield return null;
        }
        SetImageAlpha(1f);

        timer = 0f;
        Vector3 startScale = Vector3.one;
        Vector3 endScale = Vector3.zero;
        bool sceneLoaded = false;

        while (timer < shrinkDuration)
        {
            timer += Time.deltaTime;
            float t = timer / shrinkDuration;
            
            hiddenImage.transform.localScale = Vector3.Lerp(startScale, endScale, t);

            for (int i = 0; i < overlayImages.Length; i++)
            {
                if (overlayImages[i] != null)
                {
                    RectTransform rectTransform = overlayImages[i].GetComponent<RectTransform>();
                    Vector3 currentScale = originalScales[i] * (1f - t);
                    
                    // Сохраняем текущий pivot
                    Vector2 originalPivot = rectTransform.pivot;
                    
                    // Устанавливаем pivot в центр
                    rectTransform.pivot = new Vector2(0.5f, 0.5f);
                    
                    // Применяем масштаб
                    rectTransform.localScale = currentScale;
                    
                    // Возвращаем оригинальный pivot
                    rectTransform.pivot = originalPivot;
                }
            }

            if (!sceneLoaded && t >= 0.35f)
            {
                sceneLoaded = true;
                SceneManager.LoadScene(1);
            }

            yield return null;
        }

        for (int i = 0; i < overlayImages.Length; i++)
        {
            if (overlayImages[i] != null)
            {
                overlayImages[i].transform.localScale = originalScales[i];
            }
        }

        hiddenImage.gameObject.SetActive(false);
        isAnimating = false;
    }

    private void SetImageAlpha(float alpha)
    {
        Color c = hiddenImage.color;
        c.a = alpha;
        hiddenImage.color = c;
    }
}