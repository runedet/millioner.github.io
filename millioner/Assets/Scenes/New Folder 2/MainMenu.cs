using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public GameObject objectToHide;   // The object to hide
    public GameObject objectToShow1;  // The first object to show
    public GameObject objectToShow2;
    public GameObject objectToShow3;
    public GameObject objectToShow4;  // The second object to show

    void Start()
    {
        // Make the two objects invisible at the start
        objectToShow1.SetActive(false);
        objectToShow2.SetActive(false);
        objectToShow3.SetActive(false);
        objectToShow4.SetActive(false);
    }

    public void SettingsOpen2()
    {     // Show the first object
        objectToShow2.SetActive(false); 
        objectToShow3.SetActive(true);
        objectToShow4.SetActive(false);
    }
    public void SettingsOpen3()
    {     // Show the first object
        objectToShow2.SetActive(false); 
        objectToShow3.SetActive(false);
        objectToShow4.SetActive(true);
    }
    public void SettingsOpen4()
    {     // Show the first object
        objectToShow2.SetActive(true); 
        objectToShow3.SetActive(false);
        objectToShow4.SetActive(false);
    }

    public void SettingsOpen()
    {
        objectToHide.SetActive(false);   // Hide the specified object
        objectToShow1.SetActive(true);    // Show the first object
        objectToShow2.SetActive(true);    // Show the second object
    }

    public void SettingsClose()
    {
        objectToHide.SetActive(true);     // Show the hidden object again
        objectToShow1.SetActive(false);    // Hide the first object
        objectToShow2.SetActive(false);
        objectToShow3.SetActive(false);    // Hide the first object
        objectToShow4.SetActive(false);    // Hide the second object
    }

    public void ExitGame()
    {
        Debug.Log("игра закрылась");
        Application.Quit();
    }
}