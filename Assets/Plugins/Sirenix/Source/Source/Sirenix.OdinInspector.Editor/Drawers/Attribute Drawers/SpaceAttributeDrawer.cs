#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SpaceAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="SpaceAttribute"/>.
    /// </summary>
    /// <seealso cref="SpaceAttribute"/>
    [OdinDrawer]
    [DrawerPriority(0, 1000, 0)]
    public sealed class SpaceAttributeDrawer : OdinAttributeDrawer<SpaceAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, SpaceAttribute attribute, GUIContent label)
        {
            if (attribute.height == 0)
            {
                EditorGUILayout.Space();
            }
            else
            {
                GUILayout.Space(attribute.height);
            }

            this.CallNextDrawer(property, label);
        }
    }
}
#endif