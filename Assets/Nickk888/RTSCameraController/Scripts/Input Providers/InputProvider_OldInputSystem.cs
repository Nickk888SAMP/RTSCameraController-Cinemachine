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

    [SerializeField] [Tooltip("The button to move the camera Upwards.")]
    private KeyCode heightUpButton = KeyCode.R;

    [SerializeField] [Tooltip("The button to move the camera Downwards.")]
    private KeyCode heightDownButton = KeyCode.F;

    [SerializeField] [Tooltip("The button to rotate the camera Right.")]
    private KeyCode rotateRightButton = KeyCode.E;

    [SerializeField] [Tooltip("The button to rotate the camera Left.")]
    private KeyCode rotateLeftButton = KeyCode.Q;

    public bool DragButtonInput() => Input.GetMouseButton((int)dragMoveMouseButton);

    public Vector2 MouseInput()
        => new Vector2(Input.GetAxisRaw(horizontalMouseAxisName), Input.GetAxisRaw(verticalMouseAxisName));

    public Vector2 MovementInput() 
        => new Vector2(Input.GetAxis(horizontalMovementAxisName), Input.GetAxis(verticalMovementAxisName));

    public bool RotationButtonInput() => Input.GetMouseButton((int)rotationMouseButton);

    public float ZoomInput() => Input.mouseScrollDelta.y;

    public Vector2 MousePosition() => Input.mousePosition;

    public bool HeightUpButtonInput() => Input.GetKey(heightUpButton);

    public bool HeightDownButtonInput() => Input.GetKey(heightDownButton);

    public bool RotateRightButtonInput() => Input.GetKey(rotateRightButton);

    public bool RotateLeftButtonInput() => Input.GetKey(rotateLeftButton);
}
