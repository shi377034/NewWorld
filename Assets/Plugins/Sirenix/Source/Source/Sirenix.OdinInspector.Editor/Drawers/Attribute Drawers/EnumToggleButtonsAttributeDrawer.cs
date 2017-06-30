#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EnumToggleButtonsAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using System;

    /// <summary>
    /// Draws an enum in a horizontal button group instead of a dropdown.
    /// </summary>
    [OdinDrawer]
    public class EnumToggleButtonsAttributeDrawer<T> : OdinAttributeDrawer<EnumToggleButtonsAttribute, T>
    {
        /// <summary>
        /// Returns <c>true</c> if the drawer can draw the type.
        /// </summary>
        public override bool CanDrawTypeFilter(Type type)
        {
            return type.IsEnum;
        }

        private class Context
        {
            public GUIContent[] Names;
            public ulong[] Values;
            public bool IsFlagsEnum;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, EnumToggleButtonsAttribute attribute, GUIContent label)
        {
            var t = entry.WeakValues[0].GetType();
            for (int i = 1; i < entry.WeakValues.Count; i++)
            {
                if (t != entry.WeakValues[i].GetType())
                {
                    SirenixEditorGUI.ErrorMessageBox("ToggleEnum does not support multiple different enum types.");
                    return;
                }
            }

            var enumType = entry.TypeOfValue;

            var context = entry.Property.Context.Get(this, "context", (Context)null);

            if (context.Value == null)
            {
                context.Value = new Context();
                context.Value.Names = Enum.GetNames(enumType).Select(x => new GUIContent(x)).ToArray();
                context.Value.Values = new ulong[context.Value.Names.Length];
                context.Value.IsFlagsEnum = entry.TypeOfValue.IsDefined<FlagsAttribute>();

                for (int i = 0; i < context.Value.Values.Length; i++)
                {
                    context.Value.Values[i] = TypeExtensions.GetEnumBitmask(Enum.Parse(enumType, context.Value.Names[i].text), enumType);
                }
            }
            ulong value = TypeExtensions.GetEnumBitmask(entry.SmartValue, enumType);

            var rect = EditorGUILayout.GetControlRect();
            if (label != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }
            var xMax = rect.xMax;
            rect.width /= context.Value.Values.Length;
            rect.width = (int)rect.width;

            for (int i = 0; i < context.Value.Names.Length; i++)
            {
                bool selected;

                if (context.Value.IsFlagsEnum)
                {
                    var mask = TypeExtensions.GetEnumBitmask(context.Value.Values[i], enumType);
                    selected = (mask & value) == mask;
                }
                else
                {
                    selected = context.Value.Values[i] == value;
                }

                GUIStyle style;
                if (i == 0)
                    style = selected ? SirenixGUIStyles.ButtonLeftSelected : SirenixGUIStyles.ButtonLeft;
                else if (i == context.Value.Names.Length - 1)
                {
                    style = selected ? SirenixGUIStyles.ButtonRightSelected : SirenixGUIStyles.ButtonRight;
                    rect.xMax = xMax;
                }
                else
                    style = selected ? SirenixGUIStyles.ButtonMidSelected : SirenixGUIStyles.ButtonMid;

                if (GUI.Button(rect, context.Value.Names[i], style))
                {
                    if (!context.Value.IsFlagsEnum || Event.current.button == 1 || Event.current.modifiers == EventModifiers.Control)
                    {
                        entry.WeakSmartValue = Enum.ToObject(enumType, context.Value.Values[i]);
                    }
                    else
                    {
                        if (selected)
                        {
                            value &= ~context.Value.Values[i];
                        }
                        else
                        {
                            value |= context.Value.Values[i];
                        }

                        entry.WeakSmartValue = Enum.ToObject(enumType, value);
                    }

                    GUIHelper.RequestRepaint();
                }

                rect.x += rect.width;
            }
        }
    }
}
#endif