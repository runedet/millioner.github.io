using UnityEngine;
using TMPro;
using System.IO;

public class NewMonoBehaviourScript321314123 : MonoBehaviour
{
    public TMP_Text playerNameText;
    private string fileName = "ima.txt";

    void Start()
    {
        LoadPlayerName();
    }

    void LoadPlayerName()
    {
        string filePath = Path.Combine(Application.dataPath, fileName);
        
        if (File.Exists(filePath))
        {
            string playerName = File.ReadAllText(filePath);
            playerNameText.text = string.IsNullOrEmpty(playerName) ? "" : playerName;
        }
        else
        {
            playerNameText.text = "";
        }
    }
}