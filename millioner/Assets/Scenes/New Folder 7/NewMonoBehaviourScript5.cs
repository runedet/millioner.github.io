using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript311234 : MonoBehaviour
{
    string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "voice.txt");
        
        // Проверяем, существует ли файл
        if (!File.Exists(filePath))
        {
            // Если файл не существует, создаем его и записываем 0
            File.WriteAllText(filePath, "");
        }
    }

    void Update()
    {
        
    }
}