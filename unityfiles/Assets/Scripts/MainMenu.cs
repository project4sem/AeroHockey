using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenu : MonoBehaviour
{
    public void PlayGame() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void PlayLevel(string level_number)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + int.Parse(level_number));
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    

}
