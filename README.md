# Unity 3D Real Time Strategy Camera Controller for Cinemachine Virtual Camera

<img src="/Preview/1.gif" width="250" height="250"><img src="/Preview/2.gif" width="250" height="250"><img src="/Preview/3.gif" width="250" height="250">

This project is a Camera Controller for RTS style games.
It works by adding the script to a Gameobject and placing references for the Virtual Camera and the Target itself.
The script controlls the Target and the Virtual Camera.

## Features
* RTS Style Rotation (With Tilt)
* RTS Style Zoom
* RTS Style "WASD" Camera Movement
* RTS Style Mouse "Drag" Camera Movement
* RTS Style Screen Sides Camera Movement
* RTS Style Position and Transform Lock-On-Target
* Automatic Ground detection allowing Camera to move up and down depending on the ground.
* UI Controller (Compas, Zoom Slider, Drag Images, Rotate Image)
* Exposed Script as a Singleton to get access to controller from every script.

## Requirements
* Cinemachine
* Old Input System

## How To Use
* Make sure you have installed the "Cinemachine" package from the Package Manager.
* If you are using the New Input System, change the "Active Input Handling" to "Both" inside "Project Settings" > "Player" > "Other Settings"
* If any, delete the default Main Camera from the Scene.
* Place the "RTSCameraController" (located in the "Prefabs" directory) to your Scene.
* Have Fun!

WASD or Middle Mouse Button - Camera Movement

Right Mouse Button - Rotation

Mouse Scroll Wheel - Zoom-In/Zoom-Out

## Properties (Get & Set)
```csharp
RTSCameraTargetController.Instance.VirtualCamera;
RTSCameraTargetController.Instance.GroundLayer;
RTSCameraTargetController.Instance.CameraTarget;
RTSCameraTargetController.Instance.IndependentTimeScale;
RTSCameraTargetController.Instance.IndependentCinemachineBrainTimeScale;
RTSCameraTargetController.Instance.AllowRotate;
RTSCameraTargetController.Instance.AllowTiltRotate;
RTSCameraTargetController.Instance.AllowZoom;
RTSCameraTargetController.Instance.AllowDragMove;
RTSCameraTargetController.Instance.AllowTiltRotate;
RTSCameraTargetController.Instance.AllowKeysMove;
RTSCameraTargetController.Instance.AllowScreenSideMove;
RTSCameraTargetController.Instance.CameraTiltMinMax;
RTSCameraTargetController.Instance.CameraMouseSpeed;
RTSCameraTargetController.Instance.CameraRotateSpeed;
RTSCameraTargetController.Instance.CameraKeysSpeed;
RTSCameraTargetController.Instance.CameraZoomSpeed;
RTSCameraTargetController.Instance.CameraMoveDeadZone;
RTSCameraTargetController.Instance.ScreenSidesZoneSize;
RTSCameraTargetController.Instance.TargetLockSpeed;
RTSCameraTargetController.Instance.CameraTargetGroundHeightCheckSmoothTime;
RTSCameraTargetController.Instance.CameraZoomSmoothTime;
RTSCameraTargetController.Instance.CameraZoomMinMax;
RTSCameraTargetController.Instance.CameraZoomSlider;
RTSCameraTargetController.Instance.MouseDragCanvasGameObject;
RTSCameraTargetController.Instance.MouseDragStartPoint;
RTSCameraTargetController.Instance.MouseDragEndPoint;
RTSCameraTargetController.Instance.RotateCameraCanvasGameObject;
RTSCameraTargetController.Instance.CompasUiImageGameObject;
```