using UnityEngine;
using UnityEngine.UI; // Необходимо для работы с UI элементами

public class kupi : MonoBehaviour
{
    // Массив купи меня
    public Text[] Cena;
    public Image[] predpriiatia;

    // облигация игрока

    public Text[] textComponents;
    public Button[] buton;

    // Массив компонентов Image для фишек
    public Sprite[] predpriiatiakupil;
    public Sprite[] predpriiatiakupilpovernuli;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Устанавливаем все значения текстовых компонентов в "2.0м"
        foreach (Text textComponent in textComponents)
        {
            textComponent.text = "2.0м";
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Здесь можно добавить логику обновления, если это необходимо
    }
}
