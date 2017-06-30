using UnityEngine;
using System.Collections;
using UnityEditor;
using Invector;

[CanEditMultipleObjects]
[CustomEditor(typeof(vShooterWeapon), true)]
public class vShooterWeaponEditor : Editor
{
    GUISkin skin;
    string[] ignoreProperties = new string[] { "onShot","onReload","onEmptyClip","onEnableAim", "onDisableAim","onEnableScope","onDisableScope" };
    [SerializeField]
    public bool eventsOpen;
    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        var oldskin = GUI.skin;
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;        
	    GUILayout.BeginVertical("Shooter Weapon", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();        

        DrawPropertiesExcluding(serializedObject, ignoreProperties);
        GUI.skin = oldskin;
        EditorGUILayout.Space();
        if (GUILayout.Button(eventsOpen ? "Close Events" : "Open Events")) eventsOpen = !eventsOpen;
        if(eventsOpen)
            for (int i = 0; i < ignoreProperties.Length; i++)
            {
                EditorGUILayout.PropertyField(serializedObject.FindProperty(ignoreProperties[i]));
            }
       
      
        serializedObject.ApplyModifiedProperties();	    

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
    }
}
