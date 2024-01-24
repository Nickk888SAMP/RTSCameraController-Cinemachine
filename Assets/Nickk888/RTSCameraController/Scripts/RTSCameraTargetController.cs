using UnityEngine;
using Cinemachine;
using UnityEditor;
using System;

public class RTSCameraTargetController : MonoBehaviour
{
    public static RTSCameraTargetController Instance;

    public event EventHandler OnRotateStarted;
    public event EventHandler OnRotateStopped;
    public event EventHandler<OnRotateHandledEventArgs> OnRotateHandled;
    public class OnRotateHandledEventArgs : EventArgs
    {
        public Vector2 currentRotation;
        public Vector2 targetRotation;
        public bool clockwise;
    }

    public event EventHandler<OnMouseDragStartedEventArgs> OnMouseDragStarted;
    public class OnMouseDragStartedEventArgs : EventArgs
    {
        public Vector2 mouseLockPosition;
    }
    public event EventHandler OnMouseDragStopped;
    public event EventHandler<OnMouseDragHandledEventArgs> OnMouseDragHandled;
    public class OnMouseDragHandledEventArgs : EventArgs
    {
        public bool isMoving;
        public Vector2 mousePosition;
    }

    public event EventHandler<OnZoomHandledEventArgs> OnZoomHandled;
    public class OnZoomHandledEventArgs : EventArgs
    {
        public float currentZoomValue;
        public float targetZoomValue;
        public float minZoom;
        public float maxZoom;
    }
    #region Properties

    [Header("Setup")]
    [SerializeField]
    [Tooltip("The Cinemachine Virtual Camera to be controlled by the controller.")]
    public CinemachineVirtualCamera VirtualCamera;

    [SerializeField] [Tooltip("The ground layer for the height check.")]
    public LayerMask GroundLayer;

    [SerializeField] [Tooltip("The target for the camera to follow.")]
    public Transform CameraTarget;

    [Space] [Header("Time Scale")]
    [SerializeField] [Tooltip("Check to make the controller be independent on the Time Scale.")]
    public bool IndependentTimeScale = true;

    [SerializeField] [Tooltip("Check to make the Cinemachine Brain be independent on the Time Scale.")]
    public bool IndependentCinemachineBrainTimeScale = true;

    [Space][Header("Properties")]
    [SerializeField][Tooltip("Allows or Disallows rotation of the Camera.")]
    public bool AllowRotate = true;

    [SerializeField] [Tooltip("Allows or Disallows rotation of the Cameras Tilt.")]
    public bool AllowTiltRotate = true;

    [SerializeField] [Tooltip("Allows or Disallows Zooming.")]
    public bool AllowZoom = true;

    [SerializeField] [Tooltip("Allows or Disallows mouse drag movement.")]
    public bool AllowDragMove = true;

    [SerializeField] [Tooltip("Allows or Disallows camera movement with keys/gamepad input.")]
    public bool AllowKeysMove = true;

    [SerializeField] [Tooltip("Allows or Disallows camera movement using the screen sides.")]
    public bool AllowScreenSideMove = true;

    [SerializeField] [Tooltip("Lock the mouse while using the rotate feature?")]
    public bool MouseLockOnRotate = true;

    [SerializeField] [Tooltip("Invert the vertical mouse input?")]
    public bool InvertMouseVertical = false;

    [SerializeField] [Tooltip("Invert the horizontal mouse input?")]
    public bool InvertMouseHorizontal = false;

    [Space] [Header("Speed")]
    [SerializeField, Min(0)]
    public float CameraMouseSpeed = 2.0f;

    [SerializeField, Min(0)]
    public float CameraRotateSpeed = 2.0f;

    [SerializeField, Min(0)]
    public float CameraKeysSpeed = 6.0f;

    [SerializeField, Min(0)]
    public float CameraScreenSideSpeed = 6.0f;

    [SerializeField, Min(0)]
    public float CameraZoomSpeed = 4f;

    [SerializeField, Min(0)]
    public float TargetLockSpeed = 1.5f;

    [SerializeField, Min(0)]
    public float HeightOffsetSpeed = 20f;

    [Space] [Header("Screen Sides")]
    [SerializeField, Min(0)] [Tooltip("The size of the Screen Sides Zone in pixels.")]
    public float ScreenSidesZoneSize = 60f;

    [Space] [Header("Mouse Drag")]
    [SerializeField, Min(0)] [Tooltip("The Dead Zone of the drag feature. How far from the circles center has the cursor be, to start draging?")]
    public float CameraDragDeadZone = 5f;

    [Space]
    [Header("Limits")]
    [SerializeField]
    [Tooltip("The Minimum for the cameras height Offset.")]
    public float HeightOffsetMin = 0.0f;

    [SerializeField]
    [Tooltip("The Maximum for the cameras height Offset.")]
    public float HeightOffsetMax = 50f;

    [SerializeField, Range(0, 89)] [Tooltip("The Minimum Tilt of the camera.")]
    public float CameraTiltMin = 15f;

    [SerializeField, Range(0, 89)] [Tooltip("The Maximum Tilt of the camera.")]
    public float CameraTiltMax = 75f;

    [SerializeField, Min(0)] [Tooltip("The Minimum zoom factor")]
    public float CameraZoomMin = 5;

    [SerializeField, Min(0)] [Tooltip("The Maximum zoom factor")]
    public float CameraZoomMax = 200;

    [Space] [Header("Smoothing")]
    [SerializeField, Min(0)]
    public float CameraTargetGroundHeightCheckSmoothTime = 4f;

    [SerializeField, Min(0)]
    public float CameraZoomSmoothTime = 7f;

    [SerializeField, Min(0)]
    public float CameraTargetRotateSmoothTime = 4f;

    [Space]
    [Header("Boundaries")]
    [SerializeField]
    public bool enableBoundaries = true;

    [SerializeField, Range(-10000, 0)] public float BoundaryMinX = -500f;

    [SerializeField, Range(0, 10000)] public float BoundaryMaxX = 500f;

    [SerializeField, Range(-10000, 0)] public float BoundaryMinZ = -500f;

    [SerializeField, Range(0, 10000)] public float BoundaryMaxZ = 500f;

    #endregion

    #region Private Fields

    private IRTSCInputProvider _inputProvider;
    private Vector3 _mouseLockPos;
    private Vector3 _lockedOnPosition;
    private Camera _cam;
    private CinemachineBrain _cinemachineBrain;
    private CinemachineFramingTransposer _framingTransposer;
    private GameObject _virtualCameraGameObject;
    private Transform _lockedOnTransform;
    private float _currentCameraZoom;
    private float _cameraZoomSmoothDampVelRef;
    private float _currentCameraRotate;
    private float _currentCameraTilt;
    private float _targetCameraRotate;
    private float _targetCameraTilt;
    private float _cameraRotateSmoothDampVelRef;
    private float _cameraTiltSmoothDampVelRef;
    private float _lockedOnZoom;
    private float _heightOffset;
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
        _virtualCameraGameObject = VirtualCamera.gameObject;
        _currentCameraTilt = Mathf.Lerp(CameraTiltMin, CameraTiltMax, 0.5f);
        _targetCameraTilt = _currentCameraTilt;
        _virtualCameraGameObject.transform.eulerAngles = new Vector3(_currentCameraTilt, _virtualCameraGameObject.transform.eulerAngles.y, 0);
        _framingTransposer = VirtualCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        _currentCameraZoom = _framingTransposer.m_CameraDistance;
        
        _inputProvider = GetComponent<IRTSCInputProvider>();
        if (_inputProvider == null)
            Debug.LogError("No Input Provider found! Please ensure there's a input provider script attached to the game object.");

        OnZoomHandled?.Invoke(this, new OnZoomHandledEventArgs { currentZoomValue = _framingTransposer.m_CameraDistance, targetZoomValue = _currentCameraZoom, minZoom = CameraZoomMin, maxZoom = CameraZoomMax });
    }

    private void Update()
    {
        if (_inputProvider == null)
            return;

        _cinemachineBrain.m_IgnoreTimeScale = IndependentCinemachineBrainTimeScale;
        Vector2 mousePos = _inputProvider.MousePosition();

        HandleScreenSideMove(mousePos);
        HandleMouseDrag(mousePos);
        HandleKeysMove();
        HandleZoom();
        HandleRotation();
        HandleGroundHeightCorrection();
        HandleTargetLock();
        HandleBoundaries();
        HandleHeightOffset();
    }

    private void OnDrawGizmosSelected()
    {
        if(enableBoundaries)
        {
            Handles.color = Color.green;
            Handles.DrawLine(new Vector3(BoundaryMinX, 0, BoundaryMinZ), new Vector3(BoundaryMaxX, 0, BoundaryMinZ));
            Handles.DrawLine(new Vector3(BoundaryMaxX, 0, BoundaryMinZ), new Vector3(BoundaryMaxX, 0, BoundaryMaxZ));
            Handles.DrawLine(new Vector3(BoundaryMinX, 0, BoundaryMinZ), new Vector3(BoundaryMinX, 0, BoundaryMaxZ));
            Handles.DrawLine(new Vector3(BoundaryMinX, 0, BoundaryMaxZ), new Vector3(BoundaryMaxX, 0, BoundaryMaxZ));
            Handles.Label(new Vector3(BoundaryMinX, 0, 0), $"Min X: {BoundaryMinX}");
            Handles.Label(new Vector3(BoundaryMaxX, 0, 0), $"Max X: {BoundaryMaxX}");
            Handles.Label(new Vector3(0, 0, BoundaryMinZ), $"Min Z: {BoundaryMinZ}");
            Handles.Label(new Vector3(0, 0, BoundaryMaxZ), $"Max Z: {BoundaryMaxZ}");
        }
    }

    #endregion

    #region Internal Functions

    private void HandleHeightOffset()
    {
        if(_inputProvider.HeightUpButtonInput())
        {
            _heightOffset += GetTimeScale() * HeightOffsetSpeed;
        }
        else if(_inputProvider.HeightDownButtonInput())
        {
            _heightOffset -= GetTimeScale() * HeightOffsetSpeed;
        }
        _heightOffset = Mathf.Clamp(_heightOffset, HeightOffsetMin, HeightOffsetMax);
        _framingTransposer.m_TrackedObjectOffset = new Vector3(0, _heightOffset, 0);
    }

    private void HandleGroundHeightCorrection()
    {
        if (!_isLockedOnTarget)
        {
            if (Physics.Raycast(new Vector3(CameraTarget.position.x, 9999999, CameraTarget.position.z), Vector3.down, out RaycastHit hit, Mathf.Infinity, GroundLayer))
            {
                Vector3 position = CameraTarget.position;
                position = Vector3.Lerp(position, new Vector3(position.x, hit.point.y, position.z), CameraTargetGroundHeightCheckSmoothTime * GetTimeScale());
                CameraTarget.position = position;
            }
        }
    }

    private void HandleTargetLock()
    {
        if (_isLockedOnTarget)
        {
            if(_lockedOnTransform == null)
            {
                CameraTarget.position = _hardLocked ? _lockedOnPosition : Vector3.Lerp(CameraTarget.position, _lockedOnPosition, TargetLockSpeed * GetTimeScale());
            }
            else
            {
                CameraTarget.position = _hardLocked ? _lockedOnTransform.position : Vector3.Lerp(CameraTarget.position, _lockedOnTransform.position, TargetLockSpeed * GetTimeScale());
            }
            _currentCameraZoom = Mathf.Lerp(_currentCameraZoom, _lockedOnZoom, TargetLockSpeed * GetTimeScale());
        }
    }

    private void HandleScreenSideMove(Vector3 mousePos)
    {
        GetMouseScreenSide(mousePos, out int widthPos, out int heightPos);
        Vector3 moveVector = new Vector3(widthPos, 0, heightPos);
        if (moveVector != Vector3.zero && !_isDragging && !_isRotating && AllowScreenSideMove)
        {
            CancelTargetLock();
            MoveTargetRelativeToCamera(moveVector, CameraScreenSideSpeed);
            _isSideZoneMoving = true;
        }
        else
        {
            _isSideZoneMoving = false;
        }
    }

    private void HandleKeysMove()
    {
        if (!_isDragging && !_isSideZoneMoving && AllowKeysMove)
        {
            Vector2 movementInput = _inputProvider.MovementInput();
            Vector3 vectorChange = new Vector3(movementInput.x, 0, movementInput.y);
            
            // Move target relative to Camera
            if (vectorChange != Vector3.zero)
            {
                CancelTargetLock();
                MoveTargetRelativeToCamera(vectorChange, CameraKeysSpeed);
            }
        }
    }

    private void HandleMouseDrag(Vector3 mousePos)
    {
        if (!_isRotating && !_isSideZoneMoving)
        {
            if (_inputProvider.DragButtonInput() && AllowDragMove && !_isDragging)
            {
                _mouseLockPos = mousePos;
                _isDragging = true;
                CancelTargetLock();
                OnMouseDragStarted?.Invoke(this, new OnMouseDragStartedEventArgs { mouseLockPosition = mousePos });
            }
            if ((_isDragging && !_inputProvider.DragButtonInput()) || (_isDragging && !AllowDragMove))
            {
                Cursor.visible = true;
                _isDragging = false;
                OnMouseDragStopped?.Invoke(this, EventArgs.Empty);
            }
            if (_inputProvider.DragButtonInput() && _isDragging && AllowDragMove)
            {
                Vector3 vectorChange = new Vector3(_mouseLockPos.x - mousePos.x, 0, _mouseLockPos.y - mousePos.y) * -1;
                float distance = vectorChange.sqrMagnitude;
                bool canMove = distance > (CameraDragDeadZone * CameraDragDeadZone);
                Cursor.visible = !canMove;
                if (canMove)
                {
                    // Move target relative to Camera
                    MoveTargetRelativeToCamera(vectorChange, CameraMouseSpeed / 100);
                }
                OnMouseDragHandled?.Invoke(this, new OnMouseDragHandledEventArgs { isMoving = canMove, mousePosition = mousePos });
            }
        }
    }

    private void HandleZoom()
    {
        if (AllowZoom)
        {
            float zoomInput = _inputProvider.ZoomInput();
            _currentCameraZoom -= zoomInput * CameraZoomSpeed;
            _currentCameraZoom = Mathf.Clamp(_currentCameraZoom, CameraZoomMin, CameraZoomMax);
            if(zoomInput!= 0)
            {
                CancelTargetLock();
            }
        }
        _framingTransposer.m_CameraDistance = Mathf.SmoothDamp(_framingTransposer.m_CameraDistance, _currentCameraZoom, ref _cameraZoomSmoothDampVelRef, CameraZoomSmoothTime / 100, Mathf.Infinity, GetTimeScale());
        
        if(Math.Round(_framingTransposer.m_CameraDistance - _currentCameraZoom, 4) != 0)
        {
            OnZoomHandled?.Invoke(this, new OnZoomHandledEventArgs { currentZoomValue = _framingTransposer.m_CameraDistance, targetZoomValue = _currentCameraZoom, minZoom = CameraZoomMin, maxZoom = CameraZoomMax });
        }
    }

    private void HandleRotation()
    {
        if (!_isDragging)
        {
            if (_inputProvider.RotationButtonInput() && AllowRotate)
            {
                if(MouseLockOnRotate) LockMouse(true);
                _isRotating = true;
                OnRotateStarted?.Invoke(this, EventArgs.Empty);
            }
            if ((_isRotating && !_inputProvider.RotationButtonInput()) || (_isRotating && !AllowRotate))
            {
                if(MouseLockOnRotate) LockMouse(false);
                _isRotating = false;
                OnRotateStopped?.Invoke(this, EventArgs.Empty);
            }
            if (_inputProvider.RotationButtonInput() && AllowRotate)
            {
                Vector2 rotationInput = _inputProvider.MouseInput();
                if (rotationInput.x != 0)
                {
                    _currentRotateDir = rotationInput.x > 0 ? true : false;
                    _targetCameraRotate += rotationInput.x * CameraRotateSpeed * (InvertMouseHorizontal ? -1 : 1);
                }
                if (rotationInput.y != 0 && AllowTiltRotate)
                {
                    _targetCameraTilt -= rotationInput.y * CameraRotateSpeed * (InvertMouseVertical ? -1 : 1);
                    _targetCameraTilt = Mathf.Clamp(_targetCameraTilt, CameraTiltMin, CameraTiltMax);
                }
            }
        }
        _currentCameraRotate = Mathf.SmoothDamp(_currentCameraRotate, _targetCameraRotate, ref _cameraRotateSmoothDampVelRef, CameraTargetRotateSmoothTime / 100,  Mathf.Infinity, GetTimeScale());
        _currentCameraTilt = Mathf.SmoothDamp(_currentCameraTilt, _targetCameraTilt, ref _cameraTiltSmoothDampVelRef, CameraTargetRotateSmoothTime / 100, Mathf.Infinity, GetTimeScale());
        _virtualCameraGameObject.transform.eulerAngles = new Vector3(_currentCameraTilt, _currentCameraRotate, 0);
        OnRotateHandled?.Invoke(this, new OnRotateHandledEventArgs { clockwise = _currentRotateDir, currentRotation = new Vector2(_currentCameraRotate, _currentCameraTilt), targetRotation = new Vector2(_targetCameraRotate, _targetCameraTilt)});
    }

    private void LockMouse(bool lockMouse)
    {
        Cursor.lockState = lockMouse ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = lockMouse ? false : true;
    }

    private void HandleBoundaries()
    {
        if (CameraTarget.position.x > BoundaryMaxX)
            CameraTarget.position = new Vector3(BoundaryMaxX, CameraTarget.position.y, CameraTarget.position.z);
        if (CameraTarget.position.x < BoundaryMinX)
            CameraTarget.position = new Vector3(BoundaryMinX, CameraTarget.position.y, CameraTarget.position.z);
        if (CameraTarget.position.z > BoundaryMaxZ)
            CameraTarget.position = new Vector3(CameraTarget.position.x, CameraTarget.position.y, BoundaryMaxZ);
        if (CameraTarget.position.z < BoundaryMinZ)
            CameraTarget.position = new Vector3(CameraTarget.position.x, CameraTarget.position.y, BoundaryMinZ);
    }

    private void MoveTargetRelativeToCamera(Vector3 direction, float speed)
    {
        float relativeZoomCameraMoveSpeed = _framingTransposer.m_CameraDistance / CameraZoomMin;
        Vector3 camForward = _virtualCameraGameObject.transform.forward;
        Vector3 camRight = _virtualCameraGameObject.transform.right;
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();
        Vector3 relativeDir = (camForward * direction.z) + (camRight * direction.x);

        CameraTarget.Translate(relativeDir * (relativeZoomCameraMoveSpeed * speed * GetTimeScale()));
    }

    public float GetTimeScale() => IndependentTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

    private void GetMouseScreenSide(Vector3 mousePosition, out int width, out int height)
    {
        int heightPos = 0;
        int widthPos = 0;
        if(mousePosition.x >= 0 && mousePosition.x <= ScreenSidesZoneSize)
        {
            widthPos = -1;
        }
        else if(mousePosition.x >= Screen.width - ScreenSidesZoneSize && mousePosition.x <= Screen.width)
        {
            widthPos = 1;
        }
        if(mousePosition.y >= 0 && mousePosition.y <= ScreenSidesZoneSize)
        {
            heightPos = -1;
        }
        else if(mousePosition.y >= Screen.height - ScreenSidesZoneSize && mousePosition.y <= Screen.height)
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

