using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSelector : MonoBehaviour
{
    Camera mainCam;
    public RTSCameraTargetController cameraTargetController;
    public Transform currentSelected;

    // Start is called before the first frame update
    void Start()
    {
        mainCam = Camera.main;
    }

    // Update is called once per frame
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
                if (hit.transform.tag == "Cubes")
                {
                    if (currentSelected != hit.transform)
                    {
                        currentSelected = hit.transform;
                        currentSelected.GetComponent<Outline>().enabled = true;
                    }
                    if (Input.GetMouseButtonDown(0))
                    {

                        cameraTargetController.LockOnTarget(hit.transform.position, 10);
                    }
                }
            }

    }


}
