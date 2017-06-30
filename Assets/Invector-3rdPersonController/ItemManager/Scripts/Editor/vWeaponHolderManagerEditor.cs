using UnityEngine;
using UnityEditor;
using System.Collections;
using Invector;

[CanEditMultipleObjects]
[CustomEditor(typeof(Invector.ItemManager.vWeaponHolderManager),true)]
public class vWeaponHolderManagerEditor : Editor
{
    GUISkin skin;

    [MenuItem("Invector/Melee Combat/Components/WeaponHolderManager (Player Only)")]
    static void MenuComponent()
    {
        if (Selection.activeGameObject)
            Selection.activeGameObject.AddComponent<Invector.ItemManager.vWeaponHolderManager>();
        else
            Debug.Log("Please select the Player to add the component.");
    }

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        GUILayout.BeginVertical("Weapon Holder Manager", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        EditorGUILayout.BeginVertical();       
        base.OnInspectorGUI();

        EditorGUILayout.HelpBox("Create a new empty object inside a bone and add the vWeaponHolder script", MessageType.Info);
        GUILayout.EndVertical();
        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }
}