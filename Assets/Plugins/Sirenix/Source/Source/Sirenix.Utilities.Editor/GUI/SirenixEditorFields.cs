#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SirenixEditorFields.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using System.Linq;
	using UnityEditor;
	using UnityEngine;

	/// <summary>
	/// Draw mode of quaternion fields.
	/// </summary>
	/// <seealso cref="SirenixEditorFields"/>
	/// <seealso cref="Sirenix.OdinInspector.Editor.GeneralDrawerConfig"/>
	public enum QuaternionDrawMode
	{
		/// <summary>
		/// Draw the quaterion as euler angles.
		/// </summary>
		Eulers = 0,

		/// <summary>
		/// Draw the quaterion in as an angle and an axis.
		/// </summary>
		AngleAxis = 1,

		/// <summary>
		/// Draw the quaternion as raw x, y, z and w values.
		/// </summary>
		Raw = 2,
	}

	/// <summary>
	/// Field drawing functions for various types.
	/// </summary>
	public static class SirenixEditorFields
	{
		/* Reference:
		 * Types:
		 * int (byte, short, long?)
		 * float (double, decimal?)
		 * Delayed, Range, Wrap(?)
		 * string (text box)
		 * Vector2, 3, 4
		 * Object
		 * Quaternion (Euler, AngleAxis, Raw, auto?)
		 * enums ?
		 * ------------
		 * Parameters:
		 * Layout:	Rect / (none)
		 * Label:	GUIContent / string / (none)
		 * Style:	GUIStyle / (none) (?)
		 * Type:	Type, <T>
		 */

		private static readonly float[] XYFloatBuffer = new float[2];
		private static readonly float[] XYZFloatBuffer = new float[3];
		private static readonly float[] XYZWFloatBuffer = new float[4];
		private static readonly GUIContent[] XYLabelBuffer = new GUIContent[] { new GUIContent("X"), new GUIContent("Y") };
		private static readonly GUIContent[] XYZLabelBuffer = new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z") };
		private static readonly GUIContent[] XYZWLabelBuffer = new GUIContent[] { new GUIContent("X"), new GUIContent("Y"), new GUIContent("Z"), new GUIContent("W") };
		private static Vector4 vectorNormalBuffer;
		private static float vectorLengthBuffer;
		private static int localHotControl;
		private static Vector3 quaternionEulerBuffer;
		private static Vector4 quaternionAngleAxisBuffer;
		private static string delayedTextBuffer;
		private static GUIStyle minMaxSliderStyle = null;
		private static GUIStyle sliderBackground = null;
		private static GUIStyle minMaxFloatingLabelStyle = null;
		private enum MinMaxSliderLocalControl { Min = 1, Max = 2, Bar = 3 };

		private static void CheckForReleaseLocalControl(Rect rect, int controlID)
		{
			if (localHotControl != 0 && localHotControl == controlID && (
				(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) ||
				(Event.current.type == EventType.MouseDown && Event.current.button == 1) ||
				(Event.current.type == EventType.MouseDown && !rect.Contains(Event.current.mousePosition))))
			{
				localHotControl = 0;
			}
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(Rect rect, GUIContent label, int value, GUIStyle style)
		{
			return label != null ?
				EditorGUI.IntField(rect, label, value, style ?? EditorStyles.numberField) :
				EditorGUI.IntField(rect, value, style ?? EditorStyles.numberField);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(Rect rect, GUIContent label, int value)
		{
			return SirenixEditorFields.IntField(rect, label, value, null);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(Rect rect, string label, int value)
		{
			return SirenixEditorFields.IntField(rect, label != null ? GUIHelper.TempContent(label) : null, value, null);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(Rect rect, int value)
		{
			return SirenixEditorFields.IntField(rect, null, value, null);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(GUIContent label, int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return SirenixEditorFields.IntField(rect, label, value, style);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.IntField(label, value, null, options);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(string label, int value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.IntField(label != null ? GUIHelper.TempContent(label) : null, value, null, options);
		}

		/// <summary>
		/// Draws an int field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int IntField(int value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.IntField(null, value, null, options);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(Rect rect, GUIContent label, int value, GUIStyle style)
		{
			return label != null ?
				EditorGUI.DelayedIntField(rect, label, value, style ?? EditorStyles.numberField) :
				EditorGUI.DelayedIntField(rect, value, style ?? EditorStyles.numberField);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(Rect rect, GUIContent label, int value)
		{
			return SirenixEditorFields.DelayedIntField(rect, label, value, null);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(Rect rect, string label, int value)
		{
			return SirenixEditorFields.DelayedIntField(rect, label != null ? GUIHelper.TempContent(label) : null, value, null);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(Rect rect, int value)
		{
			return SirenixEditorFields.DelayedIntField(rect, null, value, null);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(GUIContent label, int value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return SirenixEditorFields.DelayedIntField(rect, label, value, style);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(GUIContent label, int value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.DelayedIntField(label, value, null, options);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(string label, int value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.DelayedIntField(label != null ? GUIHelper.TempContent(label) : null, value, null, options);
		}

		/// <summary>
		/// Draws a delayed int field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int DelayedIntField(int value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.DelayedIntField(null, value, null, options);
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(Rect rect, GUIContent label, int value, int min, int max, GUIStyle style)
		{
			return label != null ?
				(int)EditorGUI.Slider(rect, label, value, (min < max ? min : max), (max > min ? max : min)) :
				(int)EditorGUI.Slider(rect, value, (min < max ? min : max), (max > min ? max : min));
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(Rect rect, GUIContent label, int value, int min, int max)
		{
			return RangeIntField(rect, label, value, min, max, null);
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(Rect rect, string label, int value, int min, int max)
		{
			return RangeIntField(rect, label != null ? GUIHelper.TempContent(label) : null, value, min, max, null);
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(Rect rect, int value, int min, int max)
		{
			return RangeIntField(rect, null, value, min, max, null);
		}

		/// <summary>
		/// Drwas a range field for ints.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(GUIContent label, int value, int min, int max, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return SirenixEditorFields.RangeIntField(rect, label, value, min, max, style);
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(GUIContent label, int value, int min, int max, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.RangeIntField(label, value, min, max, null, options);
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(string label, int value, int min, int max, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.RangeIntField(label != null ? GUIHelper.TempContent(label) : null, value, min, max, null, options);
		}

		/// <summary>
		/// Draws a range field for ints.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int RangeIntField(int value, int min, int max, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.RangeIntField(null, value, min, max, null, options);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(Rect rect, GUIContent label, float value, GUIStyle style)
		{
			return label != null ?
				EditorGUI.FloatField(rect, label, value, style ?? EditorStyles.numberField) :
				EditorGUI.FloatField(rect, value, style ?? EditorStyles.numberField);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(Rect rect, GUIContent label, float value)
		{
			return FloatField(rect, label, value, null);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(Rect rect, string label, float value)
		{
			return FloatField(rect, label != null ? GUIHelper.TempContent(label) : null, value, null);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(Rect rect, float value)
		{
			return FloatField(rect, null, value, null);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(GUIContent label, float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return FloatField(rect, label, value, style);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			return FloatField(label, value, null, options);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(string label, float value, params GUILayoutOption[] options)
		{
			return FloatField(label != null ? GUIHelper.TempContent(label) : null, value, null, options);
		}

		/// <summary>
		/// Draws a float field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float FloatField(float value, params GUILayoutOption[] options)
		{
			return FloatField(null, value, null, options);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(Rect rect, GUIContent label, float value, GUIStyle style)
		{
			return label != null ?
				EditorGUI.DelayedFloatField(rect, label, value, style ?? EditorStyles.numberField) :
				EditorGUI.DelayedFloatField(rect, value, style ?? EditorStyles.numberField);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(Rect rect, GUIContent label, float value)
		{
			return DelayedFloatField(rect, label, value, null);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(Rect rect, string label, float value)
		{
			return DelayedFloatField(rect, label != null ? GUIHelper.TempContent(label) : null, value, null);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(Rect rect, float value)
		{
			return DelayedFloatField(rect, null, value, null);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(GUIContent label, float value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return DelayedFloatField(rect, label, value, style);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(GUIContent label, float value, params GUILayoutOption[] options)
		{
			return DelayedFloatField(label, value, null, options);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(string label, float value, params GUILayoutOption[] options)
		{
			return DelayedFloatField(label != null ? GUIHelper.TempContent(label) : null, value, null, options);
		}

		/// <summary>
		/// Draws a delayed float field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float DelayedFloatField(float value, params GUILayoutOption[] options)
		{
			return DelayedFloatField(null, value, null, options);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(Rect rect, GUIContent label, float value, float min, float max, GUIStyle style)
		{
			return label != null ?
				EditorGUI.Slider(rect, label, value, (min < max ? min : max), (max > min ? max : min)) :
				EditorGUI.Slider(rect, value, (min < max ? min : max), (max > min ? max : min));
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(Rect rect, GUIContent label, float value, float min, float max)
		{
			return RangeFloatField(rect, label, value, min, max, null);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(Rect rect, string label, float value, float min, float max)
		{
			return RangeFloatField(rect, label != null ? GUIHelper.TempContent(label) : null, value, min, max, null);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(Rect rect, float value, float min, float max)
		{
			return RangeFloatField(rect, null, value, min, max, null);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(GUIContent label, float value, float min, float max, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return RangeFloatField(rect, label, value, min, max, style);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(GUIContent label, float value, float min, float max, params GUILayoutOption[] options)
		{
			return RangeFloatField(label, value, min, max, null, options);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(string label, float value, float min, float max, params GUILayoutOption[] options)
		{
			return RangeFloatField(label != null ? GUIHelper.TempContent(label) : null, value, min, max, null, options);
		}

		/// <summary>
		/// Draws a range field for floats.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="min">Minimum value.</param>
		/// <param name="max">Maximum value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static float RangeFloatField(float value, float min, float max, params GUILayoutOption[] options)
		{
			return RangeFloatField(null, value, min, max, null, options);
		}

		// @Todo
		// DoubleField
		// DoubleDelayed
		// DoubleRange

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(Rect rect, GUIContent label, string value, GUIStyle style)
		{
			return label != null ?
				EditorGUI.TextField(rect, label, value, style ?? EditorStyles.textField) :
				EditorGUI.TextField(rect, value, style ?? EditorStyles.textField);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(Rect rect, GUIContent label, string value)
		{
			return TextField(rect, label, value, null);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(Rect rect, string label, string value)
		{
			return TextField(rect, label != null ? GUIHelper.TempContent(label) : null, value, null);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(Rect rect, string value)
		{
			return TextField(rect, null, value, null);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(GUIContent label, string value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return TextField(rect, label, value, style);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(GUIContent label, string value, params GUILayoutOption[] options)
		{
			return TextField(label, value, null, options);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(string label, string value, params GUILayoutOption[] options)
		{
			return TextField(label != null ? GUIHelper.TempContent(label) : null, value, null, options);
		}

		/// <summary>
		/// Draws a text field for strings.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string TextField(string value, params GUILayoutOption[] options)
		{
			return TextField(null, value, null, options);
		}

		// @Todo
		// Textbox

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(Rect rect, GUIContent label, string value, GUIStyle style)
		{
			int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

			string text = value;
			if (controlID == localHotControl)
			{
				text = delayedTextBuffer;
			}

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			bool cancelEvent = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Escape;
			bool confirmEvent = Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return;

			EditorGUI.BeginChangeCheck();
			text = EditorGUI.TextField(rect, text);
			if (EditorGUI.EndChangeCheck())
			{
				localHotControl = controlID;
				delayedTextBuffer = text;
			}

			if (controlID == localHotControl && confirmEvent)
			{
				localHotControl = 0;
				GUI.changed = true;
				Event.current.Use();
				return text;
			}
			else if (controlID == localHotControl && cancelEvent)
			{
				localHotControl = 0;
				Event.current.Use();
				return value;
			}
			else
			{
				return value;
			}
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(Rect rect, GUIContent label, string value)
		{
			return DelayedTextField(rect, label, value, null);
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(Rect rect, string label, string value)
		{
			return DelayedTextField(rect, label != null ? GUIHelper.TempContent(label) : null, value, null);
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(Rect rect, string value)
		{
			return DelayedTextField(rect, null, value, null);
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(GUIContent label, string value, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.textField, options);
			return DelayedTextField(rect, label, value, style);
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(GUIContent label, string value, params GUILayoutOption[] options)
		{
			return DelayedTextField(label, value, null, options);
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(string label, string value, params GUILayoutOption[] options)
		{
			return DelayedTextField(label != null ? GUIHelper.TempContent(label) : null, value, null, options);
		}

		/// <summary>
		/// Draws a delayed text field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static string DelayedTextField(string value, params GUILayoutOption[] options)
		{
			return DelayedTextField(null, value, null, options);
		}

		/// <summary>
		/// Draws a prefix label for a vector field, that implements label dragging.
		/// </summary>
		/// <param name="totalRect">The position and total size of the field.</param>
		/// <param name="label">The label content. If <c>null</c> this function does nothing.</param>
		/// <param name="value">The value for the vector field.</param>
		/// <returns>The vector scaled by label dragging.</returns>
		public static Vector4 VectorPrefixLabel(ref Rect totalRect, GUIContent label, Vector4 value)
		{
			if (label == null) { return value; }

			// Contorl ID for slide label.
			int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

			// Draw label and create label rect.
			Rect labelRect = new Rect(totalRect.x, totalRect.y, totalRect.width, totalRect.height);
			totalRect = EditorGUI.PrefixLabel(totalRect, label);
			labelRect.width -= totalRect.width;

			// Working values
			Vector4 normal = value.sqrMagnitude > 0f ? value.normalized : Vector4.one;
			float length = value.magnitude;

			if (GUIUtility.hotControl == controlID)
			{
				normal = vectorNormalBuffer;
				length = vectorLengthBuffer;
			}
			else if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
			{
				vectorNormalBuffer = normal;
				vectorLengthBuffer = length;
			}

			// Sliding rect
			EditorGUI.BeginChangeCheck();
			length = SirenixEditorGUI.SlideRect(labelRect, controlID, length);
			if (EditorGUI.EndChangeCheck())
			{
				vectorLengthBuffer = length;
				value = normal * length;
				value.x = (float)Math.Round(value.x, 2);
				value.y = (float)Math.Round(value.y, 2);
				value.z = (float)Math.Round(value.z, 2);
				value.w = (float)Math.Round(value.w, 2);
			}

			return value;
		}
		
		/// <summary>
		/// Draws a prefix label for a vector field, that implements label dragging.
		/// </summary>
		/// <param name="totalRect">The position and total size of the field.</param>
		/// <param name="label">The label content. If <c>null</c> this function does nothing.</param>
		/// <param name="value">The value for the vector field.</param>
		/// <returns>The vector scaled by label dragging.</returns>
		public static Vector4 VectorPrefixLabel(ref Rect totalRect, string label, Vector4 value)
		{
			return VectorPrefixLabel(ref totalRect, GUIHelper.TempContent(label), value);
		}
		
		/// <summary>
		/// Draws a prefix label for a vector field, that implements label dragging.
		/// </summary>
		/// <param name="label">The label content. If <c>null</c> this function does nothing.</param>
		/// <param name="value">The value for the vector field.</param>
		/// <returns>The vector scaled by label dragging.</returns>
		public static Vector4 VectorPrefixLabel(GUIContent label, Vector4 value)
		{
			if (label == null) { return value; }

			// Contorl ID for slide label.
			int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);

			// Draw label and create label rect.
			EditorGUILayout.PrefixLabel(label);
			Rect labelRect = GUILayoutUtility.GetLastRect();

			// Working values
			Vector4 normal = value.sqrMagnitude > 0f ? value.normalized : Vector4.one;
			float length = value.magnitude;

			if (GUIUtility.hotControl == controlID)
			{
				normal = vectorNormalBuffer;
				length = vectorLengthBuffer;
			}
			else if (Event.current.type == EventType.MouseDown && labelRect.Contains(Event.current.mousePosition))
			{
				vectorNormalBuffer = normal;
				vectorLengthBuffer = length;
			}

			// Sliding rect
			EditorGUI.BeginChangeCheck();
			length = SirenixEditorGUI.SlideRect(labelRect, controlID, length);
			if (EditorGUI.EndChangeCheck())
			{
				vectorLengthBuffer = length;
				value = normal * length;
				value.x = (float)Math.Round(value.x, 2);
				value.y = (float)Math.Round(value.y, 2);
				value.z = (float)Math.Round(value.z, 2);
				value.w = (float)Math.Round(value.w, 2);
			}

			return value;
		}
		
		/// <summary>
		/// Draws a prefix label for a vector field, that implements label dragging.
		/// </summary>
		/// <param name="label">The label content. If <c>null</c> this function does nothing.</param>
		/// <param name="value">The value for the vector field.</param>
		/// <returns>The vector scaled by label dragging.</returns>
		public static Vector4 VectorPrefixLabel(string label, Vector4 value)
		{
			return VectorPrefixLabel(GUIHelper.TempContent(label), value);
		}

		/// <summary>
		/// Draws a Vector2 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector2 Vector2Field(Rect rect, GUIContent label, Vector2 value)
		{
			value = (Vector2)VectorPrefixLabel(ref rect, label, (Vector4)value);

			XYFloatBuffer[0] = value.x;
			XYFloatBuffer[1] = value.y;
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(rect, XYLabelBuffer, XYFloatBuffer);
			if (EditorGUI.EndChangeCheck())
			{
				value.x = XYFloatBuffer[0];
				value.y = XYFloatBuffer[1];
			}

			return value;
		}

		/// <summary>
		/// Draws a Vector2 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector2 Vector2Field(Rect rect, string label, Vector2 value)
		{
			return SirenixEditorFields.Vector2Field(rect, label != null ? GUIHelper.TempContent(label) : null, value);
		}

		/// <summary>
		/// Draws a Vector2 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector2 Vector2Field(Rect rect, Vector2 value)
		{
			return SirenixEditorFields.Vector2Field(rect, (GUIContent)null, value);
		}

		/// <summary>
		/// Draws a Vector2 field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector2 Vector2Field(GUIContent label, Vector2 value, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return SirenixEditorFields.Vector2Field(rect, label, value);
		}

		/// <summary>
		/// Draws a Vector2 field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector2 Vector2Field(string label, Vector2 value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.Vector2Field(label != null ? GUIHelper.TempContent(label) : null, value, options);
		}

		/// <summary>
		/// Draws a Vector2 field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector2 Vector2Field(Vector2 value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.Vector2Field((GUIContent)null, value, options);
		}

		/// <summary>
		/// Draws a Vector3 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector3 Vector3Field(Rect rect, GUIContent label, Vector3 value)
		{
			// Sliding label.
			value = (Vector3)VectorPrefixLabel(ref rect, label, (Vector3)value);

			// Field
			XYZFloatBuffer[0] = value.x;
			XYZFloatBuffer[1] = value.y;
			XYZFloatBuffer[2] = value.z;
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(rect, XYZLabelBuffer, XYZFloatBuffer);
			if (EditorGUI.EndChangeCheck())
			{
				value.x = XYZFloatBuffer[0];
				value.y = XYZFloatBuffer[1];
				value.z = XYZFloatBuffer[2];
			}

			return value;
		}

		/// <summary>
		/// Draws a Vector3 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector3 Vector3Field(Rect rect, string label, Vector3 value)
		{
			return SirenixEditorFields.Vector3Field(rect, label != null ? GUIHelper.TempContent(label) : null, value);
		}

		/// <summary>
		/// Draws a Vector3 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector3 Vector3Field(Rect rect, Vector3 value)
		{
			return SirenixEditorFields.Vector3Field(rect, (GUIContent)null, value);
		}

		/// <summary>
		/// Draws a Vector3 field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector3 Vector3Field(GUIContent label, Vector3 value, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return SirenixEditorFields.Vector3Field(rect, label, value);
		}

		/// <summary>
		/// Draws a Vector3 field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector3 Vector3Field(string label, Vector3 value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.Vector3Field(label != null ? GUIHelper.TempContent(label) : null, value, options);
		}

		/// <summary>
		/// Draws a Vector3 field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector3 Vector3Field(Vector3 value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.Vector3Field((GUIContent)null, value, options);
		}

		/// <summary>
		/// Draws a Vector4 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector4 Vector4Field(Rect rect, GUIContent label, Vector4 value)
		{
			value = VectorPrefixLabel(ref rect, label, value);

			XYZWFloatBuffer[0] = value.x;
			XYZWFloatBuffer[1] = value.y;
			XYZWFloatBuffer[2] = value.z;
			XYZWFloatBuffer[3] = value.w;
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(rect, XYZWLabelBuffer, XYZWFloatBuffer);
			if (EditorGUI.EndChangeCheck())
			{
				value.x = XYZWFloatBuffer[0];
				value.y = XYZWFloatBuffer[1];
				value.z = XYZWFloatBuffer[2];
				value.w = XYZWFloatBuffer[3];
			}

			return value;
		}

		/// <summary>
		/// Draws a Vector4 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector4 Vector4Field(Rect rect, string label, Vector4 value)
		{
			return SirenixEditorFields.Vector4Field(rect, label != null ? GUIHelper.TempContent(label) : null, value);
		}

		/// <summary>
		/// Draws a Vector4 field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector4 Vector4Field(Rect rect, Vector4 value)
		{
			return SirenixEditorFields.Vector4Field(rect, (GUIContent)null, value);
		}

		/// <summary>
		/// Draws a Vector4 field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector4 Vector4Field(GUIContent label, Vector4 value, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return SirenixEditorFields.Vector4Field(rect, label, value);
		}

		/// <summary>
		/// Draws a Vector4 field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector4 Vector4Field(string label, Vector4 value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.Vector4Field(label != null ? GUIHelper.TempContent(label) : null, value, options);
		}

		/// <summary>
		/// Draws a Vector4 field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Vector4 Vector4Field(Vector4 value, params GUILayoutOption[] options)
		{
			return SirenixEditorFields.Vector4Field((GUIContent)null, value, options);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="limits">The min and max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <returns>A Vector2 with X set as min value, and Y to set as max value.</returns>
		public static Vector2 MinMaxSlider(Rect rect, GUIContent label, Vector2 value, Vector2 limits, bool showFields = false)
		{
			// Initialize styles
			if (minMaxSliderStyle == null)
			{
				minMaxSliderStyle = (GUIStyle)"MinMaxHorizontalSliderThumb";
			}
			if (sliderBackground == null)
			{
				sliderBackground = GUI.skin.horizontalSlider;
			}

			// Label
			rect = GUIHelper.IndentRect(rect);
			Rect totalRect = rect;
			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			int controlId = GUIUtility.GetControlID(FocusType.Passive);

			// Slide rects
			int fieldWidth = showFields ? Mathf.RoundToInt(rect.width * 0.3f * 0.5f) : 0;
			Rect fieldRect = rect.MoveHorizontal(fieldWidth).SetWidth(rect.width - fieldWidth * 2 - 11).HorizontalPadding(4);
			Rect controlRect = fieldRect.SetXMax(Mathf.RoundToInt(fieldRect.x + fieldRect.width * MathUtilities.LinearStep(limits.x, limits.y, value.y) + 11))
				.AddXMin(Mathf.RoundToInt(fieldRect.width * MathUtilities.LinearStep(limits.x, limits.y, value.x)));

			// Fields
			if (showFields)
			{
				GUIHelper.PushIndentLevel(0);
				EditorGUI.BeginChangeCheck();
				value.x = SirenixEditorFields.FloatField(rect.AlignLeft(fieldWidth), value.x);
				if (EditorGUI.EndChangeCheck())
				{
					value.x = Mathf.Clamp(value.x, limits.x, limits.y);
					value.y = Mathf.Max(value.x, value.y);
					GUI.changed = true;
				}
				EditorGUI.BeginChangeCheck();
				value.y = SirenixEditorFields.FloatField(rect.AlignRight(fieldWidth), value.y);
				if (EditorGUI.EndChangeCheck())
				{
					value.y = Mathf.Clamp(value.y, limits.x, limits.y);
					value.x = Mathf.Min(value.x, value.y);
					GUI.changed = true;
				}
				GUIHelper.PopIndentLevel();
			}

			// Slider controls
			if (Event.current.IsHovering(fieldRect))
			{
				GUIHelper.RequestRepaint();
			}
			if (Event.current.OnMouseDown(fieldRect.SetWidth(fieldRect.width + 11), 0, true))
			{
				GUIUtility.hotControl = controlId;
				localHotControl = (int)
					(Event.current.control ? MinMaxSliderLocalControl.Bar :
					Event.current.mousePosition.x <= controlRect.xMin ? MinMaxSliderLocalControl.Min :
					Event.current.mousePosition.x >= controlRect.xMax ? MinMaxSliderLocalControl.Max :
					Mathf.Abs(controlRect.xMin - Event.current.mousePosition.x) < Mathf.Abs(controlRect.xMax - Event.current.mousePosition.x) ? MinMaxSliderLocalControl.Min : MinMaxSliderLocalControl.Max);

				// Update value.
				if (localHotControl == (int)MinMaxSliderLocalControl.Min)
				{
					value.x = Mathf.Clamp(Mathf.Lerp(limits.x, limits.y, MathUtilities.LinearStep(fieldRect.xMin, fieldRect.xMax, Event.current.mousePosition.x)), limits.x, limits.y);
					value.x = Mathf.Min(value.x, value.y);
				}
				else if (localHotControl == (int)MinMaxSliderLocalControl.Max)
				{
					value.y = Mathf.Clamp(Mathf.Lerp(limits.x, limits.y, MathUtilities.LinearStep(fieldRect.xMin, fieldRect.xMax, Event.current.mousePosition.x)), limits.x, limits.y);
					value.y = Mathf.Max(value.x, value.y);
				}

				GUI.changed = true;
			}
			else if (GUIUtility.hotControl == controlId)
			{

				if (Event.current.rawType == EventType.MouseUp)
				{
					GUIUtility.hotControl = 0;
					Event.current.Use();
				}
				else if (Event.current.OnMouseMoveDrag(true))
				{
					if (localHotControl == (int)MinMaxSliderLocalControl.Min)
					{
						value.x = Mathf.Clamp(Mathf.Lerp(limits.x, limits.y, MathUtilities.LinearStep(fieldRect.xMin, fieldRect.xMax, Event.current.mousePosition.x)), limits.x, limits.y);
						value.x = Mathf.Min(value.x, value.y);
					}
					else if (localHotControl == (int)MinMaxSliderLocalControl.Max)
					{
						value.y = Mathf.Clamp(Mathf.Lerp(limits.x, limits.y, MathUtilities.LinearStep(fieldRect.xMin, fieldRect.xMax, Event.current.mousePosition.x)), limits.x, limits.y);
						value.y = Mathf.Max(value.x, value.y);
					}
					else
					{
						controlRect.x = Mathf.Clamp(controlRect.x + Event.current.delta.x, fieldRect.x, fieldRect.xMax + 11 - controlRect.width);
						value.x = Mathf.Lerp(limits.x, limits.y, MathUtilities.LinearStep(fieldRect.x, fieldRect.xMax, controlRect.x));
						value.y = Mathf.Lerp(limits.x, limits.y, MathUtilities.LinearStep(fieldRect.x, fieldRect.xMax, controlRect.xMax - 11));
					}

					GUIHelper.RequestRepaint();
					GUI.changed = true;
				}
			}

			if (Event.current.OnRepaint())
			{
				EditorGUIUtility.AddCursorRect(controlRect, Event.current.control || GUIUtility.hotControl == controlId && localHotControl == (int)MinMaxSliderLocalControl.Bar ? MouseCursor.Link : MouseCursor.SlideArrow);

				sliderBackground.Draw(fieldRect.SetWidth(fieldRect.width + 11).MoveVertical(-1), GUIContent.none, 0);
				minMaxSliderStyle.Draw(controlRect.MinWidth(11), GUIContent.none, controlId);

				if (Event.current.IsHovering(totalRect) || GUIUtility.hotControl == controlId)
				{
					Rect floatRect = fieldRect.SetWidth(fieldRect.width + 11);

					GUIContent xLabel = new GUIContent(MathUtilities.DiscardLeastSignificantDecimal(value.x).ToString());
					GUIContent yLabel = new GUIContent(MathUtilities.DiscardLeastSignificantDecimal(value.y).ToString());
					GUIContent minLabel = new GUIContent(limits.x.ToString());
					GUIContent maxLabel = new GUIContent(limits.y.ToString());

					if (minMaxFloatingLabelStyle == null)
					{
						minMaxFloatingLabelStyle = new GUIStyle((GUIStyle)"ProfilerBadge")
						{
							font = EditorStyles.miniButton.font,
							fontStyle = EditorStyles.miniButton.fontStyle,
							fontSize = EditorStyles.miniButton.fontSize,
							alignment = TextAnchor.MiddleCenter,
						};
					}

					var size = minMaxFloatingLabelStyle.CalcSize(xLabel);
					var xRect = floatRect.SetSize(size).SetCenterX(controlRect.xMin).MoveVertical(-size.y).Expand(4, 0);

					size = minMaxFloatingLabelStyle.CalcSize(yLabel);
					var yRect = floatRect.SetSize(size).SetCenterX(controlRect.xMax).MoveVertical(-size.y).Expand(4, 0);

					size = minMaxFloatingLabelStyle.CalcSize(minLabel);
					var minRect = floatRect.SetSize(size).SetCenterX(fieldRect.xMin).MoveVertical(-size.y).Expand(4, 0);

					size = minMaxFloatingLabelStyle.CalcSize(maxLabel);
					var maxRect = floatRect.AlignRight(size.x).SetHeight(size.y).MoveVertical(-size.y).Expand(4, 0);

					// Overlapping
					if (xRect.xMax + 4 > yRect.xMin)
					{
						float d = xRect.xMax + 4 - yRect.xMin;
						xRect.x -= Mathf.RoundToInt(d * 0.5f);
						yRect.x += Mathf.RoundToInt(d * 0.5f);
					}

					if (minRect.xMax + 4 < xRect.xMin)
					{
						minMaxFloatingLabelStyle.Draw(minRect, minLabel, -1);

					}
					if (maxRect.xMin - 4 > yRect.xMax)
					{
						minMaxFloatingLabelStyle.Draw(maxRect, maxLabel, -1);
					}

					minMaxFloatingLabelStyle.Draw(xRect, xLabel, -1);
					minMaxFloatingLabelStyle.Draw(yRect, yLabel, -1);
				}
			}

			return value;
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="limits">The min and max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <returns>A Vector2 with X set as min value, and Y to set as max value.</returns>
		public static Vector2 MinMaxSlider(Rect rect, string label, Vector2 value, Vector2 limits, bool showFields = false)
		{
			return MinMaxSlider(rect, GUIHelper.TempContent(label), value, limits, showFields);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <param name="limits">The min and max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <returns>A Vector2 with X set as min value, and Y to set as max value.</returns>
		public static Vector2 MinMaxSlider(Rect rect, Vector2 value, Vector2 limits, bool showFields)
		{
			return MinMaxSlider(rect, (GUIContent)null, value, limits, showFields);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="limits">The min and max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>A Vector2 with X set as min value, and Y to set as max value.</returns>
		public static Vector2 MinMaxSlider(GUIContent label, Vector2 value, Vector2 limits, bool showFields = false, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(options);
			return MinMaxSlider(rect, label, value, limits, showFields);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="limits">The min and max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>A Vector2 with X set as min value, and Y to set as max value.</returns>
		public static Vector2 MinMaxSlider(string label, Vector2 value, Vector2 limits, bool showFields = false, params GUILayoutOption[] options)
		{
			return MinMaxSlider(GUIHelper.TempContent(label), value, limits, showFields, options);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="limits">The min and max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>A Vector2 with X set as min value, and Y to set as max value.</returns>
		public static Vector2 MinMaxSlider(Vector2 value, Vector2 limits, bool showFields, params GUILayoutOption[] options)
		{
			return MinMaxSlider((GUIContent)null, value, limits, showFields, options);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="minValue">Current min value.</param>
		/// <param name="maxValue">Current max value.</param>
		/// <param name="minLimit">The min limit for the value.</param>
		/// <param name="maxLimit">The max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		public static void MinMaxSlider(Rect rect, GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool showFields = false)
		{
			Vector2 value = new Vector2(minValue, maxValue);
			Vector2 limits = new Vector2(minLimit, maxLimit);
			value = MinMaxSlider(rect, label, value, limits, showFields);
			minValue = value.x;
			maxValue = value.y;
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="minValue">Current min value.</param>
		/// <param name="maxValue">Current max value.</param>
		/// <param name="minLimit">The min limit for the value.</param>
		/// <param name="maxLimit">The max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		public static void MinMaxSlider(Rect rect, string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool showFields = false)
		{
			MinMaxSlider(rect, GUIHelper.TempContent(label), ref minValue, ref maxValue, minLimit, maxLimit, showFields);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="minValue">Current min value.</param>
		/// <param name="maxValue">Current max value.</param>
		/// <param name="minLimit">The min limit for the value.</param>
		/// <param name="maxLimit">The max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		public static void MinMaxSlider(Rect rect, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool showFields)
		{
			MinMaxSlider(rect, (GUIContent)null, ref minValue, ref maxValue, minLimit, maxLimit, showFields);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="minValue">Current min value.</param>
		/// <param name="maxValue">Current max value.</param>
		/// <param name="minLimit">The min limit for the value.</param>
		/// <param name="maxLimit">The max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <param name="options">Layout options.</param>
		public static void MinMaxSlider(GUIContent label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool showFields = false, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(options);
			MinMaxSlider(rect, label, ref minValue, ref maxValue, minLimit, maxLimit, showFields);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="minValue">Current min value.</param>
		/// <param name="maxValue">Current max value.</param>
		/// <param name="minLimit">The min limit for the value.</param>
		/// <param name="maxLimit">The max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <param name="options">Layout options.</param>
		public static void MinMaxSlider(string label, ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool showFields = false, params GUILayoutOption[] options)
		{
			MinMaxSlider(GUIHelper.TempContent(label), ref minValue, ref maxValue, minLimit, maxLimit, showFields, options);
		}

		/// <summary>
		/// Draws a slider for setting two values between a min and a max limit.
		/// </summary>
		/// <param name="minValue">Current min value.</param>
		/// <param name="maxValue">Current max value.</param>
		/// <param name="minLimit">The min limit for the value.</param>
		/// <param name="maxLimit">The max limit for the value.</param>
		/// <param name="showFields">Show fields for min and max value.</param>
		/// <param name="options">Layout options.</param>
		public static void MinMaxSlider(ref float minValue, ref float maxValue, float minLimit, float maxLimit, bool showFields, params GUILayoutOption[] options)
		{
			MinMaxSlider((GUIContent)null, ref minValue, ref maxValue, minLimit, maxLimit, showFields, options);
		}

		/// <summary>
		/// Draws a rotation field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="mode">Draw mode for rotation field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion RotationField(Rect rect, GUIContent label, Quaternion value, QuaternionDrawMode mode)
		{
			switch (mode)
			{
				case QuaternionDrawMode.Eulers:
					return EulerField(rect, label, value);

				case QuaternionDrawMode.AngleAxis:
					return AngleAxisField(rect, label, value);

				case QuaternionDrawMode.Raw:
					return QuaternionField(rect, label, value);

				default:
					throw new NotImplementedException("Unknown draw mode: " + mode.ToString());
			}
		}

		/// <summary>
		/// Draws a rotation field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="mode">Draw mode for rotation field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion RotationField(Rect rect, string label, Quaternion value, QuaternionDrawMode mode)
		{
			return RotationField(rect, label != null ? GUIHelper.TempContent(label) : null, value, mode);
		}

		/// <summary>
		/// Draws a rotation field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <param name="mode">Draw mode for rotation field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion RotationField(Rect rect, Quaternion value, QuaternionDrawMode mode)
		{
			return RotationField(rect, (GUIContent)null, value, mode);
		}

		/// <summary>
		/// Draws a rotation field for a quaternion.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="mode">Draw mode for rotation field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion RotationField(GUIContent label, Quaternion value, QuaternionDrawMode mode, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return RotationField(rect, label, value, mode);
		}

		/// <summary>
		/// Draws a rotation field for a quaternion.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="mode">Draw mode for rotation field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion RotationField(string label, Quaternion value, QuaternionDrawMode mode, params GUILayoutOption[] options)
		{
			return RotationField(label != null ? GUIHelper.TempContent(label) : null, value, mode, options);
		}

		/// <summary>
		/// Draws a rotation field for a quaternion.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="mode">Draw mode for rotation field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion RotationField(Quaternion value, QuaternionDrawMode mode, params GUILayoutOption[] options)
		{
			return RotationField((GUIContent)null, value, mode, options);
		}

		/// <summary>
		/// Draws an euler field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion EulerField(Rect rect, GUIContent label, Quaternion value)
		{
			int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
			CheckForReleaseLocalControl(rect, controlID);

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			Vector3 euler = value.eulerAngles;
			euler.Set((float)Math.Round(euler.x, 2), (float)Math.Round(euler.y, 2), (float)Math.Round(euler.z, 2));
			if (localHotControl == controlID)
			{
				euler = quaternionEulerBuffer;
			}

			XYZFloatBuffer[0] = euler.x;
			XYZFloatBuffer[1] = euler.y;
			XYZFloatBuffer[2] = euler.z;

			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(rect, XYZLabelBuffer, XYZFloatBuffer);
			if (EditorGUI.EndChangeCheck())
			{
				localHotControl = controlID;
				quaternionEulerBuffer = new Vector3(XYZFloatBuffer[0], XYZFloatBuffer[1], XYZFloatBuffer[2]);
				value = Quaternion.Euler(quaternionEulerBuffer);
				GUI.changed = true;
			}

			return value;
		}

		/// <summary>
		/// Draws an euler field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion EulerField(Rect rect, string label, Quaternion value)
		{
			return EulerField(rect, label != null ? GUIHelper.TempContent(label) : null, value);
		}

		/// <summary>
		/// Draws an euler field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion EulerField(Rect rect, Quaternion value)
		{
			return EulerField(rect, (GUIContent)null, value);
		}

		/// <summary>
		/// Draws an euler field for a quaternion.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion EulerField(GUIContent label, Quaternion value, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return EulerField(rect, label, value);
		}

		/// <summary>
		/// Draws an euler field for a quaternion.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion EulerField(string label, Quaternion value, params GUILayoutOption[] options)
		{
			return EulerField(label != null ? GUIHelper.TempContent(label) : null, value, options);
		}

		/// <summary>
		/// Draws an euler field for a quaternion.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion EulerField(Quaternion value, params GUILayoutOption[] options)
		{
			return EulerField((GUIContent)null, value, options);
		}

		/// <summary>
		/// Draws an angle axis field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion AngleAxisField(Rect rect, GUIContent label, Quaternion value)
		{
			int controlID = EditorGUIUtility.GetControlID(FocusType.Passive);
			CheckForReleaseLocalControl(rect, controlID);

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			float angle;
			Vector3 axis;

			value.ToAngleAxis(out angle, out axis);
			angle = (float)Math.Round(angle, 2);
			axis.Set((float)Math.Round(axis.x, 2), (float)Math.Round(axis.y, 2), (float)Math.Round(axis.z, 2));

			if (localHotControl == controlID)
			{
				axis = (Vector3)quaternionAngleAxisBuffer;
				angle = quaternionAngleAxisBuffer.w;
			}

			XYZFloatBuffer[0] = axis.x;
			XYZFloatBuffer[1] = axis.y;
			XYZFloatBuffer[2] = axis.z;

			// Rects
			Rect axisRect = rect.SetWidth(rect.width * 0.65f);
			Rect angleFieldRect = rect.AlignRight(rect.width - axisRect.width).HorizontalPadding(5f, 0f);
			Rect angleLabelRect = angleFieldRect.SetWidth(40f);
			Rect angleRect = angleFieldRect.AlignRight(angleFieldRect.width - angleLabelRect.width);

			EditorGUI.BeginChangeCheck();

			GUIHelper.PushIndentLevel(0);
			EditorGUI.MultiFloatField(axisRect, XYZLabelBuffer, XYZFloatBuffer);
			EditorGUI.LabelField(angleLabelRect, GUIHelper.TempContent("Angle", "Rotation around axis"));
			angle = EditorGUI.FloatField(angleRect, angle);
			angle = SirenixEditorGUI.SlideRect(angleLabelRect, controlID, angle);
			GUIHelper.PopIndentLevel();

			if (EditorGUI.EndChangeCheck())
			{
				localHotControl = controlID;
				quaternionAngleAxisBuffer.Set(XYZFloatBuffer[0], XYZFloatBuffer[1], XYZFloatBuffer[2], angle);
				value = Quaternion.AngleAxis(quaternionAngleAxisBuffer.w, ((Vector3)quaternionAngleAxisBuffer).normalized);
				GUI.changed = true;
			}

			return value;
		}

		/// <summary>
		/// Draws an angle axis field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion AngleAxisField(Rect rect, string label, Quaternion value)
		{
			return AngleAxisField(rect, label != null ? GUIHelper.TempContent(label) : null, value);
		}

		/// <summary>
		/// Draws an angle axis field for a quaternion.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion AngleAxisField(Rect rect, Quaternion value)
		{
			return AngleAxisField(rect, (GUIContent)null, value);
		}

		/// <summary>
		/// Draws an angle axis field for a quaternion.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion AngleAxisField(GUIContent label, Quaternion value, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return AngleAxisField(rect, label, value);
		}

		/// <summary>
		/// Draws an angle axis field for a quaternion.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion AngleAxisField(string label, Quaternion value, params GUILayoutOption[] options)
		{
			return AngleAxisField(label != null ? GUIHelper.TempContent(label) : null, value, options);
		}

		/// <summary>
		/// Draws an angle axis field for a quaternion.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion AngleAxisField(Quaternion value, params GUILayoutOption[] options)
		{
			return AngleAxisField((GUIContent)null, value, options);
		}

		/// <summary>
		/// Draws a quaternion field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion QuaternionField(Rect rect, GUIContent label, Quaternion value)
		{
			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			XYZWFloatBuffer[0] = value.x;
			XYZWFloatBuffer[1] = value.y;
			XYZWFloatBuffer[2] = value.z;
			XYZWFloatBuffer[3] = value.w;
			rect.height = EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginChangeCheck();
			EditorGUI.MultiFloatField(rect, XYZWLabelBuffer, XYZWFloatBuffer);
			if (EditorGUI.EndChangeCheck())
			{
				value.x = XYZWFloatBuffer[0];
				value.y = XYZWFloatBuffer[1];
				value.z = XYZWFloatBuffer[2];
				value.w = XYZWFloatBuffer[3];
				GUI.changed = true;
			}

			return value;
		}

		/// <summary>
		/// Draws a quaternion field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion QuaternionField(Rect rect, string label, Quaternion value)
		{
			return QuaternionField(rect, label != null ? GUIHelper.TempContent(label) : null, value);
		}

		/// <summary>
		/// Draws a quaternion field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="value">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion QuaternionField(Rect rect, Quaternion value)
		{
			return QuaternionField(rect, (GUIContent)null, value);
		}

		/// <summary>
		/// Draws a quaternion field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion QuaternionField(GUIContent label, Quaternion value, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.numberField, options);
			return QuaternionField(rect, label, value);
		}

		/// <summary>
		/// Draws a quaternion field.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion QuaternionField(string label, Quaternion value, params GUILayoutOption[] options)
		{
			return QuaternionField(label != null ? GUIHelper.TempContent(label) : null, value, options);
		}

		/// <summary>
		/// Draws a quaternion field.
		/// </summary>
		/// <param name="value">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Quaternion QuaternionField(Quaternion value, params GUILayoutOption[] options)
		{
			return QuaternionField((GUIContent)null, value, options);
		}

		// @Todo currently unable to set unity objects to null.
		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(Rect rect, object key, GUIContent label, object value, Type type, GUIStyle style, bool allowSceneObjects = true)
		{
			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			GUIContent title;

			if (EditorGUI.showMixedValue)
			{
				title = new GUIContent("— Conflict (" + type.GetNiceName() + ")", EditorIcons.StarPointer.Inactive);
			}
			else if (value == null)
			{
				title = new GUIContent("Null (" + type.GetNiceName() + ")", EditorIcons.StarPointer.Inactive);
			}
			else if (value is UnityEngine.Object)
			{
				var ubject = value as UnityEngine.Object;

				if (ubject.SafeIsUnityNull())
				{
					title = new GUIContent("None (" + type.GetNiceName() + ")", EditorIcons.StarPointer.Inactive);
				}
				else
				{
					title = new GUIContent(ubject.name + " (" + type.GetNiceName() + ")", EditorIcons.StarPointer.Inactive);
				}
			}
			else
			{
				string baseType = value.GetType() == type ? "" : (" (" + type.GetNiceName() + ")");
				title = new GUIContent(value.GetType().GetNiceName() + baseType, EditorIcons.StarPointer.Inactive);
			}

			bool isUnityObject = type.InheritsFrom<UnityEngine.Object>();

			if (isUnityObject)
			{
				// Make space for open inspector button.
				rect.width -= 18f;

				// Draw open inspector button.
				SirenixEditorGUI.DrawOpenInspector(new Rect(rect.x + rect.width, rect.y, 18f, rect.height), (UnityEngine.Object)value);
			}

			GUI.Label(rect, title, EditorStyles.objectField);

			var objectPicker = ObjectPicker.GetObjectPicker(key, type);

			if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
			{
				Event.current.Use();
				objectPicker.ShowObjectPicker(allowSceneObjects, rect);
			}

			if (objectPicker.IsReadyToClaim && Event.current.type == EventType.Repaint)
			{
				GUI.changed = true;
				return objectPicker.ClaimObject();
			}

			return value;
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(Rect rect, object key, GUIContent label, object value, Type type, bool allowSceneObjects = true)
		{
			return ObjectField(rect, key, label, value, type, null, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(Rect rect, object key, string label, object value, Type type, bool allowSceneObjects = true)
		{
			return ObjectField(rect, key, label != null ? GUIHelper.TempContent(label) : null, value, type, null, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(Rect rect, object key, object value, Type type, bool allowSceneObjects = true)
		{
			return ObjectField(rect, key, null, value, type, null, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(object key, GUIContent label, object value, Type type, GUIStyle style, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return ObjectField(rect, key, label, value, type, style, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(object key, GUIContent label, object value, Type type, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			return ObjectField(key, label, value, type, null, allowSceneObjects, options);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(object key, string label, object value, Type type, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			return ObjectField(key, label != null ? GUIHelper.TempContent(label) : null, value, type, null, allowSceneObjects, options);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value">Current value.</param>
		/// <param name="type">Base type for field.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static object ObjectField(object key, object value, Type type, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			return ObjectField(key, null, value, type, null, allowSceneObjects, options);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(Rect rect, object key, GUIContent label, T value, GUIStyle style, bool allowSceneObjects = true)
		{
			return (T)ObjectField(rect, key, label, (object)value, typeof(T), style, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(Rect rect, object key, GUIContent label, T value, bool allowSceneObjects = true)
		{
			return ObjectField<T>(rect, key, label, value, null, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(Rect rect, object key, string label, T value, bool allowSceneObjects = true)
		{
			return ObjectField<T>(rect, key, label != null ? GUIHelper.TempContent(label) : null, value, null, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="key"></param>
		/// <param name="value">Current value.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(Rect rect, object key, T value, bool allowSceneObjects = true)
		{
			return ObjectField<T>(rect, key, null, value, null, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(object key, GUIContent label, T value, GUIStyle style, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return ObjectField<T>(rect, key, label, value, style, allowSceneObjects);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(object key, GUIContent label, T value, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			return ObjectField<T>(key, label, value, null, allowSceneObjects, options);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="key"></param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="value">Current value.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(object key, string label, T value, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			return ObjectField<T>(key, label != null ? GUIHelper.TempContent(label) : null, value, null, allowSceneObjects, options);
		}

		/// <summary>
		/// Draws an object field.
		/// </summary>
		/// <typeparam name="T">Base type for field.</typeparam>
		/// <param name="key"></param>
		/// <param name="value">Current value.</param>
		/// <param name="allowSceneObjects">If <c>true</c> allows for scene objects to be assigned to the field.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T ObjectField<T>(object key, T value, bool allowSceneObjects = true, params GUILayoutOption[] options)
		{
			return ObjectField<T>(key, null, value, null, allowSceneObjects, options);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(Rect rect, GUIContent label, int selected, string[] itemNames, GUIStyle style)
		{
			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			return EditorGUI.Popup(rect, selected, itemNames, style ?? EditorStyles.popup);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(Rect rect, GUIContent label, int selected, string[] itemNames)
		{
			return Dropdown(rect, label, selected, itemNames, null);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(Rect rect, string label, int selected, string[] itemNames)
		{
			return Dropdown(rect, label != null ? GUIHelper.TempContent(label) : null, selected, itemNames, null);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(Rect rect, int selected, string[] itemNames)
		{
			return Dropdown(rect, null, selected, itemNames, null);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(GUIContent label, int selected, string[] itemNames, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return Dropdown(rect, label, selected, itemNames, style);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(GUIContent label, int selected, string[] itemNames, params GUILayoutOption[] options)
		{
			return Dropdown(label, selected, itemNames, null, options);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(string label, int selected, string[] itemNames, params GUILayoutOption[] options)
		{
			return Dropdown(label != null ? GUIHelper.TempContent(label) : null, selected, itemNames, null, options);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <param name="selected">Current value.</param>
		/// <param name="itemNames">Names of selectable items.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static int Dropdown(int selected, string[] itemNames, params GUILayoutOption[] options)
		{
			return Dropdown(null, selected, itemNames, null, options);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items">Selectable items.</param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(Rect rect, GUIContent label, T selected, T[] items, string[] itemNames, GUIStyle style)
		{
			int index = 0;
			for (int i = 0; i < items.Length; i++)
			{
				if (selected.Equals(items[i]))
				{
					index = i;
					break;
				}
			}

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, label);
			}

			index = EditorGUI.Popup(rect, index, itemNames, style ?? EditorStyles.popup);
			return items[index];
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(Rect rect, GUIContent label, T selected, T[] items, string[] itemNames)
		{
			return Dropdown<T>(rect, label, selected, items, itemNames, null);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(Rect rect, string label, T selected, T[] items, string[] itemNames)
		{
			return Dropdown<T>(rect, label != null ? GUIHelper.TempContent(label) : null, selected, items, itemNames, null);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(Rect rect, T selected, T[] items, string[] itemNames)
		{
			return Dropdown<T>(rect, null, selected, items, itemNames, null);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(GUIContent label, T selected, T[] items, string[] itemNames, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return Dropdown<T>(rect, label, selected, items, itemNames, style);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(GUIContent label, T selected, T[] items, string[] itemNames, params GUILayoutOption[] options)
		{
			return Dropdown<T>(label, selected, items, itemNames, null, options);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(string label, T selected, T[] items, string[] itemNames, params GUILayoutOption[] options)
		{
			return Dropdown<T>(label != null ? GUIHelper.TempContent(label) : null, selected, items, itemNames, null, options);
		}

		/// <summary>
		/// Draws a generic dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="selected">Current value.</param>
		/// <param name="items"></param>
		/// <param name="itemNames">Names of selectable items. If <c>null</c> ToString() will be used instead.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static T Dropdown<T>(T selected, T[] items, string[] itemNames, params GUILayoutOption[] options)
		{
			return Dropdown<T>(null, selected, items, itemNames, null, options);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(Rect rect, GUIContent label, Enum selected, GUIStyle style)
		{
			return label != null ?
				EditorGUI.EnumPopup(rect, label, selected, style ?? EditorStyles.popup) :
				EditorGUI.EnumPopup(rect, selected, style ?? EditorStyles.popup);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(Rect rect, GUIContent label, Enum selected)
		{
			return EnumDropdown(rect, label, selected, null);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(Rect rect, string label, Enum selected)
		{
			return EnumDropdown(rect, label != null ? GUIHelper.TempContent(label) : null, selected, null);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="selected">Current value.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(Rect rect, Enum selected)
		{
			return EnumDropdown(rect, null, selected, null);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return EnumDropdown(rect, label, selected, style);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(GUIContent label, Enum selected, params GUILayoutOption[] options)
		{
			return EnumDropdown(label, selected, null, options);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(string label, Enum selected, params GUILayoutOption[] options)
		{
			return EnumDropdown(label != null ? GUIHelper.TempContent(label) : null, selected, null, options);
		}

		/// <summary>
		/// Draws an enum dropdown.
		/// </summary>
		/// <param name="selected">Current value.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumDropdown(Enum selected, params GUILayoutOption[] options)
		{
			return EnumDropdown(null, selected, null, options);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="items">Avaible items in the dropdown.</param>
		/// <param name="multiSelection">If <c>true</c> then the user can select multiple items. Otherwise the user can only select one item.</param>
		/// <returns><c>true</c> when the user has changed the selection. Otherwise <c>false</c>.</returns>
		public static bool Dropdown<T>(Rect rect, GUIContent label, IList<int> selected, IList<T> items, bool multiSelection)
		{
			var controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);

			if (label != null)
			{
				rect = EditorGUI.PrefixLabel(rect, controlID, label);
			}

			string display = null;

			if (EditorGUI.showMixedValue)
			{
				display = "—";
			}
			else
			{
				for (int i = 0; i < selected.Count; i++)
				{
					string name = items[selected[i]].ToString();
					if (display == null)
					{
						display = name;
					}
					else
					{
						display = name + ", " + display;
					}
				}
			}
			display = display ?? "None";

			if (GUI.Button(rect, display, EditorStyles.popup))
			{
				GenericMenu menu = new GenericMenu();
				for (int i = 0; i < items.Count; i++)
				{
					int localI = i;
					bool isSelected = selected.Contains(i);
					string numSelected = "";
					if (isSelected)
					{
						int selectedCount = selected.Count(x => x == i);
						if (selectedCount > 1)
						{
							numSelected = " (" + selectedCount + ")";
						}
					}
					menu.AddItem(new GUIContent(items[i] + numSelected), isSelected, () =>
					{
						PopupSelector.CurrentSelectingPopupControlID = controlID;
						PopupSelector.SelectAction = () =>
						{
							if (multiSelection)
							{
								if (isSelected)
								{
									for (int j = selected.Count - 1; j >= 0; j--)
									{
										if (selected[j] == localI)
										{
											selected.RemoveAt(j);
										}
									}
								}
								else
								{
									selected.Add(localI);
								}
							}
							else
							{
								selected.Clear();
								selected.Add(localI);
							}
						};
					});
				}
				menu.DropDown(rect);
			}

			if (PopupSelector.CurrentSelectingPopupControlID == controlID && PopupSelector.SelectAction != null)
			{
				PopupSelector.SelectAction();
				PopupSelector.CurrentSelectingPopupControlID = -1;
				PopupSelector.SelectAction = null;
				return true;
			}

			return false;
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="items">Avaible items in the dropdown.</param>
		/// <param name="multiSelection">If <c>true</c> then the user can select multiple items. Otherwise the user can only select one item.</param>
		/// <returns><c>true</c> when the user has changed the selection. Otherwise <c>false</c>.</returns>
		public static bool Dropdown<T>(Rect rect, string label, IList<int> selected, IList<T> items, bool multiSelection)
		{
			return Dropdown<T>(rect, label != null ? GUIHelper.TempContent(label) : null, selected, items, multiSelection);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="items">Avaible items in the dropdown.</param>
		/// <param name="multiSelection">If <c>true</c> then the user can select multiple items. Otherwise the user can only select one item.</param>
		/// <returns><c>true</c> when the user has changed the selection. Otherwise <c>false</c>.</returns>
		public static bool Dropdown<T>(Rect rect, IList<int> selected, IList<T> items, bool multiSelection)
		{
			return Dropdown<T>(rect, (GUIContent)null, selected, items, multiSelection);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="items">Avaible items in the dropdown.</param>
		/// <param name="multiSelection">If <c>true</c> then the user can select multiple items. Otherwise the user can only select one item.</param>
		/// <param name="options">Layout options.</param>
		/// <returns><c>true</c> when the user has changed the selection. Otherwise <c>false</c>.</returns>
		public static bool Dropdown<T>(GUIContent label, IList<int> selected, IList<T> items, bool multiSelection, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, EditorStyles.popup, options);
			return Dropdown<T>(rect, label, selected, items, multiSelection);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="items">Avaible items in the dropdown.</param>
		/// <param name="multiSelection">If <c>true</c> then the user can select multiple items. Otherwise the user can only select one item.</param>
		/// <param name="options">Layout options.</param>
		/// <returns><c>true</c> when the user has changed the selection. Otherwise <c>false</c>.</returns>
		public static bool Dropdown<T>(string label, IList<int> selected, IList<T> items, bool multiSelection, params GUILayoutOption[] options)
		{
			return Dropdown<T>(label != null ? GUIHelper.TempContent(label) : null, selected, items, multiSelection, options);
		}

		/// <summary>
		/// Draws a dropdown.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="selected">Current selection.</param>
		/// <param name="items">Avaible items in the dropdown.</param>
		/// <param name="multiSelection">If <c>true</c> then the user can select multiple items. Otherwise the user can only select one item.</param>
		/// <param name="options">Layout options.</param>
		/// <returns><c>true</c> when the user has changed the selection. Otherwise <c>false</c>.</returns>
		public static bool Dropdown<T>(IList<int> selected, IList<T> items, bool multiSelection, params GUILayoutOption[] options)
		{
			return Dropdown<T>((GUIContent)null, selected, items, multiSelection, options);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(Rect rect, GUIContent label, Enum selected, GUIStyle style)
		{
			var type = selected.GetType();
			var controlID = GUIUtility.GetControlID(FocusType.Keyboard, rect);

			if (style == null)
			{
				style = EditorStyles.popup;
			}

			selected = GetCurrentMaskValue(controlID, type, selected);

			Rect buttonPosition = label != null ?
				EditorGUI.PrefixLabel(rect, controlID, label, EditorStyles.label) :
				rect;

			string display = selected.ToString();

			if (EditorGUI.showMixedValue)
			{
				display = "—";
			}
			if (string.IsNullOrEmpty(display) || display == "0")
			{
				display = "None";
			}
			else if (display.Contains(","))
			{
				var size = style.CalcSize(new GUIContent(display));

				if (size.x > buttonPosition.width)
				{
					display = "Mixed (" + (display.Count(n => n == ',') + 1) + ")...";
				}
			}

			if (GUI.Button(buttonPosition, display, style))
			{
				GenericMenu menu = new GenericMenu();

				MaskMenu.CurrentMaskControlID = controlID;
				MaskMenu.MaskChanged = false;

				ulong selectedValue = Convert.ToUInt64(selected, CultureInfo.InvariantCulture);
				var names = Enum.GetNames(type).ToList();
				var values = Enum.GetValues(type).FilterCast<object>().Select(n => Convert.ToUInt64(n, CultureInfo.InvariantCulture)).ToList();
				var noneIndex = values.IndexOf(0);
				var allIndex = values.FindIndex(n => n != 0 && values.All(m => (m & n) == n));
				ulong allValue = 0ul;
				for (int i = 0; i < values.Count; i++)
				{
					allValue |= values[i];
				}

				if (values.Count >= 16)
				{
					if (allIndex == -1)
					{
						menu.AddItem(new GUIContent("All"), selectedValue == allValue, EnumMaskSetValueDelegate, allValue);
						menu.AddItem(new GUIContent("None"), selectedValue == 0, EnumMaskSetValueDelegate, 0);
					}

					if (allIndex == -1 || noneIndex == -1)
					{
						menu.AddSeparator("");
					}
				}

				for (int i = 0; i < names.Count; i++)
				{
					ulong value = values[i];
					bool hasFlag;

					if (value == 0)
					{
						hasFlag = selectedValue == 0;
					}
					else
					{
						hasFlag = (value & selectedValue) == value;
					}

					menu.AddItem(new GUIContent(names[i]), hasFlag, EnumMaskSetValueDelegate, value);
				}

				if (values.Count < 16)
				{
					if (allIndex == -1 || noneIndex == -1)
					{
						menu.AddSeparator("");
					}

					if (allIndex == -1)
					{
						menu.AddItem(new GUIContent("All"), selectedValue == allValue, EnumMaskSetValueDelegate, allValue);
						menu.AddItem(new GUIContent("None"), selectedValue == 0, EnumMaskSetValueDelegate, (ulong)0);
					}
				}

				menu.DropDown(buttonPosition);
			}

			return selected;
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(Rect rect, GUIContent label, Enum selected)
		{
			return EnumMaskDropdown(rect, label, selected, null);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(Rect rect, string label, Enum selected)
		{
			return EnumMaskDropdown(rect, label != null ? GUIHelper.TempContent(label) : null, selected, null);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="rect">Position and size of field.</param>
		/// <param name="selected">Current selection.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(Rect rect, Enum selected)
		{
			return EnumMaskDropdown(rect, null, selected, null);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="style">GUIStyle for drawing the field. Set to <c>null</c> for default.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
		{
			Rect rect = EditorGUILayout.GetControlRect(label != null, EditorGUIUtility.singleLineHeight, style ?? EditorStyles.numberField, options);
			return EnumMaskDropdown(rect, label, selected, style);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(GUIContent label, Enum selected, params GUILayoutOption[] options)
		{
			return EnumMaskDropdown(label, selected, null, options);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="label">Label of field. Set to <c>null</c> for no label.</param>
		/// <param name="selected">Current selection.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(string label, Enum selected, params GUILayoutOption[] options)
		{
			return EnumMaskDropdown(label != null ? GUIHelper.TempContent(label) : null, selected, null, options);
		}

		/// <summary>
		/// Draws a dropdown field for enum masks.
		/// </summary>
		/// <param name="selected">Current selection.</param>
		/// <param name="options">Layout options.</param>
		/// <returns>Value assigned to the field.</returns>
		public static Enum EnumMaskDropdown(Enum selected, params GUILayoutOption[] options)
		{
			return EnumMaskDropdown(null, selected, null, options);
		}

		private static class PopupSelector
		{
			public static int CurrentSelectingPopupControlID;
			public static Action SelectAction;
		}

		private static void EnumMaskSetValueDelegate(object value)
		{
			MaskMenu.MaskChanged = true;
			MaskMenu.ChangedMaskValue = (ulong)value;

			EditorWindow.focusedWindow.SendEvent(EditorGUIUtility.CommandEvent(MaskMenu.MASK_MENU_CHANGED_EVENT_NAME));
		}

		private static Enum GetCurrentMaskValue(int controlId, Type enumType, Enum selected)
		{
			var current = Event.current;

			if (current.type == EventType.ExecuteCommand && current.commandName == MaskMenu.MASK_MENU_CHANGED_EVENT_NAME && controlId == MaskMenu.CurrentMaskControlID && MaskMenu.MaskChanged)
			{
				var value = Convert.ToUInt64(selected, CultureInfo.InvariantCulture);

				if (MaskMenu.ChangedMaskValue == 0)
				{
					value = 0;
				}
				else if ((MaskMenu.ChangedMaskValue & value) == MaskMenu.ChangedMaskValue)
				{
					// Remove flag
					value = value & ~MaskMenu.ChangedMaskValue;
				}
				else
				{
					// Add flag
					value |= MaskMenu.ChangedMaskValue;
				}

				selected = (Enum)Enum.ToObject(enumType, value);
				GUI.changed = true;
				current.Use();
			}

			return selected;
		}

		private static class MaskMenu
		{
			public const string MASK_MENU_CHANGED_EVENT_NAME = "SirenixMaskMenuChanged";

			public static ulong ChangedMaskValue { get; set; }

			public static int CurrentMaskControlID { get; set; }

			public static bool MaskChanged { get; set; }
		}
	}
}
#endif