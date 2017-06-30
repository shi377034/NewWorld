﻿using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(vCollectShooterMeleeControl))]
public class vCollectShooterMeleeControlEditor : Editor
{
    public GUISkin skin;
    void OnEnable()
    {
        skin = Resources.Load("skin") as GUISkin;
        if (skin == null) skin = GUI.skin;
    }
    public override void OnInspectorGUI()
    {
        GUI.skin = skin;
        serializedObject.Update();
        GUILayout.BeginVertical("vCollectShooterMeleeControl", "window");
        GUILayout.Space(30);
        base.OnInspectorGUI();
        EditorGUILayout.HelpBox("Work with vShooterManager and vMeleeManager", MessageType.Info);
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}