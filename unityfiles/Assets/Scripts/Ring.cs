using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;
using UnityEngine.SceneManagement;
//using movecalcVS;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Ring : MonoBehaviour
{
    private Ret_all move_data;

    private float speedX;
    private float speedY;
    private double RotSpeed;
    private float time;

    private static float pi = 3.1415926535897931f;

    private bool launched;
    public float segmentRadius;
    public float tubeRadius;
    public int segments;
    public int tubes;

   
    [DllImport("movecalcVS",EntryPoint = "movecalc")]
    private static extern Ret_all Movecalc( double _factor_v,  double _factor_w,  double _dt,  double _r,   Coord _coord);

    

    // Start is called before the first frame update
    void Start()
    {
        Torus();
        CircleCollider2D mc = gameObject.AddComponent<CircleCollider2D>() as CircleCollider2D;

        Rigidbody2D rb = gameObject.AddComponent<Rigidbody2D>();
        rb.isKinematic = false;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        launched = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.tag == "Background")
            return;
        if (collision.collider.tag == "Victory target")
            Victory();
        if (collision.collider.tag == "Fail Object")
            Defeat();
        EventManager.TriggerEvent("changeorigin" + gameObject.transform.parent.name);
        Debug.Log("collided " + collision.collider);
        time = 0f;
        speedX = -speedX;
    }

    void Update()
    {
        if (launched)
        {
                gameObject.transform.localPosition = new Vector3(move_data.GetResX(time),
                                                                    move_data.GetResY(time),
                                                                   0);
           // gameObject.transform.localPosition = new Vector3(speedX * time, speedY * time, 0);
            time += Time.deltaTime;
            move_data = Movecalc(1, 1, Time.deltaTime, 1, move_data.Coord);
        }
    }
 
    public void StartMoving()
    {
        time = 0;
        launched = true;
        //change speed (will be changed by arrow position)
        speedX = 3 * (GameObject.Find("Arrow").GetComponent<Arrow>().GetLengthX());
        speedY = 3 * (GameObject.Find("Arrow").GetComponent<Arrow>().GetLengthY());

        //Change starting rotation

        //Find the input field gameobject
        GameObject IF = GameObject.Find("InputField");
        //Find COMPONENT of an input field gameobject
        InputField IF_inputfield = IF.GetComponent<InputField>();
        Debug.Log(IF_inputfield.text);
        if (IF_inputfield.text == "")
            RotSpeed = 0;
        else
            RotSpeed = double.Parse(IF_inputfield.text);

        //Pass everything to structures and then to dll function
        move_data = new Ret_all(0, 0, speedX, speedY, RotSpeed);
        move_data = Movecalc(1, 1, Time.deltaTime, 1, move_data.Coord);
        
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
        float segmentSize = 2 * pi / segments;
        float tubeSize = 2 * pi / tubes;

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
    private void Victory()
    {
        EventManager.TriggerEvent("victory");
    }
    private void Defeat()
    {
        EventManager.TriggerEvent("reload");
    }
}

[StructLayout(LayoutKind.Sequential)]
struct Coord
{
   public double x, y;
   public double vx, vy, w;

    public Coord(double x, double y, double vx, double vy, double w)
    {
        this.x = x;
        this.y = y;
        this.vx = vx;
        this.vy = vy;
        this.w = w;
    }
};
[StructLayout(LayoutKind.Sequential)]
struct Ret_factors
{
    public double cx0, cx1, cx2;
    public double cy0, cy1, cy2;
};
[StructLayout(LayoutKind.Sequential)]
struct Ret_all
{
    public Coord Coord;
    public Ret_factors factors;

    public Ret_all(double x, double y, double vx, double vy, double w)
    {
        Coord = new Coord(x, y, vx, vy, w);
        factors = new Ret_factors();
    }
    public float GetResX(float time)
    {
        return (float) factors.cx0 + (float) factors.cx1 * time + (float) factors.cx2 * time * time;
    }
    public float GetResY(float time)
    {
        return (float) factors.cy0 + (float) factors.cy1 * time + (float) factors.cy2 * time * time;
    }
};