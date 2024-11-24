using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RTSGameSpeed_UI : MonoBehaviour
{
    [Header("RTS Game Speed Controller")]
    [SerializeField] private RTSGameSpeedController gameSpeedController;

    [Header("Images")]
    [SerializeField] private Image pausedImage;
    [SerializeField] private Image halfSpeedImage;
    [SerializeField] private Image normalSpeedImage;
    [SerializeField] private Image doubleSpeedImage;
    [SerializeField] private Image trippleSpeedImage;
    
    [Header("Colors")]
    [SerializeField] private Color activeColor = new Color(1, 1, 1, 1);
    [SerializeField] private Color notActiveColor = new Color(1, 1, 1, 0.25f);

    private void Awake() 
        => ResetImageColors();

    private void OnEnable()
        => gameSpeedController.OnGameSpeedChanged += RTSGameSpeedController_OnGameSpeedChanged;

    private void OnDisable()
        => gameSpeedController.OnGameSpeedChanged -= RTSGameSpeedController_OnGameSpeedChanged;

    private void RTSGameSpeedController_OnGameSpeedChanged(object sender, RTSGameSpeedController.OnGameSpeedChangedEventArgs e)
        => UpdateUI(e.gameSpeed);

    private void UpdateUI(RTSGameSpeedController.GameSpeed gameSpeed)
    {
        ResetImageColors();
        switch (gameSpeed)
        {
            case RTSGameSpeedController.GameSpeed.Paused:
                pausedImage.color = activeColor;
                break;
            case RTSGameSpeedController.GameSpeed.Half:
                halfSpeedImage.color = activeColor;
                break;
            case RTSGameSpeedController.GameSpeed.Normal:
                normalSpeedImage.color = activeColor;
                break;
            case RTSGameSpeedController.GameSpeed.Double:
                doubleSpeedImage.color = activeColor;
                break;
            case RTSGameSpeedController.GameSpeed.Tripple:
                trippleSpeedImage.color = activeColor;
                break;
        }
    }

    private void ResetImageColors()
    {
        pausedImage.color = notActiveColor;
        halfSpeedImage.color = notActiveColor;
        normalSpeedImage.color = notActiveColor;
        doubleSpeedImage.color = notActiveColor;
        trippleSpeedImage.color = notActiveColor;
    }
}
