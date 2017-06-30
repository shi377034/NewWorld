#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="HeaderDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="HeaderAttribute"/>.
    /// </summary>
	/// <seealso cref="HeaderAttribute"/>
	/// <seealso cref="TitleAttribute"/>
	/// <seealso cref="HideLabelAttribute"/>
	/// <seealso cref="LabelTextAttribute"/>
	/// <seealso cref="SpaceAttribute"/>
    [OdinDrawer]
    [DrawerPriority(1, 0, 0)]
    public sealed class HeaderAttributeDrawer : OdinAttributeDrawer<HeaderAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, HeaderAttribute attribute, GUIContent label)
        {
            if (property != property.Tree.GetRootProperty(0))
            {
                EditorGUILayout.Space();
            }

            EditorGUILayout.LabelField(attribute.header ?? "null", EditorStyles.boldLabel);
            this.CallNextDrawer(property, label);
        }
    }
}
#endif