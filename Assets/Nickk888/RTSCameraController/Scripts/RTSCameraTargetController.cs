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
    public CinemachineVirtualCamera VirtualCamera => virtualCamera;

    [SerializeField] [Tooltip("The ground layer for the height check.")]
    private LayerMask groundLayer;
    public LayerMask GroundLayer { get => groundLayer; set => groundLayer = value; }

    [SerializeField] [Tooltip("The target for the camera to follow.")]
    private Transform cameraTarget;
    public Transform CameraTarget { get => cameraTarget; set => cameraTarget = value; }

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
    public bool IndependentTimeScale { get => independentTimeScale; set => independentTimeScale = value; }

    [SerializeField] [Tooltip("Check to make the Cinemachine Brain be independent on the Time Scale.")]
    private bool independentCinemachineBrainTimeScale = true;
    public bool IndependentCinemachineBrainTimeScale { get => independentCinemachineBrainTimeScale; set => independentCinemachineBrainTimeScale = value; }

    [Space][Header("Properties")]
    [SerializeField][Tooltip("Allows or Disallows rotation of the Camera.")]
    private bool allowRotate = true;
    public bool AllowRotate { get => allowRotate; set => allowRotate = value; }

    [SerializeField] [Tooltip("Allows or Disallows rotation of the Cameras Tilt.")]
    private bool allowTiltRotate = true;
    public bool AllowTiltRotate { get => allowTiltRotate; set => allowTiltRotate = value; }

    [SerializeField] [Tooltip("Allows or Disallows Zooming.")]
    private bool allowZoom = true;
    public bool AllowZoom { get => allowZoom; set => allowZoom = value; }

    [SerializeField] [Tooltip("Allows or Disallows mouse drag movement.")]
    private bool allowDragMove = true;
    public bool AllowDragMove { get => allowDragMove; set => allowDragMove = value; }

    [SerializeField] [Tooltip("Allows or Disallows camera movement with keys/gamepad input.")]
    private bool allowKeysMove = true;
    public bool AllowKeysMove { get => allowKeysMove; set => allowKeysMove = value; }

    [SerializeField] [Tooltip("Allows or Disallows camera movement using the screen sides.")]
    private bool allowScreenSideMove = true;
    public bool AllowScreenSideMove { get => allowScreenSideMove; set => allowScreenSideMove = value; }

    [Space] [Header("Camera")]
    [SerializeField] [Tooltip("The Minimum and Maximum Tilt of the camera, valid range: 0-89 (Do not go 90 or above)")]
    private Vector2 cameraTiltMinMax = new Vector2(15.0f, 75.0f);
    public Vector2 CameraTiltMinMax { get => cameraTiltMinMax; set => cameraTiltMinMax = value; }

    [SerializeField]
    private float cameraMouseSpeed = 2.0f;
    public float CameraMouseSpeed { get => cameraMouseSpeed; set => cameraMouseSpeed = value; }

    [SerializeField]
    private float cameraRotateSpeed = 4.0f;
    public float CameraRotateSpeed { get => cameraRotateSpeed; set => cameraRotateSpeed = value; }

    [SerializeField]
    private float cameraKeysSpeed = 6.0f;
    public float CameraKeysSpeed { get => cameraKeysSpeed; set => cameraKeysSpeed = value; }

    [SerializeField]
    private float cameraZoomSpeed = 4f;
    public float CameraZoomSpeed { get => cameraZoomSpeed; set => cameraZoomSpeed = value; }

    [SerializeField]
    private float cameraMoveDeadZone = 5f;
    public float CameraMoveDeadZone { get => cameraMoveDeadZone; set => cameraMoveDeadZone = value; }

    [SerializeField]
    private float screenSidesZoneSize = 60f;
    public float ScreenSidesZoneSize { get => screenSidesZoneSize; set => screenSidesZoneSize = value; }

    [SerializeField]
    private float targetLockSpeed = 1.5f;
    public float TargetLockSpeed { get => targetLockSpeed; set => targetLockSpeed = value; }

    [SerializeField]
    private float cameraTargetGroundHeightCheckSmoothTime = 4f;
    public float CameraTargetGroundHeightCheckSmoothTime { get => cameraTargetGroundHeightCheckSmoothTime; set => cameraTargetGroundHeightCheckSmoothTime = value; }

    [Space] [Header("Camera Zoom")]
    [SerializeField]
    private float cameraZoomSmoothTime = 7f;
    public float CameraZoomSmoothTime { get => cameraZoomSmoothTime; set => cameraZoomSmoothTime = value; }

    [SerializeField] [Tooltip("The Minimum and Maximum zoom factor. X = Min | Y = Max")]
    private Vector2 cameraZoomMinMax = new Vector2(5, 100);
    public Vector2 CameraZoomMinMax { get => cameraZoomMinMax; set => cameraZoomMinMax = value; }

    [Space]
    [SerializeField] [Header("Camera Zoom Slider (Optional)")]
    private Slider cameraZoomSlider;
    public Slider CameraZoomSlider { get => cameraZoomSlider; set => cameraZoomSlider = value; }

    [Space] [Header("Drag Mouse Canvas (Optional)")]
    [SerializeField]
    private GameObject mouseDragCanvasGameObject;
    public GameObject MouseDragCanvasGameObject { get => mouseDragCanvasGameObject; set => mouseDragCanvasGameObject = value; }

    [SerializeField]
    private GameObject mouseDragStartPoint;
    public GameObject MouseDragStartPoint { get => mouseDragStartPoint; set => mouseDragStartPoint = value; }

    [SerializeField]
    private GameObject mouseDragEndPoint;
    public GameObject MouseDragEndPoint { get => mouseDragEndPoint; set => mouseDragEndPoint = value; }

    [Space] [Header("Rotate Camera Canvas (Optional)")]
    [SerializeField]
    private GameObject rotateCameraCanvasGameObject;
    public GameObject RotateCameraCanvasGameObject { get => rotateCameraCanvasGameObject; set => rotateCameraCanvasGameObject = value; }

    [SerializeField]
    private GameObject compassUiImageGameObject;
    public GameObject CompassUiImageGameObject { get => compassUiImageGameObject; set => compassUiImageGameObject = value; }

    #endregion

    #region Private Fields

    private Vector3 _mouseLockPos;
    private Vector3 _lockedOnPosition;
    private Camera _cam;
    private CinemachineBrain _cinemachineBrain;
    private CinemachineFramingTransposer _framingTransposer;
    private GameObject _virtualCameraGameObject;
    private Transform _lockedOnTransform;
    private float _currentCameraZoom;
    private float _cameraZoomSmoothDampVelRef;
    private float _rotateFlipSmoothDampVelRef;
    private float _currentCameraRotate;
    private float _currentCameraTilt;
    private float _lockedOnZoom;
    private bool _currentRotateDir;
    private bool _isRotating;
    private bool _isDragging;
    private bool _isSideZoneMoving;
    private bool _isLockedOnTarget;
    private bool _hardLocked;

    #endregion

    #region Callbacks
    private void Awake()
    {
        _cam = Camera.main;
        if (_cam != null) _cinemachineBrain = _cam.gameObject.GetComponent<CinemachineBrain>();
        if(Instance != null)
        {
            Destroy(Instance);
        }
        Instance = this;
    }

    private void Start() 
    {
        _virtualCameraGameObject = virtualCamera.gameObject;
        _currentCameraTilt = Mathf.Lerp(cameraTiltMinMax.x, cameraTiltMinMax.y, 0.5f);
        _virtualCameraGameObject.transform.eulerAngles = new Vector3(_currentCameraTilt, _virtualCameraGameObject.transform.eulerAngles.y, 0);
        _framingTransposer = virtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _currentCameraZoom = _framingTransposer.m_CameraDistance;
        if(mouseDragCanvasGameObject != null)
            mouseDragCanvasGameObject.SetActive(false);
        if(rotateCameraCanvasGameObject != null)
            rotateCameraCanvasGameObject.SetActive(false);
    }

    private void Update()
    {
        _cinemachineBrain.m_IgnoreTimeScale = independentCinemachineBrainTimeScale;
        Vector3 mousePos = Input.mousePosition;

        HandleScreenSideMove(mousePos);
        HandleMouseDrag(mousePos);
        HandleKeysMove();
        HandleZoom();
        HandleRotation();
        HandleTargetLock();
        GroundHeightCorrection();

        if (compassUiImageGameObject != null)
            compassUiImageGameObject.transform.rotation = Quaternion.Euler(0, 0, _virtualCameraGameObject.transform.eulerAngles.y);
    }

    #endregion

    #region Internal Functions

    private void GroundHeightCorrection()
    {
        if (!_isLockedOnTarget)
        {
            if (Physics.Raycast(new Vector3(cameraTarget.position.x, 9999999, cameraTarget.position.z), Vector3.down, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                Vector3 position = cameraTarget.position;
                position = Vector3.Lerp(position, new Vector3(position.x, hit.point.y, position.z), cameraTargetGroundHeightCheckSmoothTime * GetTimeScale());
                cameraTarget.position = position;
            }
        }
    }

    private void HandleTargetLock()
    {
        if (_isLockedOnTarget)
        {
            if(_lockedOnTransform == null)
            {
                cameraTarget.position = _hardLocked ? _lockedOnPosition : Vector3.Lerp(cameraTarget.position, _lockedOnPosition, targetLockSpeed * GetTimeScale());
            }
            else
            {
                cameraTarget.position = _hardLocked ? _lockedOnTransform.position : Vector3.Lerp(cameraTarget.position, _lockedOnTransform.position, targetLockSpeed * GetTimeScale());
            }
            _currentCameraZoom = Mathf.Lerp(_currentCameraZoom, _lockedOnZoom, targetLockSpeed * GetTimeScale());
        }
    }

    private void HandleScreenSideMove(Vector3 mousePos)
    {
        GetMouseScreenSide(mousePos, out int widthPos, out int heightPos);
        Vector3 moveVector = new Vector3(widthPos, 0, heightPos);
        if (moveVector != Vector3.zero && !_isDragging && !_isRotating && allowScreenSideMove)
        {
            CancelTargetLock();
            MoveTargetRelativeToCamera(moveVector, cameraKeysSpeed);
            _isSideZoneMoving = true;
        }
        else
        {
            _isSideZoneMoving = false;
        }
    }

    private void HandleKeysMove()
    {
        if (!_isDragging && !_isSideZoneMoving && allowKeysMove)
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
        if (!_isRotating && !_isSideZoneMoving)
        {
            if (Input.GetMouseButtonDown((int)dragMoveMouseButton) && allowDragMove)
            {
                if (mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(true);
                _mouseLockPos = mousePos;
                _isDragging = true;
                CancelTargetLock();
                if (mouseDragStartPoint != null)
                    mouseDragStartPoint.transform.position = new Vector2(_mouseLockPos.x, _mouseLockPos.y);
            }
            if ((_isDragging && Input.GetMouseButtonUp((int)dragMoveMouseButton)) || (_isDragging && !allowDragMove))
            {
                if (mouseDragCanvasGameObject != null)
                    mouseDragCanvasGameObject.SetActive(false);
                Cursor.visible = true;
                _isDragging = false;
            }
            if (Input.GetMouseButton((int)dragMoveMouseButton) && _isDragging && allowDragMove)
            {
                Vector3 vectorChange = new Vector3(_mouseLockPos.x - mousePos.x, 0, _mouseLockPos.y - mousePos.y) * -1;
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
            _currentCameraZoom -= Input.mouseScrollDelta.y * cameraZoomSpeed;
            _currentCameraZoom = Mathf.Clamp(_currentCameraZoom, cameraZoomMinMax.x, cameraZoomMinMax.y);
            if(Input.mouseScrollDelta.y != 0)
            {
                CancelTargetLock();
            }
        }
        _framingTransposer.m_CameraDistance = Mathf.SmoothDamp(_framingTransposer.m_CameraDistance, _currentCameraZoom, ref _cameraZoomSmoothDampVelRef, (cameraZoomSmoothTime / 100), Mathf.Infinity, GetTimeScale());
        if (cameraZoomSlider != null)
        {
            cameraZoomSlider.minValue = cameraZoomMinMax.x;
            cameraZoomSlider.maxValue = cameraZoomMinMax.y;
            cameraZoomSlider.value = _framingTransposer.m_CameraDistance;
        }
    }

    private void HandleRotation()
    {
        if (!_isDragging)
        {
            if (Input.GetMouseButtonDown((int)rotationMouseButton) && allowRotate)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                _isRotating = true;
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(true);
            }
            if ((_isRotating && Input.GetMouseButtonUp((int)rotationMouseButton)) || (_isRotating && !allowRotate))
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                _isRotating = false;
                if (rotateCameraCanvasGameObject != null)
                    rotateCameraCanvasGameObject.SetActive(false);
            }
            if (Input.GetMouseButton((int)rotationMouseButton) && allowRotate)
            {
                float horizontalMouse = Input.GetAxisRaw(horizontalMouseAxisName);
                float verticalMouse = Input.GetAxisRaw(verticalMouseAxisName);
                if (horizontalMouse != 0)
                {
                    _currentRotateDir = (horizontalMouse > 0 ? true : false);
                    _currentCameraRotate += horizontalMouse * cameraRotateSpeed;
                    _virtualCameraGameObject.transform.eulerAngles = new Vector3(_virtualCameraGameObject.transform.eulerAngles.x, _currentCameraRotate, 0);
                    if (rotateCameraCanvasGameObject != null)
                    {
                        var eulerAngles = rotateCameraCanvasGameObject.transform.eulerAngles;
                        rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, _virtualCameraGameObject.transform.eulerAngles.y * (_currentRotateDir ? 1 : -1));
                    }
                }
                if (verticalMouse != 0 && allowTiltRotate)
                {
                    _currentCameraTilt += verticalMouse * cameraRotateSpeed;
                    _currentCameraTilt = Mathf.Clamp(_currentCameraTilt, cameraTiltMinMax.x, cameraTiltMinMax.y);
                    _virtualCameraGameObject.transform.eulerAngles = new Vector3(_currentCameraTilt, _virtualCameraGameObject.transform.eulerAngles.y, 0);
                }
                if (rotateCameraCanvasGameObject != null)
                {
                    var eulerAngles = rotateCameraCanvasGameObject.transform.eulerAngles;
                    rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(eulerAngles.x, Mathf.SmoothDamp(eulerAngles.y, (_currentRotateDir ? 180 : 0), ref _rotateFlipSmoothDampVelRef, 0.1f, Mathf.Infinity,  GetTimeScale()), rotateCameraCanvasGameObject.transform.eulerAngles.z);
                }
            }
        }
    }

    private void MoveTargetRelativeToCamera(Vector3 direction, float speed)
    {
        float relativeZoomCameraMoveSpeed = (_framingTransposer.m_CameraDistance / cameraZoomMinMax.x);
        Vector3 camForward = _virtualCameraGameObject.transform.forward;
        Vector3 camRight = _virtualCameraGameObject.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 relativeDir = (camForward * direction.z) + (camRight * direction.x);

        cameraTarget.Translate(relativeDir * (relativeZoomCameraMoveSpeed * speed * GetTimeScale()));
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
        _lockedOnPosition = position;
        _lockedOnZoom = zoomFactor;
        _hardLocked = hardLock;
        _isLockedOnTarget = true;
    }

    /// <summary>
    /// Locks the camera to a target transform
    /// </summary>
    /// <param name="transform"></param>
    /// <param name="zoomFactor"></param>

    public void LockOnTarget(Transform transform, float zoomFactor, bool hardLock = false)
    {
        CancelTargetLock();
        _lockedOnTransform = transform;
        _lockedOnZoom = zoomFactor;
        _hardLocked = hardLock;
        _isLockedOnTarget = true;
    }

    /// <summary>
    /// Cancels the target locking
    /// </summary>

    public void CancelTargetLock()
    {
        _isLockedOnTarget = false;
        _lockedOnPosition = Vector3.zero;
        _lockedOnTransform = null;
    }

    #endregion
}

