#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ColorPaletteAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    [OdinDrawer]
    [DrawerPriority(DrawerPriorityLevel.AttributePriority)]
    public sealed class ColorPaletteAttributeDrawer : OdinAttributeDrawer<ColorPaletteAttribute, Color>
    {
        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<Color> entry, ColorPaletteAttribute attribute, GUIContent label)
        {
            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                var hideLabel = label == null;
                if (hideLabel == false)
                {
                    GUILayout.Label(label, GUILayoutOptions.Width(EditorGUIUtility.labelWidth - 4).ExpandWidth(false));
                }
                else
                {
                    GUILayout.Space(5);
                }
                var colorPaletDropDown = entry.Property.Context.Get(this, "colorPalette", 0);
                var currentName = entry.Property.Context.Get(this, "currentName", attribute.PaletteName);
                var showAlpha = entry.Property.Context.Get(this, "showAlpha", attribute.ShowAlpha);
                var names = ColorPaletteManager.Instance.GetColorPaletteNames();

                ColorPalette colorPalette;
                var rect = EditorGUILayout.BeginHorizontal();
                {
                    rect.x -= 3;
                    rect.width = 25;

                    entry.SmartValue = SirenixEditorGUI.DrawColorField(rect, entry.SmartValue, false, showAlpha.Value);
                    bool openInEditorShown = false;
                    GUILayout.Space(28);
                    SirenixEditorGUI.BeginInlineBox();
                    {
                        if (attribute.PaletteName == null || ColorPaletteManager.Instance.ShowPaletteName)
                        {
                            SirenixEditorGUI.BeginBoxHeader();
                            {
                                if (attribute.PaletteName == null)
                                {
                                    var newValue = EditorGUILayout.Popup(colorPaletDropDown.Value, names, GUILayoutOptions.ExpandWidth(true));
                                    if (colorPaletDropDown.Value != newValue)
                                    {
                                        colorPaletDropDown.Value = newValue;
                                        GUI.FocusControl(null);
                                    }
                                }
                                else
                                {
                                    GUILayout.Label(currentName.Value);
                                    GUILayout.FlexibleSpace();
                                }
                                openInEditorShown = true;
                                if (SirenixEditorGUI.IconButton(EditorIcons.SettingsCog))
                                {
                                    ColorPaletteManager.Instance.OpenInEditor();
                                }
                            }
                            SirenixEditorGUI.EndBoxHeader();
                        }

                        if (attribute.PaletteName == null)
                        {
                            colorPalette = ColorPaletteManager.Instance.GetColorPalette(names[colorPaletDropDown.Value]);
                        }
                        else
                        {
                            colorPalette = ColorPaletteManager.Instance.GetColorPalette(attribute.PaletteName);
                        }

                        if (colorPalette == null)
                        {
                            GUILayout.BeginHorizontal();
                            {
                                if (attribute.PaletteName != null)
                                {
                                    if (GUILayout.Button("Create color pallette: " + attribute.PaletteName))
                                    {
                                        ColorPaletteManager.Instance.CreateColorPalette(attribute.PaletteName);
                                        ColorPaletteManager.Instance.OpenInEditor();
                                    }
                                }
                            }
                            GUILayout.EndHorizontal();
                        }
                        else
                        {
                            currentName.Value = colorPalette.Name;
                            showAlpha.Value = attribute.ShowAlpha && colorPalette.ShowAlpha;
                            if (openInEditorShown == false)
                            {
                                GUILayout.BeginHorizontal();
                            }
                            var color = entry.SmartValue;
                            var stretch = ColorPaletteManager.Instance.StretchPalette;
                            var size = ColorPaletteManager.Instance.SwatchSize;
                            var margin = ColorPaletteManager.Instance.SwatchSpacing;
                            if (DrawColorPaletteColorPicker(entry, colorPalette, ref color, colorPalette.ShowAlpha, stretch, size, 20, margin))
                            {
                                entry.SmartValue = color;
                                entry.ApplyChanges();
                            }
                            if (openInEditorShown == false)
                            {
                                GUILayout.Space(4);
                                if (SirenixEditorGUI.IconButton(EditorIcons.SettingsCog))
                                {
                                    ColorPaletteManager.Instance.OpenInEditor();
                                }
                                GUILayout.EndHorizontal();
                            }
                        }
                    }
                    SirenixEditorGUI.EndInlineBox();
                }
                EditorGUILayout.EndHorizontal();
            }

            SirenixEditorGUI.EndIndentedHorizontal();
        }

        internal static bool DrawColorPaletteColorPicker(object key, ColorPalette colorPalette, ref Color color, bool drawAlpha, bool stretchPalette, float width = 20, float height = 20, float margin = 0)
        {
            bool result = false;

            var rect = SirenixEditorGUI.BeginHorizontalAutoScrollBox(key, GUILayoutOptions.ExpandWidth(true).ExpandHeight(false));
            {
                if (stretchPalette)
                {
                    rect.width -= margin * colorPalette.Colors.Count - margin;
                    width = Mathf.Max(width, rect.width / colorPalette.Colors.Count);
                }
                bool isMouseDown = Event.current.type == EventType.MouseDown;
                var innerRect = GUILayoutUtility.GetRect((width + margin) * colorPalette.Colors.Count, height, GUIStyle.none);
                float spacing = width + margin;
                var cellRect = innerRect;
                cellRect.width = width;
                for (int i = 0; i < colorPalette.Colors.Count; i++)
                {
                    cellRect.x = spacing * i;

                    if (drawAlpha)
                    {
                        EditorGUIUtility.DrawColorSwatch(cellRect, colorPalette.Colors[i]);
                    }
                    else
                    {
                        var c = colorPalette.Colors[i];
                        c.a = 1;
                        SirenixEditorGUI.DrawSolidRect(cellRect, c);
                    }

                    if (isMouseDown && cellRect.Contains(Event.current.mousePosition))
                    {
                        color = colorPalette.Colors[i];
                        result = true;
                        Event.current.Use();
                    }
                }
            }
            SirenixEditorGUI.EndHorizontalAutoScrollBox();
            return result;
        }
    }
}
#endif