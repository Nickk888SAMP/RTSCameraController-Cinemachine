using System;
using System.Collections;
using System.Collections.Generic;
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
    {
        cameraTargetController = GetComponent<RTSCameraTargetController>();
    }

    private void Start()
    {
        mouseDragCanvasGameObject.SetActive(false);
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

    private void RTSCameraTargetController_OnRotateHandled(object sender, RTSCameraTargetController.OnRotateHandledEventArgs e)
    {
        rotateCameraCanvasGameObject.transform.rotation = Quaternion.Euler(0, 
            Mathf.SmoothDamp(
                rotateCameraCanvasGameObject.transform.eulerAngles.y, 
                e.clockwise ? 180 : 0, 
                ref _rotateFlipSmoothDampVelRef,
                0.2f, Mathf.Infinity,  cameraTargetController.GetTimeScale()), e.currentRotation.x * (e.clockwise ? 1 : -1));

        compassUiImageGameObject.transform.rotation = Quaternion.Euler(0, 0, e.currentRotation.x);
    }

    private void RTSCameraTargetController_OnRotateStopped(object sender, EventArgs e)
    {
        rotateCameraCanvasGameObject.gameObject.SetActive(false);
    }

    private void RTSCameraTargetController_OnRotateStarted(object sender, EventArgs e)
    {
        rotateCameraCanvasGameObject.gameObject.SetActive(true);
    }

    private void RTSCameraTargetController_OnMouseDragStarted(object sender, RTSCameraTargetController.OnMouseDragStartedEventArgs e)
    {
        mouseDragStartPoint.transform.position = new Vector2(e.mouseLockPosition.x, e.mouseLockPosition.y);
        mouseDragCanvasGameObject.SetActive(true);
    }

    private void RTSCameraTargetController_OnMouseDragStopped(object sender, EventArgs e)
    {
        mouseDragCanvasGameObject.SetActive(false);
    }
   
    private void RTSCameraTargetController_OnMouseDragHandled(object sender, RTSCameraTargetController.OnMouseDragHandledEventArgs e)
    {
        mouseDragEndPoint.transform.position = e.mousePosition;
        Vector3 dir = (Vector3)e.mousePosition - mouseDragStartPoint.transform.position;
        mouseDragEndPoint.transform.right = dir;
        mouseDragEndPoint.SetActive(e.isMoving);
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
