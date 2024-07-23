using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ObjectSelector : MonoBehaviour
{    
    private Camera mainCam;
    private bool mouseButtonPressed = false;
    private Vector3 mousePosition;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (RTSCameraTargetController.Instance == null)
            return;
        
        mouseButtonPressed = false;
        mousePosition = Vector3.zero;
        
        #if ENABLE_INPUT_SYSTEM
        mouseButtonPressed = Mouse.current.leftButton.wasPressedThisFrame;
        mousePosition = Mouse.current.position.value;
        #else
        mouseButtonPressed = Input.GetMouseButtonDown(0);
        mousePosition = Input.mousePosition;
        #endif

        if(mouseButtonPressed)
        {
            Ray ray = mainCam.ScreenPointToRay(mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if (hit.transform.CompareTag($"Selectable"))
                {
                    RTSCameraTargetController.Instance.LockOnTarget(hit.transform, 20, true);
                }
            }
        }

    }


}
