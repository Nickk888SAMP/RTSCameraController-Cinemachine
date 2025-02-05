# Unity 3D Real Time Strategy/City Builder Camera Controller for Cinemachine Virtual Camera

<img src="/Preview/1.gif" width="400" height="175"><img src="/Preview/2.gif" width="400" height="175">
<img src="/Preview/3.gif" width="400" height="175"><img src="/Preview/6.gif" width="400" height="175">
<img src="/Preview/8.gif" width="400" height="175"><img src="/Preview/7.gif" width="400" height="175">
<img src="/Preview/5.gif" width="400" height="175"><img src="/Preview/4.gif" width="400" height="175">

This project is a Camera Controller for RTS style games, it'll also fit perfectly into a City Builder!
It works by adding the script to a Gameobject and placing references for the Virtual Camera and the Target itself.
The script controlls the Target and the Virtual Camera. Use ANY Input System using the Input Provider system.

## Features
* RTS Style Rotation (With Tilt)
* RTS Style Zoom
* RTS Style "WASD" Camera Movement
* RTS Style Mouse "Drag" Camera Movement
* RTS Style Screen Sides Camera Movement
* RTS Style Position and Transform Lock-On-Target
* Up and Down camera offset.
* Zoom based movement speed.
* Rotate Camera via keys.
* Boundary System confines the camera in a specific area.
* Invert mouse input.
* Limits and smoothing.
* Framerate independent, no matter how fast or slow the game runs, it works the same.
* Timescale independent, you can "pause" the game and still use the controller.
* An Input Provider System that lets you connect ANY Input System with the controller.
* Automatic Ground detection allowing Camera to move up and down depending on the ground.
* Exposed Script as a Singleton to get access to controller from every script.
* Works exceptionally well with Gamepads when used with the Unity's Input System.
* Optional UI Controller (Compas, Zoom Slider, Drag Images, Rotate Image)
* It's smooth, it's lightweight and it just works.

## Requirements
* Unity 2022.3.11f1 or higher
* Cinemachine 2
* TextMeshPro (For the UI)

## Upgrading to Cinemachine 3?
[Change to Experimental Branch](https://github.com/Nickk888SAMP/RTSCameraController-Cinemachine/tree/Cinemachine3-Experimental)

## Want to use Unity's Input System?
To get the files, unpack ```RTSCameraController_NewInputSystem.unitypackage``` that's located in the ```RTSCameraController``` directory while in the editor.
Use the ```RTSCameraController_NewInputSystem.prefab``` instead or just swap the ```InputProvider_OldInputSystem.cs``` with ```InputProvider_NewInputSystem.cs``` in the ```RTSCameraController``` Game Object. Makes sure to install the ```Input System``` package from the Package Manager of course.

## Create your own input provider script
Just create a new Script and inherit from the  ```IRTSCInputProvider``` interface and let your code editor automatically populate with all the methods from the ```IRTSCInputProvider```.
Return the values (Bool, float or Vector2) to the specific Methods.
Place the script alongside the Controller script and that's it.
Just make sure there's only one Input Provider placed as a component.
You can use the premade Input Providers as a guide.

## How To Use
* Make sure you have installed the "Cinemachine" package from the Package Manager.
* If any, delete the default Main Camera from the Scene.
* Place the "RTSCameraController" (located in the "Prefabs" directory) to your Scene.
* Have Fun!

Default Settings:

WASD or Middle Mouse Button - Camera Movement

Right Mouse Button or Q or E - Rotation

Mouse Scroll Wheel - Zoom-In/Zoom-Out

R or F - Height Change

[![forthebadge](https://forthebadge.com/images/featured/featured-built-with-love.svg)](https://forthebadge.com)
