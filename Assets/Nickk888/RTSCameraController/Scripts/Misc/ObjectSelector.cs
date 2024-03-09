using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ObjectSelector : MonoBehaviour
{
    [SerializeField] private RTSCameraTargetController cameraTargetController;
    
    private Camera mainCam;
    private bool mouseButtonPressed = false;
    private Vector3 mousePosition;

    private void Start()
    {
        mainCam = Camera.main;
        if (cameraTargetController is null)
            Debug.LogError("[ObjectSelector] Reference to Camera Target Controller not found. Please reference the RTSCameraTargetController script.", this);
    }

    private void Update()
    {
        if (cameraTargetController is null)
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
                    cameraTargetController.LockOnTarget(hit.transform, 20, true);
                }
            }
        }

    }


}
