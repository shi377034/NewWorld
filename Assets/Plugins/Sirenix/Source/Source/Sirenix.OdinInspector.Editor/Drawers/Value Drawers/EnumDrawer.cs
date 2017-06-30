#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EnumDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// Enum property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class EnumDrawer<T> : OdinValueDrawer<T>
    {
        /// <summary>
        /// Returns <c>true</c> if the drawer can draw the type.
        /// </summary>
        public override bool CanDrawTypeFilter(Type type)
        {
            return type.IsEnum;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            if (entry == null)
            {
                SirenixEditorGUI.ErrorMessageBox("Property " + entry.Property.NiceName + " has no value entry to draw with drawer " + typeof(EnumDrawer<T>).GetNiceName() + ".");
                return;
            }

			EditorGUI.BeginChangeCheck();
            if (entry.TypeOfValue.IsDefined<FlagsAttribute>())
            {
				entry.WeakSmartValue = SirenixEditorFields.EnumMaskDropdown(label, (Enum)entry.WeakSmartValue);
            }
            else
            {
				entry.WeakSmartValue = SirenixEditorFields.EnumDropdown(label, (Enum)entry.WeakSmartValue);
            }

			if(EditorGUI.EndChangeCheck())
			{
				entry.ApplyChanges();
			}
        }
    }
}
#endif