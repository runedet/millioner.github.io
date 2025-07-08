using UnityEngine;
using UnityEngine.SceneManagement;

public class pause1 : MonoBehaviour
{
    public bool PauseGame;
    public GameObject pauseGameMenu;

    void Start()
    {
        PauseGame = false;
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    void Update()
    {
    }


    public void Resume()
    {
        pauseGameMenu.SetActive(false);
        Time.timeScale = 1f;
        PauseGame = false;
    }

    public void Pause1()
    {
        pauseGameMenu.SetActive(true);
        Time.timeScale = 0f;
        PauseGame = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}