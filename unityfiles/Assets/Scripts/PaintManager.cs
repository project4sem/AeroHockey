using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PaintManager : MonoBehaviour
{
    public Material failMaterial;
    public Material colliderMaterial;
    public Material boosterMaterial;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject failObject in GameObject.FindGameObjectsWithTag("Fail Object"))
        {
            failObject.GetComponent<Renderer>().material = failMaterial;
        }
        foreach (GameObject collisionObject in GameObject.FindGameObjectsWithTag("Collision Object"))
        {
            collisionObject.GetComponent<Renderer>().material = colliderMaterial;
        }
        foreach (GameObject collisionObject in GameObject.FindGameObjectsWithTag("Booster"))
        {
            collisionObject.GetComponent<Renderer>().material = boosterMaterial;
        }
    }


}
