#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DisplayAsStringAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="DisplayAsStringAttribute"/>.
	/// Calls the properties ToString method to get the string to draw.
    /// </summary>
	/// <seealso cref="HideLabelAttribute"/>
	/// <seealso cref="LabelTextAttribute"/>
	/// <seealso cref="InfoBoxAttribute"/>
	/// <seealso cref="DetailedInfoBoxAttribute"/>
	/// <seealso cref="MultiLinePropertyAttribute"/>
	/// <seealso cref="MultilineAttribute"/>
    [OdinDrawer]
    [AllowGUIEnabledForReadonly]
    public sealed class DisplayAsStringAttributeDrawer<T> : OdinAttributeDrawer<DisplayAsStringAttribute, T>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, DisplayAsStringAttribute attribute, GUIContent label)
        {
            if (entry.ValueIsStrongList || entry.ValueIsWeakList)
            {
                this.CallNextDrawer(entry.Property, label);
                return;
            }

            string str = entry.SmartValue == null ? "Null" : entry.SmartValue.ToString();

            if (label == null)
            {
                GUILayout.Label(str);
            }
            else
            {
                var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight);
                var rect = EditorGUI.PrefixLabel(position, label);
                GUI.Label(rect, str);
            }
        }
    }
}
#endif