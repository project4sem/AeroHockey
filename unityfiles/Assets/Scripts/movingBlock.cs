using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class movingBlock : MonoBehaviour
{
    public float vx;
    public float vy;

    private void Start()
    {
        EventManager.StartListening("child hit" + gameObject.name, delegate { OnChildHit(); });
    }

    private void Update()
    {
        gameObject.transform.localPosition += new Vector3(vx * Time.deltaTime, vy * Time.deltaTime, 0);
    }

    private void OnChildHit()
    {
        //Collider myCollider = collision.contacts[0].thisCollider;
        
         vx = -vx;
    }
}
