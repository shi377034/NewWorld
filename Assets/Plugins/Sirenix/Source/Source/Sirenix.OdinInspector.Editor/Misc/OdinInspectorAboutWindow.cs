#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="OdinInspectorPreferences.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Adds menu items to the Unity Editor, draws the About window, and the preference window found under Edit > Preferences > Odin Inspector.
    /// </summary>
    public class OdinInspectorAboutWindow : EditorWindow
    {
        private GUIStyle headerStyle;
        private static Texture2D odinLogo;

        private static Texture2D OdinLogo
        {
            get
            {
                if (odinLogo == null)
                {
                    odinLogo = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/" + SirenixAssetPaths.SirenixAssetsPath + "Editor/Odin Inspector Logo.png");
                }
                return odinLogo;
            }
        }

        [MenuItem("Window/Odin Inspector/About", priority = 5)]
        private static void ShowAboutOdinInspector()
        {
            var w = GetWindow<OdinInspectorAboutWindow>();
            w.titleContent = new GUIContent("About Odin");
            w.minSize = new Vector2(385f, 125f);
            w.Show();
        }

        private void OnGUI()
        {
            if (this.headerStyle == null)
            {
                this.headerStyle = new GUIStyle(SirenixGUIStyles.SectionHeader);
                this.headerStyle.normal.textColor = new Color32(102, 102, 102, 255);
            }

            GUILayout.BeginArea(new Rect(10f, 10f, this.position.width - 20f, this.position.height - 5f));
            GUI.Label(new Rect(EditorGUILayout.GetControlRect(true, 25f)) { y = -5f }, "Odin Inspector", this.headerStyle);
            DrawAboutGUI();
            GUILayout.EndArea();
            this.RepaintIfRequested();
        }

        [PreferenceItem("Odin Inspector")]
        private static void OnPreferencesGUI()
        {
            DrawAboutGUI();
            Rect rect = EditorGUILayout.GetControlRect();

            if (GUI.Button(new Rect(rect) { y = rect.y + 90f, height = 25f, }, "Show Odin Preferences"))
            {
                SirenixPreferencesWindow.OpenSirenixPreferences();
            }

            GUIHelper.RepaintIfRequested(GUIHelper.CurrentWindow);
        }

        private static void DrawAboutGUI()
        {
            Rect position = new Rect(EditorGUILayout.GetControlRect()) { height = 500f };

            // Logo
            GUI.DrawTexture(position.SetWidth(86).SetHeight(75).MoveVertical(4).MoveHorizontal(-5), OdinLogo, ScaleMode.ScaleAndCrop);

            // About
            GUI.Label(new Rect(position) { x = position.x + 82f, y = position.y + 20f * 0f - 2f, height = 18f, }, "Version " + typeof(InspectorConfig).Assembly.GetName(false).Version, SirenixGUIStyles.LeftAlignedGreyMiniLabel);
            DrawLink(new Rect(position) { x = position.x + 82f, y = position.y + 20f * 1f - 2f, height = 18f, }, "Developed by Sirenix IVS", "http://www.sirenix.net", SirenixGUIStyles.LeftAlignedGreyMiniLabel);
            DrawLink(new Rect(position) { x = position.x + 82f, y = position.y + 20f * 2f - 2f, height = 18f, }, "Published by DevDog", "http://www.devdog.io", SirenixGUIStyles.LeftAlignedGreyMiniLabel);
            GUI.Label(new Rect(position) { x = position.x + 82f, y = position.y + 20f * 3f - 2f, height = 18f, }, "All rights reserved", SirenixGUIStyles.LeftAlignedGreyMiniLabel);

            // Links
            DrawLink(new Rect(position) { x = position.xMax - 100f, y = position.y + 20f * 0f, width = 100f, height = 14f, }, "Manuals", "http://sirenix.net/odininspector/manual/introduction/getting-started", EditorStyles.miniButton);
            DrawLink(new Rect(position) { x = position.xMax - 100f, y = position.y + 20f * 1f, width = 100f, height = 14f, }, "Documentation", "http://sirenix.net/odininspector/documentation/sirenix/odininspector/assetlistattribute", EditorStyles.miniButton);
            DrawLink(new Rect(position) { x = position.xMax - 100f, y = position.y + 20f * 2f, width = 100f, height = 14f, }, "Forums", "https://forum.unity3d.com/threads/wip-odin-inspector-serializer-looking-for-feedback.457670/", EditorStyles.miniButton);
            DrawLink(new Rect(position) { x = position.xMax - 100f, y = position.y + 20f * 3f, width = 100f, height = 14f, }, "Issue tracker", "https://bitbucket.org/sirenix/odin-inspector/issues?status=new&status=open", EditorStyles.miniButton);
        }

        private static void DrawLink(Rect rect, string label, string link, GUIStyle style)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
            if (GUI.Button(rect, label, style))
            {
                Application.OpenURL(link);
            }
        }
    }
}
#endif