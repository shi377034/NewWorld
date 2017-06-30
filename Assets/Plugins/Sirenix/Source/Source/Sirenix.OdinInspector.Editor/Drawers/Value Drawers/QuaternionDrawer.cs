#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="QuaternionValueDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Sirenix.OdinInspector.Editor;
    using Sirenix.Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
    using System;
    using Sirenix.Utilities;

    /// <summary>
    /// Quaternion property drawer.
    /// </summary>
    [OdinDrawer]
    public sealed class QuaternionDrawer : OdinValueDrawer<Quaternion>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<Quaternion> entry, GUIContent label)
        {
            entry.SmartValue = SirenixEditorFields.RotationField(label, entry.SmartValue, GeneralDrawerConfig.Instance.QuaternionDrawMode);
        }

        /// <summary>
        /// Populates the generic menu for the property.
        /// </summary>
        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            if (property.ValueEntry.WeakSmartValue == null) { return; }

            Quaternion value = (Quaternion)property.ValueEntry.WeakSmartValue;
            float angle;
            Vector3 axis;
            value.ToAngleAxis(out angle, out axis);

            // Draw mode
            genericMenu.AddSeparator("");
            var drawMode = GeneralDrawerConfig.Instance.QuaternionDrawMode;
            genericMenu.AddItem(new GUIContent("Euler"), drawMode == QuaternionDrawMode.Eulers, () => SetDrawMode(property, QuaternionDrawMode.Eulers));
            genericMenu.AddItem(new GUIContent("Angle axis"), drawMode == QuaternionDrawMode.AngleAxis, () => SetDrawMode(property, QuaternionDrawMode.AngleAxis));
            genericMenu.AddItem(new GUIContent("Raw"), drawMode == QuaternionDrawMode.Raw, () => SetDrawMode(property, QuaternionDrawMode.Raw));

            // Identity
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Zero"), value == Quaternion.identity, () =>
            {
                for (int i = 0; i < property.ValueEntry.ValueCount; i++)
                {
                    property.ValueEntry.WeakValues[i] = Quaternion.identity;
                }
                property.ValueEntry.ApplyChanges();
                property.Context.Get<Vector4>(this, "AngleAxis").Value = Vector4.zero;
            });

            // Axis shortcuts
            genericMenu.AddSeparator("");

            if (drawMode == QuaternionDrawMode.AngleAxis)
            {
                genericMenu.AddItem(new GUIContent("Right", "Set the axis to (1, 0, 0)"), axis == Vector3.right && angle != 0f, () => SetAxis(property, Vector3.right));
                genericMenu.AddItem(new GUIContent("Left", "Set the axis to (-1, 0, 0)"), axis == Vector3.left, () => SetAxis(property, Vector3.left));
                genericMenu.AddItem(new GUIContent("Up", "Set the axis to (0, 1, 0)"), axis == Vector3.up, () => SetAxis(property, Vector3.up));
                genericMenu.AddItem(new GUIContent("Down", "Set the axis to (0, -1, 0)"), axis == Vector3.down, () => SetAxis(property, Vector3.down));
                genericMenu.AddItem(new GUIContent("Forward", "Set the axis property to (0, 0, 1)"), axis == Vector3.forward, () => SetAxis(property, Vector3.forward));
                genericMenu.AddItem(new GUIContent("Back", "Set the axis property to (0, 0, -1)"), axis == Vector3.back, () => SetAxis(property, Vector3.back));
            }
            else
            {
                genericMenu.AddDisabledItem(new GUIContent("Right"));
                genericMenu.AddDisabledItem(new GUIContent("Left"));
                genericMenu.AddDisabledItem(new GUIContent("Up"));
                genericMenu.AddDisabledItem(new GUIContent("Down"));
                genericMenu.AddDisabledItem(new GUIContent("Forward"));
                genericMenu.AddDisabledItem(new GUIContent("Back"));
            }
        }

        private void SetAxis(InspectorProperty property, Vector3 value)
        {
            // Get current angle
            float angle;
            Vector3 dummy;
            ((Quaternion)property.ValueEntry.WeakSmartValue).ToAngleAxis(out angle, out dummy);

            // Set new axis
            property.ValueEntry.WeakSmartValue = Quaternion.AngleAxis(angle, value);
            property.ValueEntry.ApplyChanges();
        }

        private void SetDrawMode(InspectorProperty property, QuaternionDrawMode mode)
        {
            if (GeneralDrawerConfig.Instance.QuaternionDrawMode != mode)
            {
                GeneralDrawerConfig.Instance.QuaternionDrawMode = mode;
            }
        }
    }
}
#endif