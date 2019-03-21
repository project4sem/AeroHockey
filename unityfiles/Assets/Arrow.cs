using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float x;
    private readonly float y = 0f;
    public float z;

    public float size = 2;
    public float startpos;
    private float endpos;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateArrow() {
        Vector3[] vertices = new Vector3[13];

        vertices[0] = new  Vector3(0.25f, -0.25f, 1f); 
        vertices[1] = new  Vector3(-0.25f, -0.25f, 1f); 
        vertices[2] = new  Vector3(0.25f, 0.25f, 1f); 
        vertices[3] = new  Vector3(-0.25f, 0.25f, 1f);

        vertices[0] = new  Vector3(0.25f, -0.25f, size + 1f); 
        vertices[1] = new  Vector3(-0.25f, -0.25f, size + 1f); 
        vertices[2] = new  Vector3(0.25f, 0.25f, size + 1f); 
        vertices[3] = new  Vector3(-0.25f, 0.25f, size + 1f); 

    }
}
