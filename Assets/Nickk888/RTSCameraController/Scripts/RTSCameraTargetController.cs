using System;
using UnityEngine;
using UnityEditor;
using System.Runtime.CompilerServices;
using Unity.Cinemachine;

[assembly: InternalsVisibleTo("PlayModeTests")]
[assembly: InternalsVisibleTo("EditorModeTests")]

public class RTSCameraTargetController : MonoBehaviour
{
    public static RTSCameraTargetController Instance { get; private set; }

    #region Events

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
        public MouseDragStyle mouseDragStyle;
        public Vector2 mouseLockPosition;
    }
    public event EventHandler OnMouseDragStopped;
    public event EventHandler<OnMouseDragHandledEventArgs> OnMouseDragHandled;
    public class OnMouseDragHandledEventArgs : EventArgs
    {
        public MouseDragStyle mouseDragStyle;
        public bool isMoving;
        public Vector2 mousePosition;
        public Vector2 vectorChange;
    }

    public event EventHandler<OnZoomHandledEventArgs> OnZoomHandled;
    public class OnZoomHandledEventArgs : EventArgs
    {
        public float currentZoomValue;
        public float targetZoomValue;
        public float minZoom;
        public float maxZoom;
    }

    #endregion

    #region Public Fields

    [Header("Setup")]
    [SerializeField]
    [Tooltip("The Cinemachine Virtual Camera to be controlled by the controller.")]
    public CinemachineCamera VirtualCamera;

    [SerializeField] [Tooltip("The target for the camera to follow.")]
    public Transform CameraTarget;

    [SerializeField] [Tooltip("The UI Canvas Rect Transform")]
    public RectTransform RTSCanvasRectTransform;

    [SerializeField] [Tooltip("The ground layer for the height check.")]
    public LayerMask GroundLayer;

    [Space] [Header("Time Scale")]
    [SerializeField] [Tooltip("Check to make the controller be independent on the Time Scale.")]
    public bool IndependentTimeScale = true;

    [SerializeField] [Tooltip("Check to make the Cinemachine Brain be independent on the Time Scale.")]
    public bool IndependentCinemachineBrainTimeScale = true;

    [Space][Header("Properties")]
    [SerializeField][Tooltip("Allows or Disallows rotation of the Camera.")]
    public bool AllowMouseRotate = true;

    [SerializeField] [Tooltip("Allows or Disallows camera rotation with keys/gamepad input.")]
    public bool AllowKeysRotate = true;

    [SerializeField] [Tooltip("Allows or Disallows rotation of the Cameras Tilt.")]
    public bool AllowTiltRotate = true;

    [SerializeField] [Tooltip("Allows or Disallows Zooming.")]
    public bool AllowZoom = true;

    [SerializeField] [Tooltip("Allows or Disallows mouse drag movement.")]
    public bool AllowDragMove = true;

    public enum MouseDragStyle { MouseDirection, Direct, DirectInverted }
    [SerializeField] [Tooltip("The style of mouse Drag.")]
    private MouseDragStyle mouseDragStyle = MouseDragStyle.MouseDirection;

    [SerializeField] [Tooltip("Allows or Disallows camera movement with keys/gamepad input.")]
    public bool AllowKeysMove = true;

    [SerializeField] [Tooltip("Allows or Disallows camera movement using the screen sides.")]
    public bool AllowScreenSideMove = true;

    [SerializeField] [Tooltip("Allows or Disallows camera height offset.")]
    public bool AllowHeightOffsetChange = true;

    [SerializeField] [Tooltip("Lock the mouse while using the rotate feature?")]
    public bool MouseLockOnRotate = true;

    [SerializeField] [Tooltip("Invert the vertical mouse input?")]
    public bool InvertMouseVertical = false;

    [SerializeField] [Tooltip("Invert the horizontal mouse input?")]
    public bool InvertMouseHorizontal = false;

    [Space] [Header("Speed")]
    [SerializeField, Min(0)]
    public float CameraMouseSpeed = 16.0f;

    [SerializeField, Min(0)]
    public float CameraRotateSpeed = 2.0f;

    [SerializeField, Min(0)]
    [Tooltip("This value multiplies the Keys Rotation input which is 1 or -1 depending on the direction.")]
    public float CameraKeysRotateSpeedMultiplier = 50.0f;

    [SerializeField, Min(0)]
    public float CameraKeysSpeed = 6.0f;

    [SerializeField, Min(0)]
    public float CameraScreenSideSpeed = 6.0f;

    [SerializeField, Min(0)]
    public float CameraZoomSpeed = 4f;

    [SerializeField, Min(0)]
    public float TargetLockSpeed = 10f;

    [SerializeField, Min(0)]
    public float HeightOffsetSpeed = 20f;

    [Space] [Header("Screen Sides")]
    [SerializeField, Min(0)] [Tooltip("The size of the Screen Sides Zone in pixels.")]
    public int ScreenSidesZoneSize = 50;

    [Space] [Header("Mouse Drag")]
    [SerializeField, Min(0)] [Tooltip("The Dead Zone of the drag feature. How far from the circles center has the cursor be, to start draging?")]
    public int CameraDragDeadZone = 5;

    [Space]
    [Header("Limits")]
    [SerializeField]
    [Tooltip("The Minimum for the cameras height Offset.")]
    public float HeightOffsetMin = 1.0f;

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
    private Vector2 _currentMousePosition;
    private Vector3 _mouseLockPos;
    private Vector3 _lockedOnPosition;
    private Camera _cam;
    private CinemachineBrain _cinemachineBrain;
    private CinemachinePositionComposer _positionComposer;
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
    private bool _isFocused;

    #endregion

    #region Constants

    private const float MaxRaycastHeight = 9999999f; // Max Raycast Length for the Ground Detection 
    private const float CameraTiltMiddleValue = 0.5f; // The middle value for the camera Tilt

    #endregion

    #region Unity Methods

    protected void Awake()
    {
        _cam = Camera.main;
        if (_cam == null)
        {
            HandleCameraError();
            return;
        }

        InitializeCinemachineBrain();
        SetInstance();
    }

    protected void OnEnable() => Application.focusChanged += Application_FocusChanged;

    protected void OnDisable() => Application.focusChanged -= Application_FocusChanged;

    protected void Start()
    {
        _virtualCameraGameObject = VirtualCamera.gameObject;

        InitializeCameraTilt();
        InitializeFramingTransposer();
        InitializeMousePosition();
        InitializeInputProvider();
        TriggerZoomHandledEvent();
    }

    protected void Update()
    {
        if (_inputProvider == null || !_isFocused)
        {
            CancelTargetLock();
            return;
        }

        UpdateCinemachineBrain();
        _currentMousePosition = _inputProvider.MousePosition();

        HandleCameraInput();
        HandleMovement();
        HandleCameraHeight();
    }

    #if UNITY_EDITOR
    protected void OnDrawGizmosSelected()
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
    #endif


    #endregion

    #region Event Methods

    private void Application_FocusChanged(bool focused) => _isFocused = focused;

    #endregion

    #region Internal Methods
    
    internal void UpdateCinemachineBrain()
        => _cinemachineBrain.IgnoreTimeScale = IndependentCinemachineBrainTimeScale;

    internal void HandleCameraInput()
    {
        HandleScreenSideMove(_currentMousePosition);
        HandleMouseDrag(_currentMousePosition);
        HandleZoom();
        HandleRotation();
    }

    internal void HandleMovement()
    {
        HandleKeysMove();
        HandleTargetLock();
        HandleBoundaries();
    }

    internal void HandleCameraHeight()
    {
        HandleGroundHeightCorrection();
        HandleHeightOffset();
    }
    
    internal void HandleCameraError()
        => Debug.LogError("Main Camera wasn't found. Can't get the Cinemachine Brain.");

    internal void InitializeCinemachineBrain()
        => _cinemachineBrain = _cam.gameObject.GetComponent<CinemachineBrain>();

    internal void SetInstance() => Instance = this;

    internal void InitializeCameraTilt()
    {
        _currentCameraTilt = Mathf.Lerp(CameraTiltMin, CameraTiltMax, CameraTiltMiddleValue);
        _targetCameraTilt = _currentCameraTilt;
        _virtualCameraGameObject.transform.eulerAngles = new Vector3(_currentCameraTilt, _virtualCameraGameObject.transform.eulerAngles.y, 0);
    }

    internal void InitializeFramingTransposer()
    {
        _positionComposer = VirtualCamera.GetComponent<CinemachinePositionComposer>();
        _currentCameraZoom = _positionComposer.CameraDistance;
        _targetCameraRotate = _virtualCameraGameObject.transform.eulerAngles.y;
    }

    internal void InitializeMousePosition()
    {
        _currentMousePosition = new Vector2(Screen.width / 2, Screen.height / 2);
    }

    internal void InitializeInputProvider()
    {
        _inputProvider = GetComponent<IRTSCInputProvider>();
        if (_inputProvider == null)
        {
            Debug.LogError("No Input Provider found! Please ensure there's a input provider script attached to the game object.");
        }
    }

    internal void TriggerZoomHandledEvent()
    {
        OnZoomHandled?.Invoke(this, new OnZoomHandledEventArgs
        {
            currentZoomValue = _positionComposer.CameraDistance,
            targetZoomValue = _currentCameraZoom,
            minZoom = CameraZoomMin,
            maxZoom = CameraZoomMax
        });
    }

    internal void HandleHeightOffset()
    {
        if (!AllowHeightOffsetChange)
            return;

        if(_inputProvider.HeightUpButtonInput())
            _heightOffset += GetTimeScale() * HeightOffsetSpeed;
        else if(_inputProvider.HeightDownButtonInput())
            _heightOffset -= GetTimeScale() * HeightOffsetSpeed;

        _heightOffset = Mathf.Clamp(_heightOffset, HeightOffsetMin, HeightOffsetMax);
        _positionComposer.TargetOffset = new Vector3(0, _heightOffset, 0);
    }

    internal void HandleGroundHeightCorrection()
    {
        if (_isLockedOnTarget)
            return;

        if (TryGetGroundHeight(out float groundHeight))
        {
            CorrectTargetHeight(groundHeight);
        }
    }

    internal bool TryGetGroundHeight(out float groundHeight)
    {
        Ray ray = new Ray(new Vector3(CameraTarget.position.x, MaxRaycastHeight, CameraTarget.position.z), Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GroundLayer))
        {
            groundHeight = hit.point.y;
            return true;
        }

        groundHeight = 0f;
        return false;
    }

    internal void CorrectTargetHeight(float targetHeight)
    {
        Vector3 position = CameraTarget.position;
        position.y = Mathf.Lerp(position.y, targetHeight, CameraTargetGroundHeightCheckSmoothTime * GetTimeScale());
        CameraTarget.position = position;
    }

    internal void HandleTargetLock()
    {
        if (_isLockedOnTarget)
        {
            if(_lockedOnTransform == null)
                CameraTarget.position = _hardLocked ? _lockedOnPosition : Vector3.Lerp(CameraTarget.position, _lockedOnPosition, TargetLockSpeed * GetTimeScale());
            else
                CameraTarget.position = _hardLocked ? _lockedOnTransform.position : Vector3.Lerp(CameraTarget.position, _lockedOnTransform.position, TargetLockSpeed * GetTimeScale());
            _currentCameraZoom = Mathf.Lerp(_currentCameraZoom, _lockedOnZoom, TargetLockSpeed * GetTimeScale());
        }
    }

    internal void HandleScreenSideMove(Vector2 mousePos)
    {
        Vector3 moveVector = GetMoveVectorFromMousePosition(mousePos);
        
        if (ShouldMoveTarget(moveVector) && !IsMousePositionOutsideScreen(mousePos))
        {
            MoveTarget(moveVector);
            _isSideZoneMoving = true;
        }
        else
        {
            _isSideZoneMoving = false;
        }
    }

    internal Vector3 GetMoveVectorFromMousePosition(Vector2 mousePos)
    {
        GetMouseScreenSideXY(mousePos, out int x, out int y);
        return new Vector3(x, 0, y);
    }

    internal bool ShouldMoveTarget(Vector3 moveVector)
        => moveVector != Vector3.zero && !_isDragging && !_isRotating && AllowScreenSideMove;

    internal void MoveTarget(Vector3 moveVector)
    {
        CancelTargetLock();
        MoveTargetRelativeToCamera(moveVector, CameraScreenSideSpeed);
    }

    internal void HandleKeysMove()
    {
        if (!AllowKeysMove || !CanProcessKeyMovement())
            return;

        Vector3 movementVector = GetMovementVector();
        if (movementVector != Vector3.zero)
        {
            CancelTargetLock();
            MoveTargetRelativeToCamera(movementVector, CameraKeysSpeed);
        }
    }

    internal bool CanProcessKeyMovement() => !_isDragging && !_isSideZoneMoving;

    internal Vector3 GetMovementVector()
    {
        Vector2 movementInput = _inputProvider.MovementInput();
        return new Vector3(movementInput.x, 0, movementInput.y);
    }

    internal void HandleMouseDrag(Vector3 mousePos)
    {
        if (_isRotating || _isSideZoneMoving) return;

        switch (mouseDragStyle)
        {
            case MouseDragStyle.MouseDirection:
                HandleMouseDirectionDrag(mousePos);
                break;

            case MouseDragStyle.DirectInverted:
            case MouseDragStyle.Direct:
                HandleDirectDrag(mousePos);
                break;
        }
    }

    internal void HandleMouseDirectionDrag(Vector3 mousePos)
    {
        if (_inputProvider.DragButtonInput() && AllowDragMove && !_isDragging)
        {
            StartDrag(mousePos, MouseDragStyle.MouseDirection);
        }
        else if (ShouldStopDrag())
        {
            StopDrag();
        }
        else if (_isDragging && AllowDragMove)
        {
            ProcessMouseDrag(mousePos);
        }
    }

    internal void HandleDirectDrag(Vector3 mousePos)
    {
        bool isInverted = mouseDragStyle == MouseDragStyle.DirectInverted;

        if (_inputProvider.DragButtonInput() && AllowDragMove && !_isDragging)
        {
            StartDrag(mousePos, mouseDragStyle);
        }
        else if (ShouldStopDrag())
        {
            StopDrag();
        }
        else if (_isDragging && AllowDragMove)
        {
            Vector3 vectorChange = _inputProvider.MouseInput();
            vectorChange.z = vectorChange.y;
            vectorChange.y = 0;
            MoveTargetRelativeToCamera(isInverted ? -vectorChange : vectorChange, CameraMouseSpeed);
        }
    }

    internal void StartDrag(Vector3 mousePos, MouseDragStyle dragStyle)
    {
        bool isMouseDirectional = dragStyle == MouseDragStyle.MouseDirection;
        _mouseLockPos = mousePos;
        _isDragging = true;
        CancelTargetLock();
        LockMouse(!isMouseDirectional);
        OnMouseDragStarted?.Invoke(this, new OnMouseDragStartedEventArgs { mouseLockPosition = mousePos, mouseDragStyle = dragStyle });
    }

    internal void StopDrag()
    {
        _isDragging = false;
        LockMouse(false);
        OnMouseDragStopped?.Invoke(this, EventArgs.Empty);
    }

    internal bool ShouldStopDrag()
        => _isDragging && (!_inputProvider.DragButtonInput() || !AllowDragMove);

    internal void ProcessMouseDrag(Vector3 mousePos)
    {
        Vector3 vectorChange = new Vector3(_mouseLockPos.x - mousePos.x, 0, _mouseLockPos.y - mousePos.y) * -1;
        float distance = vectorChange.sqrMagnitude;
        bool canMove = distance > (CameraDragDeadZone * CameraDragDeadZone);
        Cursor.visible = !canMove;

        if (canMove)
            MoveTargetRelativeToCamera(vectorChange, CameraMouseSpeed / 100);

        OnMouseDragHandled?.Invoke(this, new OnMouseDragHandledEventArgs { isMoving = canMove, mousePosition = mousePos });
    }

    internal void HandleZoom()
    {
        if (AllowZoom)
        {
            HandleZoomInput();
        }

        UpdateCameraDistance();
        CheckAndInvokeZoomEvent();
    }

    internal void HandleZoomInput()
    {
        float zoomInput = _inputProvider.ZoomInput();
        if (zoomInput != 0)
        {
            // A hacky fix for a WebGL Build Bug
            #if !UNITY_EDITOR && UNITY_WEBGL
            _currentCameraZoom -= zoomInput * (CameraZoomSpeed * 0.01f);
            #else
            _currentCameraZoom -= zoomInput * CameraZoomSpeed;
            #endif
            //
            _currentCameraZoom = Mathf.Clamp(_currentCameraZoom, CameraZoomMin, CameraZoomMax);
            CancelTargetLock();
        }
    }

    internal void UpdateCameraDistance()
    {
        _positionComposer.CameraDistance = Mathf.SmoothDamp(
            _positionComposer.CameraDistance,
            _currentCameraZoom,
            ref _cameraZoomSmoothDampVelRef,
            CameraZoomSmoothTime / 100,
            Mathf.Infinity,
            GetTimeScale()
        );
    }

    internal void CheckAndInvokeZoomEvent()
    {
        if (Math.Round(_positionComposer.CameraDistance - _currentCameraZoom, 4) != 0)
        {
            OnZoomHandled?.Invoke(this, new OnZoomHandledEventArgs
            {
                currentZoomValue = _positionComposer.CameraDistance,
                targetZoomValue = _currentCameraZoom,
                minZoom = CameraZoomMin,
                maxZoom = CameraZoomMax
            });
        }
    }
    
    internal void HandleRotation()
    {
        if (!_isDragging)
        {
            HandleRotationStartStop();

            if (_isRotating)
            {
                HandleRotationInput();
            }
        }

        SmoothRotate();
    }

    internal void HandleRotationStartStop()
    {
        bool rotationMousePressed = _inputProvider.RotationButtonInput() && AllowMouseRotate;
        bool rotationKeysPressed = (_inputProvider.RotateRightButtonInput() || _inputProvider.RotateLeftButtonInput()) && AllowKeysRotate;

        // Start Rotation
        if (!_isRotating && (rotationMousePressed || rotationKeysPressed))
        {
            if (MouseLockOnRotate) LockMouse(true);
            _isRotating = true;
            OnRotateStarted?.Invoke(this, EventArgs.Empty);
        }

        // Stop Rotation
        bool mouseRelease = !_inputProvider.RotationButtonInput() && AllowMouseRotate || !AllowMouseRotate;
        bool keysRelease = (!_inputProvider.RotateRightButtonInput() && !_inputProvider.RotateLeftButtonInput() && AllowKeysRotate) || !AllowKeysRotate;
        if (_isRotating && mouseRelease && keysRelease)
        {
            if (MouseLockOnRotate) LockMouse(false);
            _isRotating = false;
            OnRotateStopped?.Invoke(this, EventArgs.Empty);
        }
    }

    internal void HandleRotationInput()
    {
        Vector2 rotationInput = GetRotationInput();

        if (rotationInput.x != 0)
        {
            _currentRotateDir = rotationInput.x > 0;
            _targetCameraRotate += rotationInput.x * CameraRotateSpeed * (InvertMouseHorizontal ? -1 : 1);
        }

        if (rotationInput.y != 0 && AllowTiltRotate)
        {
            _targetCameraTilt -= rotationInput.y * CameraRotateSpeed * (InvertMouseVertical ? -1 : 1);
            _targetCameraTilt = Mathf.Clamp(_targetCameraTilt, CameraTiltMin, CameraTiltMax);
        }
    }

    internal Vector2 GetRotationInput()
    {
        if (_inputProvider.MouseInput() != Vector2.zero && AllowMouseRotate && !_inputProvider.RotateRightButtonInput() && !_inputProvider.RotateLeftButtonInput())
            return _inputProvider.MouseInput();

        if (_inputProvider.RotateRightButtonInput() && AllowKeysRotate)
            return -Vector2.right * GetTimeScale() * CameraKeysRotateSpeedMultiplier;

        if (_inputProvider.RotateLeftButtonInput() && AllowKeysRotate)
            return Vector2.right * GetTimeScale() * CameraKeysRotateSpeedMultiplier;

        return Vector2.zero;
    }

    internal void SmoothRotate()
    {
        _currentCameraRotate = Mathf.SmoothDamp(_currentCameraRotate, _targetCameraRotate, ref _cameraRotateSmoothDampVelRef, CameraTargetRotateSmoothTime / 100, Mathf.Infinity, GetTimeScale());
        _currentCameraTilt = Mathf.SmoothDamp(_currentCameraTilt, _targetCameraTilt, ref _cameraTiltSmoothDampVelRef, CameraTargetRotateSmoothTime / 100, Mathf.Infinity, GetTimeScale());
        _virtualCameraGameObject.transform.eulerAngles = new Vector3(_currentCameraTilt, _currentCameraRotate, 0);
        OnRotateHandled?.Invoke(this, new OnRotateHandledEventArgs { clockwise = _currentRotateDir, currentRotation = new Vector2(_currentCameraRotate, _currentCameraTilt), targetRotation = new Vector2(_targetCameraRotate, _targetCameraTilt) });
    }

    internal void LockMouse(bool lockMouse)
    {
        Cursor.lockState = lockMouse ? CursorLockMode.Locked : CursorLockMode.None;
        Cursor.visible = lockMouse ? false : true;
    }

    internal void HandleBoundaries()
    {
        float clampedX = Mathf.Clamp(CameraTarget.position.x, BoundaryMinX, BoundaryMaxX);
        float clampedZ = Mathf.Clamp(CameraTarget.position.z, BoundaryMinZ, BoundaryMaxZ);
        
        CameraTarget.position = new Vector3(clampedX, CameraTarget.position.y, clampedZ);
    }

    internal void MoveTargetRelativeToCamera(Vector3 direction, float speed)
    {
        float zoomAdjustedSpeed = CalculateZoomAdjustedSpeed(speed);
        Vector3 relativeDirection = GetRelativeDirection(direction);

        CameraTarget.Translate(relativeDirection * zoomAdjustedSpeed * GetTimeScale());
    }

    internal float CalculateZoomAdjustedSpeed(float speed) => _positionComposer.CameraDistance / CameraZoomMin * speed;

    internal Vector3 GetRelativeDirection(Vector3 inputDirection)
    {
        Vector3 camForward = _virtualCameraGameObject.transform.forward;
        Vector3 camRight = _virtualCameraGameObject.transform.right;

        // Project onto the XZ plane
        camForward.y = 0f;
        camRight.y = 0f;
        camForward.Normalize();
        camRight.Normalize();

        return (camForward * inputDirection.z) + (camRight * inputDirection.x);
    }

    internal void GetMouseScreenSideXY(Vector2 mousePosition, out int x, out int y)
    {
        x = GetEdgeDirection(mousePosition.x, Screen.width);
        y = GetEdgeDirection(mousePosition.y, Screen.height);
    }

    internal bool IsMousePositionOutsideScreen(Vector2 mousePosition)
        => IsOutsideBounds(mousePosition.x, 0, Screen.width) || IsOutsideBounds(mousePosition.y, 0, Screen.height);

    internal bool IsOutsideBounds(float value, float min, float max)
        => value < min || value > max;

    internal int GetEdgeDirection(float position, int screenSize)
    {
        // Canvas Scale to make the Sides Zone the same Size at any Resolution
        float canvasSize = RTSCanvasRectTransform.localScale.x;

        if (position >= 0 && position <= (ScreenSidesZoneSize * canvasSize))
            return -1; // Near the start (Left/Top)
        else if (position >= screenSize - (ScreenSidesZoneSize * canvasSize) && position <= screenSize)
            return 1; // Near the end (Right/Bottom)
        else
            return 0; // Not near any edge
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Locks the camera to a target (position or transform)
    /// </summary>
    /// <param name="target">The target (either position or transform) to lock to</param>
    /// <param name="zoomFactor">The zoom factor to apply</param>
    /// <param name="hardLock">Whether the lock is hard or soft</param>
    public void LockOnTarget(object target, float zoomFactor, bool hardLock = false)
    {
        CancelTargetLock();

        if (target is Vector3 position)
        {
            _lockedOnPosition = position;
            _lockedOnTransform = null;
        }
        else if (target is Transform transform)
        {
            _lockedOnTransform = transform;
            _lockedOnPosition = Vector3.zero;
        }
        else
        {
            Debug.LogError("Invalid target type. Expected Vector3 or Transform.");
            return;
        }

        _heightOffset = HeightOffsetMin;
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

    /// <summary>
    /// Get's the corresponding Delta Time depending on the Independent Time Scale variable.
    /// </summary>
    /// <returns></returns>
    public float GetTimeScale() => IndependentTimeScale ? Time.unscaledDeltaTime : Time.deltaTime;

    #endregion
}

