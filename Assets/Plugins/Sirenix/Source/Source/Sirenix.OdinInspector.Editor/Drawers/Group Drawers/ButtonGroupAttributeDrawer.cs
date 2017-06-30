#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ButtonGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="ButtonGroupAttribute"/>
    /// </summary>
    /// <seealso cref="ButtonGroupAttribute"/>
    [OdinDrawer]
    public class ButtonGroupAttributeDrawer : OdinGroupDrawer<ButtonGroupAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, ButtonGroupAttribute attribute, GUIContent label)
        {
            SirenixEditorGUI.BeginIndentedHorizontal();

            for (int i = 0; i < property.Children.Count; i++)
            {
                var style = (GUIStyle)null;

                if (property.Children.Count != 1)
                {
                    if (i == 0)
                    {
                        style = SirenixGUIStyles.ButtonLeft;
                    }
                    else if (i == property.Children.Count - 1)
                    {
                        style = SirenixGUIStyles.ButtonRight;
                    }
                    else
                    {
                        style = SirenixGUIStyles.ButtonMid;
                    }
                }

                property.Children[i].Context.GetGlobal("ButtonStyle", style).Value = style;
                InspectorUtilities.DrawProperty(property.Children[i]);
            }

            SirenixEditorGUI.EndIndentedHorizontal();
        }
    }
}
#endif