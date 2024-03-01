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

    public bool DragButtonInput() => inputActions.RTSCC.Drag.IsPressed();

    public Vector2 MouseInput() => inputActions.RTSCC.Mouse.ReadValue<Vector2>();

    public Vector2 MousePosition() => inputActions.RTSCC.Position.ReadValue<Vector2>();

    public Vector2 MovementInput() => inputActions.RTSCC.Move.ReadValue<Vector2>();

    public bool RotationButtonInput() => inputActions.RTSCC.Rotate.IsPressed();

    public float ZoomInput() => inputActions.RTSCC.Zoom.ReadValue<float>();

    public bool HeightUpButtonInput() => inputActions.RTSCC.HeightUp.IsPressed();

    public bool HeightDownButtonInput() => inputActions.RTSCC.HeightDown.IsPressed();

    public bool RotateRightButtonInput() => inputActions.RTSCC.RotateRight.IsPressed();

    public bool RotateLeftButtonInput() => inputActions.RTSCC.RotateLeft.IsPressed();
}
