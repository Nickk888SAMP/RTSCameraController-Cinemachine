using System.Linq;
using UnityEditor;
using UnityEngine;

[ExecuteInEditMode]
public class RTSScreenSidesDebug : MonoBehaviour
{
    #if UNITY_EDITOR
    [Header("Controller")]
    [SerializeField] private RTSCameraTargetController rTSCameraTargetController;
    [SerializeField] private Color color = new Color(1, 1, 1, 0.5f);

    private void OnGUI()
    {
        if (rTSCameraTargetController == null)
            return;

        bool isSelected = rTSCameraTargetController.AllowScreenSideMove && Selection.transforms.FirstOrDefault(i => i == rTSCameraTargetController.transform) != null;
        if(isSelected)
        {
            float canvasScale = rTSCameraTargetController.RTSCanvasRectTransform.localScale.x;
            Rect top = new Rect(0, 0, Screen.width, rTSCameraTargetController.ScreenSidesZoneSize * canvasScale);
            Rect bottom = new Rect(0, Screen.height, Screen.width, -rTSCameraTargetController.ScreenSidesZoneSize * canvasScale);
            Rect left = new Rect(0, 0, rTSCameraTargetController.ScreenSidesZoneSize * canvasScale, Screen.height);
            Rect right = new Rect(Screen.width, 0, -rTSCameraTargetController.ScreenSidesZoneSize * canvasScale, Screen.height);
            EditorGUI.DrawRect(top, color);
            EditorGUI.DrawRect(bottom, color);
            EditorGUI.DrawRect(left, color);
            EditorGUI.DrawRect(right, color);
        }
    }
    #endif
}
