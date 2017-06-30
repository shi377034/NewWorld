using UnityEngine;
using System.Collections;
using UnityEditor;
using Invector;

[CanEditMultipleObjects]
[CustomEditor(typeof(vMoveSetSpeed), true)]
public class vMoveSetSpeedEditor : Editor
{
    GUISkin skin;

    [MenuItem("Invector/Basic Locomotion/Components/MoveSetSpeed")]
    static void MenuComponent()
    {
        if (Selection.activeGameObject)
            Selection.activeGameObject.AddComponent<vMoveSetSpeed>();
        else
            Debug.Log("Please select a GameObject to add the component.");
    }

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;        
        GUILayout.BeginVertical("MoveSetSpeed", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("Use this to add extra speed into a specific MoveSet", MessageType.Info);

        base.OnInspectorGUI();        

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
    }
}
