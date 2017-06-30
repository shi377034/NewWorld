using UnityEngine;
using System.Collections;
using UnityEditor;
using Invector;
using Invector.CharacterController;

[CanEditMultipleObjects]
[CustomEditor(typeof(vShooterManager), true)]
public class vShooterManagerEditor : Editor
{
    GUISkin skin;
    
    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;        

        vShooterManager manager = (vShooterManager)target;
        if (!manager) return;

        GUILayout.BeginVertical("Shooter Manager", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
        base.OnInspectorGUI();

        if (Application.isPlaying)
        {
            EditorGUILayout.BeginVertical("box");
            EditorGUILayout.HelpBox("Playmode Debug - Equip a weapon first", MessageType.Info);
            EditorGUILayout.Space();
            if (manager.tpCamera != null)
            {
                if (GUILayout.Button(manager.tpCamera.lockCamera ? "Unlock Camera" : "Lock Camera", EditorStyles.toolbarButton))
                {
                    manager.tpCamera.lockCamera = !manager.tpCamera.lockCamera;
                }
            }       
            EditorGUILayout.Space();
            if (GUILayout.Button(manager.alwaysAiming ? "Unlock Aiming" : "Lock Aiming", EditorStyles.toolbarButton))
            {            
                manager.alwaysAiming = !manager.alwaysAiming;
            }
            EditorGUILayout.Space();
            if (GUILayout.Button(manager.showCheckAimGizmos ? "Hide Aim Gizmos" : "Show Aim Gizmos", EditorStyles.toolbarButton))
            {
                manager.showCheckAimGizmos = !manager.showCheckAimGizmos;
            }
            EditorGUILayout.EndVertical();
        }
        
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
    }
}
