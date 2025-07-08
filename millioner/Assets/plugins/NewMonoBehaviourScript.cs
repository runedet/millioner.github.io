using UnityEngine;
using TMPro;
using System.Data;
using PUSHKA.MySQL;

public class NewMonoBehaviourScript1234 : MonoBehaviour
{
    public TMP_Text textElement;
    private SqlDataBase dataBase;
    
    void Start()
    {
        dataBase = new SqlDataBase("185.105.91.114;SslMode=None","a","a","1");
        
        dataBase.SelectQuery("SELECT * FROM a.b;", out DataTable questions);
        
        string resultText = "";
        foreach (DataRow row in questions.Rows)
        {
            resultText += $"{row[0]}    {row[1]}   {row[2]}\n";
        }
        
        textElement.text = resultText + "End";
    }
}