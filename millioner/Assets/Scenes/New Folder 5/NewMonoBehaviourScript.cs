using UnityEngine;

public class NewMonoBehaviourScript500 : MonoBehaviour
{
    public GameObject firstObject;  // Первый объект
    public GameObject secondObject; // Второй объект
    
    private Vector3 firstObjectFinalPosition;  // Конечная позиция первого объекта
    private Vector3 secondObjectFinalPosition; // Конечная позиция второго объекта
    
    public float speed = 5f;  // Скорость перемещения
    private bool isAnimating = true;

    void Start()
    {
        // Сохраняем конечные позиции объектов
        firstObjectFinalPosition = firstObject.transform.position;
        secondObjectFinalPosition = secondObject.transform.position;
        
        // Устанавливаем начальные позиции
        firstObject.transform.position = new Vector3(1940f, 
            firstObjectFinalPosition.y, 
            firstObjectFinalPosition.z);
            
        secondObject.transform.position = new Vector3(-20f, 
            secondObjectFinalPosition.y, 
            secondObjectFinalPosition.z);
    }

    void Update()
    {
        if (isAnimating)
        {
            // Перемещаем первый объект
            firstObject.transform.position = Vector3.Lerp(
                firstObject.transform.position, 
                firstObjectFinalPosition, 
                speed * Time.deltaTime);

            // Перемещаем второй объект
            secondObject.transform.position = Vector3.Lerp(
                secondObject.transform.position, 
                secondObjectFinalPosition, 
                speed * Time.deltaTime);

            // Проверяем, достигли ли объекты своих позиций
            if (Vector3.Distance(firstObject.transform.position, firstObjectFinalPosition) < 0.01f &&
                Vector3.Distance(secondObject.transform.position, secondObjectFinalPosition) < 0.01f)
            {
                isAnimating = false;
            }
        }
    }
}