using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript2354622 : MonoBehaviour
{
    void Start()
    {
        string path = Path.Combine(Application.dataPath, "ima.txt");
        
        // Создаем файл, если он не существует
        if (!File.Exists(path))
        {
            File.WriteAllText(path, "");
            Debug.Log("Файл ima.txt создан в: " + path);
        }
    }

    void Update()
    {
        
    }
}