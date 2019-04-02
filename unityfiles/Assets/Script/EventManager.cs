using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventManager : MonoBehaviour, IPointerClickHandler
{

    public delegate void MouseClickAction();
    public static event MouseClickAction MClicked;

    public void OnPointerClick(PointerEventData eventData) {
        Debug.Log("Clicked");
        if(MClicked != null)
            MClicked();
    }
    
}
