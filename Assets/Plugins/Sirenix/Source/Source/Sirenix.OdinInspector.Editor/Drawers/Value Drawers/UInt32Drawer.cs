#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="UInt32Drawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Uint property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class UInt32Drawer : OdinValueDrawer<uint>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<uint> entry, GUIContent label)
        {
            long value = label == null ?
                EditorGUILayout.LongField(entry.SmartValue) :
                EditorGUILayout.LongField(label, entry.SmartValue);

            if (value > uint.MaxValue)
            {
                value = uint.MaxValue;
            }
            else if (value < 0)
            {
                value = 0;
            }

            entry.SmartValue = (uint)value;
        }
    }
}
#endif