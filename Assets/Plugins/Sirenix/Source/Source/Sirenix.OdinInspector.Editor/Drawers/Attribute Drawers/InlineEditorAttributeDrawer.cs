#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="InlineAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System.Collections.Generic;

    /// <summary>
    /// Draws properties marked with <see cref="InlineEditorAttribute"/>.
    /// </summary>
	/// <seealso cref="InlineEditorAttribute"/>
	/// <seealso cref="DrawWithUnityAttribute"/>
    [OdinDrawer]
    [DrawerPriority(0, 0, 3000)]
    public class InlineEditorAttributeDrawer<T> : OdinAttributeDrawer<InlineEditorAttribute, T> where T : UnityEngine.Object
    {
        private class InlineAttributeContext
        {
            public bool Toggled;
            public Editor Editor;
            public Editor PreviewEditor;
            public Rect InlineEditorRect;
            public UnityEngine.Object Target;
            public bool DrawHeader;
            internal bool DrawPreview;
            internal bool DrawGUI;

            public void DestroyEditors()
            {
                if (this.PreviewEditor != this.Editor && this.PreviewEditor != null)
                {
                    UnityEngine.Object.DestroyImmediate(this.PreviewEditor);
                    this.PreviewEditor = null;
                }

                if (this.Editor != null)
                {
                    UnityEngine.Object.DestroyImmediate(this.Editor);
                    this.Editor = null;
                }

                Selection.selectionChanged -= this.DestroyEditors;
            }
        }

        private static int InlineLevel = 0;

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, InlineEditorAttribute attribute, GUIContent label)
        {
            var config = entry.Property.Context.Get(this, "InlineAttributeConfig", (InlineAttributeContext)null);
            if (config.Value == null)
            {
                config.Value = new InlineAttributeContext();
                config.Value.Toggled = attribute.Expanded && InlineLevel == 0;
                Selection.selectionChanged += config.Value.DestroyEditors;
            }

            var unityObj = (UnityEngine.Object)entry.WeakSmartValue;

            SirenixEditorGUI.BeginBox();
            SirenixEditorGUI.BeginBoxHeader();
            {
                GUIHelper.PushGUIEnabled(GUI.enabled && entry.IsEditable);

                bool makeRoomForFoldoutIfNoLabel = false;

                if (entry.ValueState != PropertyValueState.NullReference && label != null)
                {
                    label = GUIHelper.TempContent("   " + label.text, label.image, label.tooltip);
                }
                else if (label == null)
                {
                    makeRoomForFoldoutIfNoLabel = true;
                }
                if (makeRoomForFoldoutIfNoLabel)
                {
                    EditorGUI.indentLevel++;
                }

                GUIUtility.GetControlID(999, FocusType.Passive);

                var prev = EditorGUI.showMixedValue;

                if (entry.ValueState == PropertyValueState.ReferenceValueConflict)
                {
                    EditorGUI.showMixedValue = true;
                }

                var newValue = SirenixEditorGUI.ObjectField(
                    label,
                    unityObj,
                    entry.BaseValueType,
                    entry.Property.Info.GetAttribute<AssetsOnlyAttribute>() == null);

                EditorGUI.showMixedValue = prev;

                if (makeRoomForFoldoutIfNoLabel)
                {
                    EditorGUI.indentLevel--;
                }

                if (entry.ValueState != PropertyValueState.NullReference)
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    config.Value.Toggled = SirenixEditorGUI.Foldout(rect, config.Value.Toggled, GUIContent.none);
                }

                GUIHelper.PopGUIEnabled();

                if (newValue != unityObj)
                {
                    entry.Property.Tree.DelayActionUntilRepaint(() => entry.WeakSmartValue = newValue);
                }

                if (newValue == null && config.Value.Editor != null)
                {
                    config.Value.DestroyEditors();
                }
            }
            SirenixEditorGUI.EndBoxHeader();
            if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(entry.Property, this), config.Value.Toggled, GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
            {
                if (entry.ValueState == PropertyValueState.ReferenceValueConflict || entry.ValueState == PropertyValueState.ReferencePathConflict)
                {
                    SirenixEditorGUI.InfoMessageBox("Multi object editing is currently not supported for inline editors.");
                }
                else
                {
                    bool createNewEditor = unityObj != null && (config.Value.Editor == null || config.Value.Target != unityObj || config.Value.Target == null);
                    if (createNewEditor)
                    {
                        config.Value.Target = unityObj;

                        bool isGameObject = unityObj as GameObject;

                        config.Value.DrawHeader = isGameObject ? attribute.DrawHeader : attribute.DrawHeader;
                        config.Value.DrawGUI = isGameObject ? false : attribute.DrawGUI;
                        config.Value.DrawPreview = attribute.DrawPreview || isGameObject && attribute.DrawGUI;

                        if (config.Value.Editor != null)
                        {
                            config.Value.DestroyEditors();
                        }

                        config.Value.Editor = Editor.CreateEditor(config.Value.Target);

                        var component = config.Value.Target as Component;
                        if (component != null)
                        {
                            config.Value.PreviewEditor = Editor.CreateEditor(component.gameObject);
                        }
                        else
                        {
                            config.Value.PreviewEditor = config.Value.Editor;
                        }

                        var materialEditor = config.Value.Editor as MaterialEditor;
                        if (materialEditor != null)
                        {
                            typeof(MaterialEditor).GetProperty("forceVisible", Flags.AllMembers).SetValue(materialEditor, true, null);
                        }
                    }

                    if (config.Value.Editor != null && config.Value.Editor.SafeIsUnityNull() == false)
                    {
                        SaveLayoutSettings();
                        InlineLevel++;
                        try
                        {
                            bool split = config.Value.DrawGUI && config.Value.DrawPreview;
                            if (split)
                            {
                                GUILayout.BeginHorizontal();
                                if (Event.current.type == EventType.Repaint)
                                {
                                    config.Value.InlineEditorRect = GUIHelper.GetCurrentLayoutRect();
                                }
                                GUILayout.BeginVertical();
                            }

                            if (config.Value.DrawHeader)
                            {
                                // Surtan objects such as GameObjects causes some weird layout issues.
                                // So lots of hacks here to prevent GUI popping and other weird layout issues.
                                var tmp = Event.current.rawType;
                                EditorGUILayout.BeginFadeGroup(0.9999f); // This one fixes some layout issues, but locks the input.
                                Event.current.type = tmp; // Lets undo that shall we?
                                GUILayout.Space(0); // Yeah i know. But it removes some top padding.
                                config.Value.Editor.DrawHeader();
                                GUILayout.Space(1); // This adds the the 1 pixel border clipped from the fade group.
                                EditorGUILayout.EndFadeGroup();
                            }
                            else
                            {
                                // Many of unity editors, wont work if the header is not drawn.
                                GUIHelper.BeginDrawToNothing();
                                config.Value.Editor.DrawHeader();
                                GUIHelper.EndDrawToNothing();
                            }

                            if (config.Value.DrawGUI)
                            {
                                var prev = GeneralDrawerConfig.Instance.ShowMonoScriptInEditor;
                                GeneralDrawerConfig.Instance.ShowMonoScriptInEditor = false;
                                config.Value.Editor.OnInspectorGUI();
                                GeneralDrawerConfig.Instance.ShowMonoScriptInEditor = prev;
                            }

                            if (split)
                            {
                                GUILayout.EndVertical();
                            }

                            if (config.Value.DrawPreview && config.Value.PreviewEditor.HasPreviewGUI())
                            {
                                Rect tmpRect;

                                var size = split ? attribute.PreviewWidth : attribute.PreviewHeight;

                                if (split)
                                {
                                    tmpRect = GUILayoutUtility.GetRect(size + 15, size, GUILayoutOptions.Width(size).Height(size));
                                }
                                else
                                {
                                    tmpRect = GUILayoutUtility.GetRect(0, size, GUILayoutOptions.Height(size).ExpandWidth(true));
                                }

                                if (!split && Event.current.type == EventType.Repaint || config.Value.InlineEditorRect.height < 1)
                                {
                                    config.Value.InlineEditorRect = tmpRect;
                                }

                                var rect = config.Value.InlineEditorRect;
                                if (split)
                                {
                                    rect.xMin += rect.width - size;
                                }
                                rect.height = Mathf.Clamp(rect.height, 30, 1000);
                                rect.width = Mathf.Clamp(rect.width, 30, 1000);
                                var tmp = GUI.enabled;
                                GUI.enabled = true;
                                config.Value.PreviewEditor.DrawPreview(rect);
                                GUI.enabled = tmp;
                            }

                            if (split)
                            {
                                GUILayout.EndHorizontal();
                            }
                        }
                        catch (Exception ex)
                        {
                            if (ex is ExitGUIException || ex.InnerException is ExitGUIException)
                            {
                                throw ex;
                            }
                            else
                            {
                                Debug.LogException(ex);
                            }
                        }

                        InlineLevel--;
                        RestoreLayout();
                    }
                }
            }
            else
            {
                if (config.Value.Editor != null)
                {
                    config.Value.DestroyEditors();
                    config.Value.Editor = null;
                }
            }
            SirenixEditorGUI.EndFadeGroup();
            SirenixEditorGUI.EndBox();
        }

        private static Stack<LayoutSettings> layoutSettingsStack = new Stack<LayoutSettings>();

        private static void SaveLayoutSettings()
        {
            layoutSettingsStack.Push(new LayoutSettings()
            {
                Skin = GUI.skin,
                Color = GUI.color,
                ContentColor = GUI.contentColor,
                BackgroundColor = GUI.backgroundColor,
                Enabled = GUI.enabled,
                Changed = GUI.changed,
                IndentLevel = EditorGUI.indentLevel,
                FieldWidth = EditorGUIUtility.fieldWidth,
                LabelWidth = EditorGUIUtility.labelWidth,
                HierarchyMode = EditorGUIUtility.hierarchyMode,
                WideMode = EditorGUIUtility.wideMode,
            });
        }

        private static void RestoreLayout()
        {
            var settings = layoutSettingsStack.Pop();

            GUI.skin = settings.Skin;
            GUI.color = settings.Color;
            GUI.contentColor = settings.ContentColor;
            GUI.backgroundColor = settings.BackgroundColor;
            GUI.enabled = settings.Enabled;
            GUI.changed = settings.Changed;
            EditorGUI.indentLevel = settings.IndentLevel;
            EditorGUIUtility.fieldWidth = settings.FieldWidth;
            EditorGUIUtility.labelWidth = settings.LabelWidth;
            EditorGUIUtility.hierarchyMode = settings.HierarchyMode;
            EditorGUIUtility.wideMode = settings.WideMode;
        }

        private struct LayoutSettings
        {
            public GUISkin Skin;
            public Color Color;
            public Color ContentColor;
            public Color BackgroundColor;
            public bool Enabled;
            public bool Changed;
            public int IndentLevel;
            public float FieldWidth;
            public float LabelWidth;
            public bool HierarchyMode;
            public bool WideMode;
        }
    }
}
#endif