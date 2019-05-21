using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    GameObject[] pauseObjects;
    GameObject[] controlObjects;
    public InputField rotSpeed_IF;
    public InputField mu_IF;

    void Start()
    {
        Time.timeScale = 1;
        pauseObjects = GameObject.FindGameObjectsWithTag("ShowOnPause");
        
        HidePaused();
        controlObjects = GameObject.FindGameObjectsWithTag("SetInactiveOnPause");
        
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseControl();
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
            HideControl();
        }
        else if (Time.timeScale == 0)
        {
            Time.timeScale = 1;
            HidePaused();
            ShowControl();
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
    public void ShowControl()
    {
        foreach (GameObject g in controlObjects)
        {
            g.SetActive(true);
        }
    }

    //hides objects with ShowOnPause tag
    public void HideControl()
    {
        foreach (GameObject g in controlObjects)
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
