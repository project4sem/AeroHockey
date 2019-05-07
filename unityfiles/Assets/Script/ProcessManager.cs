using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Diagnostics;
using UnityEngine;


public class ProcessManager : MonoBehaviour
{
    private int i;
    public string[] data;
    private StreamWriter messageStream;
    private Process process;

    // Start is called before the first frame update
    void Start()
    {
        i = 0;
        data = new string[100];
        StartPhysics();
    }

    private void StartPhysics() {
        try
        {
            process = new Process();
            process.EnableRaisingEvents = false;
            process.StartInfo.FileName = "D:\\Unity\\github\\AeroHockey\\physics\\testproc.exe";
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.RedirectStandardError = true;
            process.OutputDataReceived += new DataReceivedEventHandler(DataReceived);
            process.ErrorDataReceived += new DataReceivedEventHandler(ErrorReceived);
            process.Start();
            process.BeginOutputReadLine();
            messageStream = process.StandardInput;

            UnityEngine.Debug.Log("Successfully launched app");
        }
        catch (Exception e)
        {
            UnityEngine.Debug.LogError("Unable to launch app: " + e.Message);
        }
    }

    void DataReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        data[i] = eventArgs.Data;
        i++;
    }


    void ErrorReceived(object sender, DataReceivedEventArgs eventArgs)
    {
        UnityEngine.Debug.LogError(eventArgs.Data);
    }


    void OnApplicationQuit()
    {
        if (process != null)
        {
            process.Kill();
        }
    }

}
