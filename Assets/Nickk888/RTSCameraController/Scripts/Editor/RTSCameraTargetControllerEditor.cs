using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RTSCameraTargetController))]
public class RTSCameraTargetControllerEditor : Editor 
{
    Texture2D logo;

    public override void OnInspectorGUI()
    {
        RTSCameraTargetController ctc = (RTSCameraTargetController)target;
        if (!logo) {
            logo = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Nickk888/RTSCameraController/Textures/RTSCC_Logo.png", typeof(Texture2D));
        }
        GUILayout.Box(logo, new GUIStyle { fixedHeight = 60, alignment = TextAnchor.MiddleCenter } );
        GUILayout.Space(20);
        if (GUILayout.Button("Open Virtual Camera Settings"))
        {
            if (ctc.VirtualCamera != null)
            {
                Selection.activeGameObject = ctc.VirtualCamera.gameObject;
            }
        }

        GUILayout.Space(20);
        DrawDefaultInspector();
    }
}
