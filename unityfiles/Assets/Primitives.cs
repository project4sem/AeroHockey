using System;
using System.IO;
using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshFilter),typeof(MeshRenderer))]
public class Primitives : MonoBehaviour {
    private RaycastHit hit;

    private static float Pi = 3.14159f;

    public float rotSpeed;
    public float speedX, speedY, speedZ;
    private Vector3 position;
    private Vector3 startingPosition;

    public float segmentRadius;
	public float tubeRadius;
	public int segments;
	public int tubes;

    public delegate void mydel();
    public mydel myevent;

    Camera cam;

void Start() {
        //Set torus position depending on tube radius
        transform.position = new Vector3(0, tubeRadius , 0);
        startingPosition = transform.position;
        Torus();
        
        MeshCollider mc = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;
        Rigidbody r = gameObject.AddComponent(typeof(Rigidbody)) as Rigidbody;

        r.useGravity = false;
        mc.convex = true;

        cam = GetComponentInChildren(typeof(Camera)) as Camera;
        


    }
    void Update() {
        transform.Rotate(rotSpeed * Time.deltaTime, 0, 0);
        Move();


        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Debug.DrawRay(ray.origin, ray.direction * 10, Color.yellow);
        Vector3 vec_ray = ray.direction - ray.origin;
        if (Input.GetMouseButtonDown(0)) {
            if (Physics.Raycast(ray, out hit)) {
                Debug.Log("MD hit " + hit.collider.name);
            }
        }
    }

    private void Torus() {
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
            for (int i = 0; i < segments; i++) {
                tubeList = new ArrayList();

            	for (int j = 0; j < tubes; j++) {
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
            for (int i = 0; i < segmentList.Count; i++)  {
                // Find next (or first) segment offset
                int n = (i + 1) % segmentList.Count;

                // Find current and next segments
                currentTube = (ArrayList)segmentList[i];
                nextTube = (ArrayList)segmentList[n];

                // Loop through the vertices in the tube
                for (int j = 0; j < currentTube.Count; j++) {
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
            
            MeshFilter mFilter = GetComponent(typeof(MeshFilter)) as MeshFilter;
            mFilter.mesh = mesh;
            
        }
    /* private void ReadDat() {
         StreamReader b = File.OpenText("testd.txt");
         string s = b.ReadLine();

         Debug.Log("" + s);
     }*/

    public void ButtonHandler() {
        speedX = 0;
        speedY = 1;
    }
    private void Move() {
        Debug.Log("move_init");
        Vector3 speed = new Vector3(speedX, speedY, speedZ);

        transform.position = transform.position + speed*Time.deltaTime;
    }
    void OnCollisionEnter(Collision collision)
    {
        Debug.Log("We hit " + collision.gameObject);
    }

    private static void Del() {
        Debug.Log("Del entered");   
    
    }


}
