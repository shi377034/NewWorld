using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class vCreateShooterWeaponEditor : EditorWindow
{
    GUISkin skin;
    GameObject weaponObj;
    Vector2 rect = new Vector2(400, 100);

    [MenuItem("Invector/Shooter/Create Shooter Weapon", false, 1)]
    public static void CreateNewCharacter()
    {
        GetWindow<vCreateShooterWeaponEditor>();
    }
    void OnGUI()
    {
        if (!skin) skin = Resources.Load("skin") as GUISkin;
        GUI.skin = skin;

        this.minSize = rect;
        this.titleContent = new GUIContent("ShooterWeapon", null, "Window Creator");

        GUILayout.BeginVertical("Shooter Weapon Creator Window", "window");
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        GUILayout.BeginVertical("box");
        weaponObj = EditorGUILayout.ObjectField("FBX Model", weaponObj, typeof(GameObject), true, GUILayout.ExpandWidth(true)) as GameObject;

        if (weaponObj != null)
        {
            if (GUILayout.Button("Create"))
                Create();
        }

        GUILayout.EndVertical();
        GUILayout.EndVertical();
    }

    private void Create()
    {
        var template = Resources.Load("ShooterWeaponTemplate") as GameObject;
        GameObject weapon;

        if (template)
            weapon = Instantiate(template);
        else
            weapon = new GameObject(" ", typeof(vShooterWeapon));

        var _weaponObj = Instantiate(weaponObj);
        _weaponObj.transform.SetParent(weapon.transform);
        _weaponObj.transform.localPosition = Vector3.zero;
        _weaponObj.transform.localEulerAngles = Vector3.zero;
        Selection.activeGameObject = weapon;
        SceneView.lastActiveSceneView.FrameSelected();

        this.Close();
    }
}
