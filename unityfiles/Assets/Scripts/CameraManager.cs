using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public Camera mainCam;
    public Camera mapOverviewCam;

    private Camera current;

    private void Start()
    {
        current = mainCam;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (current == mainCam)
            {
                ShowMap();
                current = mapOverviewCam;
            }
            else if (!(current == mainCam))
            {
                HideMap();
                current = mainCam;
            }
        }
    }
    private void ShowMap()
    {
        mainCam.gameObject.SetActive(false);
        mapOverviewCam.gameObject.SetActive(true);
    }
    private void HideMap()
    {
        mainCam.gameObject.SetActive(true);
        mapOverviewCam.gameObject.SetActive(false);
    }
}
