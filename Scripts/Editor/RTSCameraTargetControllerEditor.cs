using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RTSCameraTargetController))]
public class RTSCameraTargetControllerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        RTSCameraTargetController ctc = (RTSCameraTargetController)target;
        if(GUILayout.Button("Open Virtual Camera Settings"))
        {
            if(ctc.VirtualCamera != null)
            {
                Selection.activeGameObject = ctc.VirtualCamera.gameObject;
            }
        }
        GUILayout.Space(20);
        DrawDefaultInspector();
    }
}
