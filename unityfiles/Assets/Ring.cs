using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Ring : MonoBehaviour
{
    private static float Pi = 3.14159f;

    public float segmentRadius;
    public float tubeRadius;
    public int segments;
    public int tubes;

    // Start is called before the first frame update
    void Start()
    {
        //Creating torus(or ring if r << R)
        Torus();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Torus()
    {        int totalVertices = segments * tubes;
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
                tubeList.Add(new Vector3(x, z, y));

                // Add the vertex to global vertex list
                verticesList.Add(new Vector3(x, z, y));
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

        Mesh mesh = new Mesh();

        Vector3[] vertices = new Vector3[totalVertices];
        verticesList.CopyTo(vertices);
        int[] triangles = new int[totalIndices];
        indicesList.CopyTo(triangles);
        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();
        ;
        MeshFilter mFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
        mFilter.mesh = mesh;

    }
}
