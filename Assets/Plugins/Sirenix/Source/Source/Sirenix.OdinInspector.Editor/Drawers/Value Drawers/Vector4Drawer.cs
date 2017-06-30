#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="Vector4Drawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEditor;
    using UnityEngine;
	using Sirenix.Utilities;

	/// <summary>
	/// Vector4 property drawer.
	/// </summary>
	[OdinDrawer]
    public sealed class Vector4Drawer : OdinValueDrawer<Vector4>, IDefinesGenericMenuItems
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<Vector4> entry, GUIContent label)
        {
            GUILayout.BeginHorizontal();

			if (label != null)
			{
				EditorGUI.BeginChangeCheck();
				var value = SirenixEditorFields.VectorPrefixLabel(label, entry.SmartValue);
				if (EditorGUI.EndChangeCheck())
				{
					entry.SmartValue = value;
				}
			}

			GUIHelper.PushLabelWidth(15f);
			GUIHelper.PushIndentLevel(0);
			entry.Property.Children[0].Draw(GUIHelper.TempContent("X"));
			entry.Property.Children[1].Draw(GUIHelper.TempContent("Y"));
			entry.Property.Children[2].Draw(GUIHelper.TempContent("Z"));
			entry.Property.Children[3].Draw(GUIHelper.TempContent("W"));
			GUIHelper.PopIndentLevel();
			GUIHelper.PopLabelWidth();
			GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Populates the generic menu for the property.
        /// </summary>
        public void PopulateGenericMenu(InspectorProperty property, GenericMenu genericMenu)
        {
            Vector4 value = (Vector4)property.ValueEntry.WeakSmartValue;

            if (genericMenu.GetItemCount() > 0)
            {
                genericMenu.AddSeparator("");
            }
            genericMenu.AddItem(new GUIContent("Normalize"), Mathf.Approximately(value.magnitude, 1f), () => NormalizeEntries(property));
            genericMenu.AddItem(new GUIContent("Zero", "Set the vector to (0, 0, 0, 0)"), value == Vector4.zero, () => SetVector(property, Vector3.zero));
            genericMenu.AddItem(new GUIContent("One", "Set the vector to (1, 1, 1, 1)"), value == Vector4.one, () => SetVector(property, Vector4.one));
            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("Right", "Set the vector to (1, 0, 0, 0)"), (Vector3)value == Vector3.right, () => SetVector(property, Vector3.right));
            genericMenu.AddItem(new GUIContent("Left", "Set the vector to (-1, 0, 0, 0)"), (Vector3)value == Vector3.left, () => SetVector(property, Vector3.left));
            genericMenu.AddItem(new GUIContent("Up", "Set the vector to (0, 1, 0, 0)"), (Vector3)value == Vector3.up, () => SetVector(property, Vector3.up));
            genericMenu.AddItem(new GUIContent("Down", "Set the vector to (0, -1, 0, 0)"), (Vector3)value == Vector3.down, () => SetVector(property, Vector3.down));
            genericMenu.AddItem(new GUIContent("Forward", "Set the vector property to (0, 0, 1, 0)"), (Vector3)value == Vector3.forward, () => SetVector(property, Vector3.forward));
            genericMenu.AddItem(new GUIContent("Back", "Set the vector property to (0, 0, -1, 0)"), (Vector3)value == Vector3.back, () => SetVector(property, Vector3.back));
        }

        private void SetVector(InspectorProperty property, Vector4 value)
        {
            for (int i = 0; i < property.ValueEntry.ValueCount; i++)
            {
                property.ValueEntry.WeakValues[i] = value;
            }
            property.ValueEntry.ApplyChanges();
        }

        private void NormalizeEntries(InspectorProperty property)
        {
            for (int i = 0; i < property.ValueEntry.ValueCount; i++)
            {
                property.ValueEntry.WeakValues[i] = Vector4.Normalize((Vector4)property.ValueEntry.WeakValues[i]);
            }
            property.ValueEntry.ApplyChanges();
        }
    }
}
#endif