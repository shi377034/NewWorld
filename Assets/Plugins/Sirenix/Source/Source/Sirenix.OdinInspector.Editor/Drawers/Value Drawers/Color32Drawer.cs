#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="Color32Drawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Color32 property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class Color32Drawer : PrimitiveCompositeDrawer<Color32>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyField(IPropertyValueEntry<Color32> entry, GUIContent label)
        {
            //var rect = entry.Property.Context.Get<Rect>(this, "rect");

            //ColorDrawer.HandleRightClick(entry, rect);

            entry.SmartValue = label == null ?
                EditorGUILayout.ColorField(entry.SmartValue) :
                EditorGUILayout.ColorField(label, entry.SmartValue);
            //if (Event.current.type == EventType.Repaint)
            //{
            //    rect.Value = GUILayoutUtility.GetLastRect();
            //}
        }

        void IDefinesGenericMenuItems.PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            ColorDrawer.PopulateGenericMenu((IPropertyValueEntry<Color32>)property.ValueEntry, genericMenu);
        }
    }
}
#endif