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

## Requirenment
* Cinemachine
* Old Input System

## How To Use
* IMPORTANT: Make sure you have installed the "Cinemachine" package before importing!
Delete the Camera from the Scene (If exists).
Place the "RTSCameraController" (located in the "Prefabs" directory) to your Scene.
That's all.

WASD or Middle Mouse Button - Camera Movement

Right Mouse Button - Rotation

Mouse Scroll Wheel - Zoom-In/Zoom-Out
