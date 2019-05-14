using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Origin : MonoBehaviour
{
    private float time;
    private float speedX;

    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening("changeorigin" + gameObject.name, delegate { ChangePos(); });
    }

    private void ChangePos()
    {
        Transform ring = gameObject.transform.GetChild(0);
        transform.position = new Vector3(ring.position.x, ring.position.y, transform.position.z);
        ring.position = Vector3.zero;
        
    }
}
