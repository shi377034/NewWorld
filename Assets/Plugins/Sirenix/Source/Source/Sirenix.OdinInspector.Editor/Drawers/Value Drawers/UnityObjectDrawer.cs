#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="UnityObjectDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Unity object drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class UnityObjectDrawer<T> : OdinValueDrawer<T> where T : UnityEngine.Object
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            entry.WeakSmartValue = SirenixEditorGUI.ObjectField(
                label,
                (UnityEngine.Object)entry.WeakSmartValue,
                entry.BaseValueType,
                entry.Property.Info.GetAttribute<AssetsOnlyAttribute>() == null);
        }
    }
}
#endif