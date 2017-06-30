#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="LabelTextAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="LabelTextAttribute"/>.
	/// Creates a new GUIContent, with the provided label text, before calling further down in the drawer chain.
    /// </summary>
	/// <seealso cref="LabelTextAttribute"/>
	/// <seealso cref="HideLabelAttribute"/>
	/// <seealso cref="TooltipAttribute"/>
	/// <seealso cref="TitleAttribute"/>
	/// <seealso cref="HeaderAttribute"/>
	/// <seealso cref="GUIColorAttribute"/>
    [OdinDrawer]
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class LabelTextAttributeDrawer : OdinAttributeDrawer<LabelTextAttribute>
    {
        /// <summary>
        /// Draws the attribute.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, LabelTextAttribute attribute, GUIContent label)
        {
            this.CallNextDrawer(property, GUIHelper.TempContent(attribute.Text));
        }
    }
}
#endif