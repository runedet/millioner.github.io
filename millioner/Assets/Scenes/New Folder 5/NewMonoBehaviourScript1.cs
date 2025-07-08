using UnityEngine;
using UnityEngine.SceneManagement;

public class NewMonoBehaviourScript200 : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
    }
}