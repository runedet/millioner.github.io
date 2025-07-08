using UnityEngine;
using System.IO;

public class NewMonoBehaviourScript22134 : MonoBehaviour
{
    public GameObject targetObject;
     // Объект, которым будем управлять
    private string filePath;
    
    void Start()
    {
        // Получаем путь к файлу (в папке с проектом)
        filePath = Path.Combine(Application.dataPath, "ima.txt");
        CheckFileAndUpdateObject();
    }

    void Update()
    {
        CheckFileAndUpdateObject();
    }

    void CheckFileAndUpdateObject()
    {
        if (targetObject != null)
        {
            // Проверяем существует ли файл
            if (File.Exists(filePath))
            {
                // Читаем содержимое файла
                string fileContent = File.ReadAllText(filePath);
                
                // Если файл пустой - показываем объект
                // Если не пустой - скрываем
                targetObject.SetActive(string.IsNullOrEmpty(fileContent));
            }
            else
            {
                // Если файл не существует, считаем его пустым и показываем объект
                targetObject.SetActive(true);
            }
        }
    }
}