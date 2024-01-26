using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    Camera mainCam;
    public RTSCameraTargetController cameraTargetController;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.CompareTag($"Selectable"))
            {
                if (Input.GetMouseButtonDown(0))
                {
                    cameraTargetController.LockOnTarget(hit.transform, 20, true);
                }
            }
        }

    }


}
