using UnityEngine;
using System.IO;

public class MovementBetweenObjects : MonoBehaviour
{
    [SerializeField] private Transform[] points; // Точки перемещения
    [SerializeField] private Transform movingImage; // Изображение, которое будет перемещаться
    private int currentPointIndex = 0; // Индекс текущей точки
    private string filePath; // Путь к файлу

    void Start()
    {
        // Получаем путь к файлу dice_results.txt
        filePath = Path.Combine(Application.dataPath, "dice_results.txt");
        
        // Устанавливаем начальную позицию изображения
        if (points.Length > 0 && movingImage != null)
        {
            movingImage.position = points[currentPointIndex].position; // Начальная позиция
        }
        else
        {
            Debug.LogWarning("Нет доступных точек или изображение не назначено!");
        }
    }

    void Update()
    {
        if (points.Length == 0 || movingImage == null)
            return;

        // Проверяем существование файла
        if (File.Exists(filePath))
        {
            // Читаем все строки из файла
            string[] lines = File.ReadAllLines(filePath);
            int totalSteps = 0;

            // Проходим по всем строкам и суммируем значения
            foreach (string line in lines)
            {
                if (int.TryParse(line, out int number))
                {
                    totalSteps += number;
                }
            }

            // Если есть шаги для перемещения
            if (totalSteps > 0)
            {
                MoveForward(totalSteps); // Перемещение изображения

                // Очищаем файл после использования
                File.WriteAllText(filePath, string.Empty);
            }
        }
    }

    private void MoveForward(int steps)
    {
        // Вычисляем новый индекс с учетом зацикливания массива
        currentPointIndex = (currentPointIndex + steps) % points.Length;
        
        // Обновляем позицию изображения
        movingImage.position = points[currentPointIndex].position;

        Debug.Log($"Изображение перемещено на точку: {currentPointIndex}"); // Логируем текущее положение
    }
}
