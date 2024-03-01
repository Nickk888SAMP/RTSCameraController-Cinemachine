using UnityEngine;

public interface IRTSCInputProvider 
{
    public Vector2 MovementInput();
    public Vector2 MouseInput();
    public float ZoomInput();
    public bool RotationButtonInput();
    public bool DragButtonInput();
    public bool HeightUpButtonInput();
    public bool HeightDownButtonInput();
    public bool RotateRightButtonInput();
    public bool RotateLeftButtonInput();
    public Vector2 MousePosition();
}
