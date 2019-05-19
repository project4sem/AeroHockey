using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameObject[] pauseObjects;
    public InputField rotSpeed_IF;
    public InputField mu_IF;

    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        HidePaused();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 1)
            {
                Time.timeScale = 0;
                ShowPaused();
            }
            else if (Time.timeScale == 0)
            {
                Time.timeScale = 1;
                HidePaused();
            }
        }

        double mu, rotSpeed;

        if (double.TryParse(mu_IF.text.Replace('.', ','), out mu))
        {
            if (mu > 0.8)
                mu_IF.text = "0.8";
            if (mu < 0)
                mu_IF.text = "0";
        }
        if (double.TryParse(rotSpeed_IF.text.Replace('.', ','), out rotSpeed))
        {
            if (rotSpeed > 100)
                rotSpeed_IF.text = "100";
            if (rotSpeed < -100)
                rotSpeed_IF.text = "-100";
        }
    }


    //Reloads the Level
    public void Reload()
    {
        EventManager.TriggerEvent("reload");
    }

    //controls the pausing of the scene
    public void PauseControl()
    {
        if (Time.timeScale == 1)
        {
            Time.timeScale = 0;
            ShowPaused();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            HidePaused();
        }
    }

    //shows objects with ShowOnPause tag
    public void ShowPaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void HidePaused()
    {
        foreach (GameObject g in pauseObjects)
        {
            g.SetActive(false);
        }
    }

    //loads inputted level
    public void LoadLevel(string level)
    {
        SceneManager.LoadScene(level);
    }
}
