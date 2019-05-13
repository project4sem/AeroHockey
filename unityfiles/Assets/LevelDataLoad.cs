using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using UnityEngine.UI;

public class LevelDataLoad : MonoBehaviour
{
    private List<string> levelData;

    private void Awake()
    {
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
                break;
            }
        }
        Save();
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
    }
}

[Serializable]
class LevelData
{
    public List<string> data;
}