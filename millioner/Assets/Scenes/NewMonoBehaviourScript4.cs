using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript41234 : MonoBehaviour
{
    void Start()
    {
        string filePath = Path.Combine(Application.dataPath, "ima.txt");
        
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