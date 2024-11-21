using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class ObjectSelector : MonoBehaviour
{
    private Camera mainCam;
    private bool mouseButtonPressed;
    private Vector3 mousePosition;

    private void Start()
    {
        mainCam = Camera.main;
    }

    private void Update()
    {
        if (RTSCameraTargetController.Instance == null)
            return;

        UpdateInput();

        if (mouseButtonPressed)
        {
            TrySelectObject();
        }
    }

    /// <summary>
    /// Updates the input state (mouse button press and mouse position).
    /// </summary>
    private void UpdateInput()
    {
        #if ENABLE_INPUT_SYSTEM
        mouseButtonPressed = Mouse.current.leftButton.wasPressedThisFrame;
        mousePosition = Mouse.current.position.ReadValue();
        #else
        mouseButtonPressed = Input.GetMouseButtonDown(0);
        mousePosition = Input.mousePosition;
        #endif
    }

    /// <summary>
    /// Attempts to select a selectable object based on the mouse click.
    /// </summary>
    private void TrySelectObject()
    {
        Ray ray = mainCam.ScreenPointToRay(mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.transform.CompareTag("Selectable"))
            {
                RTSCameraTargetController.Instance.LockOnTarget(hit.transform, 20, true);
            }
        }
    }
}
