using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public delegate void ClickAction();
    public static event ClickAction OnClicked;

    public delegate void MouseClickAction();
    public static event MouseClickAction MClicked;


    private void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 150, 100), "Start"))
        {
            if (OnClicked != null)
                OnClicked();
        }

        if (Input.GetKeyDown("mouse 0")) {
            if (MClicked != null)
                MClicked();
        }
            
    }
}
