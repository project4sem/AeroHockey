using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingWall : MonoBehaviour
{
    private void Start()
    {
        gameObject.GetComponent<Rigidbody2D>().constraints = RigidbodyConstraints2D.FreezeAll;
    }

    public float vx;
    public float vy;

    

    private void Update()
    {
        gameObject.transform.localPosition += new Vector3(vx * Time.deltaTime, vy * Time.deltaTime, 0);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log(collision.collider);
        if (collision.collider.name != "Table" && collision.collider.name != "Ring")
        {
            vx = -vx;
            vy = -vy;
        }
    }
    
}
