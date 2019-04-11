using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour
{
    private Dictionary<string, UnityEvent> eventDictionary;
    private static EventManager eventManager;

    //try finding existing eventmanager, if there's none existing, create it
    private static EventManager instance {
        get
        {
            if (eventManager == null)
            {
                eventManager = FindObjectOfType(typeof(EventManager)) as EventManager;

                if (eventManager == null)
                    Debug.Log("ERR eventManager not found\n");
                else
                {
                    eventManager.Init();
                }

            }
            return eventManager;
        }
    }
    //initialize event dictionary
    void Init() {
        if (eventDictionary == null)
            eventDictionary = new Dictionary<string, UnityEvent>();
    }
    //subscribe for an event
    public static void StartListening(string eventName, UnityAction listener) {
        UnityEvent thisEvent = null;

        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.AddListener(listener);
        else {
            thisEvent = new UnityEvent();
            thisEvent.AddListener(listener);
            instance.eventDictionary.Add(eventName ,thisEvent);
        }
    }
    //unsub from an event
    public static void StopListening(string eventName, UnityAction listener) {
        if (eventManager == null) return;

        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent))
            thisEvent.RemoveListener(listener);
    }
    public static void TriggerEvent(string eventName) {
        UnityEvent thisEvent = null;
        if (instance.eventDictionary.TryGetValue(eventName, out thisEvent)) {
            thisEvent.Invoke();
        }
    }
}
