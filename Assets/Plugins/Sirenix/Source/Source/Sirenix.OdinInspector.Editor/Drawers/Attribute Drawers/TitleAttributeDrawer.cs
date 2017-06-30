#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="TitleAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="TitleAttribute"/>.
    /// </summary>
    /// <seealso cref="TitleAttribute"/>
    [OdinDrawer]
    [DrawerPriority(1, 0, 0)]
    public sealed class TitleAttributeDrawer : OdinAttributeDrawer<TitleAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, TitleAttribute attribute, GUIContent label)
        {
            if (property != property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(attribute.Title ?? "null", attribute.Bold ? EditorStyles.boldLabel : EditorStyles.label);
            this.CallNextDrawer(property, label);
        }
    }
}
#endif