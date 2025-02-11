using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class RTSGameSpeedController : MonoBehaviour
{
    public event EventHandler<OnGameSpeedChangedEventArgs> OnGameSpeedChanged;
    public class OnGameSpeedChangedEventArgs : EventArgs
    {
        public GameSpeed gameSpeed;
    }

    private const float pausedTimeScale = 0f; 
    private const float halfTimeScale = 0.5f; 
    private const float normalTimeScale = 1.0f; 
    private const float doubleTimeScale = 2.0f; 
    private const float trippleTimeScale = 3.0f;

    public enum GameSpeed
    {
        Paused,
        Half,
        Normal,
        Double,
        Tripple
    }

    private void Start()
        => SetTimeScale(GameSpeed.Normal);

    private void Update()
        => CheckForKeyInput();

    private void CheckForKeyInput()
    {
        #if ENABLE_INPUT_SYSTEM
        if(Keyboard.current.digit1Key.wasPressedThisFrame)
            SetTimeScale(GameSpeed.Paused);
        else if(Keyboard.current.digit2Key.wasPressedThisFrame)
            SetTimeScale(GameSpeed.Half);
        else if(Keyboard.current.digit3Key.wasPressedThisFrame)
            SetTimeScale(GameSpeed.Normal);
        else if(Keyboard.current.digit4Key.wasPressedThisFrame)
            SetTimeScale(GameSpeed.Double);
        else if(Keyboard.current.digit5Key.wasPressedThisFrame)
            SetTimeScale(GameSpeed.Tripple);
        #else
        if(Input.GetKeyDown(KeyCode.Alpha1))
            SetTimeScale(GameSpeed.Paused);
        else if(Input.GetKeyDown(KeyCode.Alpha2))
            SetTimeScale(GameSpeed.Half);
        else if(Input.GetKeyDown(KeyCode.Alpha3))
            SetTimeScale(GameSpeed.Normal);
        else if(Input.GetKeyDown(KeyCode.Alpha4))
            SetTimeScale(GameSpeed.Double);
        else if(Input.GetKeyDown(KeyCode.Alpha5))
            SetTimeScale(GameSpeed.Tripple);
        #endif
    }

    public void SetTimeScale(GameSpeed gameSpeed)
    {
        Time.timeScale = GetTimeScaleFromGameSpeed(gameSpeed);
        OnGameSpeedChanged?.Invoke(this, new OnGameSpeedChangedEventArgs { gameSpeed = gameSpeed });
    }

    private float GetTimeScaleFromGameSpeed(GameSpeed gameSpeed) => gameSpeed switch
    {
        GameSpeed.Paused => pausedTimeScale,
        GameSpeed.Half => halfTimeScale,
        GameSpeed.Normal => normalTimeScale,
        GameSpeed.Double => doubleTimeScale,
        GameSpeed.Tripple => trippleTimeScale,
        _ => normalTimeScale
    };
}
