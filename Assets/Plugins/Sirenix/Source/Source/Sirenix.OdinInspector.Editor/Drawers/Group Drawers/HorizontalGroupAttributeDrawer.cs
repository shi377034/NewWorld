#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="HorizontalGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities;
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Drawer for the <see cref="HorizontalGroupAttribute"/>
    /// </summary>
    /// <seealso cref="HorizontalGroupAttribute"/>
    [OdinDrawer]
    public class HorizontalGroupAttributeDrawer : OdinGroupDrawer<HorizontalGroupAttribute>
    {
        private class Context
        {
            public float[] PropertySizes;
            public float FixedWidthAllocated;
            public float TotalWidth;
            public GUIStyle[] LayoutStyles;
            public int flexiblePropertyIndex = 0;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, HorizontalGroupAttribute attribute, GUIContent label)
        {
            var context = property.Context.Get(this, "Context", (Context)null);

            if (context.Value == null)
            {
                context.Value = new Context();

                context.Value.PropertySizes = new float[property.Children.Count];
                context.Value.LayoutStyles = new GUIStyle[property.Children.Count];

                float autoWidthPropertyCount = 0;
                float percentageAllocated = 0;

                // flexiblePropertyIndex
                // At least one property must have a flexible width in order to prevent
                // a horizontal scroll-bar from appearing when resizing the editor.
                context.Value.flexiblePropertyIndex = property.Children.Count - 1;
                for (int i = 0; i < property.Children.Count; i++)
                {
                    var attr = property.Children[i].Info.GetAttribute<HorizontalGroupAttribute>();
                    context.Value.PropertySizes[i] = attr.Width;
                    if (attr.Width > 1f)
                    {
                        context.Value.FixedWidthAllocated += attr.Width;
                    }
                    else
                    {
                        percentageAllocated += attr.Width;
                        context.Value.flexiblePropertyIndex = i;
                    }
                    context.Value.FixedWidthAllocated += attr.MarginLeft + attr.MarginRight;
                    context.Value.LayoutStyles[i] = new GUIStyle() { margin = new RectOffset(attr.MarginLeft, attr.MarginRight, 0, 0) };

                    if (attr.Width == 0)
                    {
                        autoWidthPropertyCount += 1;
                    }
                }

                float percentage = (1 / autoWidthPropertyCount) - percentageAllocated;

                for (int i = 0; i < property.Children.Count; i++)
                {
                    var attr = property.Children[i].Info.GetAttribute<HorizontalGroupAttribute>();

                    if (attr.Width == 0)
                    {
                        context.Value.PropertySizes[i] = percentage;
                    }
                }
            }

            SirenixEditorGUI.BeginIndentedHorizontal();
            {
                if (Event.current.type == EventType.Repaint)
                {
                    context.Value.TotalWidth = GUIHelper.GetCurrentLayoutRect().width - context.Value.FixedWidthAllocated;
                }

                for (int i = 0; i < property.Children.Count; i++)
                {
                    float width = context.Value.PropertySizes[i];

                    if (width <= 1)
                    {
                        width = width * context.Value.TotalWidth;
                    }

                    float percentage = width / context.Value.TotalWidth;

                    var prevFieldWidth = UnityEditor.EditorGUIUtility.fieldWidth;
                    var prevLabelWidth = UnityEditor.EditorGUIUtility.labelWidth;

                    UnityEditor.EditorGUIUtility.fieldWidth = prevFieldWidth * percentage;
                    UnityEditor.EditorGUIUtility.labelWidth = prevLabelWidth * percentage;

                    if (i == context.Value.flexiblePropertyIndex)
                    {
                        GUILayout.BeginVertical(context.Value.LayoutStyles[i]);
                    }
                    else
                    {
                        GUILayout.BeginVertical(context.Value.LayoutStyles[i], GUILayoutOptions.Width(width));
                    }

                    InspectorUtilities.DrawProperty(property.Children[i], property.Children[i].Label);
                    GUILayout.EndVertical();

                    UnityEditor.EditorGUIUtility.fieldWidth = prevFieldWidth;
                    UnityEditor.EditorGUIUtility.labelWidth = prevLabelWidth;
                }
            }
            SirenixEditorGUI.EndIndentedHorizontal();
        }
    }
}
#endif