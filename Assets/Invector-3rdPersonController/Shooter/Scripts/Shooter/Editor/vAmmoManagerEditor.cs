using UnityEngine;
using System.Collections;
using UnityEditor;
using Invector;

[CanEditMultipleObjects]
[CustomEditor(typeof(vAmmoManager), true)]
public class vAmmoManagerEditor : Editor
{
    GUISkin skin;   

    public override void OnInspectorGUI()
    {        
        var manager = target as vAmmoManager;
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;        
        GUILayout.BeginVertical("Ammo Manager", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.HelpBox("You can add and manage ammo here if you're NOT using the Inventory. Otherwise the weapon will search for ammo inside the Inventory.", MessageType.Info);

        base.OnInspectorGUI();        

        if(manager.ammoListData)
        {
            var ammoList = new SerializedObject(manager.ammoListData);
            ammoList.Update();
            DrawPropertiesExcluding(ammoList, "m_Script");
            if (GUI.changed)
                ammoList.ApplyModifiedProperties();
        }

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        
    }
}
