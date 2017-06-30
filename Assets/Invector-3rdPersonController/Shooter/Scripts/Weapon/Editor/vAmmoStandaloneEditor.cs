using UnityEngine;
using System.Collections;
using UnityEditor;
[CustomEditor(typeof(vAmmoStandalone))]
public class vAmmoStandaloneEditor : Editor
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
        GUILayout.BeginVertical("Ammo Standalone", "window");
        GUILayout.Space(30);
        base.OnInspectorGUI();        
        GUILayout.EndVertical();
        serializedObject.ApplyModifiedProperties();
    }
}