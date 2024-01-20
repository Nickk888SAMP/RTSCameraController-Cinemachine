using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputProvider_NewInputSystem : MonoBehaviour, IRTSCInputProvider
{
    private RTSCC_InputActions inputActions;

    private void Awake()
    {
        inputActions = new RTSCC_InputActions();
        inputActions.Enable();
    }

    public bool DragButtonInput()
    {
        return inputActions.RTSCC.Drag.IsPressed();
    }

    public Vector2 MouseInput()
    {
        return inputActions.RTSCC.Mouse.ReadValue<Vector2>();
    }

    public Vector2 MousePosition()
    {
        return inputActions.RTSCC.Position.ReadValue<Vector2>();
    }

    public Vector2 MovementInput()
    {
        return inputActions.RTSCC.Move.ReadValue<Vector2>();
    }

    public bool RotationButtonInput()
    {
        return inputActions.RTSCC.Rotate.IsPressed();
    }

    public float ZoomInput()
    {
        return inputActions.RTSCC.Zoom.ReadValue<float>();
    }
}
