using UnityEngine;
using UnityEngine.UI;

public class ArrayAnimationController : MonoBehaviour
{
    public GameObject[] topArray;
    public GameObject[] bottomArray;
    public Button showButton;
    public Button hideButton;
    
    public float animationDuration = 1.0f;
    public float spaceBetweenObjects = 2.0f;
    public float offscreenOffset = 2f;
    
    private Vector3[] topArrayTargetPositions;
    private Vector3[] bottomArrayTargetPositions;
    private bool isAnimating = false;
    private float animationTimer = 0f;
    private bool isShowing = false;

    void Start()
    {
        topArrayTargetPositions = new Vector3[topArray.Length];
        bottomArrayTargetPositions = new Vector3[bottomArray.Length];

        for (int i = 0; i < topArray.Length; i++)
        {
            topArrayTargetPositions[i] = topArray[i].transform.position;
            topArray[i].transform.position = topArrayTargetPositions[i] + Vector3.up * (Screen.height * offscreenOffset);
        }

        for (int i = 0; i < bottomArray.Length; i++)
        {
            bottomArrayTargetPositions[i] = bottomArray[i].transform.position;
            bottomArray[i].transform.position = bottomArrayTargetPositions[i] - Vector3.up * (Screen.height * offscreenOffset);
        }

        showButton.onClick.AddListener(ShowArrays);
        hideButton.onClick.AddListener(HideArrays);
    }

    void Update()
    {
        // Проверка нажатия клавиши Escape
        if (Input.GetKeyDown(KeyCode.Escape) && !isAnimating && isShowing)
        {
            HideArrays();
        }

        if (isAnimating)
        {
            animationTimer += Time.deltaTime;
            float progress = animationTimer / animationDuration;

            if (progress >= 1f)
            {
                progress = 1f;
                isAnimating = false;
            }

            if (isShowing)
            {
                for (int i = 0; i < topArray.Length; i++)
                {
                    Vector3 startPos = topArrayTargetPositions[i] + Vector3.up * (Screen.height * offscreenOffset);
                    topArray[i].transform.position = Vector3.Lerp(startPos, topArrayTargetPositions[i], progress);
                }

                for (int i = 0; i < bottomArray.Length; i++)
                {
                    Vector3 startPos = bottomArrayTargetPositions[i] - Vector3.up * (Screen.height * offscreenOffset);
                    bottomArray[i].transform.position = Vector3.Lerp(startPos, bottomArrayTargetPositions[i], progress);
                }
            }
            else
            {
                for (int i = 0; i < topArray.Length; i++)
                {
                    Vector3 endPos = topArrayTargetPositions[i] + Vector3.up * (Screen.height * offscreenOffset);
                    topArray[i].transform.position = Vector3.Lerp(topArrayTargetPositions[i], endPos, progress);
                }

                for (int i = 0; i < bottomArray.Length; i++)
                {
                    Vector3 endPos = bottomArrayTargetPositions[i] - Vector3.up * (Screen.height * offscreenOffset);
                    bottomArray[i].transform.position = Vector3.Lerp(bottomArrayTargetPositions[i], endPos, progress);
                }
            }
        }
    }

    void ShowArrays()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            isShowing = true;
            animationTimer = 0f;
        }
    }

    void HideArrays()
    {
        if (!isAnimating)
        {
            isAnimating = true;
            isShowing = false;
            animationTimer = 0f;
        }
    }
}