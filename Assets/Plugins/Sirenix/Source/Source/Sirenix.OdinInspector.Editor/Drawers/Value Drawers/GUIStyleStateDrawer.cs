#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="GUIStyleStateDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Not yet documented.
    /// </summary>
    [OdinDrawer]
    public class GUIStyleStateDrawer : OdinValueDrawer<GUIStyleState>
    {
        /// <summary>
        /// Not yet documented.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<GUIStyleState> entry, GUIContent label)
        {
            var property = entry.Property;

            var isVisible = property.Context.Get(this, "isVisible", !GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
            isVisible.Value = SirenixEditorGUI.Foldout(isVisible.Value, label);

            if (SirenixEditorGUI.BeginFadeGroup(isVisible, isVisible.Value, GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
            {
                EditorGUI.indentLevel++;
                entry.SmartValue.background = (Texture2D)SirenixEditorGUI.ObjectField(label, entry.SmartValue.background, typeof(Texture2D), true, false);
                entry.SmartValue.textColor = EditorGUILayout.ColorField(label, entry.SmartValue.textColor);
                EditorGUI.indentLevel--;
            }
            SirenixEditorGUI.EndFadeGroup();
        }
    }
}
#endif