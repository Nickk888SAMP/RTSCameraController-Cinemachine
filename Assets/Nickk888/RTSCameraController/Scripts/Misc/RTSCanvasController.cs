using System;
using UnityEngine;
using UnityEngine.UI;

public class RTSCanvasController : MonoBehaviour 
{
    [Space]
    [SerializeField] [Header("Camera Zoom Slider")]
    private Slider cameraZoomSlider;
    public Slider CameraZoomSlider { get => cameraZoomSlider; set => cameraZoomSlider = value; }

    [Space] [Header("Drag Mouse Canvas")]
    [SerializeField]
    private GameObject mouseDragCanvasGameObject;
    public GameObject MouseDragCanvasGameObject { get => mouseDragCanvasGameObject; set => mouseDragCanvasGameObject = value; }

    [SerializeField]
    private GameObject mouseDragStartPoint;
    public GameObject MouseDragStartPoint { get => mouseDragStartPoint; set => mouseDragStartPoint = value; }

    [SerializeField]
    private GameObject mouseDragEndPoint;
    public GameObject MouseDragEndPoint { get => mouseDragEndPoint; set => mouseDragEndPoint = value; }

    [Space] [Header("Direct Drag Canvas")]
    [SerializeField]
    private GameObject dragUiImageGameObject;
    public GameObject DragUiImageGameObject { get => dragUiImageGameObject; set => dragUiImageGameObject = value; }

    [Space] [Header("Rotate Camera Canvas")]
    [SerializeField]
    private GameObject rotateCameraCanvasGameObject;
    public GameObject RotateCameraCanvasGameObject { get => rotateCameraCanvasGameObject; set => rotateCameraCanvasGameObject = value; }

    [SerializeField]
    private GameObject compassUiImageGameObject;
    public GameObject CompassUiImageGameObject { get => compassUiImageGameObject; set => compassUiImageGameObject = value; }


    private RTSCameraTargetController cameraTargetController;
    private float _rotateFlipSmoothDampVelRef;

    private void Awake() 
        => cameraTargetController = GetComponent<RTSCameraTargetController>();

    private void Start()
    {
        mouseDragCanvasGameObject.SetActive(false);
        dragUiImageGameObject.gameObject.SetActive(false);
        rotateCameraCanvasGameObject.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        cameraTargetController.OnZoomHandled += RTSCameraTargetController_OnZoomHandled;

        cameraTargetController.OnMouseDragHandled += RTSCameraTargetController_OnMouseDragHandled;
        cameraTargetController.OnMouseDragStarted += RTSCameraTargetController_OnMouseDragStarted;
        cameraTargetController.OnMouseDragStopped += RTSCameraTargetController_OnMouseDragStopped;

        cameraTargetController.OnRotateStarted += RTSCameraTargetController_OnRotateStarted;
        cameraTargetController.OnRotateStopped += RTSCameraTargetController_OnRotateStopped;
        cameraTargetController.OnRotateHandled += RTSCameraTargetController_OnRotateHandled;
    }


    private void OnDisable()
    {
        cameraTargetController.OnZoomHandled -= RTSCameraTargetController_OnZoomHandled;

        cameraTargetController.OnMouseDragStarted -= RTSCameraTargetController_OnMouseDragStarted;
        cameraTargetController.OnMouseDragStopped -= RTSCameraTargetController_OnMouseDragStopped;
        cameraTargetController.OnMouseDragHandled -= RTSCameraTargetController_OnMouseDragHandled;

        cameraTargetController.OnRotateStarted -= RTSCameraTargetController_OnRotateStarted;
        cameraTargetController.OnRotateStopped -= RTSCameraTargetController_OnRotateStopped;
        cameraTargetController.OnRotateHandled -= RTSCameraTargetController_OnRotateHandled;
    }

    internal void RTSCameraTargetController_OnRotateHandled(object sender, RTSCameraTargetController.OnRotateHandledEventArgs e)
    {
        float smoothSpeed = 0.2f;
        rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(0, 
            Mathf.SmoothDamp(
                rotateCameraCanvasGameObject.transform.eulerAngles.y, 
                e.clockwise ? 180 : 0, 
                ref _rotateFlipSmoothDampVelRef,
                smoothSpeed, Mathf.Infinity,  cameraTargetController.GetTimeScale()), e.currentRotation.x * (e.clockwise ? 1 : -1));

        compassUiImageGameObject.transform.rotation = Quaternion.Euler(0, 0, e.currentRotation.x);
    }

    private void RTSCameraTargetController_OnRotateStopped(object sender, EventArgs e) 
        => rotateCameraCanvasGameObject.gameObject.SetActive(false);

    private void RTSCameraTargetController_OnRotateStarted(object sender, EventArgs e) 
        => rotateCameraCanvasGameObject.gameObject.SetActive(true);

    private void RTSCameraTargetController_OnMouseDragStarted(object sender, RTSCameraTargetController.OnMouseDragStartedEventArgs e)
    {
        if(e.mouseDragStyle == RTSCameraTargetController.MouseDragStyle.MouseDirection)
        {
            mouseDragStartPoint.transform.position = new Vector2(e.mouseLockPosition.x, e.mouseLockPosition.y);
            mouseDragCanvasGameObject.SetActive(true);
        }
        else if(e.mouseDragStyle == RTSCameraTargetController.MouseDragStyle.Direct || e.mouseDragStyle == RTSCameraTargetController.MouseDragStyle.DirectInverted)
        {
            dragUiImageGameObject.gameObject.SetActive(true);
        }
    }

    private void RTSCameraTargetController_OnMouseDragStopped(object sender, EventArgs e) 
    {
        mouseDragCanvasGameObject.SetActive(false);
        dragUiImageGameObject.gameObject.SetActive(false);
    }
   
    private void RTSCameraTargetController_OnMouseDragHandled(object sender, RTSCameraTargetController.OnMouseDragHandledEventArgs e)
    {
        if (e.mouseDragStyle == RTSCameraTargetController.MouseDragStyle.MouseDirection)
        {
            mouseDragEndPoint.transform.position = e.mousePosition;
            Vector3 dir = (Vector3)e.mousePosition - mouseDragStartPoint.transform.position;
            mouseDragEndPoint.transform.right = dir;
            mouseDragEndPoint.SetActive(e.isMoving);
        }
    }

    private void RTSCameraTargetController_OnZoomHandled(object sender, RTSCameraTargetController.OnZoomHandledEventArgs e)
    {
        if (cameraZoomSlider != null)
        {
            cameraZoomSlider.minValue = e.minZoom;
            cameraZoomSlider.maxValue = e.maxZoom;
            cameraZoomSlider.value = e.currentZoomValue;
        }
    }
}
