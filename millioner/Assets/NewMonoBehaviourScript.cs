using UnityEngine;
using TMPro;
using System.Data;
using MySql.Data.MySqlClient;

public class DatabaseManager : MonoBehaviour
{
    public TMP_Text textElement;
    private MySqlConnection connection;
    
    void Start()
    {
        string connectionString = "Server=185.105.91.114;Database=a;Uid=a;Pwd=1;SslMode=None";
        try
        {
            connection = new MySqlConnection(connectionString);
            connection.Open();
            
            string query = "SELECT * FROM a.b;";
            MySqlCommand cmd = new MySqlCommand(query, connection);
            
            DataTable questions = new DataTable();
            questions.Load(cmd.ExecuteReader());
            
            string resultText = "";
            foreach (DataRow row in questions.Rows)
            {
                resultText += $"{row[0]}    {row[1]}   {row[2]}\n";
            }
            
            textElement.text = resultText + "End";
        }
        catch (MySqlException ex)
        {
            Debug.LogError($"Database error: {ex.Message}");
            textElement.text = "Database connection failed";
        }
        finally
        {
            if (connection != null)
                connection.Close();
        }
    }
}