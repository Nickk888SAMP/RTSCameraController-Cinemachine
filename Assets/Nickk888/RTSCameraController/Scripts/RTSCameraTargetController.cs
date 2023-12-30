using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class RTSCameraTargetController : MonoBehaviour
{
    public static RTSCameraTargetController Instance;

    private enum MouseButton
    {
        Left,
        Right,
        Middle,
        forward,
        Back
    }

    #region Properties

    [Header("Setup")]
    [SerializeField]
    [Tooltip("The Cinemachine Virtual Camera to be controlled by the controller.")]
    private CinemachineVirtualCamera virtualCamera;
    public CinemachineVirtualCamera VirtualCamera { get => virtualCamera; }

    [SerializeField] [Tooltip("The ground layer for the height check.")]
    private LayerMask groundLayer;
    public LayerMask GroundLayer { get { return groundLayer; } set { groundLayer = value; } }

    [SerializeField] [Tooltip("The target for the camera to follow.")]
    private Transform cameraTarget;
    public Transform CameraTarget { get { return cameraTarget; } set { cameraTarget = value; } }

    [Space] [Header("Input")]
    [SerializeField] [Tooltip("The Horizontal movement Input Axis.")]
    private string horizontalMovementAxisName = "Horizontal";
    
    [SerializeField] [Tooltip("The Vertical movement Input Axis.")]
    private string verticalMovementAxisName = "Vertical";

    [SerializeField] [Tooltip("The Horizontal mouse Input Axis.")]
    private string horizontalMouseAxisName = "Mouse X";
    
    [SerializeField] [Tooltip("The Vertical mouse Input Axis.")]
    private string verticalMouseAxisName = "Mouse Y";

    [SerializeField] [Tooltip("The mouse button for rotating.")]
    private MouseButton rotationMouseButton = MouseButton.Right;

    [SerializeField] [Tooltip("The mouse button for drag move.")]
    private MouseButton dragMoveMouseButton = MouseButton.Middle;

    [Space] [Header("Time Scale")]
    [SerializeField] [Tooltip("Check to make the controller be independent on the Time Scale.")]
    private bool independentTimeScale = true;
    public bool IndependentTimeScale { get { return independentTimeScale; } set { independentTimeScale = value; } }

    [SerializeField] [Tooltip("Check to make the Cinemachine Brain be independent on the Time Scale.")]
    private bool independentCinemachineBrainTimeScale = true;
    public bool IndependentCinemachineBrainTimeScale { get { return independentCinemachineBrainTimeScale; } set { independentCinemachineBrainTimeScale = value; } }

    [Space][Header("Properties")]
    [SerializeField][Tooltip("Allows or Disallows rotation of the Camera.")]
    private bool allowRotate = true;
    public bool AllowRotate { get { return allowRotate; } set { allowRotate = value; } }

    [SerializeField] [Tooltip("Allows or Disallows rotation of the Cameras Tilt.")]
    private bool allowTiltRotate = true;
    public bool AllowTiltRotate { get { return allowTiltRotate; } set { allowTiltRotate = value; } }

    [SerializeField] [Tooltip("Allows or Disallows Zooming.")]
    private bool allowZoom = true;
    public bool AllowZoom { get { return allowZoom; } set { allowZoom = value; } }

    [SerializeField] [Tooltip("Allows or Disallows mouse drag movement.")]
    private bool allowDragMove = true;
    public bool AllowDragMove { get { return allowDragMove; } set { allowDragMove = value; } }

    [SerializeField] [Tooltip("Allows or Disallows camera movement with keys/gamepad input.")]
    private bool allowKeysMove = true;
    public bool AllowKeysMove { get { return allowKeysMove; } set { allowKeysMove = value; } }

    [SerializeField] [Tooltip("Allows or Disallows camera movement using the screen sides.")]
    private bool allowScreenSideMove = true;
    public bool AllowScreenSideMove { get { return allowScreenSideMove; } set { allowScreenSideMove = value; } }

    [Space] [Header("Camera")]
    [SerializeField] [Tooltip("The Minimum and Maximum Tilt of the camera, valid range: 0-89 (Do not go 90 or above)")]
    private Vector2 cameraTiltMinMax = new Vector2(15.0f, 75.0f);
    public Vector2 CameraTiltMinMax { get { return cameraTiltMinMax; } set { cameraTiltMinMax = value; } }

    [SerializeField]
    private float cameraMouseSpeed = 2.0f;
    public float CameraMouseSpeed { get { return cameraMouseSpeed; } set { cameraMouseSpeed = value; } }

    [SerializeField]
    private float cameraRotateSpeed = 4.0f;
    public float CameraRotateSpeed { get { return cameraRotateSpeed; } set { cameraRotateSpeed = value; } }

    [SerializeField]
    private float cameraKeysSpeed = 6.0f;
    public float CameraKeysSpeed { get { return cameraKeysSpeed; } set { cameraKeysSpeed = value; } }

    [SerializeField]
    private float cameraZoomSpeed = 4f;
    public float CameraZoomSpeed { get { return cameraZoomSpeed; } set { cameraZoomSpeed = value; } }

    [SerializeField]
    private float cameraMoveDeadZone = 5f;
    public float CameraMoveDeadZone { get { return cameraMoveDeadZone; } set { cameraMoveDeadZone = value; } }

    [SerializeField]
    private float screenSidesZoneSize = 60f;
    public float ScreenSidesZoneSize { get { return screenSidesZoneSize; } set { screenSidesZoneSize = value; } }

    [SerializeField]
    private float targetLockSpeed = 1.5f;
    public float TargetLockSpeed { get { return targetLockSpeed; } set { targetLockSpeed = value; } }

    [SerializeField]
    private float cameraTargetGroundHeightCheckSmoothTime = 4f;
    public float CameraTargetGroundHeightCheckSmoothTime { get { return cameraTargetGroundHeightCheckSmoothTime; } set { cameraTargetGroundHeightCheckSmoothTime = value; } }

    [Space] [Header("Camera Zoom")]
    [SerializeField]
    private float cameraZoomSmoothTime = 7f;
    public float CameraZoomSmoothTime { get { return cameraZoomSmoothTime; } set { cameraZoomSmoothTime = value; } }

    [SerializeField] [Tooltip("The Minimum and Maximum zoom factor. X = Min | Y = Max")]
    private Vector2 cameraZoomMinMax = new Vector2(5, 100);
    public Vector2 CameraZoomMinMax { get { return cameraZoomMinMax; } set { cameraZoomMinMax = value; } }

    [Space]
    [SerializeField] [Header("Camera Zoom Slider (Optional)")]
    private Slider cameraZoomSlider;
    public Slider CameraZoomSlider { get { return cameraZoomSlider; } set { cameraZoomSlider = value; } }

    [Space] [Header("Drag Mouse Canvas (Optional)")]
    [SerializeField]
    private GameObject mouseDragCanvasGameObject;
    public GameObject MouseDragCanvasGameObject { get { return mouseDragCanvasGameObject; } set { mouseDragCanvasGameObject = value; } }

    [SerializeField]
    private GameObject mouseDragStartPoint;
    public GameObject MouseDragStartPoint { get { return mouseDragStartPoint; } set { mouseDragStartPoint = value; } }

    [SerializeField]
    private GameObject mouseDragEndPoint;
    public GameObject MouseDragEndPoint { get { return mouseDragEndPoint; } set { mouseDragEndPoint = value; } }

    [Space] [Header("Rotate Camera Canvas (Optional)")]
    [SerializeField]
    private GameObject rotateCameraCanvasGameObject;
    public GameObject RotateCameraCanvasGameObject { get { return rotateCameraCanvasGameObject; } set { rotateCameraCanvasGameObject = value; } }

    [SerializeField]
    private GameObject compasUiImageGameObject;
    public GameObject CompasUiImageGameObject { get { return compasUiImageGameObject; } set { compasUiImageGameObject = value; } }

    #endregion

    #region Private Fields

    private Vector3 mouseLockPos;
    private Vector3 lockedOnPosition;
    private Camera cam;
    private CinemachineBrain cinemachineBrain;
    private CinemachineFramingTransposer framingTransposer;
    private GameObject virtualCameraGameObject;
    private Transform lockedOnTransform;
    private float currentCameraZoom;
    private float cameraZoomSmoothDampVel_ref;
    private float RotateFlipSmoothDampVel_ref;
    private float currentCameraRotate;
    private float currentCameraTilt;
    private float lockedOnZoom;
    private bool currentRotateDir;
    private bool isRotating;
    private bool isDragging;
    private bool isSideZoneMoving;
    private bool isLockedOnTarget;
    private bool hardLocked;

    #endregion

    #region Callbacks
    private void Awake()
    {
        cam = Camera.main;
        cinemachineBrain = cam.gameObject.GetComponent<CinemachineBrain>();
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start() 
    {
        virtualCameraGameObject = virtualCamera.gameObject;
        currentCameraTilt = Mathf.Lerp(cameraTiltMinMax.x, cameraTiltMinMax.y, 0.5f);
        virtualCameraGameObject.transform.eulerAngles = new Vector3(currentCameraTilt, virtualCameraGameObject.transform.eulerAngles.y, 0);
        framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        currentCameraZoom = framingTransposer.m_CameraDistance;
        if(mouseDragCanvasGameObject != null)
            mouseDragCanvasGameObject.SetActive(false);
        if(rotateCameraCanvasGameObject != null)
            rotateCameraCanvasGameObject.SetActive(false);
    }

    private void Update()
    {
        cinemachineBrain.m_IgnoreTimeScale = independentCinemachineBrainTimeScale;
        Vector3 vectorChange = Vector3.zero;
        Vector3 mousePos = Input.mousePosition;

        HandleScreenSideMove(mousePos);
        HandleMouseDrag(mousePos);
        HandleKeysMove();
        HandleZoom();
        HandleRotation();
        HandleTargetLock();
        GroundHeightCorrection();

        if (compasUiImageGameObject != null)
            compasUiImageGameObject.transform.rotation = Quaternion.Euler(0, 0, virtualCameraGameObject.transform.eulerAngles.y);
    }

    #endregion

    #region Internal Functions

    private void GroundHeightCorrection()
    {
        if (!isLockedOnTarget)
        {
            if (Physics.Raycast(new Vector3(cameraTarget.position.x, 9999999, cameraTarget.position.z), Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                cameraTarget.position = Vector3.Lerp(cameraTarget.position, new Vector3(cameraTarget.position.x, hit.point.y, cameraTarget.position.z), cameraTargetGroundHeightCheckSmoothTime * GetTimeScale());
            }
        }
    }

    private void HandleTargetLock()
    {
        if (isLockedOnTarget)
        {
            if(lockedOnTransform == null)
            {
                if(hardLocked)
                {
                    cameraTarget.position = lockedOnPosition;
                }
                else
                {
                    cameraTarget.position = Vector3.Lerp(cameraTarget.position, lockedOnPosition, targetLockSpeed * GetTimeScale());
                }
            }
            else
            {
                if(hardLocked)
                {
                    cameraTarget.position = lockedOnTransform.position;
                }
                else
                {
                    cameraTarget.position = Vector3.Lerp(cameraTarget.position, lockedOnTransform.position, targetLockSpeed * GetTimeScale());
                }
            }
            currentCameraZoom = Mathf.Lerp(currentCameraZoom, lockedOnZoom, targetLockSpeed * GetTimeScale());
        }
    }

    private void HandleScreenSideMove(Vector3 mousePos)
    {
        GetMouseScreenSide(mousePos, out int widthPos, out int heightPos);
        Vector3 moveVector = new Vector3(widthPos, 0, heightPos);
        if (moveVector != Vector3.zero && !isDragging && !isRotating && allowScreenSideMove)
        {
            CancelTargetLock();
            MoveTargetRelativeToCamera(moveVector, cameraKeysSpeed);
            isSideZoneMoving = true;
        }
        else
        {
            isSideZoneMoving = false;
        }
    }

    private void HandleKeysMove()
    {
        if (!isDragging && !isSideZoneMoving && allowKeysMove)
        {
            float horizontalInput = Input.GetAxisRaw(horizontalMovementAxisName);
            float verticalInput = Input.GetAxisRaw(verticalMovementAxisName);

            Vector3 vectorChange = new Vector3(horizontalInput, 0, verticalInput);
            // Move target relative to Camera
            if (vectorChange != Vector3.zero)
            {
                CancelTargetLock();
                MoveTargetRelativeToCamera(vectorChange, cameraKeysSpeed);
            }
        }
    }

    private void HandleMouseDrag(Vector3 mousePos)
    {
        if (!isRotating && !isSideZoneMoving)
        {
            if (Input.GetMouseButtonDown((int)dragMoveMouseButton) && allowDragMove)
            {
                if (mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(true);
                mouseLockPos = mousePos;
                isDragging = true;
                CancelTargetLock();
                if (mouseDragStartPoint != null)
                    mouseDragStartPoint.transform.position = new Vector2(mouseLockPos.x, mouseLockPos.y);
            }
            if ((isDragging && Input.GetMouseButtonUp((int)dragMoveMouseButton)) || (isDragging && !allowDragMove))
            {
                if (mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(false);
                Cursor.visible = true;
                isDragging = false;
            }
            if (Input.GetMouseButton((int)dragMoveMouseButton) && isDragging && allowDragMove)
            {
                Vector3 vectorChange = new Vector3(mouseLockPos.x - mousePos.x, 0, mouseLockPos.y - mousePos.y) * -1;
                float distance = vectorChange.sqrMagnitude;
                if (distance > (cameraMoveDeadZone * cameraMoveDeadZone))
                {
                    // Get the Drag direction
                    if (mouseDragStartPoint != null)
                    {
                        mouseDragEndPoint.transform.position = mousePos;
                        Vector3 dir = (mousePos - mouseDragStartPoint.transform.position);
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
    }

    private void HandleZoom()
    {
        if (allowZoom)
        {
            currentCameraZoom -= Input.mouseScrollDelta.y * cameraZoomSpeed;
            currentCameraZoom = Mathf.Clamp(currentCameraZoom, cameraZoomMinMax.x, cameraZoomMinMax.y);
            if(Input.mouseScrollDelta.y != 0)
            {
                CancelTargetLock();
            }
        }
        framingTransposer.m_CameraDistance = Mathf.SmoothDamp(framingTransposer.m_CameraDistance, currentCameraZoom, ref cameraZoomSmoothDampVel_ref, (cameraZoomSmoothTime / 100), Mathf.Infinity, GetTimeScale());
        if (cameraZoomSlider != null)
        {
            cameraZoomSlider.minValue = cameraZoomMinMax.x;
            cameraZoomSlider.maxValue = cameraZoomMinMax.y;
            cameraZoomSlider.value = framingTransposer.m_CameraDistance;
        }
    }

    private void HandleRotation()
    {
        if (!isDragging)
        {
            if (Input.GetMouseButtonDown((int)rotationMouseButton) && allowRotate)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isRotating = true;
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(true);
            }
            if ((isRotating && Input.GetMouseButtonUp((int)rotationMouseButton)) || (isRotating && !allowRotate))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isRotating = false;
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(false);
            }
            if (Input.GetMouseButton((int)rotationMouseButton) && allowRotate)
            {
                float horizontalMouse = Input.GetAxisRaw(horizontalMouseAxisName);
                float verticalMouse = Input.GetAxisRaw(verticalMouseAxisName);
                if (horizontalMouse != 0)
                {
                    currentRotateDir = (horizontalMouse > 0 ? true : false);
                    currentCameraRotate += horizontalMouse * cameraRotateSpeed;
                    virtualCameraGameObject.transform.eulerAngles = new Vector3(virtualCameraGameObject.transform.eulerAngles.x, currentCameraRotate, 0);
                    if (rotateCameraCanvasGameObject != null)
                        rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(rotateCameraCanvasGameObject.transform.eulerAngles.x, rotateCameraCanvasGameObject.transform.eulerAngles.y, virtualCameraGameObject.transform.eulerAngles.y * (currentRotateDir ? 1 : -1));
                }
                if (verticalMouse != 0 && allowTiltRotate)
                {
                    currentCameraTilt += verticalMouse * cameraRotateSpeed;
                    currentCameraTilt = Mathf.Clamp(currentCameraTilt, cameraTiltMinMax.x, cameraTiltMinMax.y);
                    virtualCameraGameObject.transform.eulerAngles = new Vector3(currentCameraTilt, virtualCameraGameObject.transform.eulerAngles.y, 0);
                }
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(rotateCameraCanvasGameObject.transform.eulerAngles.x, Mathf.SmoothDamp(rotateCameraCanvasGameObject.transform.eulerAngles.y, (currentRotateDir ? 180 : 0), ref RotateFlipSmoothDampVel_ref, 0.1f, Mathf.Infinity,  GetTimeScale()), rotateCameraCanvasGameObject.transform.eulerAngles.z);
            }
        }
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

        cameraTarget.Translate(relativeDir * (relativeZoomCameraMoveSpeed * speed) * GetTimeScale());
    }

    private float GetTimeScale()
    {
        return (independentTimeScale ? Time.unscaledDeltaTime : Time.deltaTime);
    }

    private void GetMouseScreenSide(Vector3 mousePosition, out int width, out int height)
    {
        int heightPos = 0;
        int widthPos = 0;
        if(mousePosition.x >= 0 && mousePosition.x <= screenSidesZoneSize)
        {
            widthPos = -1;
        }
        else if(mousePosition.x >= Screen.width - screenSidesZoneSize && mousePosition.x <= Screen.width)
        {
            widthPos = 1;
        }
        if(mousePosition.y >= 0 && mousePosition.y <= screenSidesZoneSize)
        {
            heightPos = -1;
        }
        else if(mousePosition.y >= Screen.height - screenSidesZoneSize && mousePosition.y <= Screen.height)
        {
            heightPos = 1;
        }
        width = widthPos;
        height = heightPos;
    }

    #endregion

    #region Public Functions

    /// <summary>
    /// Locks the camera to a target position
    /// </summary>
    /// <param name="position"></param>
    /// <param name="zoomFactor"></param>
        
    public void LockOnTarget(Vector3 position, float zoomFactor, bool hardLock = false)
    {
        CancelTargetLock();
        lockedOnPosition = position;
        lockedOnZoom = zoomFactor;
        hardLocked = hardLock;
        isLockedOnTarget = true;
    }

    /// <summary>
    /// Locks the camera to a target transform
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="zoomFactor"></param>

    public void LockOnTarget(Transform transform, float zoomFactor, bool hardLock = false)
    {
        CancelTargetLock();
        lockedOnTransform = transform;
        lockedOnZoom = zoomFactor;
        hardLocked = hardLock;
        isLockedOnTarget = true;
    }

    /// <summary>
    /// Cancels the target locking
    /// </summary>

    public void CancelTargetLock()
    {
        isLockedOnTarget = false;
        lockedOnPosition = Vector3.zero;
        lockedOnTransform = null;
    }

    #endregion
}

