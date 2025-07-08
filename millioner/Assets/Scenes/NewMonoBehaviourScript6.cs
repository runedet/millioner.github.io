using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript412341234 : MonoBehaviour
{
    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "birkin.txt");
        
        // Проверяем, существует ли файл
        if (!File.Exists(filePath))
        {
            // Создаем файл
            File.Create(filePath).Dispose();
            Debug.Log("Файл создан: " + filePath);
        }
        else
        {
            Debug.Log("Файл уже существует: " + filePath);
        }
    }

    void Update()
    {
        
    }
}