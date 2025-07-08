using UnityEngine;
using System.IO;
using UnityEngine.UI;

public class ObjectSwitcher : MonoBehaviour
{
    public GameObject object1; // меню
    public GameObject object2; // настройки
    public GameObject object3; // окно подтверждения
    public GameObject object4;
    public GameObject object5;
    public GameObject object6;
    
    public Button button;
    private string filePath;

    void Start()
    {
        filePath = Path.Combine(Application.dataPath, "birkin.txt");
        object6.SetActive(false);
        object1.SetActive(true);
        object3.SetActive(false);
    }

    void Update()
    {
        button.gameObject.SetActive(false);
        if (Input.GetKeyDown(KeyCode.Escape))
        {
                string fileContent = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    if (object2.activeSelf){
                        
                    object3.SetActive(true);
                    }
                    
                }
                
                if (string.IsNullOrWhiteSpace(fileContent)&&object6.activeSelf==false){
                        SwitchToFirstObject();

                }
                if(object6.activeSelf){
                      object6.SetActive(false);
                }
                
            
        }
    }

    public void SwitchToFirstObject()
    {
        button.gameObject.SetActive(true);
        object1.SetActive(true);
        object2.SetActive(false);
        object3.SetActive(false);
        object4.SetActive(false);
        object5.SetActive(false);
        object6.SetActive(false);
    }
    public void SwitchToFirstObject2()
    {
        object1.SetActive(false);
        object2.SetActive(true);
    }
    public void SwitchToFirstObject333()
    {
        object6.SetActive(false);
    }
    public void SwitchToFirstObject3()
    {
        object3.SetActive(false);
    }
    public void SwitchToFirstObject33()
    {
        object3.SetActive(true);
    }

    public void SwitchToFirstObject1()
    {
       string fileContent = File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(fileContent))
                {
                    if (object2.activeSelf){
                        
                    object3.SetActive(true);
                    }
                    
                }
                if (string.IsNullOrWhiteSpace(fileContent))
                    {
                        SwitchToFirstObject();

                    }
    }
}