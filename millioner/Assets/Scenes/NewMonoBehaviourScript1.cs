using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TextChanger : MonoBehaviour
{
    public Text uiText; // Перетащите сюда ваш элемент Text из инспектора
    private string[] messages = { "Привет, я неграааа я козел аааа", "в каком году была подписана кревская уния и так варианты ответов а в 1385 б в 2345 в в 1234 г 1829" };
    private int currentIndex = 0;

    void Start()
    {
        uiText.text = messages[currentIndex];
        StartCoroutine(ChangeText());
    }

    private IEnumerator ChangeText()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f); // Ждать 10 секунд
            currentIndex = (currentIndex + 1) % messages.Length; // Переключаться между сообщениями
            uiText.text = messages[currentIndex];
        }
    }
}