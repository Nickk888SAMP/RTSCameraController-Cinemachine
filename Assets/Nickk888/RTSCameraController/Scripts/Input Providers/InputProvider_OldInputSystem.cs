using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputProvider_OldInputSystem : MonoBehaviour, IRTSCInputProvider
{
    private enum MouseButton
    {
        Left,
        Right,
        Middle,
        forward,
        Back
    }

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

    public bool DragButtonInput()
    {
        return Input.GetMouseButton((int)dragMoveMouseButton);
    }

    public Vector2 MouseInput()
    {
        return new Vector2(Input.GetAxisRaw(horizontalMouseAxisName), Input.GetAxisRaw(verticalMouseAxisName));
    }

    public Vector2 MovementInput()
    {
        return new Vector2(Input.GetAxis(horizontalMovementAxisName), Input.GetAxis(verticalMovementAxisName));
    }

    public bool RotationButtonInput()
    {
        return Input.GetMouseButton((int)rotationMouseButton);
    }

    public float ZoomInput()
    {
        return Input.mouseScrollDelta.y;
    }

    public Vector2 MousePosition()
    {
        return Input.mousePosition;
    }
}
