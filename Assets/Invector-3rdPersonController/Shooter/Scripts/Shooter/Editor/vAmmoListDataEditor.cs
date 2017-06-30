using UnityEngine;
using System.Collections;
using UnityEditor;

[CanEditMultipleObjects]
[CustomEditor(typeof(vAmmoListData), true)]
public class vAmmoListDataEditor : Editor
{
    GUISkin skin;

    public override void OnInspectorGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;
        GUILayout.BeginVertical("Ammo Manager List Data", "window");

        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        //EditorGUILayout.HelpBox("You can add and manage ammo here if you're NOT using the Inventory. Otherwise the weapon will search for ammo inside the Inventory.", MessageType.Info);

        base.OnInspectorGUI();
        //DrawPropertiesExcluding(serializedObject, "m_Script");

        EditorGUILayout.EndVertical();

        EditorGUILayout.Space();
        EditorGUILayout.Space();
    }

    [MenuItem("Invector/Shooter/Create new AmmoListData")]
    public static void CreateAmmoList()
    {
        vScriptableObjectUtility.CreateAsset<vAmmoListData>();
    }
}
