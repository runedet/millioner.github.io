using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    
    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }
    public void SettingsOpen()
{
    SceneManager.LoadScene(1);
}
public void SettingsClose()
{
    SceneManager.LoadScene(0);
}
public void igray()
{
    SceneManager.LoadScene(3);
}
public void igray1()
{
    SceneManager.LoadScene(4);
}
    public void ExitGame()
    {
        Debug.Log("игра закрылась");
        Application.Quit();
    }
    
}
