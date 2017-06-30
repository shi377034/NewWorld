#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="CompositeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Drawer for composite properties.
    /// </summary>
    [DrawerPriority(0, 0, 0)]
    public sealed class CompositeDrawer : OdinDrawer
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyImplementation(InspectorProperty property, GUIContent label)
        {
            if (label == null)
            {
                for (int i = 0; i < property.Children.Count; i++)
                {
                    InspectorUtilities.DrawProperty(property.Children[i]);
                }
            }
            else
            {
                var isVisible = property.Context.Get(this, "isVisible", !GeneralDrawerConfig.Instance.ExpandFoldoutByDefault);
                isVisible.Value = SirenixEditorGUI.Foldout(isVisible.Value, label);

                if (SirenixEditorGUI.BeginFadeGroup(isVisible, isVisible.Value, GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
                {
                    EditorGUI.indentLevel++;
                    for (int i = 0; i < property.Children.Count; i++)
                    {
                        InspectorUtilities.DrawProperty(property.Children[i]);
                    }
                    EditorGUI.indentLevel--;
                }
                SirenixEditorGUI.EndFadeGroup();
            }
        }
    }
}
#endif