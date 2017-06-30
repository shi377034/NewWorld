#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SirenixEditorGUILayout.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using System.Globalization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// Collection of various editor GUI functions.
    /// </summary>
    [InitializeOnLoad]
    public static class SirenixEditorGUI
    {
        private const float DEFAULT_FADE_GROUP_DURATION = 0.2f;

        private static readonly object fadeGroupKey = new object();

        private static readonly object shakeableGroupKey = new object();

        private static readonly object menuListKey = new object();

        private static readonly object animatedTabGroupKey = new object();

        private static readonly GUIScopeStack<Rect> verticalListBorderRects = new GUIScopeStack<Rect>();

        private static readonly List<int> currentListItemIndecies = new List<int>();

        private static readonly float[] Vector2FloatBuffer = new float[2];

        private static readonly GUIContent[] XYLabelBuffer = new GUIContent[]
        {
            new GUIContent("X"),
            new GUIContent("Y")
        };

        private static readonly float[] Vector3FloatBuffer = new float[3];

        private static readonly GUIContent[] XYZLabelBuffer = new GUIContent[]
        {
            new GUIContent("X"),
            new GUIContent("Y"),
            new GUIContent("Z")
        };

        private static readonly float[] Vector4FloatBuffer = new float[4];

        private static readonly GUIContent[] XYZWLabelBuffer = new GUIContent[]
        {
            new GUIContent("X"),
            new GUIContent("Y"),
            new GUIContent("Z"),
            new GUIContent("W")
        };

        private static int animatingFadeGroupIndex = -1;

        private static int currentFadeGroupIndex = 0;

        private static int currentScope = 0;

        private static int currentDrawingToolbarHeight;

        private static float slideRectSensitivity = 0f;

        /// <summary>
        /// Draws an GUI field for objects.
        /// </summary>
        /// <param name="rect">The rect to draw the field in.</param>
        /// <param name="label">The label of the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="objectType">The object type for the field.</param>
        /// <param name="allowSceneObjects">If set to <c>true</c> then allow scene objects to be assigned to the field.</param>
        /// <param name="isReadOnly">If set to <c>true</c> the field is readonly.</param>
        /// <returns>The object assigned to the field.</returns>
        public static UnityEngine.Object ObjectField(Rect rect, GUIContent label, UnityEngine.Object value, Type objectType, bool allowSceneObjects, bool isReadOnly = false)
        {
            rect.width -= 20;
            bool removeFocus = false;
            if (isReadOnly)
            {
                var objectFieldRect = new Rect(rect.x + 20, rect.y, rect.width - 20, 16);

                if (Event.current.type == EventType.MouseDown)
                {
                    if (new Rect(rect.xMax - 20, rect.y, 20, rect.height).Contains(Event.current.mousePosition))
                    {
                        Event.current.Use();
                    }
                    else if (objectFieldRect.Contains(Event.current.mousePosition))
                    {
                        removeFocus = true;
                    }
                }
            }

            // Sometimes a Unity value is *so null* that Unity can't even handle it
            // In these cases, we just pass in a straight null instead, to make Unity
            // feel better about itself.
            bool isNull = value == null;// || value.SafeIsUnityNull();

            //rect.xMin += 4;
            value = label == null ?
                EditorGUI.ObjectField(rect, isNull ? null : value, objectType, allowSceneObjects) :
                EditorGUI.ObjectField(rect, label, isNull ? null : value, objectType, allowSceneObjects);

            rect = new Rect(rect.x + rect.width, rect.y - 1, 19, 19);

            SirenixEditorGUI.DrawOpenInspector(rect, value);

            if (removeFocus)
            {
                GUI.FocusControl(null);
            }

            return value;
        }

        /// <summary>
        /// Draws an GUI field for objects.
        /// </summary>
        /// <param name="key">The key for the field.</param>
        /// <param name="type">The type.</param>
        /// <param name="label">The label for the field.</param>
        /// <param name="value">The current value for the field.</param>
        /// <param name="allowSceneObjects">If set to <c>true</c> then allow scene objects to be assigned to the field.</param>
        /// <returns>
        /// The object assigned to the field.
        /// </returns>
        public static object ObjectField(object key, Type type, GUIContent label, object value, bool allowSceneObjects = true)
        {
            var rect = EditorGUILayout.GetControlRect();

            if (label != null && label.text != null)
            {
                rect = EditorGUI.PrefixLabel(rect, label);
            }
            else
            {
                rect = EditorGUI.IndentedRect(rect);
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
            else
            {
                string baseType = value.GetType() == type ? "" : " : " + type.GetNiceName();
                title = new GUIContent(value.GetType().GetNiceName() + baseType, EditorIcons.StarPointer.Inactive);
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
        /// Draws an GUI field for objects.
        /// </summary>
        /// <typeparam name="T">The object type for the field.</typeparam>
        /// <param name="key">The key for the field.</param>
        /// <param name="label">The label for the field.</param>
        /// <param name="value">The current value for the field.</param>
        /// <param name="allowSceneObjects">If set to <c>true</c> then allow scene objects to be assigned to the field.</param>
        /// <returns>The object assigned to the field.</returns>
        public static T ObjectField<T>(object key, GUIContent label, T value, bool allowSceneObjects = true)
        {
            return (T)ObjectField(key, typeof(T), label, value, allowSceneObjects);
        }

        /// <summary>
        /// Draws an open inspector button.
        /// </summary>
        /// <param name="rect">The rect to draw the buton in.</param>
        /// <param name="obj">The object to open an inspector for..</param>
        public static void DrawOpenInspector(Rect rect, UnityEngine.Object obj)
        {
            var prevEnabled = GUI.enabled;
            GUI.enabled = obj != null;
            if (GUI.enabled && Event.current.isMouse && rect.Contains(Event.current.mousePosition))
            {
                GUIHelper.RequestRepaint();
            }
            if (SirenixEditorGUI.IconButton(rect, EditorIcons.Pen, "Inspect object") && Event.current.button == 0)
            {
                GUIHelper.OpenInspectorWindow(obj);
            }
            GUI.enabled = prevEnabled;
        }

        /// <summary>
        /// Draws an open inspector button.
        /// </summary>
        /// <param name="obj">The object to open the inspector for.</param>
        public static void DrawOpenInspector(UnityEngine.Object obj)
        {
            var prevEnabled = GUI.enabled;
            GUI.enabled = obj != null && prevEnabled;
            if (SirenixEditorGUI.IconButton(EditorIcons.Pen, "Inspect object") && Event.current.button == 0)
            {
                GUIHelper.OpenInspectorWindow(obj);
            }
            GUI.enabled = prevEnabled;
        }

        /// <summary>
        /// Draws an GUI field for objects.
        /// </summary>
        /// <param name="label">The label for the field.</param>
        /// <param name="value">The value of the field.</param>
        /// <param name="objectType">The object type for the field.</param>
        /// <param name="allowSceneObjects">If set to <c>true</c> then allow scene objects to be assigned to the field.</param>
        /// <param name="isReadOnly">If set to <c>true</c> the field is readonly.</param>
        /// <returns>The object assigned to the field.</returns>
        public static UnityEngine.Object ObjectField(GUIContent label, UnityEngine.Object value, Type objectType, bool allowSceneObjects, bool isReadOnly = false)
        {
            var rect = EditorGUILayout.GetControlRect();
            //var rect = GUILayoutUtility.GetRect(16, 16, GUILayoutOptions.ExpandWidth(true));
            return ObjectField(rect, label, value, objectType, allowSceneObjects, isReadOnly);
        }

        /// <summary>
        /// Draws a GUI color field.
        /// </summary>
        /// <param name="rect">The rect to draw the field in.</param>
        /// <param name="color">The color of the field.</param>
        /// <param name="useAlphaInPreview">If set to <c>true</c> then use alpha in the preview.</param>
        /// <param name="showAlphaBar">If set to <c>true</c> then show alpha bar in the preview.</param>
        /// <returns>The color assigned to the field.</returns>
        public static Color DrawColorField(Rect rect, Color color, bool useAlphaInPreview = true, bool showAlphaBar = false)
        {
            const int margin = 3;

            EditorGUI.LabelField(rect, GUIContent.none, SirenixGUIStyles.ColorFieldBackground);

            rect.x += margin;
            rect.y += margin;
            rect.width -= margin * 2;
            rect.height -= margin * 2;

            if (Event.current.type != EventType.Repaint)
            {
                color = EditorGUI.ColorField(rect, GUIContent.none, color, false, true, false, null);
            }

            DrawSolidRect(rect, useAlphaInPreview ? color : new Color(color.r, color.g, color.b, 1f));

            if (showAlphaBar)
            {
                rect.y += rect.height - 7;
                rect.height = 7;
                DrawSolidRect(rect, Color.black);
                rect.width *= color.a;
                DrawSolidRect(rect, Color.white);
            }

            return color;
        }

        /// <summary>
        ///	Draws a warning message box.
        /// </summary>
        /// <remarks>
        /// Also triggers a warning during validation checks done by <see cref="OdinInspectorValidationChecker"/>
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        public static void WarningMessageBox(string message, bool wide = true)
        {
            BeginIndentedVertical(SirenixGUIStyles.PropertyMessagePaddingTest);
            EditorGUILayout.HelpBox(message, MessageType.Warning, wide);

            if (OdinInspectorValidationChecker.IsRunningValidationCheck)
            {
                OdinInspectorValidationChecker.LogWarning(message);
            }
            else
            {
                if (Event.current.button == 1 && Event.current.type == EventType.MouseDown && GUIHelper.GetCurrentLayoutRect().Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy warning message"), false, () => { Clipboard.Copy(message); });
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            EndIndentedVertical();
        }

        /// <summary>
        ///	Draws an error message box.
        /// </summary>
        /// <remarks>
        /// Also triggers an error during validation checks done by <see cref="OdinInspectorValidationChecker"/>
        /// </remarks>
        /// <param name="message">The message.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        public static void ErrorMessageBox(string message, bool wide = true)
        {
            BeginIndentedHorizontal();
            GUILayout.Space(3);
            EditorGUILayout.HelpBox(message, MessageType.Error, wide);

            if (OdinInspectorValidationChecker.IsRunningValidationCheck)
            {
                OdinInspectorValidationChecker.LogError(message);
            }
            else
            {
                if (Event.current.button == 1 && Event.current.type == EventType.MouseDown && GUIHelper.GetCurrentLayoutRect().Contains(Event.current.mousePosition))
                {
                    GenericMenu menu = new GenericMenu();
                    menu.AddItem(new GUIContent("Copy error message"), false, () => { Clipboard.Copy(message); });
                    menu.ShowAsContext();
                    Event.current.Use();
                }
            }

            EndIndentedHorizontal();
        }

        /// <summary>
        /// Draws a info message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        public static void InfoMessageBox(string message, bool wide = true)
        {
            BeginIndentedHorizontal();
            GUILayout.Space(3);
            EditorGUILayout.HelpBox(message, MessageType.Info, wide);
            EndIndentedHorizontal();
        }

        /// <summary>
        /// Draws a message box.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        public static void MessageBox(string message, bool wide = true)
        {
            BeginIndentedHorizontal();
            GUILayout.Space(3);
            EditorGUILayout.HelpBox(message, MessageType.None, wide);
            EndIndentedHorizontal();
        }

        /// <summary>
        /// Draws a message box that can be expanded to show more details.
        /// </summary>
        /// <param name="message">The message of the message box.</param>
        /// <param name="detailedMessage">The detailed message of the message box.</param>
        /// <param name="messageType">Type of the message box.</param>
        /// <param name="isFolded">If set to <c>true</c> the detailed message is hidden.</param>
        /// <param name="wide">If set to <c>true</c> the message box will be wide.</param>
        /// <returns>State of isFolded.</returns>
        public static bool DetailedMessageBox(string message, string detailedMessage, MessageType messageType, bool isFolded, bool wide = true)
        {
            BeginIndentedHorizontal();
            GUILayout.Space(3);
            EditorGUILayout.HelpBox((isFolded ? message : message + "\n\n" + detailedMessage) + "\n", messageType, wide);
            var position = GUILayoutUtility.GetLastRect();

            if (!wide)
            {
                position.x += EditorGUIUtility.labelWidth;
                position.width -= EditorGUIUtility.labelWidth;
            }

            var iconRect = new Rect(position.x + position.width * 0.5f - 9f, position.y + position.height - 18f + 2f, 18f, 18f);
            var arrow = isFolded ? EditorIcons.TriangleDown : EditorIcons.TriangleUp;
            bool isMouseOver = position.Contains(Event.current.mousePosition);

            if (SirenixEditorGUI.IconButton(
                Event.current.type == EventType.Repaint ? iconRect : position,
                isMouseOver ? arrow.Highlighted : arrow.Inactive,
                isFolded ? "Show details" : "Hide details") || Event.current.rawType == EventType.MouseDown && isMouseOver)
            {
                isFolded = !isFolded;

                GUIHelper.PushGUIEnabled(true);
                Event.current.Use();
                GUIHelper.PopGUIEnabled();
            }
            EndIndentedHorizontal();
            return isFolded;
        }

        /// <summary>
        /// Draws a horizontal line seperator.
        /// </summary>
        /// <param name="lineWidth">Width of the line.</param>
        public static void HorizontalLineSeperator(int lineWidth = 1)
        {
            HorizontalLineSeperator(SirenixGUIStyles.BorderColor, lineWidth);
        }

        /// <summary>
        /// Draws a horizontal line seperator.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="lineWidth">The size of the line.</param>
        public static void HorizontalLineSeperator(Color color, int lineWidth = 1)
        {
            Rect rect = GUILayoutUtility.GetRect(lineWidth, lineWidth, GUILayoutOptions.ExpandWidth(true));
            DrawSolidRect(rect, color, true);
        }

        /// <summary>
        /// Draws a vertical line seperator.
        /// </summary>
        /// <param name="lineWidth">Width of the line.</param>
        public static void VerticalLineSeperator(int lineWidth = 1)
        {
            VerticalLineSeperator(SirenixGUIStyles.BorderColor, lineWidth);
        }

        /// <summary>
        /// Draws a vertical line seperator.
        /// </summary>
        /// <param name="color">The color of the line.</param>
        /// <param name="lineWidth">Width of the line.</param>
        public static void VerticalLineSeperator(Color color, int lineWidth = 1)
        {
            Rect rect = GUILayoutUtility.GetRect(lineWidth, lineWidth, GUILayoutOptions.ExpandHeight(true).Width(lineWidth));
            DrawSolidRect(rect, color, true);
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="icon">The editor icon for the button.</param>
        /// <param name="width">The width of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <param name="tooltip">The tooltip of the button.</param>
        /// <returns><c>true</c> if the button was pressed. Otherwise <c>false</c>.</returns>
        public static bool IconButton(EditorIcon icon, int width = 18, int height = 18, string tooltip = "")
        {
            return IconButton(icon, null, width, height, tooltip);
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="icon">The editor icon for the button.</param>
        /// <param name="style">The GUI style for the button.</param>
        /// <param name="width">The width of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <param name="tooltip">The tooltip of the button.</param>
        /// <returns><c>true</c> if the button was pressed. Otherwise <c>false</c>.</returns>
        public static bool IconButton(EditorIcon icon, GUIStyle style, int width = 18, int height = 18, string tooltip = "")
        {
            style = style ?? SirenixGUIStyles.IconButton;
            Rect rect = GUILayoutUtility.GetRect(icon.HighlightedGUIContent, style, GUILayoutOptions.ExpandWidth(false).Width(width).Height(height));
            return IconButton(rect, icon, style, tooltip);
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="rect">The rect to draw the button in.</param>
        /// <param name="icon">The editor icon for the button.</param>
        /// <returns><c>true</c> if the button was pressed. Otherwise <c>false</c>.</returns>
        public static bool IconButton(Rect rect, EditorIcon icon)
        {
            return IconButton(rect, icon, null, "");
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="rect">The rect to draw the button in.</param>
        /// <param name="icon">The editor icon for the button.</param>
        /// <param name="tooltip">The tooltip of the button.</param>
        /// <returns><c>true</c> if the button was pressed. Otherwise <c>false</c>.</returns>
        public static bool IconButton(Rect rect, EditorIcon icon, string tooltip)
        {
            return IconButton(rect, icon, null, tooltip);
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="rect">The rect to draw the button in.</param>
        /// <param name="icon">The editor icon for the button.</param>
        /// <param name="style">The GUI style for the button.</param>
        /// <param name="tooltip">The tooltip of the button.</param>
        /// <returns><c>true</c> if the button was pressed. Otherwise <c>false</c>.</returns>
        public static bool IconButton(Rect rect, EditorIcon icon, GUIStyle style, string tooltip)
        {
            bool hover = GUI.enabled && Event.current.type == EventType.Repaint ? rect.Contains(Event.current.mousePosition) : false;
            style = style ?? SirenixGUIStyles.IconButton;
            if (GUI.Button(rect, new GUIContent(hover ? icon.Highlighted : icon.Inactive, tooltip), style))
            {
                GUI.FocusControl(null);
                return true;
            }

            if (Event.current.isMouse)
            {
                GUIHelper.RequestRepaint();
            }

            return false;
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="rect">The rect to draw the button in.</param>
        /// <param name="icon">The icon texture.</param>
        /// <param name="tooltip">The tooltip for the button.</param>
        /// <returns><c>true</c> when the button is pressed.</returns>
        public static bool IconButton(Rect rect, Texture icon, string tooltip)
        {
            return IconButton(rect, icon, null, tooltip);
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="rect">The rect to draw the button in.</param>
        /// <param name="icon">The icon texture.</param>
        /// <param name="style">Style for the button.</param>
        /// <param name="tooltip">The tooltip for the button.</param>
        /// <returns><c>true</c> when the button is pressed.</returns>
        public static bool IconButton(Rect rect, Texture icon, GUIStyle style, string tooltip)
        {
            style = style ?? SirenixGUIStyles.IconButton;
            if (GUI.Button(rect, new GUIContent(icon, tooltip), style))
            {
                GUI.FocusControl(null);
                return true;
            }

            if (Event.current.isMouse)
            {
                GUIHelper.RequestRepaint();
            }

            return false;
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="icon">The icon texture.</param>
        /// <param name="width">Width of the button in pixels.</param>
        /// <param name="height">Height of the button in pixels.</param>
        /// <param name="tooltip">The tooltip for the button.</param>
        /// <returns><c>true</c> when the button is pressed.</returns>
        public static bool IconButton(Texture icon, int width = 18, int height = 18, string tooltip = "")
        {
            return IconButton(icon, SirenixGUIStyles.IconButton, width, height, tooltip);
        }

        /// <summary>
        /// Draws a GUI button with an icon.
        /// </summary>
        /// <param name="icon">The icon texture.</param>
        /// <param name="style">Style for the button.</param>
        /// <param name="width">Width of the button in pixels.</param>
        /// <param name="height">Height of the button in pixels.</param>
        /// <param name="tooltip">The tooltip for the button.</param>
        /// <returns><c>true</c> when the button is pressed.</returns>
        public static bool IconButton(Texture icon, GUIStyle style, int width = 18, int height = 18, string tooltip = "")
        {
            style = style ?? SirenixGUIStyles.IconButton;
            Rect rect = GUILayoutUtility.GetRect(GUIHelper.TempContent(icon), style, GUILayoutOptions.ExpandWidth(false).Width(width).Height(height));
            return IconButton(rect, icon, style, tooltip);
        }

        /// <summary>
        /// Draws a repeating icon button.
        /// </summary>
        /// <param name="icon">The icon for the button.</param>
        /// <returns><c>true</c> while the button is active. Otherwise <c>false</c>.</returns>
        public static bool IconRepeatButton(EditorIcon icon)
        {
            if (IconRepeatButton(icon, 18))
            {
                GUI.FocusControl(null);
                GUIHelper.RequestRepaint();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws a repeating icon button.
        /// </summary>
        /// <param name="icon">The icon for the button.</param>
        /// <param name="size">The size.</param>
        /// <returns><c>true</c> while the button is active. Otherwise <c>false</c>.</returns>
        public static bool IconRepeatButton(EditorIcon icon, int size)
        {
            if (IconRepeatButton(icon, size, size))
            {
                GUI.FocusControl(null);
                GUIHelper.RequestRepaint();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws a repeating icon button.
        /// </summary>
        /// <param name="icon">The icon for the button.</param>
        /// <param name="width">The width of the button.</param>
        /// <param name="height">The height of the button.</param>
        /// <returns><c>true</c> while the button is active. Otherwise <c>false</c>.</returns>
        public static bool IconRepeatButton(EditorIcon icon, int width, int height)
        {
            if (GUILayout.RepeatButton(icon.InactiveGUIContent, SirenixGUIStyles.None, GUILayoutOptions.Width(width).Height(height)))
            {
                GUI.FocusControl(null);
                GUIHelper.RequestRepaint();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws a toolbar icon button.
        /// </summary>
        /// <param name="icon">The icon for the button.</param>
        /// <param name="ignoreGUIEnabled">If true, the button clickable while GUI.enabled == false.</param>
        /// <returns>
        ///   <c>true</c> if the button was pressed. Otherwise <c>false</c>.
        /// </returns>
        public static bool ToolbarButton(EditorIcon icon, bool ignoreGUIEnabled = false)
        {
            if (GUILayout.Button(icon.InactiveGUIContent, SirenixGUIStyles.ToolbarButton, GUILayoutOptions.Width(currentDrawingToolbarHeight).Height(currentDrawingToolbarHeight)))
            {
                GUI.FocusControl(null);
                GUIHelper.RequestRepaint();
                return true;
            }

            if (ignoreGUIEnabled)
            {
                if (Event.current.button == 0 && Event.current.rawType == EventType.MouseDown)
                {
                    if (GUILayoutUtility.GetLastRect().Contains(Event.current.mousePosition))
                    {
                        GUI.FocusControl(null);
                        GUIHelper.RequestRepaint();
                        GUIHelper.PushGUIEnabled(true);
                        Event.current.Use();
                        GUIHelper.PopGUIEnabled();
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Draws a toolbar icon button.
        /// </summary>
        /// <param name="content">The GUI content for the button.</param>
        /// <param name="selected">Whether the button state is selected or not</param>
        /// <returns><c>true</c> if the button was pressed. Otherwise <c>false</c>.</returns>
        public static bool ToolbarButton(GUIContent content, bool selected = false)
        {
            if (GUILayout.Button(content, selected ? SirenixGUIStyles.ToolbarButtonSelected : SirenixGUIStyles.ToolbarButton, GUILayoutOptions.Height(currentDrawingToolbarHeight)))
            {
                GUI.FocusControl(null);
                GUIHelper.RequestRepaint();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Draws a toolbar toggle.
        /// </summary>
        /// <param name="isActive">Current state of the toggle.</param>
        /// <param name="icon">The icon for the toggle.</param>
        /// <returns>The state of the toggle.</returns>
        public static bool ToolbarToggle(bool isActive, EditorIcon icon)
        {
            if (GUILayout.Toggle(isActive, icon.Highlighted, SirenixGUIStyles.ToolbarButton, GUILayoutOptions.Width(currentDrawingToolbarHeight).Height(currentDrawingToolbarHeight)))
            {
                if (isActive == false)
                {
                    GUI.FocusControl(null);
                    GUIHelper.RequestRepaint();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Draws a toolbar toggle.
        /// </summary>
        /// <param name="isActive">Current state of the toggle.</param>
        /// <param name="content">The GUI content for the button.</param>
        /// <returns>The state of the toggle.</returns>
        public static bool ToolbarToggle(bool isActive, GUIContent content)
        {
            if (GUILayout.Toggle(isActive, content, SirenixGUIStyles.ToolbarButton, GUILayoutOptions.Height(currentDrawingToolbarHeight)))
            {
                if (isActive == false)
                {
                    GUI.FocusControl(null);
                    GUIHelper.RequestRepaint();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Draws a toolbar toggle.
        /// </summary>
        /// <param name="isActive">Current state of the toggle.</param>
        /// <param name="text">The text for the toggle.</param>
        /// <returns>The state of the toggle.</returns>
        public static bool ToolbarToggle(bool isActive, string text)
        {
            return ToolbarToggle(isActive, GUIHelper.TempContent(text));

            //EditorStyles.toolbarButton.fixedHeight = currentDrawingToolbarHeight;
            //if (GUILayout.Toggle(isActive, text, EditorStyles.toolbarButton, GUILayoutOptions.Height(currentDrawingToolbarHeight)))
            //{
            //    if (isActive == false)
            //    {
            //        GUI.FocusControl(null);
            //        GUIHelper.RequestRepaint();
            //    }
            //    return true;
            //}
            //return false;
        }

        /// <summary>
        /// Draws a toolbar tab.
        /// </summary>
        /// <param name="isActive">If <c>true</c> the tab will be the active tab.</param>
        /// <param name="label">Name for the tab.</param>
        /// <returns>State of isActive.</returns>
        public static bool ToolbarTab(bool isActive, string label)
        {
            return ToolbarTab(isActive, GUIHelper.TempContent(label));
        }

        /// <summary>
        /// Draws a toolbar tab.
        /// </summary>
        /// <param name="isActive">If <c>true</c> the tab will be the active tab.</param>
        /// <param name="label">Label for the tab.</param>
        /// <returns>State of isActive.</returns>
        public static bool ToolbarTab(bool isActive, GUIContent label)
        {
            if (GUILayout.Toggle(isActive, label, SirenixGUIStyles.ToolbarTab, GUILayoutOptions.Height(currentDrawingToolbarHeight)))
            {
                if (isActive == false)
                {
                    GUI.FocusControl(null);
                    GUIHelper.RequestRepaint();
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Draws a solid color rectangle.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="color">The color.</param>
		/// <param name="usePlaymodeTint">If <c>true</c> applies the user's playmdoe tint to the rect in playmode.</param>
        public static void DrawSolidRect(Rect rect, Color color, bool usePlaymodeTint = true)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (usePlaymodeTint)
                {
                    EditorGUI.DrawRect(rect, color);
                }
                else
                {
                    GUIHelper.PushColor(color);
                    GUI.DrawTexture(rect, EditorGUIUtility.whiteTexture);
                    GUIHelper.PopColor();
                }
            }
        }

        /// <summary>
        /// Draws a solid color rectangle.
        /// </summary>
        /// <param name="width">The width.</param>
        /// <param name="height">The height.</param>
        /// <param name="color">The color.</param>
		/// <param name="usePlaymodeTint">If <c>true</c> applies the user's playmdoe tint to the rect in playmode.</param>
        /// <returns>The rect created.</returns>
        public static Rect DrawSolidRect(float width, float height, Color color, bool usePlaymodeTint = true)
        {
            var rect = GUILayoutUtility.GetRect(width, height, GUILayoutOptions.ExpandWidth(false));
            DrawSolidRect(rect, color, usePlaymodeTint);
            return rect;
        }

        /// <summary>
        /// Draws borders around a rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="borderWidth">The width of the border on all sides.</param>
		/// <param name="usePlaymodeTint">If <c>true</c> applies the user's playmdoe tint to the rect in playmode.</param>
        public static void DrawBorders(Rect rect, int borderWidth, bool usePlaymodeTint = true)
        {
            DrawBorders(rect, borderWidth, borderWidth, borderWidth, borderWidth, SirenixGUIStyles.BorderColor, usePlaymodeTint);
        }

        /// <summary>
        /// Draws borders around a rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="borderWidth">The width of the border on all sides.</param>
        /// <param name="color">The color of the border.</param>
		/// <param name="usePlaymodeTint">If <c>true</c> applies the user's playmdoe tint to the rect in playmode.</param>
        public static void DrawBorders(Rect rect, int borderWidth, Color color, bool usePlaymodeTint = true)
        {
            DrawBorders(rect, borderWidth, borderWidth, borderWidth, borderWidth, color, usePlaymodeTint);
        }

        /// <summary>
        /// Draws borders around a rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="left">The left size.</param>
        /// <param name="right">The right size.</param>
        /// <param name="top">The top size.</param>
        /// <param name="bottom">The bottom size.</param>
		/// <param name="usePlaymodeTint">If <c>true</c> applies the user's playmdoe tint to the rect in playmode.</param>
        public static void DrawBorders(Rect rect, int left, int right, int top, int bottom, bool usePlaymodeTint = true)
        {
            DrawBorders(rect, left, right, top, bottom, SirenixGUIStyles.BorderColor, usePlaymodeTint);
        }

        /// <summary>
        /// Draws borders around a rect.
        /// </summary>
        /// <param name="rect">The rect.</param>
        /// <param name="left">The left size.</param>
        /// <param name="right">The right size.</param>
        /// <param name="top">The top size.</param>
        /// <param name="bottom">The bottom size.</param>
        /// <param name="color">The color of the borders.</param>
		/// <param name="usePlaymodeTint">If <c>true</c> applies the user's playmdoe tint to the rect in playmode.</param>
        public static void DrawBorders(Rect rect, int left, int right, int top, int bottom, Color color, bool usePlaymodeTint = true)
        {
            if (Event.current.type == EventType.Repaint)
            {
                if (left > 0)
                {
                    var borderRect = rect;
                    borderRect.width = left;
                    DrawSolidRect(borderRect, color, usePlaymodeTint);
                }

                if (top > 0)
                {
                    var borderRect = rect;
                    borderRect.height = top;
                    DrawSolidRect(borderRect, color, usePlaymodeTint);
                }

                if (right > 0)
                {
                    var borderRect = rect;
                    borderRect.x += rect.width - right;
                    borderRect.width = right;
                    DrawSolidRect(borderRect, color, usePlaymodeTint);
                }

                if (bottom > 0)
                {
                    var borderRect = rect;
                    borderRect.y += rect.height - bottom;
                    borderRect.height = bottom;
                    DrawSolidRect(borderRect, color, usePlaymodeTint);
                }
            }
        }

        /// <summary>
        /// Draws a toolbar search field.
        /// </summary>
        /// <param name="searchText">The current search text.</param>
        /// <param name="forceFocus">If set to <c>true</c> the force focus on the field.</param>
        /// <param name="marginLeftRight">The left and right margin.</param>
        /// <returns>The current search text.</returns>
        public static string ToolbarSearchField(string searchText, bool forceFocus = false, float marginLeftRight = 5)
        {
            searchText = searchText ?? "";
            var rect = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, SirenixGUIStyles.ToolbarSeachTextField, GUILayoutOptions.ExpandWidth(true));
            rect.y += currentDrawingToolbarPaddingTop * 0.5f;
            rect.x += marginLeftRight;
            rect.width -= marginLeftRight * 2;

            if (forceFocus)
            {
                GUI.SetNextControlName("ToolbarSearchField");
            }

            rect.width -= 16;
            searchText = EditorGUI.TextField(rect, searchText, SirenixGUIStyles.ToolbarSeachTextField);

            if (forceFocus)
            {
                GUI.FocusControl("ToolbarSearchField");
            }

            rect.x += rect.width;
            rect.width = 16;

            if (GUI.Button(rect, GUIContent.none, SirenixGUIStyles.ToolbarSeachCancelButton))
            {
                searchText = "";
                GUI.FocusControl(null);
                GUIHelper.RequestRepaint();
            }

            return searchText;
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="selected">The enum current enum.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(Enum selected, params GUILayoutOption[] options)
        {
            return SirenixEditorGUI.EnumMaskPopup(selected, EditorStyles.popup, options);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="selected">The current enum.</param>
        /// <param name="style">The style for the field.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            var position = EditorGUILayout.GetControlRect(false, EditorGUIUtility.singleLineHeight, style, options);
            return SirenixEditorGUI.EnumMaskPopup(position, GUIContent.none, selected, style);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="label">The label for the field.</param>
        /// <param name="selected">The current enum.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(string label, Enum selected, params GUILayoutOption[] options)
        {
            GUIStyle popup = EditorStyles.popup;
            return SirenixEditorGUI.EnumMaskPopup(new GUIContent(label), selected, popup, options);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="label">The GUI content for the label.</param>
        /// <param name="selected">The current enum.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(GUIContent label, Enum selected, params GUILayoutOption[] options)
        {
            GUIStyle popup = EditorStyles.popup;
            return SirenixEditorGUI.EnumMaskPopup(label, selected, popup, options);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="label">The GUI content for the label.</param>
        /// <param name="selected">The current enum.</param>
        /// <param name="style">The style for the field.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns></returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(GUIContent label, Enum selected, GUIStyle style, params GUILayoutOption[] options)
        {
            if (label == null)
            {
                var position = GUILayoutUtility.GetRect(EditorGUIUtility.singleLineHeight, EditorGUIUtility.singleLineHeight, style, options);
                return SirenixEditorGUI.EnumMaskPopup(position, label, selected, style);
            }
            else
            {
                var position = EditorGUILayout.GetControlRect(true, EditorGUIUtility.singleLineHeight, style, options);
                return SirenixEditorGUI.EnumMaskPopup(position, label, selected, style);
            }
        }

        /// <summary>
        /// Begins a horizontal toolbar. Remember to end with <see cref="EndHorizontalToolbar"/>.
        /// </summary>
        /// <param name="height">The height of the toolbar.</param>
        /// <param name="paddingTop">Padding for the top of the toolbar.</param>
        /// <returns>The rect of the horizontal toolbar.</returns>
        public static Rect BeginHorizontalToolbar(int height = 23, int paddingTop = 4)
        {
            var rect = BeginHorizontalToolbar(SirenixGUIStyles.ToolbarBackground, height, paddingTop);
            return rect;
        }

        /// <summary>
        /// Begins a horizontal toolbar. Remember to end with <see cref="EndHorizontalToolbar" />.
        /// </summary>
        /// <param name="style">The style for the toolbar.</param>
        /// <param name="height">The height of the toolbar.</param>
        /// <param name="topPadding">The top padding.</param>
        /// <returns>
        /// The rect of the horizontal toolbar.
        /// </returns>
        public static Rect BeginHorizontalToolbar(GUIStyle style, int height = 23, int topPadding = 4)
        {
            var prevPadding = style.padding.top;
            style.padding.top = topPadding;
            currentDrawingToolbarPaddingTop = topPadding;
            currentDrawingToolbarHeight = height;
            var prev = style.fixedHeight;
            style.fixedHeight = height;
            var rect = EditorGUILayout.BeginHorizontal(style, GUILayoutOptions.Height(height).ExpandWidth(false));
            style.fixedHeight = prev;
            GUIHelper.PushHierarchyMode(true);
            GUIHelper.PushIndentLevel(0);
            style.padding.top = prevPadding;
            return rect;
        }

        /// <summary>
        /// Ends a horizontal toolbar started by <see cref="BeginHorizontalToolbar(int, int)"/>.
        /// </summary>
        public static void EndHorizontalToolbar()
        {
            currentDrawingToolbarPaddingTop = 0;
            GUIHelper.PopIndentLevel();
            GUIHelper.PopHierarchyMode();
            EditorGUILayout.EndHorizontal();
        }

        /// <summary>
        /// Begins a horizontal indentation. Remember to end with <see cref="EndIndentedHorizontal"/>.
        /// </summary>
        /// <param name="options">The GUI layout options.</param>
        public static void BeginIndentedHorizontal(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(options);
            if (EditorGUI.indentLevel != 0)
            {
                IndentSpace();
            }
            GUIHelper.PushIndentLevel(0);
        }

        /// <summary>
        /// Begins a horizontal indentation. Remember to end with <see cref="EndIndentedHorizontal"/>.
        /// </summary>
        /// <param name="style">The style of the indentation.</param>
        /// <param name="options">The GUI layout options.</param>
        public static void BeginIndentedHorizontal(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(style, options);
            if (EditorGUI.indentLevel != 0)
            {
                IndentSpace();
            }
            GUIHelper.PushIndentLevel(0);
        }

        /// <summary>
        /// Ends a identation horizontal layout group started by <see cref="BeginIndentedHorizontal(GUILayoutOption[])"/>.
        /// </summary>
        public static void EndIndentedHorizontal()
        {
            GUIHelper.PopIndentLevel();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Begins a vertical indentation. Remember to end with <see cref="EndIndentedVertical"/>.
        /// </summary>
        /// <param name="options">The GUI layout options.</param>
        public static void BeginIndentedVertical(params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(GUIStyle.none);
            if (EditorGUI.indentLevel != 0)
            {
                IndentSpace();
            }
            GUIHelper.PushIndentLevel(0);
            GUILayout.BeginVertical(options);
        }

        /// <summary>
        /// Begins a vertical indentation. Remember to end with <see cref="EndIndentedVertical"/>.
        /// </summary>
        /// <param name="style">The style of the indentation.</param>
        /// <param name="options">The GUI layout options.</param>
        public static void BeginIndentedVertical(GUIStyle style, params GUILayoutOption[] options)
        {
            GUILayout.BeginHorizontal(GUIStyle.none);
            if (EditorGUI.indentLevel != 0)
            {
                IndentSpace();
            }
            GUIHelper.PushIndentLevel(0);
            GUILayout.BeginVertical(style, options);
        }

        /// <summary>
        /// Ends a identation vertical layout group started by <see cref="BeginIndentedVertical(GUILayoutOption[])"/>.
        /// </summary>
        public static void EndIndentedVertical()
        {
            GUILayout.EndVertical();
            GUIHelper.PopIndentLevel();
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Indents by the current indent value, <see cref="GUIHelper.CurrentIndentAmount"/>.
        /// </summary>
        public static void IndentSpace()
        {
            GUILayout.Space(GUIHelper.CurrentIndentAmount);
        }

        /// <summary>
        /// Draws a menu button.
        /// </summary>
        /// <param name="indent">The indent of the button.</param>
        /// <param name="text">The text of the button.</param>
        /// <param name="isActive">The current state of the button.</param>
        /// <param name="icon">The texture icon for the button.</param>
        /// <returns>The current state of the button.</returns>
        public static bool MenuButton(int indent, string text, bool isActive, Texture icon)
        {
            bool isDown = false;

            var rect = EditorGUILayout.BeginHorizontal(SirenixGUIStyles.MenuButtonBackground);
            bool isMouseOver = rect.Contains(Event.current.mousePosition);
            if (isActive)
            {
                DrawSolidRect(rect, isMouseOver ? SirenixGUIStyles.MenuButtonActiveBgColor : SirenixGUIStyles.MenuButtonActiveBgColor);
            }
            else
            {
                DrawSolidRect(rect, isMouseOver ? SirenixGUIStyles.MenuButtonHoverColor : SirenixGUIStyles.MenuButtonColor);
            }

            DrawBorders(rect, 0, 0, 0, 1, SirenixGUIStyles.MenuButtonBorderColor);

            if (Event.current.type == EventType.MouseDown)
            {
                if (isMouseOver)
                {
                    Event.current.Use();
                    isDown = true;
                }
                GUIHelper.RequestRepaint();
            }

            var style = new GUIStyle(EditorStyles.label);
            style.fixedHeight = 20;
            if (isActive)
            {
                style.normal.textColor = Color.white;
            }

            GUILayout.Space(indent * 10);

            GUILayout.Label(new GUIContent(text, icon), style);
            EditorGUILayout.EndHorizontal();

            return isDown;
        }

        /// <summary>
        /// Begins a fade group. Remember to end with <see cref="EndFadeGroup"/>.
        /// </summary>
        /// <param name="key">The key for the fade group.</param>
        /// <param name="isVisible">Current state of the fade group.</param>
        /// <param name="duration">The duration of fade in and out.</param>
        /// <returns>The current state of the fade group.</returns>
        public static bool BeginFadeGroup(object key, bool isVisible, float duration = DEFAULT_FADE_GROUP_DURATION)
        {
            EditorTimeHelper.Time.Update();
            var t = GUIHelper.GetTemporaryContext(fadeGroupKey, key, isVisible ? 1f : 0f);
            if (Event.current.type == EventType.Layout)
            {
                t.Value = Mathf.MoveTowards(t.Value, isVisible ? 1 : 0, EditorTimeHelper.Time.DeltaTime * (1f / duration));
            }
            return BeginFadeGroup(t.Value);
        }

        /// <summary>
        /// Begins a fade group. Remember to end with <see cref="EndFadeGroup" />.
        /// </summary>
        /// <param name="key">The key for the fade group.</param>
        /// <param name="isVisible">Current state of the fade group.</param>
        /// <param name="t">A value between 0 and 1 indicating how expanded the fade group is.</param>
        /// <param name="duration">The duration of fade in and out.</param>
        /// <returns>
        /// The current state of the fade group.
        /// </returns>
        public static bool BeginFadeGroup(object key, bool isVisible, out float t, float duration = DEFAULT_FADE_GROUP_DURATION)
        {
            EditorTimeHelper.Time.Update();
            var tt = GUIHelper.GetTemporaryContext(fadeGroupKey, key, isVisible ? 1f : 0f);
            if (Event.current.type == EventType.Layout)
            {
                tt.Value = Mathf.MoveTowards(tt.Value, isVisible ? 1 : 0, EditorTimeHelper.Time.DeltaTime * (1f / duration));
            }
            t = tt.Value;
            return BeginFadeGroup(tt.Value);
        }

        /// <summary>
        /// Begins a fade group. Remember to end with <see cref="EndFadeGroup"/>.
        /// </summary>
        /// <param name="primaryKey">The primary key for the fade group.</param>
        /// <param name="secondaryKey">The secondly key for the fade group.</param>
        /// <param name="isVisible">Current state of the fade group.</param>
        /// <param name="duration">The duration of fade in and out.</param>
        /// <returns>The current state of the fade group.</returns>
        public static bool BeginFadeGroup(object primaryKey, object secondaryKey, bool isVisible, float duration = DEFAULT_FADE_GROUP_DURATION)
        {
            EditorTimeHelper.Time.Update();
            var t = GUIHelper.GetTemporaryContext(primaryKey, secondaryKey, isVisible ? 1f : 0f);
            if (Event.current.type == EventType.Layout)
            {
                t.Value = Mathf.MoveTowards(t.Value, isVisible ? 1 : 0, EditorTimeHelper.Time.DeltaTime * (1f / duration));
            }
            return BeginFadeGroup(t.Value);
        }

        /// <summary>
        /// Begins a fade group. Remember to end with <see cref="EndFadeGroup"/>.
        /// </summary>
        /// <param name="key">The key for the fade group.</param>
        /// <param name="name">The name of the fade group.</param>
        /// <param name="isVisible">Current state of the fade group.</param>
        /// <param name="duration">The duration of fade in and out.</param>
        /// <returns>The current state of the fade group.</returns>
        public static bool BeginFadeGroup(object key, string name, bool isVisible, float duration = DEFAULT_FADE_GROUP_DURATION)
        {
            EditorTimeHelper.Time.Update();
            var t = GUIHelper.GetTemporaryContext(key, name, isVisible ? 1f : 0f);
            if (Event.current.type == EventType.Layout)
            {
                t.Value = Mathf.MoveTowards(t.Value, isVisible ? 1 : 0, EditorTimeHelper.Time.DeltaTime * (1f / duration));
            }
            return BeginFadeGroup(t.Value);
        }

        /// <summary>
        /// Begins a fade group. Remember to end with <see cref="EndFadeGroup"/>.
        /// </summary>
        /// <param name="t">The current fading value between 0 and 1.</param>
        /// <returns></returns>
        public static bool BeginFadeGroup(float t)
        {
            //GUILayout.BeginVertical(); // This is en theory not necessary, but without some weird layout popping occurs.
            if (t > 0 && t < 1 && animatingFadeGroupIndex == -1)
            {
                t = Mathf.Clamp01(t * t * (3f - 2f * t));
                EditorGUILayout.BeginFadeGroup(t);
                GUIHelper.PushColor(GUI.color * new Color(1, 1, 1, t));
                animatingFadeGroupIndex = currentFadeGroupIndex;
                GUIHelper.RequestRepaint();
            }
            currentFadeGroupIndex++;
            return t > 0;
        }

        /// <summary>
        /// Ends a fade group started by any BeginFadeGroup.
        /// </summary>
        public static void EndFadeGroup()
        {
            currentFadeGroupIndex--;
            if (animatingFadeGroupIndex == currentFadeGroupIndex)
            {
                EditorGUILayout.EndFadeGroup();
                GUIHelper.PopColor();
                animatingFadeGroupIndex = -1;
            }
            //GUILayout.EndVertical();
        }

        /// <summary>
        /// Draws a foldout field where clicking on the label toggles to the foldout too.
        /// </summary>
        /// <param name="isVisible">The current state of the foldout.</param>
        /// <param name="label">The label of the foldout.</param>
        /// <param name="guiStyle">The GUI style.</param>
        /// <returns>
        /// The current state of the foldout.
        /// </returns>
        public static bool Foldout(bool isVisible, string label, GUIStyle style = null)
        {
            return Foldout(isVisible, new GUIContent(label), style);
        }

        /// <summary>
        /// Draws a foldout field where clicking on the label toggles to the foldout too.
        /// </summary>
        /// <param name="isVisible">The current state of the foldout.</param>
        /// <param name="label">The label of the foldout.</param>
        /// <param name="style">The GUI style.</param>
        /// <returns>
        /// The current state of the foldout.
        /// </returns>
        public static bool Foldout(bool isVisible, GUIContent label, GUIStyle style = null)
        {
            Rect rect = GUILayoutUtility.GetRect(label, SirenixGUIStyles.Foldout);
            rect.height = EditorGUIUtility.singleLineHeight;

            return Foldout(rect, isVisible, label, style);
        }

        /// <summary>
        /// Draws a foldout field where clicking on the label toggles to the foldout too.
        /// </summary>
        /// <param name="rect">The rect to draw the foldout field in.</param>
        /// <param name="isVisible">The current state of the foldout.</param>
        /// <param name="label">The label of the foldout.</param>
        /// <param name="style">The style.</param>
        /// <returns>
        /// The current state of the foldout.
        /// </returns>
        public static bool Foldout(Rect rect, bool isVisible, GUIContent label, GUIStyle style = null)
        {
            style = style ?? SirenixGUIStyles.Foldout;

            var e = Event.current.rawType;
            bool isHovering = false;
            if (e != EventType.Layout)
            {
                // Swallow foldout icon as well
                rect.x -= 9;
                rect.width += 9;
                isHovering = rect.Contains(Event.current.mousePosition);
                rect.width -= 9;
                rect.x += 9;
            }

            if (isHovering)
            {
                GUIHelper.PushLabelColor(SirenixGUIStyles.HighlightedTextColor);
            }

            if (e == EventType.MouseDown)
            {
                // Foldout works when GUI.enabled = false
                // Enable GUI, en order to Use() the the event properly.

                if (isHovering && Event.current.button == 0)
                {
                    isVisible = !isVisible;
                    GUIHelper.RequestRepaint();
                    GUIHelper.PushGUIEnabled(true);
                    Event.current.Use();
                    GUIHelper.PopGUIEnabled();
                }
                GUI.FocusControl(null);
            }

            isVisible = EditorGUI.Foldout(rect, isVisible, label, style);

            if (isHovering)
            {
                GUIHelper.PopLabelColor();
            }

            return isVisible;
        }

        /// <summary>
        /// Begins drawing a box. Remember to end with <see cref="EndBox"/>.
        /// </summary>
        /// <param name="label">The label of the box.</param>
        /// <param name="centerLabel">If set to <c>true</c> then center label.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginBox(string label, bool centerLabel = false, params GUILayoutOption[] options)
        {
            if (string.IsNullOrEmpty(label))
            {
                return BeginBox(options);
            }
            else
            {
                return BeginBox(GUIHelper.TempContent(label), centerLabel, options);
            }
        }

        /// <summary>
        /// Begins drawing a box. Remember to end with <see cref="EndBox"/>.
        /// </summary>
        /// <param name="label">The label of the box.</param>
        /// <param name="centerLabel">If set to <c>true</c> then center label.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginBox(GUIContent label, bool centerLabel = false, params GUILayoutOption[] options)
        {
            var rect = BeginBox(options);

            if (label != null)
            {
                BeginBoxHeader();
                GUILayout.Label(label, centerLabel ? SirenixGUIStyles.LabelCentered : SirenixGUIStyles.Label);
                EndBoxHeader();
            }

            return rect;
        }

        /// <summary>
        /// Begins drawing a box. Remember to end with <see cref="EndBox"/>.
        /// </summary>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginBox(params GUILayoutOption[] options)
        {
            BeginIndentedVertical(SirenixGUIStyles.PropertyMessagePaddingTest);
            GUILayout.BeginVertical(SirenixGUIStyles.BoxContainer, options);
            EditorGUIUtility.labelWidth -= 4;
            GUIHelper.PushHierarchyMode(false);
            return GUIHelper.GetCurrentLayoutRect();
        }

        /// <summary>
        /// Ends drawing a box started by any BeginBox.
        /// </summary>
        public static void EndBox()
        {
            GUIHelper.PopHierarchyMode();
            EditorGUIUtility.labelWidth += 4;
            GUILayout.EndVertical();
            EndIndentedVertical();
        }

        /// <summary>
        /// Begins drawing an inline box. Remember to end with <see cref="EndInlineBox"/>.
        /// </summary>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect of the box.</returns>
        public static Rect BeginInlineBox(params GUILayoutOption[] options)
        {
            GUILayout.BeginVertical(SirenixGUIStyles.BoxContainer, options);
            EditorGUIUtility.labelWidth -= 4;
            GUIHelper.PushHierarchyMode(false);
            return GUIHelper.GetCurrentLayoutRect();
        }

        /// <summary>
        /// Ends drawing an inline box started by any BeginInlineBox.
        /// </summary>
        public static void EndInlineBox()
        {
            GUIHelper.PopHierarchyMode();
            EditorGUIUtility.labelWidth += 4;
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Begins drawing a box header. Remember to end with <seealso cref="EndBoxHeader"/>.
        /// </summary>
        public static void BeginBoxHeader()
        {
            EditorGUIUtility.labelWidth += 4;

            var headerBgRect = EditorGUILayout.BeginHorizontal(SirenixGUIStyles.BoxHeaderStyle, GUILayoutOptions.ExpandWidth(true));

            if (Event.current.type == EventType.Repaint)
            {
                headerBgRect.x -= 3;
                headerBgRect.width += 6;
                headerBgRect.y -= 2;
                headerBgRect.height += 4;
                GUIHelper.PushColor(SirenixGUIStyles.HeaderBoxBackgroundColor);
                GUI.DrawTexture(headerBgRect, Texture2D.whiteTexture);
                GUIHelper.PopColor();
                //if (EditorGUIUtility.isProSkin == false)
                //{
                DrawBorders(headerBgRect, 0, 0, 0, 1, new Color(0, 0, 0, 0.4f));
                //}
            }
        }

        /// <summary>
        /// Ends drawing a box header started by <see cref="BeginBoxHeader"/>,
        /// </summary>
        public static void EndBoxHeader()
        {
            EditorGUILayout.EndHorizontal();
            EditorGUIUtility.labelWidth -= 4;
        }

        private struct ShakeGroup
        {
            public bool IsShaking;
            public float StartTime;
            public float Duration;

            public static ShakeGroup Default = new ShakeGroup() { StartTime = -100, IsShaking = false, Duration = 0.5f };
        }

        /// <summary>
        /// Starts the shaking animation of a shaking group.
        /// </summary>
        public static void StartShakingGroup(object key, float duration = 0.5f)
        {
            var c = GUIHelper.GetTemporaryContext<ShakeGroup>(shakeableGroupKey, key, ShakeGroup.Default);
            c.Value.StartTime = (float)EditorApplication.timeSinceStartup;
            c.Value.Duration = Mathf.Max(0.1f, 1 / duration);
            c.Value.IsShaking = true;
        }

        /// <summary>
        /// Begins a shakeable group.
        /// </summary>
        public static void BeginShakeableGroup(object key)
        {
            if (Event.current.OnRepaint())
            {
                const float speed = 50;
                const float dist = 5;
                float time = (float)EditorApplication.timeSinceStartup;
                var c = GUIHelper.GetTemporaryContext<ShakeGroup>(shakeableGroupKey, key, ShakeGroup.Default);
                var t = 1 - (time - c.Value.StartTime) * c.Value.Duration;
                c.Value.IsShaking = t > 0;
                if (c.Value.IsShaking)
                {
                    GUIHelper.PushMatrix(
                        GUI.matrix *
                        Matrix4x4.TRS(                                                           // ease in fast!
                            new Vector3(Mathf.Cos(time * speed) * dist * MathUtilities.EaseInOut(t * t * t), 0f),
                            Quaternion.identity,
                            Vector3.one));
                    GUIHelper.RequestRepaint();
                }
            }
        }

        /// <summary>
        /// Ends the shakeable group.
        /// </summary>
        public static void EndShakeableGroup(object key)
        {
            if (Event.current.OnRepaint())
            {
                var c = GUIHelper.GetTemporaryContext<ShakeGroup>(shakeableGroupKey, key, ShakeGroup.Default);
                if (c.Value.IsShaking)
                {
                    GUIHelper.PopMatrix();
                }
            }
        }

        private class VerticalMenuListInfo
        {
            public int SelectedItemIndex;
            public int MenuItemCount;
            public int CurrentIndex;
            public int? NextSelectedIndex = null;
            public float Height;
            public Vector2 ScrollPositiion;
            public bool ScrollToCurrent;
        }

        private static VerticalMenuListInfo currentVerticalMenuListInfo;

        /// <summary>
        /// Begins drawing a vertical menu list.
        /// </summary>
        /// <param name="key">The key for the menu list.</param>
        /// <returns>The rect created.</returns>
        public static Rect BeginVerticalMenuList(object key)
        {
            var context = GUIHelper.GetTemporaryContext<VerticalMenuListInfo>(menuListKey, key).Value;
            var rect = GUIHelper.BeginLayoutMeasuring();

            if (Event.current.type == EventType.Repaint)
            {
                context.Height = rect.height;
            }

            currentVerticalMenuListInfo = context;
            currentVerticalMenuListInfo.CurrentIndex = 0;
            context.ScrollPositiion = EditorGUILayout.BeginScrollView(context.ScrollPositiion, false, false);

            if (Event.current.type == EventType.KeyDown && context.MenuItemCount > 0)
            {
                if (Event.current.keyCode == KeyCode.DownArrow)
                {
                    context.NextSelectedIndex = context.SelectedItemIndex + 1;
                    context.ScrollToCurrent = true;
                    Event.current.Use();
                }
                else if (Event.current.keyCode == KeyCode.UpArrow)
                {
                    context.NextSelectedIndex = context.SelectedItemIndex - 1;
                    context.ScrollToCurrent = true;
                    Event.current.Use();
                }
            }

            if (context.NextSelectedIndex.HasValue && Event.current.type == EventType.Layout)
            {
                context.SelectedItemIndex = Mathf.Clamp(context.NextSelectedIndex.Value, 0, context.MenuItemCount - 1);
                context.NextSelectedIndex = null;
            }

            return BeginVerticalList(false);
        }

        private static bool popMenuItemLabelColor = false;

        /// <summary>
        /// Begins drawing a menu list item. Remember to end with <see cref="EndMenuListItem"/>
        /// </summary>
        /// <param name="isSelected">Value indicating whether the item is selected.</param>
        /// <param name="isMouseDown">Value indicating if the mouse is pressed on the item.</param>
        /// <param name="setAsSelected">If set to <c>true</c> the item is set as selected..</param>
        /// <returns>The rect used for the item.</returns>
        public static Rect BeginMenuListItem(out bool isSelected, out bool isMouseDown, bool setAsSelected = false)
        {
            isMouseDown = false;

            if (setAsSelected)
            {
                currentVerticalMenuListInfo.NextSelectedIndex = currentVerticalMenuListInfo.CurrentIndex;
            }

            isSelected = currentVerticalMenuListInfo.SelectedItemIndex == currentVerticalMenuListInfo.CurrentIndex;

            var rect = EditorGUILayout.BeginVertical(SirenixGUIStyles.ListItem);

            if (isSelected)
            {
                popMenuItemLabelColor = true;
                GUIHelper.PushLabelColor(Color.white);
                isSelected = true;

                if (currentVerticalMenuListInfo.ScrollToCurrent && Event.current.type == EventType.Repaint)
                {
                    float from = currentVerticalMenuListInfo.ScrollPositiion.y;
                    float to = from + currentVerticalMenuListInfo.Height;

                    if (rect.y < from)
                    {
                        currentVerticalMenuListInfo.ScrollPositiion.y = rect.y;
                    }
                    else if (rect.yMax > to)
                    {
                        currentVerticalMenuListInfo.ScrollPositiion.y = rect.yMax - currentVerticalMenuListInfo.Height;
                    }

                    currentVerticalMenuListInfo.ScrollToCurrent = false;
                }
            }

            if (Event.current.type == EventType.MouseMove && rect.Contains(Event.current.mousePosition))
            {
                currentVerticalMenuListInfo.NextSelectedIndex = currentVerticalMenuListInfo.CurrentIndex;
            }
            else if (Event.current.type == EventType.MouseDown && rect.Contains(Event.current.mousePosition))
            {
                isMouseDown = true;
                Event.current.Use();
            }

            currentVerticalMenuListInfo.CurrentIndex++;

            DrawSolidRect(rect, isSelected ? SirenixGUIStyles.MenuButtonActiveBgColor : (currentVerticalMenuListInfo.CurrentIndex % 2 == 0 ? SirenixGUIStyles.ListItemColorEven : SirenixGUIStyles.ListItemColorOdd));

            return rect;
        }

        /// <summary>
        /// Ends drawing a menu list item started by <see cref="BeginMenuListItem(out bool, out bool, bool)"/>
        /// </summary>
        public static void EndMenuListItem()
        {
            if (popMenuItemLabelColor)
            {
                GUIHelper.PopLabelColor();
                popMenuItemLabelColor = false;
            }
            EditorGUILayout.EndVertical();
        }

        /// <summary>
        /// Ends drawing a vertical menu list started by <see cref="BeginVerticalMenuList(object)"/>
        /// </summary>
        public static void EndVerticalMenuList()
        {
            EndVerticalList();
            EditorGUILayout.EndScrollView();

            currentVerticalMenuListInfo.MenuItemCount = currentVerticalMenuListInfo.CurrentIndex;
            currentVerticalMenuListInfo = null;
            GUIHelper.EndLayoutMeasuring();
        }

        /// <summary>
        /// Begins drawing a vertical list.
        /// </summary>
        /// <param name="drawBorder">If set to <c>true</c> borders will be drawn around the vertical list.</param>
        /// <param name="drawDarkBg">If set to <c>true</c> a dark background will be drawn.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect used for the list.</returns>
        public static Rect BeginVerticalList(bool drawBorder = true, bool drawDarkBg = true, params GUILayoutOption[] options)
        {
            currentScope++;
            currentListItemIndecies.SetLength(Mathf.Max(currentListItemIndecies.Count, currentScope + 1));
            currentListItemIndecies[currentScope] = 0;

            GUIHelper.PushHierarchyMode(false);

            if (Event.current.type == EventType.MouseMove)
            {
                GUIHelper.RequestRepaint();
            }

            var rect = EditorGUILayout.BeginVertical(options);
            if (drawDarkBg)
            {
                DrawSolidRect(rect, SirenixGUIStyles.ListItemDragBgColor);
            }
            if (drawBorder)
            {
                verticalListBorderRects.Push(rect);
            }
            else
            {
                verticalListBorderRects.Push(new Rect(-1, rect.y, rect.width, rect.height));
            }
            return rect;
        }

        /// <summary>
        /// Ends drawing a vertical list started by <see cref="BeginVerticalList(bool, bool, GUILayoutOption[])"/>.
        /// </summary>
        public static void EndVerticalList()
        {
            currentScope--;
            var rect = verticalListBorderRects.Pop();
            if (rect.x > 0)
            {
                rect.y -= 1;
                rect.height += 1;
                DrawBorders(rect, 1, 1, 1, 1);
            }
            rect = GUIHelper.GetCurrentLayoutRect();
            EditorGUILayout.EndVertical();
            GUIHelper.PopHierarchyMode();
        }

        /// <summary>
        /// Begins drawing a list item.
        /// </summary>
        /// <param name="allowHover">If set to <c>true</c> the item can be hovered with the mouse.</param>
        /// <param name="style">The style for the vertical list item.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect used for the item.</returns>
        public static Rect BeginListItem(bool allowHover = true, GUIStyle style = null, params GUILayoutOption[] options)
        {
            bool isMouseOver;
            return BeginListItem(allowHover, style, out isMouseOver, options);
        }

        /// <summary>
        /// Begins drawing a list item.
        /// </summary>
        /// <param name="allowHover">If set to <c>true</c> the item can be hovered with the mouse.</param>
        /// <param name="style">The style for the vertical list item.</param>
        /// <param name="isMouseOver">Value indicating if the mouse is hovering in the item.</param>
        /// <param name="options">The GUI layout options.</param>
        /// <returns>The rect used for the item.</returns>
        private static Rect BeginListItem(bool allowHover, GUIStyle style, out bool isMouseOver, params GUILayoutOption[] options)
        {
            currentListItemIndecies.SetLength(Mathf.Max(currentListItemIndecies.Count, currentScope));
            int i = currentListItemIndecies[currentScope];
            currentListItemIndecies[currentScope] = i + 1;

            GUILayout.BeginVertical(style ?? SirenixGUIStyles.ListItem, options);
            var rect = GUIHelper.GetCurrentLayoutRect();
            isMouseOver = rect.Contains(Event.current.mousePosition);

            if (Event.current.type == EventType.Repaint)
            {
                Color color = i % 2 == 0 ? SirenixGUIStyles.ListItemColorEven : SirenixGUIStyles.ListItemColorOdd;
                Color hover = color;
                if (DragAndDropManager.IsDragInProgress == false && allowHover)
                {
                    hover = i % 2 == 0 ? SirenixGUIStyles.ListItemColorHoverEven : SirenixGUIStyles.ListItemColorHoverOdd;
                }
                DrawSolidRect(rect, isMouseOver ? hover : color);
            }

            return rect;
        }

        /// <summary>
        /// Ends drawing a list item started by <see cref="BeginListItem(bool, GUIStyle, GUILayoutOption[])"/>.
        /// </summary>
        public static void EndListItem()
        {
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Creates a animated tab group.
        /// </summary>
        /// <param name="key">The key for the tab group..</param>
        /// <returns>An animated tab group.</returns>
        public static GUITabGroup CreateAnimatedTabGroup(object key)
        {
            return GUIHelper.GetTemporaryContext<GUITabGroup>(animatedTabGroupKey, key);
        }

        /// <summary>
        /// Begins drawing a toggle group. Remember to end with <see cref="EndToggleGroup"/>.
        /// </summary>
        /// <param name="key">The key of the group.</param>
        /// <param name="enabled">Value indicating if the group is enabled.</param>
        /// <param name="visible">Value indicating if the group is visible.</param>
        /// <param name="title">The title of the group.</param>
        /// <param name="animationDuration">Duration of the animation.</param>
        /// <returns>Value indicating if the group is toggled.</returns>
        public static bool BeginToggleGroup(object key, ref bool enabled, ref bool visible, string title, float animationDuration = DEFAULT_FADE_GROUP_DURATION)
        {
            var rect = GUILayoutUtility.GetRect(16, SirenixGUIStyles.ToggleGroupTitleBg.fixedHeight, SirenixGUIStyles.ToggleGroupTitleBg);
            rect = EditorGUI.IndentedRect(rect);
            GUIHelper.IndentRect(ref rect);
            rect.xMin += 3;
            rect.xMax -= 3;
            GUI.Box(rect, title, SirenixGUIStyles.ToggleGroupTitleBg);
            if (Event.current.type == EventType.Repaint)
            {
                var toggleIconRect = rect;
                toggleIconRect.xMin = toggleIconRect.xMax - 20;
                toggleIconRect.size *= 0.8f;
                GUI.DrawTexture(toggleIconRect, visible ? EditorIcons.TriangleDown.Active : EditorIcons.TriangleLeft.Active);
            }

            var toggleRect = new Rect(rect.x + 4f, rect.y + 4f, 13f, 13f);
            var e = Event.current;

            if (Event.current.type == EventType.Repaint)
                SirenixGUIStyles.ToggleGroupCheckbox.Draw(toggleRect, false, false, enabled, false);

            if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
            {
                toggleRect.x -= 2;
                toggleRect.y -= 2;
                toggleRect.width += 4;
                toggleRect.height += 4;

                if (toggleRect.Contains(e.mousePosition))
                {
                    GUIHelper.RequestRepaint();
                    enabled = !enabled;
                    e.Use();
                }
                else if (rect.Contains(e.mousePosition))
                {
                    GUIHelper.RequestRepaint();
                    visible = !visible;
                }
            }

            var result = BeginFadeGroup(key, visible, animationDuration);
            GUILayout.BeginVertical(SirenixGUIStyles.None);
            GUIHelper.PushGUIEnabled(enabled);
            GUIHelper.PushHierarchyMode(false);
            EditorGUI.indentLevel++;
            return result;
        }

        /// <summary>
        /// Ends drawing a toggle group started by <see cref="BeginToggleGroup(object, ref bool, ref bool, string, float)"/>.
        /// </summary>
        public static void EndToggleGroup()
        {
            EditorGUI.indentLevel--;
            GUIHelper.PopHierarchyMode();
            GUIHelper.PopGUIEnabled();
            GUILayout.EndVertical();
            EndFadeGroup();
        }

        private static object drawColorPaletteColorPickerKey = new object();

        private static BeginAutoScrollBoxInfo currentBeginAutoScrollBoxInfo;

        private static int currentDrawingToolbarPaddingTop;

        /// <summary>
        /// Begins drawing a horizontal auto scroll box. Remember to end with <see cref="EndHorizontalAutoScrollBox"/>.
        /// </summary>
        /// <param name="key">The for the field.</param>
        /// <param name="options">The GUILayout options.</param>
        /// <returns>The rect used for the field.</returns>
        public static Rect BeginHorizontalAutoScrollBox(object key, params GUILayoutOption[] options)
        {
            if (currentBeginAutoScrollBoxInfo != null && currentBeginAutoScrollBoxInfo.IsActive == true)
            {
                Debug.LogError("EndAutoScrollBox must be called before beginning another.");
            }

            var config = GUIHelper.GetTemporaryNullableContext<BeginAutoScrollBoxInfo>(drawColorPaletteColorPickerKey, key);
            config.Value = currentBeginAutoScrollBoxInfo = config.Value ?? new BeginAutoScrollBoxInfo();
            config.Value.IsActive = true;
            config.Value.TmpOuterRect = EditorGUILayout.BeginVertical(options);

            if (Event.current.type == EventType.Repaint)
            {
                currentBeginAutoScrollBoxInfo.OuterRect = currentBeginAutoScrollBoxInfo.TmpOuterRect;
            }

            bool wasScrollWheel = Event.current.type == EventType.scrollWheel;
            if (wasScrollWheel) Event.current.type = EventType.ignore;
            GUILayout.BeginScrollView(config.Value.ScrollPosition, GUIStyle.none, GUIStyle.none, options);
            if (wasScrollWheel) Event.current.type = EventType.scrollWheel;

            currentBeginAutoScrollBoxInfo.InnerRect = EditorGUILayout.BeginHorizontal(options);
            return currentBeginAutoScrollBoxInfo.OuterRect;
        }

        /// <summary>
        /// Ends drawing a horizontal auto scroll box started by <see cref="BeginHorizontalAutoScrollBox(object, GUILayoutOption[])"/>.
        /// </summary>
        public static void EndHorizontalAutoScrollBox()
        {
            if (currentBeginAutoScrollBoxInfo == null || currentBeginAutoScrollBoxInfo.IsActive == false)
            {
                Debug.LogError("EndAutoScrollBox was called before BeginAutoScrollBox.");
            }
            EditorGUILayout.EndHorizontal();

            bool wasScrollWheel = Event.current.type == EventType.scrollWheel;
            if (wasScrollWheel) Event.current.type = EventType.ignore;
            GUILayout.EndScrollView();
            if (wasScrollWheel) Event.current.type = EventType.scrollWheel;

            EditorGUILayout.EndVertical();

            currentBeginAutoScrollBoxInfo.TmpOuterRect.x += 10;
            currentBeginAutoScrollBoxInfo.TmpOuterRect.width -= 20;
            currentBeginAutoScrollBoxInfo.TmpOuterRect.y -= 10;
            currentBeginAutoScrollBoxInfo.TmpOuterRect.height += 20;
            if (currentBeginAutoScrollBoxInfo.TmpOuterRect.Contains(Event.current.mousePosition))
            {
                float overflow = Mathf.Max(0, currentBeginAutoScrollBoxInfo.InnerRect.width - currentBeginAutoScrollBoxInfo.TmpOuterRect.width);
                float percentage = (Event.current.mousePosition.x - currentBeginAutoScrollBoxInfo.TmpOuterRect.x) / currentBeginAutoScrollBoxInfo.TmpOuterRect.width;
                currentBeginAutoScrollBoxInfo.ScrollPosition.x = percentage * overflow;
                GUIHelper.RequestRepaint();
            }
            currentBeginAutoScrollBoxInfo.IsActive = false;
        }

        /// <summary>
        /// Draws an enum dropdown that writes 'Mixed' when multiple different values are selected.
        /// </summary>
        /// <typeparam name="TEnum">The type of the enum.</typeparam>
        /// <param name="selectedValues">The selected values.</param>
        /// <param name="value">The value.</param>
        /// <returns></returns>
		[Obsolete("Use SirenixEditorFields.Dropdown instead.")]
        public static bool DropDownMixed<TEnum>(IEnumerable<TEnum> selectedValues, out TEnum value)
                   where TEnum : struct, IConvertible
        {
            if (typeof(TEnum).IsEnum == false)
            {
                throw new ArgumentException("T must be an enumerated type");
            }

            int selectedIndex = 0;

            string[] options;
            string[] nameEndings = null;

            if (selectedValues == null || selectedValues.Count() == 0)
            {
                value = default(TEnum);
                EditorGUILayout.Popup(selectedIndex, new string[] { "None" }, EditorStyles.popup);
                return false;
            }
            else if (selectedValues.Count() == 1 || selectedValues.GroupBy(x => x).Count() == 1)
            {
                options = Enum.GetNames(typeof(TEnum));
                selectedIndex = ((IList<string>)options).IndexOf(selectedValues.First().ToString());
            }
            else
            {
                var names = Enum.GetNames(typeof(TEnum));
                options = new string[names.Length + 1];
                nameEndings = new string[names.Length + 1];
                options[0] = "Mixed";
                for (int i = 0; i < names.Length; i++)
                {
                    int count = selectedValues.Count(x => x.ToString() == names[i]);
                    nameEndings[i + 1] = (count == 0 ? "" : " (" + count + ")");
                    options[i + 1] = names[i] + nameEndings[i + 1];
                }
            }

            if (selectedIndex != (selectedIndex = EditorGUILayout.Popup(selectedIndex, options, EditorStyles.popup)))
            {
                string selectedName = options[selectedIndex];
                if (nameEndings != null)
                {
                    string nameEnding = nameEndings[selectedIndex];
                    if (!(nameEnding != null && String.IsNullOrEmpty(nameEnding)) && selectedName.EndsWith(nameEnding, StringComparison.InvariantCulture))
                    {
                        selectedName = selectedName.Substring(0, selectedName.Length - nameEnding.Length);
                    }
                }
                value = (TEnum)Enum.Parse(typeof(TEnum), selectedName);
                return true;
            }
            value = default(TEnum);
            return false;
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="position">The rect to draw the enum mask field in.</param>
        /// <param name="selected">The current enum value.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(Rect position, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return SirenixEditorGUI.EnumMaskPopup(position, new GUIContent(), selected, popup);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="position">The rect to draw the enum mask field in.</param>
        /// <param name="label">The label for the field.</param>
        /// <param name="selected">The current enum value.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(Rect position, string label, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return SirenixEditorGUI.EnumMaskPopup(position, new GUIContent(label), selected, popup);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="position">The rect to draw the enum mask field in.</param>
        /// <param name="label">The label for the field.</param>
        /// <param name="selected">The current enum value.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(Rect position, GUIContent label, Enum selected)
        {
            GUIStyle popup = EditorStyles.popup;
            return SirenixEditorGUI.EnumMaskPopup(position, label, selected, popup);
        }

        /// <summary>
        /// Draws an enum mask field.
        /// </summary>
        /// <param name="position">The rect to draw the enum mask field in.</param>
        /// <param name="label">The label for the field.</param>
        /// <param name="selected">The current enum value.</param>
        /// <param name="style">The style for the field.</param>
        /// <returns>The current enum value.</returns>
		[Obsolete("Use SirenixEditorFields.EnumMaskDropdown instead.")]
        public static Enum EnumMaskPopup(Rect position, GUIContent label, Enum selected, GUIStyle style)
        {
            var type = selected.GetType();
            var controlID = GUIUtility.GetControlID(FocusType.Keyboard, position);

            if (style == null)
            {
                style = GUIStyle.none;
            }

            selected = GetCurrentMaskValue(controlID, type, selected);

            Rect buttonPosition = label != null ?
                EditorGUI.PrefixLabel(position, controlID, label, EditorStyles.label) :
                position;

            string display = selected.ToString();

            if (EditorGUI.showMixedValue)
            {
                display = "—";
            }
            else if (string.IsNullOrEmpty(display))
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
                GUI.FocusControl(null);
                GenericMenu menu = new GenericMenu();

                MaskMenu.CurrentMaskControlID = controlID;
                MaskMenu.MaskChanged = false;

                ulong selectedValue;

                try
                {
                    selectedValue = Convert.ToUInt64(selected, CultureInfo.InvariantCulture);
                }
                catch (OverflowException)
                {
                    unchecked
                    {
                        selectedValue = (ulong)Convert.ToInt64(selected, CultureInfo.InvariantCulture);
                    }
                }

                var names = Enum.GetNames(type).ToList();
                var values = Enum.GetValues(type).FilterCast<object>().Select(n =>
                {
                    try
                    {
                        return Convert.ToUInt64(n, CultureInfo.InvariantCulture);
                    }
                    catch (OverflowException)
                    {
                        unchecked
                        {
                            return (ulong)Convert.ToInt64(n, CultureInfo.InvariantCulture);
                        }
                    }
                }).ToList();
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
        /// Creates field with a dropdown for selecting values.
        /// </summary>
        /// <typeparam name="T">The type for selection.</typeparam>
        /// <param name="label">The label for the field.</param>
        /// <param name="selectedValues">Indices indicating currently selected values.</param>
        /// <param name="values">The values for selecting from.</param>
        /// <param name="selectSingleValue">If set to <c>true</c> only a single value can be selected.</param>
        /// <returns><c>true</c> when the selection has changed. Otherwise <c>false</c>.</returns>
		[Obsolete("Use SirenixEditorFields.Dropdown instead.")]
        public static bool Popup<T>(string label, IList<int> selectedValues, IList<T> values, bool selectSingleValue = true)
        {
            var guiLabel = GUIHelper.TempContent(label);
            var controlID = GUIUtility.GetControlID(guiLabel, FocusType.Keyboard);
            var rect = EditorGUILayout.GetControlRect();//  GUILayoutUtility.GetRect(1, EditorGUIUtility.singleLineHeight, GUILayoutOptions.ExpandWidth(true));

            var btnRect = EditorGUI.PrefixLabel(rect, controlID, guiLabel, EditorStyles.label);

            string display = null;

            for (int i = 0; i < selectedValues.Count; i++)
            {
                string name = values[selectedValues[i]].ToString();
                if (display == null)
                {
                    display = name;
                }
                else
                {
                    display = name + ", " + display;
                }
            }
            display = display ?? "None";

            if (GUI.Button(btnRect, display, EditorStyles.popup))
            {
                GUI.FocusControl(null);
                GenericMenu menu = new GenericMenu();
                for (int i = 0; i < values.Count; i++)
                {
                    int localI = i;
                    bool selected = selectedValues.Contains(i);
                    string numSelected = "";
                    if (selected)
                    {
                        int selectedCount = selectedValues.Count(x => x == i);
                        if (selectedCount > 1)
                        {
                            numSelected = " (" + selectedCount + ")";
                        }
                    }
                    menu.AddItem(new GUIContent(values[i] + numSelected), selected, () =>
                    {
                        PopupSelector.CurrentSelectingPopupControlID = controlID;
                        PopupSelector.SelectAction = () =>
                        {
                            if (selectSingleValue)
                            {
                                selectedValues.Clear();
                                selectedValues.Add(localI);
                            }
                            else
                            {
                                if (selected)
                                {
                                    for (int j = selectedValues.Count - 1; j >= 0; j--)
                                    {
                                        if (selectedValues[i] == localI)
                                        {
                                            selectedValues.RemoveAt(j);
                                        }
                                    }
                                }
                            }
                        };
                    });
                }
                menu.DropDown(btnRect);
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
                ulong value;

                try
                {
                    value = Convert.ToUInt64(selected, CultureInfo.InvariantCulture);
                }
                catch (OverflowException)
                {
                    unchecked
                    {
                        value = (ulong)Convert.ToInt64(selected, CultureInfo.InvariantCulture);
                    }
                }

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

        /// <summary>
        /// Creates a rect that can be grabbed and pulled to change a value up or down.
        /// </summary>
        /// <param name="rect">The grabbable rect.</param>
        /// <param name="id">The control ID for the sliding.</param>
        /// <param name="t">The current value.</param>
        /// <returns>
        /// The current value.
        /// </returns>
        public static float SlideRect(Rect rect, int id, float t)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SlideArrow);

            if (GUI.enabled && Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = id;
                EditorGUIUtility.SetWantsMouseJumping(1);
                Event.current.Use();

                if (float.IsInfinity(t) || float.IsNaN(t))
                {
                    slideRectSensitivity = 0f;
                }
                else
                {
                    slideRectSensitivity = Mathf.Max(1.0f, Mathf.Pow(Math.Abs(t), 0.5f)) * 0.03f;
                }
            }
            else if (GUIUtility.hotControl == id)
            {
                // Update T
                if (Event.current.type == EventType.MouseDrag)
                {
                    //t += Mathf.Sign(Event.current.delta.x) * slideRectSensitivity;
                    t += HandleUtility.niceMouseDelta * slideRectSensitivity;
                    t = (float)MathUtilities.RoundBasedOnMinimumDifference(t, slideRectSensitivity);

                    GUI.changed = true;
                    Event.current.Use();
                }

                // Release
                else if (Event.current.rawType == EventType.MouseUp)
                {
                    GUIUtility.hotControl = 0;
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    Event.current.Use();
                }
            }

            return t;
        }

        /// <summary>
        /// Creates a rect that can be grabbed and pulled
        /// </summary>
        /// <param name="rect">The grabbable rect.</param>
        /// <returns>
        /// The the mouse delta position.
        /// </returns>
        public static Vector2 SlideRect(Rect rect)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SlideArrow);

            var id = GUIUtility.GetControlID(FocusType.Passive);

            if (GUI.enabled && Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = id;
                EditorGUIUtility.SetWantsMouseJumping(1);
                Event.current.Use();
            }
            else if (GUIUtility.hotControl == id)
            {
                // Update T
                if (Event.current.type == EventType.MouseDrag)
                {
                    GUI.changed = true;
                    Event.current.Use();
                    return Event.current.delta;
                }

                // Release
                else if (Event.current.type == EventType.MouseUp)
                {
                    GUIUtility.hotControl = 0;
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    Event.current.Use();
                }
            }
            return Vector2.zero;
        }

        /// <summary>
        /// Creates a rect that can be grabbed and pulled
        /// </summary>
        /// <param name="position">The position.</param>
        /// <param name="rect">The grabbable rect.</param>
        /// <returns>
        /// The the mouse delta position.
        /// </returns>
        public static Vector2 SlideRect(Vector2 position, Rect rect)
        {
            EditorGUIUtility.AddCursorRect(rect, MouseCursor.SlideArrow);

            var id = GUIUtility.GetControlID(FocusType.Passive);

            if (GUI.enabled && Event.current.type == EventType.MouseDown && Event.current.button == 0 && rect.Contains(Event.current.mousePosition))
            {
                GUIUtility.hotControl = id;
                Event.current.Use();
                var p = Event.current.mousePosition - rect.position;
                return p;
            }
            else if (GUIUtility.hotControl == id)
            {
                if (Event.current.type == EventType.MouseDrag)
                {
                    GUI.changed = true;
                    Event.current.Use();
                    var p = Event.current.mousePosition - rect.position;
                    return p;
                }
                else if (Event.current.type == EventType.MouseUp)
                {
                    GUIUtility.hotControl = 0;
                    Event.current.Use();
                }
            }
            return position;
        }

        /// <summary>
        /// Draws a field for a value of type T - dynamically choosing an appropriate drawer for the type.
        /// Currently supported are: char, string, sbyte, byte, short, ushort, int, uint, long, ulong, float, double and decimal.
        /// </summary>
        /// <typeparam name="T">The type of the value to draw.</typeparam>
        /// <param name="label">The label of the fields.</param>
        /// <param name="value">The value to draw.</param>
        /// <param name="options">The layout options.</param>
        /// <returns>The possibly changed value.</returns>
        public static T DynamicPrimitiveField<T>(GUIContent label, T value, params GUILayoutOption[] options)
        {
            options = options ?? GUILayoutOptions.EmptyGUIOptions;

            Delegate del;

            if (!DynamicFieldDrawers.TryGetValue(typeof(T), out del))
            {
                EditorGUILayout.LabelField(label, new GUIContent("DynamicPrimitiveField does not support drawing the type '" + typeof(T).GetNiceName() + "'."));
                return value;
            }
            else
            {
                var drawerFunc = (Func<GUIContent, T, GUILayoutOption[], T>)del;
                return drawerFunc(label, value, options);
            }
        }

        /// <summary>
        /// Checks whether a given type can be drawn as a dynamic field by <see cref="DynamicPrimitiveField{T}(GUIContent, T, GUILayoutOption[])"/>
        /// </summary>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns>True if the type can be drawn, otherwise false.</returns>
        public static bool DynamicPrimitiveFieldCanDraw<T>()
        {
            return DynamicFieldDrawers.ContainsKey(typeof(T));
        }

        private static readonly Dictionary<Type, Delegate> DynamicFieldDrawers = new Dictionary<Type, Delegate>()
        {
            { typeof(char),     (Func<GUIContent, char, GUILayoutOption[], char>)       ((label, value, options) => {
                string str = value.ToString();
                str = SirenixEditorFields.TextField(label, str, options);
                if (str.Length == 0) return default(char);
                else return str[0];
            }) },
            { typeof(string),   (Func<GUIContent, string, GUILayoutOption[], string>)   ((label, value, options) => SirenixEditorFields.TextField(label, value, options)) },
            { typeof(sbyte),    (Func<GUIContent, sbyte, GUILayoutOption[], sbyte>)     ((label, value, options) => (sbyte)Mathf.Clamp(SirenixEditorFields.IntField(label, value, options), sbyte.MinValue, sbyte.MaxValue)) },
            { typeof(byte),     (Func<GUIContent, byte, GUILayoutOption[], byte>)       ((label, value, options) => (byte)Mathf.Clamp(SirenixEditorFields.IntField(label, value, options), byte.MinValue, byte.MaxValue)) },
            { typeof(short),    (Func<GUIContent, short, GUILayoutOption[], short>)     ((label, value, options) => (short)Mathf.Clamp(SirenixEditorFields.IntField(label, value, options), short.MinValue, short.MaxValue)) },
            { typeof(ushort),   (Func<GUIContent, ushort, GUILayoutOption[], ushort>)   ((label, value, options) => (ushort)Mathf.Clamp(SirenixEditorFields.IntField(label, value, options), ushort.MinValue, ushort.MaxValue)) },
            { typeof(int),      (Func<GUIContent, int, GUILayoutOption[], int>)         ((label, value, options) => SirenixEditorFields.IntField(label, value, options)) },
            { typeof(uint),     (Func<GUIContent, uint, GUILayoutOption[], uint>)       ((label, value, options) => (uint)Mathf.Clamp(EditorGUILayout.LongField(label, value, options), uint.MinValue, uint.MaxValue)) },
            { typeof(long),     (Func<GUIContent, long, GUILayoutOption[], long>)       ((label, value, options) => EditorGUILayout.LongField(label, value, options)) },
            { typeof(ulong),    (Func<GUIContent, ulong, GUILayoutOption[], ulong>)     ((label, value, options) => {
                string str = value.ToString(CultureInfo.InvariantCulture);
                str = EditorGUILayout.DelayedTextField(label, str, EditorStyles.textField);

                ulong newValue;

                if (GUI.changed && ulong.TryParse(str, NumberStyles.Any, null, out newValue))
                {
                    value = newValue;
                }

                return value;
            }) },
            { typeof(float),    (Func<GUIContent, float, GUILayoutOption[], float>)     ((label, value, options) => SirenixEditorFields.FloatField(label, value, options)) },
            { typeof(double),   (Func<GUIContent, double, GUILayoutOption[], double>)   ((label, value, options) => EditorGUILayout.DoubleField(label, value, options)) },
            { typeof(decimal),  (Func<GUIContent, decimal, GUILayoutOption[], decimal>) ((label, value, options) => {
                string str = value.ToString(CultureInfo.InvariantCulture);
                str = EditorGUILayout.DelayedTextField(label, str, EditorStyles.textField);
                decimal newValue;

                if (GUI.changed && decimal.TryParse(str, NumberStyles.Any, null, out newValue))
                {
                    value = newValue;
                }

                return value;
            }) },
        };

        private class BeginAutoScrollBoxInfo
        {
            public Rect TmpOuterRect;
            public Rect InnerRect;
            public bool IsActive;
            public Rect OuterRect;
            public Vector2 ScrollPosition;
        }

        private static class PopupSelector
        {
            public static int CurrentSelectingPopupControlID;
            public static Action SelectAction;
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