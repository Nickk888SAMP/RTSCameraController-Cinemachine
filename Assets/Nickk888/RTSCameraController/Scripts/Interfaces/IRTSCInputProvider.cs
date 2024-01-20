using UnityEngine;

public interface IRTSCInputProvider 
{
    public Vector2 MovementInput();
    public Vector2 MouseInput();
    public float ZoomInput();
    public bool RotationButtonInput();
    public bool DragButtonInput();
    public Vector2 MousePosition();
}
