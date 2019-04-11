using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MoveArrowTrigger : MonoBehaviour, IPointerClickHandler
{
    //Background detects click on it, than triggers change arrow pos event
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log("Clicked");
        EventManager.TriggerEvent("changearrowpos");
    }
}
