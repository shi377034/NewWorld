using UnityEngine;
using UnityEditor;
using System.Collections;
using Invector;

[CustomEditor(typeof(vGameController), true)]
public class vGameControllerEditor : Editor
{
    GUISkin skin;

    public override void OnInspectorGUI()
    {
        var oldSkin = GUI.skin;
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        //vGameController gameController = (vGameController)target;       

        GUILayout.BeginVertical("GameController by Invector", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();
        GUI.skin = oldSkin;
        base.OnInspectorGUI();
        GUI.skin = skin;
        //EditorGUILayout.HelpBox("You can create weapon handles (empty gameobject) for each weapon inside the RightHand or LeftForeArm bones.", MessageType.Info);
        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}