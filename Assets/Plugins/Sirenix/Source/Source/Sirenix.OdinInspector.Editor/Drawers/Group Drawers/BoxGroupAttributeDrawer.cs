#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="BoxGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="BoxGroupAttribute"/>
    /// </summary>
    /// <seealso cref="BoxGroupAttribute"/>
    [OdinDrawer]
    public class BoxGroupAttributeDrawer : OdinGroupDrawer<BoxGroupAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, BoxGroupAttribute attribute, GUIContent label)
        {
            SirenixEditorGUI.BeginBox(attribute.ShowLabel ? attribute.Label : null, attribute.CenterLabel);

            for (int i = 0; i < property.Children.Count; i++)
            {
                InspectorUtilities.DrawProperty(property.Children[i]);
            }

            SirenixEditorGUI.EndBox();
        }
    }
}
#endif