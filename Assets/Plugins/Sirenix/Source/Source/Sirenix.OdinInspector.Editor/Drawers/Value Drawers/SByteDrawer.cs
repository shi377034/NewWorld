#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SByteDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// SByte property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class SByteDrawer : OdinValueDrawer<sbyte>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<sbyte> entry, GUIContent label)
        {
            int value = label == null ?
                EditorGUILayout.IntField(entry.SmartValue) :
                EditorGUILayout.IntField(label, entry.SmartValue);

            if (value < sbyte.MinValue)
            {
                value = sbyte.MinValue;
            }
            else if (value > sbyte.MaxValue)
            {
                value = sbyte.MaxValue;
            }

            entry.SmartValue = (sbyte)value;
        }
    }
}
#endif