#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ColorDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities;
    using UnityEditor;
    using UnityEngine;
    using System;

    /// <summary>
    /// Color property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class ColorDrawer : PrimitiveCompositeDrawer<Color>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyField(IPropertyValueEntry<Color> entry, GUIContent label)
        {
            //var rect = entry.Property.Context.Get<Rect>(this, "rect");

            //HandleRightClick(entry, rect);

            entry.SmartValue = label == null ?
                EditorGUILayout.ColorField(entry.SmartValue) :
                EditorGUILayout.ColorField(label, entry.SmartValue);
            //if (Event.current.type == EventType.Repaint)
            //{
            //    rect.Value = GUILayoutUtility.GetLastRect();
            //}
        }

        //internal static void HandleRightClick<T>(IPropertyValueEntry<T> entry, PropertyContext<Rect> rect) where T : struct
        //{
        //    if (Event.current.type == EventType.ContextClick || Event.current.type == EventType.MouseDown && Event.current.button == 1)
        //    {
        //        Color color = (Color)(object)entry.SmartValue;
        //        Vector2 mousePos = Event.current.mousePosition;
        //        if (rect.Value.Contains(mousePos))
        //        {
        //            menu.ShowAsContext();
        //            Event.current.Use();
        //        }
        //    }
        //}

        internal static void PopulateGenericMenu<T>(IPropertyValueEntry<T> entry, GenericMenu genericMenu)
        {
            Color color = (Color)(object)entry.SmartValue;

            Color colorInClipboard;
            bool hasColorInClipboard = ColorExtensions.TryParseString(EditorGUIUtility.systemCopyBuffer, out colorInClipboard);

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }

            genericMenu.AddItem(new GUIContent("Copy RGBA"), false, (x) =>
            {
                EditorGUIUtility.systemCopyBuffer = entry.SmartValue.ToString();
            }, null);
            genericMenu.AddItem(new GUIContent("Copy HEX"), false, (x) =>
            {
                EditorGUIUtility.systemCopyBuffer = "#" + ColorUtility.ToHtmlStringRGBA(color);
            }, null);
            genericMenu.AddItem(new GUIContent("Copy Color Code Declaration"), false, (x) =>
            {
                EditorGUIUtility.systemCopyBuffer = ColorExtensions.ToCSharpColor(color);
            }, null);
            if (hasColorInClipboard)
            {
                genericMenu.AddItem(new GUIContent("Paste"), false, (x) =>
                {
                    entry.SmartValue = (T)(object)colorInClipboard;
                    entry.ApplyChanges();
                }, "Paste");
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Paste"));
            }
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            PopulateGenericMenu((IPropertyValueEntry<Color>)property.ValueEntry, genericMenu);
        }
    }
}
#endif