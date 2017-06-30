#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="GlobalConfigWindow.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Serialization;
    using Sirenix.Utilities.Editor;
    using Sirenix.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using System.Text;
    using System.Globalization;

    // TODO: Search for the SirenixPreferenceWindow is broken, I've out-commented it for now.

    /// <summary>
    /// Sirenix preferences window.
    /// </summary>
    public class SirenixPreferencesWindow : EditorWindow, ISerializationCallbackReceiver
    {
        [SerializeField]
        private byte[] serializedMenuTreeBytes;

        [SerializeField]
        private byte[] configAttributeTypeBytes;

        [NonSerialized]
        private List<ConfigMenuItem> serializedMenuTree;

        [NonSerialized]
        private List<ConfigMenuItem> menuTree;

        private string selectedId;

        [SerializeField]
        private List<UnityEngine.Object> unityObjects;

        [NonSerialized]
        private string searchTerm;

        private Type configAttributeType;

        [NonSerialized]
        private ConfigMenuItem activeMenu;

        [NonSerialized]
        private ConfigMenuItem nextActiveMenu;

        [NonSerialized]
        private EditorTimeHelper time = new EditorTimeHelper();

        private UnityEngine.Object configToSelect;

        private Vector2 leftMenuScrollPos;

        private Vector2 rightMenuScrollPos;

#pragma warning disable 0649
        private bool isSearching;
#pragma warning restore 0649

        // Search has been out-commented for now.
        //private bool searchChanged;

        private void OnGUI()
        {
            this.wantsMouseMove = true;
            var rect = EditorGUILayout.BeginHorizontal(SirenixGUIStyles.None, GUILayoutOptions.ExpandWidth(true).ExpandHeight(true));
            {
                SirenixEditorGUI.DrawSolidRect(rect, SirenixGUIStyles.DarkEditorBackground);

                rect = EditorGUILayout.BeginVertical(SirenixGUIStyles.None, GUILayoutOptions.ExpandHeight(true).Width(180));
                {
                    SirenixEditorGUI.DrawSolidRect(rect, EditorGUIUtility.isProSkin ? SirenixGUIStyles.BoxBackgroundColor : new Color(0.87f, 0.87f, 0.87f, 1f));
                    SirenixEditorGUI.DrawBorders(rect, 0, 1, 0, 0);

                    this.DrawLeftMenu();
                }
                EditorGUILayout.EndVertical();
                rect = EditorGUILayout.BeginVertical(GUILayoutOptions.ExpandHeight(true));
                {
                    this.DrawRightMenu();
                }
                EditorGUILayout.EndVertical();
            }
            EditorGUILayout.EndHorizontal();

            this.time.Update();

            if (this.nextActiveMenu != null && Event.current.type == EventType.repaint)
            {
                this.activeMenu = this.nextActiveMenu;
                this.nextActiveMenu = null;
                GUIHelper.RequestRepaint();
            }

            this.RepaintIfRequested();
        }

        private void DrawRightMenu()
        {
            this.BuildMenuTree();

            var expand = GUILayoutOptions.ExpandHeight(true).ExpandWidth(true);
            this.rightMenuScrollPos = EditorGUILayout.BeginScrollView(this.rightMenuScrollPos, expand);
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(15);
                GUILayout.Label(this.activeMenu == null ? "-" : this.activeMenu.Name, SirenixGUIStyles.SectionHeader);
                GUILayout.Space(15);
                GUILayout.EndHorizontal();
                //SirenixEditorGUI.BeginHorizontalToolbar(SirenixGUIStyles.ToolbarBackgroundChained);
                //{
                //    EditorGUILayout.LabelField(this.activeMenu == null ? "-" : this.activeMenu.Name, EditorStyles.boldLabel);
                //    GUILayout.FlexibleSpace();

                //    if (this.activeMenu != null)
                //    {
                //        if (SirenixEditorGUI.ToolbarButton(EditorIcons.SettingsCog))
                //        {
                //            this.activeMenu.ShowContextMenu();
                //        }
                //    }
                //}
                //SirenixEditorGUI.EndHorizontalToolbar();

                GUILayout.Space(15);
                GUILayout.BeginHorizontal(expand);
                {
                    GUILayout.Space(10);
                    var rect = EditorGUILayout.BeginVertical(expand);
                    {
                        var prevLabelWidth = EditorGUIUtility.labelWidth;
                        EditorGUIUtility.labelWidth = Mathf.Clamp(rect.width * 0.4f, 100, 300);

                        if (this.activeMenu != null)
                        {
                            this.activeMenu.DrawConfig();
                        }

                        EditorGUIUtility.labelWidth = prevLabelWidth;
                    }
                    EditorGUILayout.EndVertical();
                    GUILayout.Space(10);
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(15);
            }
            EditorGUILayout.EndScrollView();
        }

        private void BuildMenuTree()
        {
            if (this.menuTree == null)
            {
                this.menuTree = new List<ConfigMenuItem>();
                foreach (var configType in AssemblyUtilities.GetTypes(AssemblyTypeFlags.CustomTypes).Where(t => t.HasCustomAttribute(this.configAttributeType, true) && t.InheritsFrom(typeof(GlobalConfig<>))))
                {
                    var attr = configType.GetAttribute<EditorGlobalConfigAttribute>(true);

                    string[] path = attr.MenuItemPath.Split('/');
                    ConfigMenuItem current = null;
                    for (int i = 0; i < path.Length; i++)
                    {
                        string name = path[i];

                        if (i == 0)
                        {
                            current = this.menuTree.FirstOrDefault(x => x.Name == name);
                            if (current == null)
                            {
                                current = new ConfigMenuItem(this, name);
                                this.menuTree.Add(current);
                            }
                        }
                        else
                        {
                            ConfigMenuItem child = current.GetChild(name);
                            if (child == null)
                            {
                                child = new ConfigMenuItem(this, name);
                                current.AddChild(child);
                            }
                            current = child;
                        }

                        if (i == path.Length - 1)
                        {
                            current.SetConfig(configType);
                        }
                    }
                }

                if (this.serializedMenuTree != null)
                {
                    foreach (var item in this.menuTree.SelectMany(x => x.GetAllChilds(true)))
                    {
                        var treeItem = this.serializedMenuTree.SelectMany(x => x.GetAllChilds(true)).FirstOrDefault(x => x.Id == item.Id);
                        if (treeItem != null)
                        {
                            item.IsOpen = treeItem.IsOpen;
                        }
                        else
                        {
                            item.IsOpen = true;
                        }
                    }
                }

                if (string.IsNullOrEmpty(this.selectedId) == false)
                {
                    this.activeMenu = this.menuTree.SelectMany(x => x.GetAllChilds(true)).FirstOrDefault(x => x.Id == this.selectedId) ?? this.activeMenu;
                }

                this.menuTree = this.menuTree.OrderBy(x => x.Name).ToList();
                for (int i = 0; i < this.menuTree.Count; i++)
                {
                    this.menuTree[i].Sort();
                }

                if (this.activeMenu == null)
                {
                    this.configToSelect = this.menuTree.SelectMany(x => x.GetAllChilds(true)).FirstOrDefault(x => x.UnityObject != null).UnityObject;
                }
            }

            if (this.configToSelect != null)
            {
                var newMenu = this.menuTree.SelectMany(x => x.GetAllChilds(true)).Where(x => x.UnityObject == this.configToSelect).FirstOrDefault();

                if (newMenu != null)
                {
                    this.activeMenu = newMenu;
                    this.activeMenu.Open();
                }

                this.configToSelect = null;
                this.Repaint();
            }
        }

        private void DrawLeftMenu()
        {
            this.BuildMenuTree();

            // Search has been out-commented for now.
            //GUILayout.BeginVertical();
            //GUILayout.Space(10);
            //this.searchChanged = this.searchTerm != (this.searchTerm = SirenixEditorGUI.ToolbarSearchField(this.searchTerm));
            //GUILayout.Space(6);
            //GUILayout.EndVertical();
            //SirenixEditorGUI.HorizontalLineSeperator();
            this.leftMenuScrollPos = EditorGUILayout.BeginScrollView(this.leftMenuScrollPos, GUILayoutOptions.ExpandHeight(true).ExpandWidth(true));
            //this.isSearching = string.IsNullOrEmpty(this.searchTerm) == false;
            //if (this.isSearching && this.searchChanged)
            //{
            //    for (int i = 0; i < this.menuTree.Count; i++)
            //    {
            //        this.menuTree[i].ResetSearchMatch();
            //    }
            //    for (int i = 0; i < this.menuTree.Count; i++)
            //    {
            //        this.menuTree[i].SearchConfig();
            //    }
            //}

            for (int i = 0; i < this.menuTree.Count; i++)
            {
                this.menuTree[i].DrawMenu();
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// Open preferences page for configuration object.
        /// </summary>
        public static void OpenGlobalConfigWindow<T>(string title, UnityEngine.Object selectedConfig) where T : EditorGlobalConfigAttribute
        {
            var windowSize = new Vector2(900, 600);
            var windowRect = GUIHelper.GetEditorWindowRect();
            var rect = new Rect(windowRect.center - windowSize * 0.5f, windowSize);

            var window = GetWindow<SirenixPreferencesWindow>(false);

            window.titleContent = new GUIContent(title);
            window.configAttributeType = typeof(T);
            window.wantsMouseMove = true;
            window.configToSelect = selectedConfig;
            //window.Show();
            //window.Focus();
            window.position = rect;
        }

        /// <summary>
        /// Opens the Odin inspector preferences window.
        /// </summary>
        [MenuItem("Window/Odin Inspector/Preferences", priority = 4)]
        public static void OpenSirenixPreferences()
        {
            EditorGlobalConfigAttribute.OpenWindow<SirenixGlobalConfigAttribute>("Odin Preferences");
        }

        void ISerializationCallbackReceiver.OnBeforeSerialize()
        {
            this.serializedMenuTreeBytes = SerializationUtility.SerializeValue(this.menuTree, DataFormat.Binary, out this.unityObjects);
            this.configAttributeTypeBytes = SerializationUtility.SerializeValue(this.configAttributeType, DataFormat.Binary);
            this.selectedId = this.activeMenu != null ? this.activeMenu.Id : null;
        }

        void ISerializationCallbackReceiver.OnAfterDeserialize()
        {
            if (this.unityObjects == null)
            {
                this.unityObjects = new List<UnityEngine.Object>();
            }

            if (this.serializedMenuTreeBytes != null)
            {
                this.serializedMenuTree = SerializationUtility.DeserializeValue<List<ConfigMenuItem>>(this.serializedMenuTreeBytes, DataFormat.Binary, this.unityObjects);
            }
            if (this.configAttributeTypeBytes != null)
            {
                this.configAttributeType = SerializationUtility.DeserializeValue<Type>(this.configAttributeTypeBytes, DataFormat.Binary);
            }
        }

        // Search has been out-commented for now.
        //private class SearchHelper
        //{
        //    private static Dictionary<Type, string> keywordsa;

        //    public static string GetSearchTerm(UnityEngine.Object obj)
        //    {
        //        if (keywordsa == null)
        //        {
        //            keywordsa = new Dictionary<Type, string>();
        //        }

        //        string words = null;

        //        if (keywordsa.TryGetValue(obj.GetType(), out words) == false)
        //        {
        //            var members = obj.GetType().GetMembers(Flags.AllMembers).Where(x => x is PropertyInfo || x is FieldInfo);

        //            HashSet<string> names = new HashSet<string>();

        //            foreach (var item in members)
        //            {
        //                names.Add(item.GetNiceName());
        //            }

        //            keywordsa[obj.GetType()] = words = ItemsToString(names, " ");
        //        }

        //        return words.ToString(CultureInfo.InvariantCulture);
        //    }

        //    private static string ItemsToString<TValue>(IEnumerable<TValue> items, string seperator)
        //    {
        //        StringBuilder sb = new StringBuilder();

        //        bool isFirst = true;

        //        foreach (var item in items)
        //        {
        //            if (isFirst == false)
        //            {
        //                sb.Append(seperator);
        //            }
        //            sb.Append(item.ToString());
        //            isFirst = false;
        //        }

        //        return sb.ToString();
        //    }
        //}

        [Serializable]
        private class ConfigMenuItem
        {
            [SerializeField]
            private List<ConfigMenuItem> childMenuItems;

            [SerializeField]
            public bool IsOpen = true;

            [SerializeField]
            public string Id { get; private set; }

            private readonly SirenixPreferencesWindow configWindow;
            private UnityEngine.Object unityObject;
            private bool shouldDrawUnityEditor;
            private Editor unityEditor;

            public UnityEngine.Object UnityObject
            {
                get
                {
                    this.InitializeUnityObject();
                    return this.unityObject;
                }
            }

            private void InitializeUnityObject()
            {
                if (this.getUnityObject != null && this.unityObject == null)
                {
                    this.unityObject = this.getUnityObject.GetValue(null, null) as UnityEngine.Object;
                    this.propertyTree = PropertyTree.Create(new object[] { this.unityObject });
                    this.shouldDrawUnityEditor = this.unityObject.GetType().IsDefined<DrawUnityEditorInGlobalConfigWindowAttribute>();

                    if (this.shouldDrawUnityEditor)
                    {
                        this.unityEditor = Editor.CreateEditor(this.unityObject);
                    }
                }
            }

            private PropertyTree propertyTree;
            private PropertyInfo getUnityObject;
            private ConfigMenuItem parent;
            private bool hasSearchMatch;
            private bool childHasSearchMatch;
            private float t;

            public readonly string Name;

            public ConfigMenuItem(SirenixPreferencesWindow window, string name)
            {
                this.Name = name;
                this.configWindow = window;
                this.childMenuItems = new List<ConfigMenuItem>();
            }

            public void SetConfig(Type configType)
            {
                this.getUnityObject = configType.GetGenericBaseType(typeof(GlobalConfig<>)).GetProperty("Instance", BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty);
                this.Id = configType.FullName;
            }

            public ConfigMenuItem GetChild(string name)
            {
                return this.childMenuItems.FirstOrDefault(x => x.Name == name);
            }

            public void AddChild(ConfigMenuItem config)
            {
                config.parent = this;
                this.childMenuItems.Add(config);
            }

            public IEnumerable<ConfigMenuItem> GetAllChilds(bool includeSelf = true)
            {
                if (includeSelf)
                {
                    yield return this;
                }
                foreach (var item in this.childMenuItems)
                {
                    foreach (var c in item.GetAllChilds(true))
                    {
                        yield return c;
                    }
                }
            }

            public void DrawMenu(int indent = 0)
            {
                this.InitializeUnityObject();
                bool isActive = this.configWindow.activeMenu == this;
                EditorIcon iconType;
                if (this.childMenuItems.Count > 0)
                {
                    iconType = this.IsOpen ? EditorIcons.TriangleDown : EditorIcons.TriangleRight;
                }
                else
                {
                    iconType = EditorIcons.Transparent;
                }

                if ((this.configWindow.isSearching && (this.childHasSearchMatch || this.hasSearchMatch)) || this.configWindow.isSearching == false)
                {
                    Texture icon = isActive ? iconType.Highlighted : iconType.Inactive;
                    var tmp = GUI.enabled;
                    if (this.configWindow.isSearching && this.hasSearchMatch == false)
                    {
                        GUI.enabled = false;
                    }
                    if (SirenixEditorGUI.MenuButton(indent, this.Name, isActive, icon))
                    {
                        if (Event.current.button == 1)
                        {
                            this.ShowContextMenu();
                        }
                        else
                        {
                            if (this.configWindow.isSearching == false)
                            {
                                if ((this.IsOpen && isActive == false) == false || this.propertyTree == null)
                                {
                                    this.IsOpen = !this.IsOpen;
                                }

                                if (Event.current.modifiers == EventModifiers.Alt)
                                {
                                    foreach (var item in this.GetAllChilds())
                                    {
                                        item.IsOpen = this.IsOpen;
                                    }
                                }
                            }

                            if (this.unityObject != null)
                            {
                                this.configWindow.nextActiveMenu = this;
                            }
                        }
                    }
                    GUI.enabled = tmp;
                }

                if (this.childMenuItems.Count > 0)
                {
                    bool m = this.IsOpen || this.configWindow.isSearching;
                    if (Event.current.type == EventType.Layout)
                    {
                        this.t = Mathf.MoveTowards(this.t, m ? 1 : 0, this.configWindow.time.DeltaTime * 5f);
                    }
                    if (SirenixEditorGUI.BeginFadeGroup(this.t))
                    {
                        indent += 1;
                        for (int i = 0; i < this.childMenuItems.Count; i++)
                        {
                            if (this.configWindow.isSearching == false || this.configWindow.isSearching && (this.childMenuItems[i].hasSearchMatch || this.childMenuItems[i].childHasSearchMatch))
                            {
                                this.childMenuItems[i].DrawMenu(indent);
                            }
                        }
                    }
                    SirenixEditorGUI.EndFadeGroup();
                }
            }

            public void DrawConfig()
            {
                if (this.shouldDrawUnityEditor)
                {
                    this.unityEditor.OnInspectorGUI();
                }
                else if (this.propertyTree != null)
                {
                    this.propertyTree.Draw(true);
                }
            }

            public void Sort()
            {
                this.childMenuItems = this.childMenuItems.OrderBy(x => x.childMenuItems == null ? 0 : (x.childMenuItems.Count > 0 ? 1 : 0)).ThenBy(x => x.Name).ToList();
                for (int i = 0; i < this.childMenuItems.Count; i++)
                {
                    this.childMenuItems[i].Sort();
                }
            }

            public void SearchConfig()
            {
                this.InitializeUnityObject();
                if (this.unityObject != null)
                {
                    // Search has been out-commented for now.
                    //string source = SearchHelper.GetSearchTerm(this.unityObject) + this.Name;
                    //if (FuzzySearch.Contains(ref source, ref this.configWindow.searchTerm))
                    //{
                    //    this.SetChildHasSearchMatchOnParent();
                    //}
                }

                for (int i = 0; i < this.childMenuItems.Count; i++)
                {
                    this.childMenuItems[i].SearchConfig();
                }
            }

            private void SetChildHasSearchMatchOnParent()
            {
                if (this.parent != null)
                {
                    this.parent.childHasSearchMatch = true;
                    this.parent.SetChildHasSearchMatchOnParent();
                }
            }

            public void ResetSearchMatch()
            {
                this.hasSearchMatch = false;
                this.childHasSearchMatch = false;
                for (int i = 0; i < this.childMenuItems.Count; i++)
                {
                    this.childMenuItems[i].ResetSearchMatch();
                }
            }

            public void ShowContextMenu()
            {
                this.InitializeUnityObject();

                if (this.unityObject != null)
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Locate in Project Explorer"), false, x =>
                    {
                        EditorGUIUtility.PingObject(this.unityObject);
                    }, null);
                    menu.AddItem(new GUIContent("Reset to default"), false, x =>
                    {
                        if (EditorUtility.DisplayDialog("Reset " + this.Name + " to default", "Are you sure you want to reset all settings on " + this.Name + " to default values? This cannot be undone.", "Yes", "No"))
                        {
                            AssetDatabase.DeleteAsset(AssetDatabase.GetAssetPath(this.unityObject));
                            AssetDatabase.Refresh();
                            DestroyImmediate(this.unityObject);
                        }
                    }, null);
                    menu.ShowAsContext();
                }
            }

            internal void Open()
            {
                this.IsOpen = true;
                if (this.parent != null)
                {
                    this.parent.Open();
                }
            }
        }
    }
}
#endif