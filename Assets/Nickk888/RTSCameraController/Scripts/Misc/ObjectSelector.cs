using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    Camera mainCam;
    public RTSCameraTargetController cameraTargetController;
    public Transform currentSelected;

    void Start()
    {
        mainCam = Camera.main;
    }

    void Update()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (currentSelected != hit.transform)
            {
                if (currentSelected != null && currentSelected != hit.transform)
                {
                    currentSelected.GetComponent<Outline>().enabled = false;
                    currentSelected = null;
                }
            }
            if (hit.transform.CompareTag($"Cubes"))
            {
                if (currentSelected != hit.transform)
                {
                    currentSelected = hit.transform;
                    currentSelected.GetComponent<Outline>().enabled = true;
                }
                if (Input.GetMouseButtonDown(0))
                {
                    if(hit.transform.gameObject.GetComponent<MoveToRandomPosition>() == null)
                    {
                        cameraTargetController.LockOnTarget(hit.transform.position, 10);
                    }
                    else
                    {
                        cameraTargetController.LockOnTarget(hit.transform, 20, true);
                    }
                }
            }
        }

    }


}
