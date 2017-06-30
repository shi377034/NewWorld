#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="UnityEventDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Unity event drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class UnityEventDrawer<T> : OdinValueDrawer<T> where T : UnityEventBase
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, GUIContent label)
        {
            var unityProperty = entry.Property.Tree.GetUnityPropertyForPath(entry.Property.Path);

            if (unityProperty == null)
            {
                SirenixEditorGUI.ErrorMessageBox("Could not create an alias UnityEditor.SerializedProperty for the property '" + entry.Property.Name + "'.");
                return;
            }

            bool isEmitted = unityProperty.serializedObject.targetObject is EmittedScriptableObject<T>;

            if (isEmitted || entry.Property.Tree.UnitySerializedObject == null || (typeof(Component).IsAssignableFrom(entry.Property.Tree.UnitySerializedObject.targetObject.GetType()) == false))
            {
                SirenixEditorGUI.WarningMessageBox("Cannot properly draw UnityEvents for properties that are not directly serialized by Unity from a component. To get the classic Unity event appearance, please turn " + entry.Property.Name + " into a public field, or a private field with the [SerializedField] attribute on, and ensure that it is defined on a component.");
                this.CallNextDrawer(entry.Property, label);
            }
            else
            {
                EditorGUILayout.PropertyField(unityProperty, true);
            }
        }
    }
}
#endif