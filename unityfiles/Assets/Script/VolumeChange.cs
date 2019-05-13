using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VolumeChange : MonoBehaviour
{

    public Slider musicSlider;
    public Slider sfxSlider;
    public Slider masterSlider;
    public AudioSource myMusic;

    private float music;
    private float sfx;
    private float master;

    void Awake()
    {
        if (File.Exists(Application.persistentDataPath + "/volumeInfo.dat"))
        {
            Load();
        }
        else
        {
            music = musicSlider.value;
            sfx = sfxSlider.value;
            master = masterSlider.value;
        }

        foreach (GameObject SFXsrc in GameObject.FindGameObjectsWithTag("SFX player"))
        {
            SFXsrc.GetComponent<AudioSource>().volume = sfx;
        }
        foreach (GameObject musicsrc in GameObject.FindGameObjectsWithTag("MusicPlayer"))
        {
            musicsrc.GetComponent<AudioSource>().volume = music;
        }
        AudioListener.volume = master;
    }
    public void SFXupdate()
    {
        sfx = sfxSlider.value;

        foreach (GameObject SFXsrc in GameObject.FindGameObjectsWithTag("SFX player"))
        {
            SFXsrc.GetComponent<AudioSource>().volume = sfx;
        }
    }
    public void Musicupdate()
    {
        music = musicSlider.value;

        foreach (GameObject musicsrc in GameObject.FindGameObjectsWithTag("MusicPlayer"))
        {
            musicsrc.GetComponent<AudioSource>().volume = music;
        }
    }
    public void Masterupdate()
    {
        master = masterSlider.value;
        AudioListener.volume = master;
    }

    public void Save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        
        file = File.Open(Application.persistentDataPath + "/volumeInfo.dat", FileMode.Create);
        

        VolumeData data = new VolumeData();
        data.master = master;
        data.music = music;
        data.sfx = sfx;

        bf.Serialize(file, data);
        file.Close();
    }

    public void Load()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file = File.Open(Application.persistentDataPath + "/volumeInfo.dat", FileMode.Open);
        VolumeData data = bf.Deserialize(file) as VolumeData;

        sfx = data.sfx;
        music = data.music;
        master = data.master;

        musicSlider.value = music;
        sfxSlider.value = sfx;
        masterSlider.value = master;

        file.Close();
    }
}


[Serializable]
class VolumeData
{
    public float sfx;
    public float music;
    public float master;
}