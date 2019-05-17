using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    private List<string> levelData;

    private void Awake()
    {
        EventManager.StartListening("victory", delegate { UpdateLevelInfo(); });
        EventManager.StartListening("reload", delegate { Reload(); });

        if (File.Exists(Application.persistentDataPath + "/levelInfo.dat"))
        {
            Load();
        }
        else
        {
            levelData = new List<string>();
            levelData.Add("1");
        }
        foreach (GameObject levelButton in GameObject.FindGameObjectsWithTag("LevelButton"))
        {
            if (levelData.BinarySearch("" + levelButton.name[0]) >= 0)
            {
                levelButton.GetComponent<Selectable>().interactable = true;
            }
        }
        
    }

    private void UpdateLevelInfo()
    {
        
        int completedLvlIndex = SceneManager.GetActiveScene().buildIndex;
        if (levelData.BinarySearch((completedLvlIndex + 1).ToString()) < 0)
        {
            levelData.Add((completedLvlIndex + 1).ToString());
            Debug.Log("upd lvl data" + levelData[1] + levelData[0]);
            Save();
        }
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);

    }
    public void Reload()
    {
        Scene curScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(curScene.name);
    }
    public void PlayLevel(string level_number)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + int.Parse(level_number));
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Create(Application.persistentDataPath + "/levelInfo.dat");

        LevelData data = new LevelData();
        data.data = levelData;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {


        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/levelInfo.dat", FileMode.Open);
        LevelData data = bf.Deserialize(file) as LevelData;

        levelData = data.data;
        file.Close();

        Debug.Log("levels:" + levelData[1] + levelData[0]);
    }
}

[Serializable]
class LevelData
{
    public List<string> data;
}