using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class RTSCameraTargetController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private Transform cameraTarget;

    [Space]
    [Header("Camera")]
    [SerializeField]
    private float cameraTilt = 50.0f;

    [SerializeField]
    private float cameraMouseSpeed = 2.0f;

    [SerializeField]
    private float cameraKeysSpeed = 20.0f;

    [SerializeField]
    private float cameraZoomSpeed = 4f;

    [SerializeField]
    private float cameraMoveDeadZone = 5f;

    
    [Space] [Header("Camera Zoom")]
    [SerializeField]
    private float cameraZoomSmoothTime = 0.075f;

    [SerializeField]
    private Slider cameraZoomSlider;

    [SerializeField]
    [Tooltip("The Minimum and Maximum zoom factor. X = Min | Y = Max")]
    private Vector2 cameraZoomMinMax = new Vector2(5, 100);

    [Space] [Header("Drag Mouse Canvas (Optional)")]
    [SerializeField]
    private GameObject mouseDragCanvasGameObject;

    [SerializeField]
    private GameObject mouseDragStartPoint;

    [SerializeField]
    private GameObject mouseDragEndPoint;

    [Space] [Header("Rotate Camera Canvas (Optional)")]
    [SerializeField]
    private GameObject rotateCameraCanvasGameObject;

    [SerializeField]
    private GameObject compasUiImageGameObject;

    private Vector3 mouseLockPos;
    private Camera cam;
    private CinemachineFramingTransposer framingTransposer;
    private GameObject virtualCameraGameObject;
    private float cameraZoomSmoothDamp;
    private float cameraZoomSmoothDampVel_ref;
    private float RotateFlipSmoothDampVel_ref;
    private bool currentRotateDir;
    private bool isRotating;
    private bool isDragging;

    private void Awake()
    {
        cam = Camera.main;
    }

    private void Start() 
    {
        virtualCameraGameObject = virtualCamera.gameObject;
        virtualCameraGameObject.transform.rotation = Quaternion.Euler(cameraTilt, virtualCameraGameObject.transform.rotation.y, 0);
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        cameraZoomSmoothDamp = framingTransposer.m_CameraDistance;
        if(mouseDragCanvasGameObject != null)
            mouseDragCanvasGameObject.SetActive(false);
        if(rotateCameraCanvasGameObject != null)
            rotateCameraCanvasGameObject.SetActive(false);
    }

    void Update()
    {
        Vector3 vectorChange = Vector3.zero;

        // Mouse Drag Camera Target
        if (!isRotating)
        {
            if (Input.GetMouseButtonDown(2))
            {
                if(mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(true);
                mouseLockPos = Input.mousePosition;
                isDragging = true;
                if(mouseDragStartPoint != null)
                    mouseDragStartPoint.transform.position = new Vector2(mouseLockPos.x, mouseLockPos.y);
            }
            if (Input.GetMouseButtonUp(2))
            {
                if(mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(false);
                Cursor.visible = true;
                isDragging = false;
            }
            if (Input.GetMouseButton(2))
            {
                vectorChange = new Vector3(mouseLockPos.x - Input.mousePosition.x, 0, mouseLockPos.y - Input.mousePosition.y) * -1;
                float distance = vectorChange.magnitude;
                if (distance > cameraMoveDeadZone)
                {
                    // Get the Drag direction
                    if (mouseDragStartPoint != null)
                    {
                        mouseDragEndPoint.transform.position = Input.mousePosition;
                        Vector3 dir = (Input.mousePosition - mouseDragStartPoint.transform.position);
                        mouseDragEndPoint.transform.right = dir;
                        mouseDragEndPoint.SetActive(true);
                    }

                    // Move target relative to Camera
                    MoveTargetRelativeToCamera(vectorChange, cameraMouseSpeed / 100);

                    
                    Cursor.visible = false;
                }
                else
                {
                    if (mouseDragStartPoint != null)
                        mouseDragEndPoint.SetActive(false);
                    Cursor.visible = true;
                }
            }
        }
        if (!isDragging)
        {
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

            vectorChange = new Vector3(horizontalInput, 0, verticalInput);
            // Move target relative to Camera
            if (vectorChange != Vector3.zero)
            {
                MoveTargetRelativeToCamera(vectorChange, cameraKeysSpeed);
            }
        }


        // Mouse Scroll
        cameraZoomSmoothDamp -= Input.mouseScrollDelta.y * (cameraZoomSpeed * 100) * Time.deltaTime;
        cameraZoomSmoothDamp = Mathf.Clamp(cameraZoomSmoothDamp, cameraZoomMinMax.x, cameraZoomMinMax.y);
        framingTransposer.m_CameraDistance = Mathf.SmoothDamp(framingTransposer.m_CameraDistance, cameraZoomSmoothDamp, ref cameraZoomSmoothDampVel_ref, cameraZoomSmoothTime);
        if (cameraZoomSlider != null)
        {
            cameraZoomSlider.minValue = cameraZoomMinMax.x;
            cameraZoomSlider.maxValue = cameraZoomMinMax.y;
            cameraZoomSlider.value = framingTransposer.m_CameraDistance;
        }

        // Mouse Rotate
        if (!isDragging)
        {
            if (Input.GetMouseButtonDown(1))
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isRotating = true;
                if(rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(true);
            }
            if (Input.GetMouseButtonUp(1))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isRotating = false;
                if(rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(false);
            }
            if (Input.GetMouseButton(1))
            {
                float horizontalMouse = Input.GetAxis("Mouse X");
                if (horizontalMouse != 0)
                {
                    currentRotateDir = (horizontalMouse > 0 ? true : false);
                    virtualCameraGameObject.transform.Rotate(0, horizontalMouse, 0, Space.World);
                    if(rotateCameraCanvasGameObject != null)
                        rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(rotateCameraCanvasGameObject.transform.eulerAngles.x, rotateCameraCanvasGameObject.transform.eulerAngles.y, virtualCameraGameObject.transform.eulerAngles.y * (currentRotateDir ? 1 : -1));
                }
                if(rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(rotateCameraCanvasGameObject.transform.eulerAngles.x, Mathf.SmoothDamp(rotateCameraCanvasGameObject.transform.eulerAngles.y, (currentRotateDir ? 180 : 0), ref RotateFlipSmoothDampVel_ref, 0.1f), rotateCameraCanvasGameObject.transform.eulerAngles.z);
            }
        }
        if(compasUiImageGameObject != null)
            compasUiImageGameObject.transform.rotation = Quaternion.Euler(0, 0, virtualCameraGameObject.transform.eulerAngles.y);
    }

    private void MoveTargetRelativeToCamera(Vector3 direction, float speed)
    {
        float relativeZoomCameraMoveSpeed = (framingTransposer.m_CameraDistance / cameraZoomMinMax.x);
        Vector3 camForward = virtualCameraGameObject.transform.forward;
        Vector3 camRight = virtualCameraGameObject.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 relativeDir = (camForward * direction.z) + (camRight * direction.x);

        cameraTarget.Translate(relativeDir * (relativeZoomCameraMoveSpeed * speed) * Time.deltaTime);
    }
}
