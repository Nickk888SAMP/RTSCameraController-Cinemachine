using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class RTSCameraTargetController : MonoBehaviour
{
    [SerializeField]
    private CinemachineVirtualCamera virtualCamera;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private Transform cameraTarget;

    [Space]
    [Header("Properties")]
    [SerializeField][Tooltip("Allows or Disallows rotation of the Camera.")]
    private bool allowRotate = true;

    [SerializeField][Tooltip("Allows or Disallows rotation of the Cameras Tilt.")]
    private bool allowTiltRotate = true;

    [SerializeField][Tooltip("Allows or Disallows Zooming.")]
    private bool allowZoom = true;

    [SerializeField][Tooltip("Allows or Disallows mouse drag movement.")]
    private bool allowDragMove = true;

    [SerializeField][Tooltip("Allows or Disallows camera movement with keys/gamepad input.")]
    private bool allowKeysMove = true;

    [SerializeField][Tooltip("Allows or Disallows camera movement using the screen sides.")]
    private bool allowScreenSideMove = true;


    [Space]
    [Header("Camera")]
    [SerializeField][Tooltip("The Minimum and Maximum Tilt of the camera, valid range: 0-89 (Do not go 90 or above)")]
    private Vector2 cameraTiltMinMax = new Vector2(15.0f, 75.0f);

    [SerializeField]
    private float cameraMouseSpeed = 2.0f;

    [SerializeField]
    private float cameraRotateSpeed = 4.0f;

    [SerializeField]
    private float cameraKeysSpeed = 6.0f;

    [SerializeField]
    private float cameraZoomSpeed = 4f;

    [SerializeField]
    private float cameraMoveDeadZone = 5f;

    [SerializeField]
    private float screenSidesZoneSize = 60f;

    [SerializeField]
    private float targetLockSpeed = 1.5f;

    [SerializeField]
    private float cameraTargetGroundHeightCheckSmoothTime = 4f;


    [Space] [Header("Camera Zoom")]
    [SerializeField]
    private float cameraZoomSmoothTime = 7f;

    [SerializeField]
    [Tooltip("The Minimum and Maximum zoom factor. X = Min | Y = Max")]
    private Vector2 cameraZoomMinMax = new Vector2(5, 100);

    [Space]
    [SerializeField] [Header("Camera Zoom Slider (Optional)")]
    private Slider cameraZoomSlider;

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
    private float currentCameraZoom;
    private float cameraZoomSmoothDampVel_ref;
    private float RotateFlipSmoothDampVel_ref;
    private float currentCameraRotate;
    private float currentCameraTilt;
    private bool currentRotateDir;
    private bool isRotating;
    private bool isDragging;
    private bool isSideZoneMoving;
    private bool isLockedOnTarget;

    private Vector3 lockedOnPosition;
    private Transform lockedOnTransform;
    private bool hardLocked;
    private float lockedOnZoom;

    private void Awake()
    {
        cam = Camera.main;
        Application.targetFrameRate = 999;
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

    void Update()
    {
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

    #region Internal Functions

    private void GroundHeightCorrection()
    {
        if (!isLockedOnTarget)
        {
            if (Physics.Raycast(new Vector3(cameraTarget.position.x, 9999999, cameraTarget.position.z), Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                cameraTarget.position = Vector3.Lerp(cameraTarget.position, new Vector3(cameraTarget.position.x, hit.point.y, cameraTarget.position.z), cameraTargetGroundHeightCheckSmoothTime * Time.deltaTime);
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
                    cameraTarget.position = Vector3.Lerp(cameraTarget.position, lockedOnPosition, targetLockSpeed * Time.deltaTime);
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
                    cameraTarget.position = Vector3.Lerp(cameraTarget.position, lockedOnTransform.position, targetLockSpeed * Time.deltaTime);
                }
            }
            currentCameraZoom = Mathf.Lerp(currentCameraZoom, lockedOnZoom, targetLockSpeed * Time.deltaTime);
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
            float horizontalInput = Input.GetAxis("Horizontal");
            float verticalInput = Input.GetAxis("Vertical");

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
            if (Input.GetMouseButtonDown(2) && allowDragMove)
            {
                if (mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(true);
                mouseLockPos = mousePos;
                isDragging = true;
                CancelTargetLock();
                if (mouseDragStartPoint != null)
                    mouseDragStartPoint.transform.position = new Vector2(mouseLockPos.x, mouseLockPos.y);
            }
            if ((isDragging && Input.GetMouseButtonUp(2)) || (isDragging && !allowDragMove))
            {
                if (mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(false);
                Cursor.visible = true;
                isDragging = false;
            }
            if (Input.GetMouseButton(2) && isDragging && allowDragMove)
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
        framingTransposer.m_CameraDistance = Mathf.SmoothDamp(framingTransposer.m_CameraDistance, currentCameraZoom, ref cameraZoomSmoothDampVel_ref, (cameraZoomSmoothTime / 100));
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
            if (Input.GetMouseButtonDown(1) && allowRotate)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                isRotating = true;
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(true);
            }
            if ((isRotating && Input.GetMouseButtonUp(1)) || (isRotating && !allowRotate))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                isRotating = false;
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(false);
            }
            if (Input.GetMouseButton(1) && allowRotate)
            {
                float horizontalMouse = Input.GetAxis("Mouse X");
                float verticalMouse = Input.GetAxis("Mouse Y");
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
                    currentRotateDir = (verticalMouse > 0 ? true : false);
                    currentCameraTilt += verticalMouse * cameraRotateSpeed;
                    currentCameraTilt = Mathf.Clamp(currentCameraTilt, cameraTiltMinMax.x, cameraTiltMinMax.y);
                    virtualCameraGameObject.transform.eulerAngles = new Vector3(currentCameraTilt, virtualCameraGameObject.transform.eulerAngles.y, 0);
                    if (rotateCameraCanvasGameObject != null)
                        rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(rotateCameraCanvasGameObject.transform.eulerAngles.x, rotateCameraCanvasGameObject.transform.eulerAngles.y, virtualCameraGameObject.transform.eulerAngles.y * (currentRotateDir ? 1 : -1));
                }
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(rotateCameraCanvasGameObject.transform.eulerAngles.x, Mathf.SmoothDamp(rotateCameraCanvasGameObject.transform.eulerAngles.y, (currentRotateDir ? 180 : 0), ref RotateFlipSmoothDampVel_ref, 0.1f), rotateCameraCanvasGameObject.transform.eulerAngles.z);
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

        cameraTarget.Translate(relativeDir * (relativeZoomCameraMoveSpeed * speed) * Time.deltaTime);
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

    #region Functions

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

    #region Getters and Setters

    public void SetAllowRotate(bool allow) => this.allowRotate = allow;
    public bool GetAllowRotate() => this.allowRotate;

    public void SetAllowTiltRotate(bool allow) => this.allowTiltRotate = allow;
    public bool GetAllowTiltRotate() => this.allowTiltRotate;

    public void SetAllowZoomRotate(bool allow) => this.allowZoom = allow;
    public bool GetAllowZoomRotate() => this.allowZoom;

    public void SetAllowDragMove(bool allow) => this.allowDragMove = allow;
    public bool GetAllowDragMove() => this.allowDragMove;

    public void SetAllowKeysMove(bool allow) => this.allowKeysMove = allow;
    public bool GetAllowKeysMove() => this.allowKeysMove;

    public void SetAllowScreenSideMove(bool allow) => this.allowScreenSideMove = allow;
    public bool GetAllowScreenSideMove() => this.allowScreenSideMove;

    #endregion
}

