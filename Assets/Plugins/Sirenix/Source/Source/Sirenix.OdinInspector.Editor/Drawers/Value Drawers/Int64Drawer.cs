#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="Int64Drawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Long property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class Int64Drawer : OdinValueDrawer<long>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<long> entry, GUIContent label)
        {
            entry.SmartValue = label == null ?
                EditorGUILayout.LongField(entry.SmartValue) :
                EditorGUILayout.LongField(label, entry.SmartValue);
        }
    }
}
#endif