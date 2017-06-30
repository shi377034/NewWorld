#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="AnimationCurveDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Animation curve property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class AnimationCurveDrawer : OdinValueDrawer<AnimationCurve>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<AnimationCurve> entry, GUIContent label)
        {
            if (label == null)
            {
                entry.SmartValue = EditorGUILayout.CurveField(entry.SmartValue);
            }
            else
            {
                entry.SmartValue = EditorGUILayout.CurveField(label, entry.SmartValue);
            }
        }
    }
}
#endif