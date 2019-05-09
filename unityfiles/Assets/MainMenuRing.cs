using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MainMenuRing : MonoBehaviour
{
    public float speedX;
    public float speedY;
    private float time;
    

    private static float Pi = 3.1415926535897931f;

    public float segmentRadius;
    public float tubeRadius;
    public int segments;
    public int tubes;

    // Start is called before the first frame update
    void Start()
    {
        Torus();
        CircleCollider2D mc = gameObject.AddComponent<CircleCollider2D>() as CircleCollider2D;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name == "Background")
            return;
        EventManager.TriggerEvent("changeorigin" + gameObject.transform.parent.name);
        time = 0f;
        if (collision.collider.transform.eulerAngles.z == 90)
            speedY = -speedY;
        else
            speedX = -speedX;
    }

    void Update()
    {
        gameObject.transform.localPosition = new Vector3(speedX * time, speedY * time, 0);
        time += Time.deltaTime;
    }

    
    private void Torus()
    {
        int totalVertices = segments * tubes;
        int totalPrimitives = totalVertices * 2;
        int totalIndices = totalPrimitives * 3;

        // Initialise vertexList and indexList
        ArrayList verticesList = new ArrayList();
        ArrayList indicesList = new ArrayList();

        // Calculate size of segment and tube
        float segmentSize = 2 * Pi / segments;
        float tubeSize = 2 * Pi / tubes;

        // Create floats for our xyz coordinates
        float x = 0;
        float y = 0;
        float z = 0;

        // Initialise temp lists with tubes and segments
        ArrayList segmentList = new ArrayList();
        ArrayList tubeList;

        // Loop through tubes
        for (int i = 0; i < segments; i++)
        {
            tubeList = new ArrayList();

            for (int j = 0; j < tubes; j++)
            {
                // Calculate X, Y, Z coordinates.
                x = (segmentRadius + tubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Cos(i * segmentSize);
                y = (segmentRadius + tubeRadius * Mathf.Cos(j * tubeSize)) * Mathf.Sin(i * segmentSize);
                z = tubeRadius * Mathf.Sin(j * tubeSize);

                // Add the vertex to the tubeList
                tubeList.Add(new Vector3(x, y, z));

                // Add the vertex to global vertex list
                verticesList.Add(new Vector3(x, y, z));
            }

            // Add the filled tubeList to the segmentList
            segmentList.Add(tubeList);
        }

        ArrayList currentTube;
        ArrayList nextTube;
        Vector3 v1;
        Vector3 v2;
        Vector3 v3;
        Vector3 v4;

        // Loop through the segments
        for (int i = 0; i < segmentList.Count; i++)
        {
            // Find next (or first) segment offset
            int n = (i + 1) % segmentList.Count;

            // Find current and next segments
            currentTube = (ArrayList)segmentList[i];
            nextTube = (ArrayList)segmentList[n];

            // Loop through the vertices in the tube
            for (int j = 0; j < currentTube.Count; j++)
            {
                // Find next (or first) vertex offset
                int m = (j + 1) % currentTube.Count;

                // Find the 4 vertices that make up a quad
                v1 = (Vector3)currentTube[j];
                v2 = (Vector3)currentTube[m];
                v3 = (Vector3)nextTube[m];
                v4 = (Vector3)nextTube[j];

                // Draw the first triangle
                indicesList.Add((int)verticesList.IndexOf(v1));
                indicesList.Add((int)verticesList.IndexOf(v2));
                indicesList.Add((int)verticesList.IndexOf(v3));

                // Finish the quad
                indicesList.Add((int)verticesList.IndexOf(v3));
                indicesList.Add((int)verticesList.IndexOf(v4));
                indicesList.Add((int)verticesList.IndexOf(v1));
            }
        }
        //Set mesh
        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[totalVertices];
        verticesList.CopyTo(vertices);
        int[] triangles = new int[totalIndices];
        indicesList.CopyTo(triangles);
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        MeshFilter mFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        mFilter.mesh = mesh;

    }

}
