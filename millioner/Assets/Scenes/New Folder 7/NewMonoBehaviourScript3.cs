using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript31 : MonoBehaviour
{
    string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, "money.txt");
        
        // Проверяем, существует ли файл
        if (!File.Exists(filePath))
        {
            // Если файл не существует, создаем его и записываем 0
            File.WriteAllText(filePath, "0");
        }
    }

    void Update()
    {
        
    }
}