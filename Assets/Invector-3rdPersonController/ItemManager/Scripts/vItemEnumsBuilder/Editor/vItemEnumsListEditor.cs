
using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Reflection.Emit;
using UnityEditor;
using UnityEngine;
using System.Linq;

namespace Invector.ItemManager.DynamicEnum
{

    [CustomEditor(typeof(vItemEnumsList))]
    public class vItemEnumsListEditor : Editor
    {
        public GUISkin skin;
        void OnEnable()
        {
            skin = Resources.Load("skin") as GUISkin;
        }
        public override void OnInspectorGUI()
        {
            if (skin) GUI.skin = skin;
            var assetPath = AssetDatabase.GetAssetPath(target);
            GUILayout.BeginVertical("vItemEnums List", "window");
            GUILayout.Space(30);
            if (assetPath.Contains("Resources"))
            {
                GUILayout.BeginVertical("box");
                base.OnInspectorGUI();
                GUILayout.EndHorizontal();
                EditorGUILayout.Space();
                if (GUILayout.Button("Open ItemEnums Editor"))
                {
                    vItemEnumsWindow.CreateWindow();
                }
                EditorGUILayout.Space();
                if (GUILayout.Button("Refresh ItemEnums"))
                {
                    vItemEnumsBuilder.RefreshItemEnums();
                }

                EditorGUILayout.HelpBox("-This list will be merged with other lists and create the enums.\n- The Enum Generator will ignore equal values.\n- If our change causes errors, check which enum value is missing and adds to the list and press the refresh button.", MessageType.Info);
            }
            else
            {
                EditorGUILayout.HelpBox("Please put this list in Resources folder", MessageType.Warning);
            }
            GUILayout.EndVertical();
        }


        [MenuItem("Invector/Inventory/ItemEnums/Create New vItemEnumsList")]
        internal static void CreateDefaultItemEnum()
        {
            ScriptableObjectUtility.CreateAsset<vItemEnumsList>();
        }

    }
}
public static class ScriptableObjectUtility
{
    /// <summary>
    //	This makes it easy to create, name and place unique new ScriptableObject asset files.
    /// </summary>
    public static void CreateAsset<T>() where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        string path = AssetDatabase.GetAssetPath(Selection.activeObject);
        if (path == "")
        {
            path = "Assets";
        }
        else if (System.IO.Path.GetExtension(path) != "")
        {
            path = path.Replace(System.IO.Path.GetFileName(AssetDatabase.GetAssetPath(Selection.activeObject)), "");
        }

        string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(path + "/New " + typeof(T).ToString() + ".asset");

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        EditorUtility.FocusProjectWindow();
        Selection.activeObject = asset;
    }
}