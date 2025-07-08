using UnityEngine;

public class ObjectSwitcher1 : MonoBehaviour
{
    public GameObject object2;
    

    void Start()
    {
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {  
            object2.SetActive(false);   
            
        }
    }
}