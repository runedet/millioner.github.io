using UnityEngine;
using TMPro;
using System.IO;

public class NewMonoBehaviourScript11235 : MonoBehaviour
{
    public TextMeshProUGUI[] moneyTexts;
    private string filePath;
    private float updateInterval = 0.1f;
    private float timer = 0f;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "money.txt");
        
        if (moneyTexts == null || moneyTexts.Length == 0)
        {
            Debug.LogError("Массив текстов TextMeshPro пуст!");
            return;
        }

        for (int i = 0; i < moneyTexts.Length; i++)
        {
            if (moneyTexts[i] == null)
            {
                Debug.LogError($"Элемент TextMeshPro {i} не присвоен!");
            }
        }
        
        UpdateMoneyTexts();
    }

    void Update()
    {
        timer += Time.deltaTime;
        
        if (timer >= updateInterval)
        {
            UpdateMoneyTexts();
            timer = 0f;
        }
    }

    void UpdateMoneyTexts()
    {
        try
        {
            if (File.Exists(filePath))
            {
                string money = File.ReadAllText(filePath);
                foreach(TextMeshProUGUI text in moneyTexts)
                {
                    if(text != null)
                    {
                        text.text = money;
                    }
                }
            }
            else
            {
                Debug.LogWarning("Файл money.txt не найден!");
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError("Ошибка при чтении файла: " + e.Message);
        }
    }
}