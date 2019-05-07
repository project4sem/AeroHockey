using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Arrow : MonoBehaviour
{
    private float length = 1.5f;
    public float lengthX;
    public float lengthY;
    private float start_length;
    private float scale;

    private Camera cam;
    Vector3 defaultPos;
    private  Quaternion defaultQ;
    private float PI = 3.1415926535897931f;

    // Start is called before the first frame update
    void Start()
    {
        start_length = length;
        lengthX = 0;
        lengthY = length;
        CreateArrow();
        cam = Camera.main;
        //Define basic quaternion position of the arrow
        defaultQ = gameObject.transform.rotation;
        //Define basic vector position of the arrow
        defaultPos = new Vector3(0, length + length / 5, 0);
    }

    private void OnEnable()
    {
        EventManager.StartListening("changearrowpos", delegate { ChangeArrowPos(); });
    }
    private void OnDisable()
    {
        EventManager.StopListening("changearrowpos", delegate { ChangeArrowPos(); });
    }
    public void RemoveArrow() {
        Debug.Log("ArrowDestroyed1");
        Destroy(this.gameObject);
    }
    void ChangeArrowPos() {
        Event curEvent = Event.current;
        Vector3 point = new Vector3();
        Vector2 mousePos = new Vector2();
        //Define mouse position relative to UI in pixels
        mousePos.x = Input.mousePosition.x;
        mousePos.y = Input.mousePosition.y;
        //Define vector connecting camera and mouse position
        point = cam.ScreenToWorldPoint(new Vector3(mousePos.x, mousePos.y, cam.nearClipPlane));
        //Set z coordinate of vector to 0 because we are in OXY plane,
        point.z = 0;

        Debug.Log(point);
        Debug.Log(mousePos + "," + cam.nearClipPlane);

        scale = Vector3.Distance(Vector3.zero, point) / start_length;
        float angle = Vector3.Angle(point, defaultPos);
        //Vecor3.Angle return abs(angle), so if angle is negative changes are required,
        if (point.x > 0) {
            angle = -angle;
        }

        lengthY = point.y;
        lengthX = point.x;
        //Set rotation quaternion around Oz and change the rotation angle of the object
        Quaternion rotQ = new Quaternion(0, 0,
                                    (float)Math.Sin(PI * (angle/180)/2),
                                    (float)Math.Cos(PI * (angle / 180) / 2));
        gameObject.transform.rotation =  defaultQ * rotQ;
        gameObject.transform.localScale = new Vector3(1, scale);
        length = Vector3.Distance(Vector3.zero, point);
    }
    private void CreateArrow() {

        float width;

        width = (GameObject.Find("Ring").GetComponent<Ring>().segmentRadius)/10;

        //Set vertices
        Vector3[] vertices = new Vector3[7];

        vertices[0] = new Vector3(-width, 0, 0);
        vertices[1] = new Vector3(width, 0, 0);

        vertices[2] = new Vector3(-width, length, 0);
        vertices[3] = new Vector3(width, length, 0);

        vertices[4] = new Vector3(-2*width, length, 0);
        vertices[5] = new Vector3(2*width, length, 0);

        vertices[6] = new Vector3(0, length + length/5, 0);

        //Draw triangles
        int[] triangles = new int[] {2, 1 ,0, 2 ,3, 1, 6, 5 , 4};

        //Set mesh
        Mesh mesh = new Mesh();
        MeshFilter mFilter = GetComponent<MeshFilter>() as MeshFilter;

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();

        mFilter.mesh = mesh;
    }
    
}
