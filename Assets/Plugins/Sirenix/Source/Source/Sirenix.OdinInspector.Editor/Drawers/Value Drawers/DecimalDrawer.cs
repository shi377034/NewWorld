#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DecimalDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System.Globalization;

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Decimal property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class DecimalDrawer : OdinValueDrawer<decimal>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<decimal> entry, GUIContent label)
        {
            string value = entry.SmartValue.ToString(CultureInfo.InvariantCulture);
            value = EditorGUILayout.DelayedTextField(label, value, EditorStyles.textField);
            decimal dec;

            if (GUI.changed && decimal.TryParse(value, NumberStyles.Any, null, out dec))
            {
                entry.SmartValue = dec;
            }
        }
    }
}
#endif