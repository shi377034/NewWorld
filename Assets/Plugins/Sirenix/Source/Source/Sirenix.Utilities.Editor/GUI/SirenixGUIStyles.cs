#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="SirenixGUIStyles.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Collection of GUIStyles used by Sirenix.
    /// </summary>
    [InitializeOnLoad]
    public static class SirenixGUIStyles
    {
        /// <summary>
        /// Border color.
        /// </summary>
        public static readonly Color BorderColor = EditorGUIUtility.isProSkin ? new Color(0.11f, 0.11f, 0.11f, 1f) : new Color(0.38f, 0.38f, 0.38f, 1f);

        /// <summary>
        /// Box background color.
        /// </summary>
        public static readonly Color BoxBackgroundColor = new Color(1, 1, 1, 0.05f);

        /// <summary>
        /// Dark editor background color.
        /// </summary>
        public static readonly Color DarkEditorBackground = EditorGUIUtility.isProSkin ? new Color(0.192f, 0.192f, 0.192f, 1f) : new Color(0, 0, 0, 0);

        /// <summary>
        /// Editor window background color.
        /// </summary>
        public static readonly Color EditorWindowBackgroundColor = EditorGUIUtility.isProSkin ? new Color(0.22f, 0.22f, 0.22f, 1f) : new Color(0.76f, 0.76f, 0.76f, 1f);

        /// <summary>
        /// Header box background color.
        /// </summary>
        public static readonly Color HeaderBoxBackgroundColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.06f) : new Color(1, 1, 1, 0.26f);

        /// <summary>
        /// Highlight text color.
        /// </summary>
        public static readonly Color HighlightedTextColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 1) : new Color(0, 0, 0, 1);

        /// <summary>
        /// Highlight property color.
        /// </summary>
        public static readonly Color HighlightPropertyColor = EditorGUIUtility.isProSkin ? new Color(1, 1, 1, 0.6f) : new Color(0, 0, 0, 0.6f);

        /// <summary>
        /// List item color for every other item.
        /// </summary>
        public static readonly Color ListItemColorEven = EditorGUIUtility.isProSkin ? new Color(0.247f * 0.8f, 0.247f * 0.8f, 0.247f * 0.8f, 1f) : new Color(0.838f, 0.838f, 0.838f, 1.000f);

        /// <summary>
        /// List item hover color for every other item.
        /// </summary>
        public static readonly Color ListItemColorHoverEven = EditorGUIUtility.isProSkin ? new Color(0.279f * 0.8f, 0.279f * 0.8f, 0.279f * 0.8f, 1f) : new Color(0.890f, 0.890f, 0.890f, 1.000f);

        /// <summary>
        /// List item hover color for every other item.
        /// </summary>
        public static readonly Color ListItemColorHoverOdd = EditorGUIUtility.isProSkin ? new Color(0.309f * 0.8f, 0.309f * 0.8f, 0.309f * 0.8f, 1f) : new Color(0.904f, 0.904f, 0.904f, 1.000f);

        /// <summary>
        /// List item color for every other item.
        /// </summary>
        public static readonly Color ListItemColorOdd = EditorGUIUtility.isProSkin ? new Color(0.272f * 0.8f, 0.272f * 0.8f, 0.272f * 0.8f, 1f) : new Color(0.801f, 0.801f, 0.801f, 1.000f);

        /// <summary>
        /// List item drag background color.
        /// </summary>
        public static readonly Color ListItemDragBg = new Color(0.1f, 0.1f, 0.1f, 1f);

        /// <summary>
        /// List item drag background color.
        /// </summary>
        public static readonly Color ListItemDragBgColor = EditorGUIUtility.isProSkin ? new Color(0.1f, 0.1f, 0.1f, 1f) : new Color(0.338f, 0.338f, 0.338f, 1.000f);

        /// <summary>
        /// List item background color for every other item.
        /// </summary>
        public static readonly Color ListItemEven = new Color(0.4f, 0.4f, 0.4f, 1f);

        /// <summary>
        /// List item background color for every other item.
        /// </summary>
        public static readonly Color ListItemOdd = new Color(0.4f, 0.4f, 0.4f, 1f);

        /// <summary>
        /// Menu button active background color.
        /// </summary>
        public static readonly Color MenuButtonActiveBgColor = new Color(0.243f, 0.490f, 0.906f, 1.000f);

        /// <summary>
        /// Menu button border color.
        /// </summary>
        public static readonly Color MenuButtonBorderColor = new Color( EditorWindowBackgroundColor.r * 0.8f, EditorWindowBackgroundColor.g * 0.8f, EditorWindowBackgroundColor.b * 0.8f );

        /// <summary>
        /// Menu button color.
        /// </summary>
        public static readonly Color MenuButtonColor = new Color(0, 0, 0, 0);

        /// <summary>
        /// Menu button hover color.
        /// </summary>
        public static readonly Color MenuButtonHoverColor = new Color(1, 1, 1, 0.08f);

        private static GUIStyle boldLabel;
        private static GUIStyle boldLabelCentered;
        private static GUIStyle boxContainer;
        private static GUIStyle boxHeaderStyle;
        private static GUIStyle buttonLeft;
        private static GUIStyle buttonLeftSelected;
        private static GUIStyle buttonMid;
        private static GUIStyle buttonMidSelected;
        private static GUIStyle buttonRight;
        private static GUIStyle buttonRightSelected;
        private static GUIStyle colorFieldBackground;
        private static GUIStyle foldout;
        private static GUIStyle iconButton;
        private static GUIStyle label;
        private static GUIStyle labelCentered;
        private static GUIStyle leftAlignedGreyMiniLabel;
        private static GUIStyle leftRightAlignedWhiteMiniLabel;
        private static GUIStyle listItem;
        private static GUIStyle menuButtonBackground;
        private static GUIStyle none;
        private static GUIStyle paddingLessBox;
        private static GUIStyle propertyMessagePaddingTest;
        private static GUIStyle propertyPadding;
        private static GUIStyle richTextLabel;
        private static GUIStyle rightAlignedGreyMiniLabel;
        private static GUIStyle rightAlignedWhiteMiniLabel;
        private static GUIStyle sectionHeader;
        private static GUIStyle toggleGroupBackground;
        private static GUIStyle toggleGroupCheckbox;
        private static GUIStyle toggleGroupPadding;
        private static GUIStyle toggleGroupTitleBg;
        private static GUIStyle toolbarBackground;
        private static GUIStyle toolbarButton;
        private static GUIStyle toolbarButtonSelected;
        private static GUIStyle toolbarSeachCancelButton;
        private static GUIStyle toolbarSeachTextField;
        private static GUIStyle toolbarTab;

        /// <summary>
        /// Bold label style.
        /// </summary>
        public static GUIStyle BoldLabel
        {
            get
            {
                if (boldLabel == null)
                {
                    boldLabel = new GUIStyle(EditorStyles.boldLabel) { contentOffset = new Vector2(0, 0), margin = new RectOffset(0, 0, 0, 0) };
                }
                return boldLabel;
            }
        }

        /// <summary>
        /// Centered bold label style.
        /// </summary>
        public static GUIStyle BoldLabelCentered
        {
            get
            {
                if (boldLabelCentered == null)
                {
                    boldLabelCentered = new GUIStyle(BoldLabel) { alignment = TextAnchor.MiddleCenter };
                }
                return boldLabelCentered;
            }
        }

        /// <summary>
        /// Box container style.
        /// </summary>
        public static GUIStyle BoxContainer
        {
            get
            {
                if (boxContainer == null)
                {
                    boxContainer = new GUIStyle(EditorStyles.helpBox) { margin = new RectOffset(0, 0, 3, 3) };
                }
                return boxContainer;
            }
        }

        /// <summary>
        /// Box header style.
        /// </summary>
        public static GUIStyle BoxHeaderStyle
        {
            get
            {
                if (boxHeaderStyle == null)
                {
                    boxHeaderStyle = new GUIStyle(None) { margin = new RectOffset(0, 0, 0, 5) };
                }
                return boxHeaderStyle;
            }
        }

        /// <summary>
        /// Left button style.
        /// </summary>
        public static GUIStyle ButtonLeft
        {
            get
            {
                if (buttonLeft == null)
                {
                    buttonLeft = new GUIStyle("ButtonLeft");
                }
                return buttonLeft;
            }
        }

        /// <summary>
        /// Left button selected style.
        /// </summary>
        public static GUIStyle ButtonLeftSelected
        {
            get
            {
                if (buttonLeftSelected == null)
                {
                    buttonLeftSelected = new GUIStyle("ButtonLeft") { onNormal = new GUIStyle("ButtonLeft").active, normal = new GUIStyle("ButtonLeft").active };
                }
                return buttonLeftSelected;
            }
        }

        /// <summary>
        /// Mid button style.
        /// </summary>
        public static GUIStyle ButtonMid
        {
            get
            {
                if (buttonMid == null)
                {
                    buttonMid = new GUIStyle("ButtonMid");
                }
                return buttonMid;
            }
        }

        /// <summary>
        /// Mid button selected style.
        /// </summary>
        public static GUIStyle ButtonMidSelected
        {
            get
            {
                if (buttonMidSelected == null)
                {
                    buttonMidSelected = new GUIStyle("ButtonMid") { onNormal = new GUIStyle("ButtonMid").active, normal = new GUIStyle("ButtonMid").active };
                }
                return buttonMidSelected;
            }
        }

        /// <summary>
        /// Right button style.
        /// </summary>
        public static GUIStyle ButtonRight
        {
            get
            {
                if (buttonRight == null)
                {
                    buttonRight = new GUIStyle("ButtonRight");
                }
                return buttonRight;
            }
        }

        /// <summary>
        /// Right button selected style.
        /// </summary>
        public static GUIStyle ButtonRightSelected
        {
            get
            {
                if (buttonRightSelected == null)
                {
                    buttonRightSelected = new GUIStyle("ButtonRight") { onNormal = new GUIStyle("ButtonRight").active, normal = new GUIStyle("ButtonRight").active };
                }
                return buttonRightSelected;
            }
        }

        /// <summary>
        /// Color field background style.
        /// </summary>
        public static GUIStyle ColorFieldBackground
        {
            get
            {
                if (colorFieldBackground == null)
                {
                    colorFieldBackground = new GUIStyle("ShurikenEffectBg");
                }
                return colorFieldBackground;
            }
        }

        /// <summary>
        /// Foldout style.
        /// </summary>
        public static GUIStyle Foldout
        {
            get
            {
                if (foldout == null)
                {
                    foldout = new GUIStyle(EditorStyles.foldout) { fixedWidth = 0, fixedHeight = 16, stretchHeight = false, stretchWidth = true };
                }
                return foldout;
            }
        }

        /// <summary>
        /// Icon button style.
        /// </summary>
        public static GUIStyle IconButton
        {
            get
            {
                if (iconButton == null)
                {
                    iconButton = new GUIStyle(GUIStyle.none) { padding = new RectOffset(1, 1, 1, 1), };
                }
                return iconButton;
            }
        }

        /// <summary>
        /// Label style.
        /// </summary>
        public static GUIStyle Label
        {
            get
            {
                if (label == null)
                {
                    label = new GUIStyle(EditorStyles.label) { margin = new RectOffset(0, 0, 0, 0) };
                }
                return label;
            }
        }

        /// <summary>
        /// Centered label style.
        /// </summary>
        public static GUIStyle LabelCentered
        {
            get
            {
                if (labelCentered == null)
                {
                    labelCentered = new GUIStyle(Label) { alignment = TextAnchor.MiddleCenter, margin = new RectOffset(0, 0, 0, 0) };
                }
                return labelCentered;
            }
        }

        /// <summary>
        /// Left aligned grey mini label style.
        /// </summary>
        public static GUIStyle LeftAlignedGreyMiniLabel
        {
            get
            {
                if (leftAlignedGreyMiniLabel == null)
                {
                    leftAlignedGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Overflow, };
                }
                return leftAlignedGreyMiniLabel;
            }
        }

        /// <summary>
        /// Left right aligned white mini label style.
        /// </summary>
        public static GUIStyle LeftRightAlignedWhiteMiniLabel
        {
            get
            {
                if (leftRightAlignedWhiteMiniLabel == null)
                {
                    leftRightAlignedWhiteMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleLeft, clipping = TextClipping.Overflow, normal = new GUIStyleState() { textColor = Color.white } };
                }
                return leftRightAlignedWhiteMiniLabel;
            }
        }

        /// <summary>
        /// List item style.
        /// </summary>
        public static GUIStyle ListItem
        {
            get
            {
                if (listItem == null)
                {
                    listItem = new GUIStyle(None) { padding = new RectOffset(0, 0, 3, 3) };
                }
                return listItem;
            }
        }

        /// <summary>
        /// Menu button background style.
        /// </summary>
        public static GUIStyle MenuButtonBackground
        {
            get
            {
                if (menuButtonBackground == null)
                {
                    menuButtonBackground = new GUIStyle() { margin = new RectOffset(0, 1, 0, 0), padding = new RectOffset(0, 0, 4, 4), border = new RectOffset(0, 0, 0, 0) };
                }
                return menuButtonBackground;
            }
        }

        /// <summary>
        /// No style.
        /// </summary>
        public static GUIStyle None
        {
            get
            {
                if (none == null)
                {
                    none = new GUIStyle() { margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0), border = new RectOffset(0, 0, 0, 0) };
                }
                return none;
            }
        }

        /// <summary>
        /// Padding less box style.
        /// </summary>
        public static GUIStyle PaddingLessBox
        {
            get
            {
                if (paddingLessBox == null)
                {
                    paddingLessBox = new GUIStyle("box") { padding = new RectOffset(1, 1, 0, 0) };
                }
                return paddingLessBox;
            }
        }

        /// <summary>
        /// Property padding
        /// </summary>
        public static GUIStyle PropertyMessagePaddingTest
        {
            get
            {
                if (propertyMessagePaddingTest == null)
                {
                    propertyMessagePaddingTest = new GUIStyle() { padding = new RectOffset(3, 3, 0, 0) };
                }
                return propertyMessagePaddingTest;
            }
        }

        /// <summary>
        /// Property padding
        /// </summary>
        public static GUIStyle PropertyPadding
        {
            get
            {
                if (propertyPadding == null)
                {
                    propertyPadding = new GUIStyle(GUIStyle.none) { padding = new RectOffset(3, 3, 0, 3) };
                }
                return propertyPadding;
            }
        }

        /// <summary>
        /// Rich text label style.
        /// </summary>
        public static GUIStyle RichTextLabel
        {
            get
            {
                if (richTextLabel == null)
                {
                    richTextLabel = new GUIStyle(EditorStyles.label) { richText = true, };
                }
                return richTextLabel;
            }
        }

        /// <summary>
        /// Right aligned grey mini label style.
        /// </summary>
        public static GUIStyle RightAlignedGreyMiniLabel
        {
            get
            {
                if (rightAlignedGreyMiniLabel == null)
                {
                    rightAlignedGreyMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleRight, clipping = TextClipping.Overflow, };
                }
                return rightAlignedGreyMiniLabel;
            }
        }

        /// <summary>
        /// Right aligned white mini label style.
        /// </summary>
        public static GUIStyle RightAlignedWhiteMiniLabel
        {
            get
            {
                if (rightAlignedWhiteMiniLabel == null)
                {
                    rightAlignedWhiteMiniLabel = new GUIStyle(EditorStyles.centeredGreyMiniLabel) { alignment = TextAnchor.MiddleRight, clipping = TextClipping.Overflow, normal = new GUIStyleState() { textColor = Color.white } };
                }
                return rightAlignedWhiteMiniLabel;
            }
        }

        /// <summary>
        /// Section header style.
        /// </summary>
        public static GUIStyle SectionHeader
        {
            get
            {
                if (sectionHeader == null)
                {
                    sectionHeader = new GUIStyle(EditorStyles.largeLabel) { fontSize = 18, margin = new RectOffset(0, 0, 10, 0), fontStyle = FontStyle.Bold, font = EditorStyles.largeLabel.font, overflow = new RectOffset(0, 0, 0, 0), };
                }
                return sectionHeader;
            }
        }

        /// <summary>
        /// Toggle group background style.
        /// </summary>
        public static GUIStyle ToggleGroupBackground
        {
            get
            {
                if (toggleGroupBackground == null)
                {
                    toggleGroupBackground = new GUIStyle(EditorStyles.helpBox) { overflow = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0), padding = new RectOffset(0, 0, 0, 0) };
                }
                return toggleGroupBackground;
            }
        }

        /// <summary>
        /// Toggle group checkbox style.
        /// </summary>
        public static GUIStyle ToggleGroupCheckbox
        {
            get
            {
                if (toggleGroupCheckbox == null)
                {
                    toggleGroupCheckbox = new GUIStyle("ShurikenCheckMark");
                }
                return toggleGroupCheckbox;
            }
        }

        /// <summary>
        /// Toggle group padding style.
        /// </summary>
        public static GUIStyle ToggleGroupPadding
        {
            get
            {
                if (toggleGroupPadding == null)
                {
                    toggleGroupPadding = new GUIStyle(GUIStyle.none) { padding = new RectOffset(5, 5, 5, 5) };
                }
                return toggleGroupPadding;
            }
        }

        /// <summary>
        /// Toggle group title background style.
        /// </summary>
        public static GUIStyle ToggleGroupTitleBg
        {
            get
            {
                if (toggleGroupTitleBg == null)
                {
                    toggleGroupTitleBg = new GUIStyle("ShurikenModuleTitle") { font = (new GUIStyle("Label")).font, border = new RectOffset(15, 7, 4, 4), fixedHeight = 22, contentOffset = new Vector2(20f, -2f), margin = new RectOffset(0, 0, 3, 3) };
                }
                return toggleGroupTitleBg;
            }
        }

        /// <summary>
        /// Toolbar background style.
        /// </summary>
        public static GUIStyle ToolbarBackground
        {
            get
            {
                if (toolbarBackground == null)
                {
                    toolbarBackground = new GUIStyle("OL title") { fixedHeight = 0, fixedWidth = 0, stretchHeight = true, stretchWidth = true, padding = new RectOffset(0, 0, 0, 0), margin = new RectOffset(0, 0, 0, 0), overflow = new RectOffset(0, 0, 0, 0), };
                }
                return toolbarBackground;
            }
        }

        /// <summary>
        /// Toolbar button style.
        /// </summary>
        public static GUIStyle ToolbarButton
        {
            get
            {
                if (toolbarButton == null)
                {
                    toolbarButton = new GUIStyle("OL Title TextRight") { stretchHeight = true, stretchWidth = false, fixedHeight = 0f, alignment = TextAnchor.MiddleCenter, font = EditorStyles.toolbarButton.font, fontSize = EditorStyles.toolbarButton.fontSize, fontStyle = EditorStyles.toolbarButton.fontStyle, overflow = new RectOffset(1, 0, 0, 0), };
                }
                return toolbarButton;
            }
        }

        /// <summary>
        /// Toolbar button selected style.
        /// </summary>
        public static GUIStyle ToolbarButtonSelected
        {
            get
            {
                if (toolbarButtonSelected == null)
                {
                    toolbarButtonSelected = new GUIStyle("OL Title TextRight") { stretchHeight = true, stretchWidth = false, fixedHeight = 0f, alignment = TextAnchor.MiddleCenter, overflow = new RectOffset(1, 0, 0, 0), font = EditorStyles.toolbarButton.font, fontSize = EditorStyles.toolbarButton.fontSize, fontStyle = EditorStyles.toolbarButton.fontStyle, onNormal = new GUIStyle("OL Title TextRight").active, normal = new GUIStyle("OL Title TextRight").active };
                }

                return toolbarButtonSelected;
            }
        }

        /// <summary>
        /// Toolbar search cancel button style.
        /// </summary>
        public static GUIStyle ToolbarSeachCancelButton
        {
            get
            {
                if (toolbarSeachCancelButton == null)
                {
                    toolbarSeachCancelButton = GUI.skin.FindStyle("ToolbarSeachCancelButton");
                }
                return toolbarSeachCancelButton;
            }
        }

        /// <summary>
        /// Toolbar search field style.
        /// </summary>
        public static GUIStyle ToolbarSeachTextField
        {
            get
            {
                if (toolbarSeachTextField == null)
                {
                    toolbarSeachTextField = GUI.skin.FindStyle("ToolbarSeachTextField");
                }
                return toolbarSeachTextField;
            }
        }

        /// <summary>
        /// Toolbar tab style.
        /// </summary>
        public static GUIStyle ToolbarTab
        {
            get
            {
                if (toolbarTab == null)
                {
                    toolbarTab = new GUIStyle("OL Title TextRight") { stretchHeight = true, stretchWidth = true, fixedHeight = 0f, alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Normal, overflow = new RectOffset(1, 0, 0, 0), font = GUI.skin.font, };
                }
                return toolbarTab;
            }
        }
    }
}
#endif