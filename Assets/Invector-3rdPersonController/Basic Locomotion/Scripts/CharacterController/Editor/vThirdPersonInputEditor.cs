using UnityEngine;
using UnityEditor;
using System.Collections;
using Invector;

[CanEditMultipleObjects]
[CustomEditor(typeof(Invector.CharacterController.vThirdPersonInput),true)]
public class vThirdPersonInputEditor : Editor
{
    GUISkin skin;
    bool openWindow;

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        GUILayout.BeginVertical("INPUT MANAGER", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();

        GUILayout.Space(10);
        EditorGUILayout.HelpBox("Open to edit Inputs or change between TopDown and ThirdPerson", MessageType.Info);
        GUILayout.Space(10);

        openWindow = GUILayout.Toggle(openWindow, openWindow ? "Close InputManager" : "Open InputManager", EditorStyles.toolbarButton);
        if (openWindow)
        {
            GUILayout.Space(10);
            EditorGUILayout.HelpBox("Open this script to change the method GetButton, GetButtonDown, GetButtonUp, etc...", MessageType.Info);
            GUILayout.Space(10);
            base.OnInspectorGUI();
        }
        GUILayout.Space(10);
        

        var tpInput = (Invector.CharacterController.vThirdPersonInput)target;
        if(tpInput.gameplayInputStyle == Invector.CharacterController.vThirdPersonInput.GameplayInputStyle.ClickAndMove)
            EditorGUILayout.HelpBox("Click and Move mode works only with the FixedAngle Camera Mode", MessageType.Warning);

        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}