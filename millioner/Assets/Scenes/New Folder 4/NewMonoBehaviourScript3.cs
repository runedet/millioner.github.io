using UnityEngine;

[DefaultExecutionOrder(1000)]
public class ObjectSwitcher100 : MonoBehaviour
{
    public GameObject object2;
    public GameObject object3;

    void LateUpdate()  // Используем LateUpdate вместо Update
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {  
            object2.SetActive(false);
            object3.SetActive(true);
        }
    }
}