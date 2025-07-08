using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript11 : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateEmptyTextFile("birkin.txt");
    }

    void CreateEmptyTextFile(string fileName)
    {
        string path = Path.Combine(Application.persistentDataPath, fileName);
        File.WriteAllText(path, string.Empty); // Создаёт пустой файл
        Debug.Log($"Файл создан: {path}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}