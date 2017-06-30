#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="GeneralDrawerConfig.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Sirenix.Utilities;
    using Sirenix.Utilities.Editor;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// <para>Contains general configuration for all Odin drawers.</para>
    /// <para>
    /// You can modify the configuration in the Odin Preferences window found in 'Window -> Odin Preferences -> Drawers -> General',
    /// or by locating the configuration file stored as a serialized object in the Sirenix folder under 'Odin Inspector/Config/Editor/GeneralDrawerConfig'.
    /// </para>
    /// </summary>
    [SirenixEditorConfig("Odin Inspector/Drawers/General")]
    public class GeneralDrawerConfig : GlobalConfig<GeneralDrawerConfig>
    {
        /// <summary>
        /// Specify weather or not the script selector above components should be drawn.
        /// </summary>
        [Title("General")]
        [Indent]
        [Tooltip("Specify weather or not the script selector above components should be drawn")]
        [PropertyOrder(-2)]
        public bool ShowMonoScriptInEditor = true;

        /// <summary>
        /// If set to true, most foldouts throughout the inspector will be expanded by default.
        /// </summary>
        [Title("Foldout")]
        [Indent]
        [Tooltip("If set to true, most foldouts throughout the inspector will be expanded by default.")]
        public bool ExpandFoldoutByDefault = true;

        /// <summary>
        /// Specify the animation speed for most foldouts throughout the inspector.
        /// </summary>
        [Title("Animations")]
        [Indent]
        [Range(0.001f, 4f)]
        [Tooltip("Specify the animation speed for most foldouts throughout the inspector.")]
        public float GUIFoldoutAnimationDuration = 0.2f;

        /// <summary>
        /// Specify the animation speed for <see cref="Sirenix.OdinInspector.TabGroupAttribute"/>
        /// </summary>
        [Indent]
        [Range(0.001f, 4f)]
        public float TabPageSlideAnimationDuration = 0.2f;

		/// <summary>
		/// Specify the shaking duration for most shaking animations throughout the inspector.
		/// </summary>
		[Indent]
		[Tooltip("Specify the shaking duration for most shaking animations throughout the inspector.")]
		[Range(0f, 4f)]
		public float ShakingAnimationDuration = 0.5f;

        /// <summary>
        /// Specify how the Quaternion struct should be shown in the inspector.
        /// </summary>
        [Title("Quaternions")]
        [Indent]
        [PropertyOrder(-1)]
        [ShowInInspector, PropertyTooltip("Current mode for how quaternions are edited in the inspector.\n\nEuler: Rotations as yaw, pitch and roll.\n\nAngle axis: Rotations as a axis of rotation, and an angle of rotation around that axis.\n\nRaw: Directly edit the x, y, z and w components of a quaternion.")]
        public QuaternionDrawMode QuaternionDrawMode
        {
            get { return (QuaternionDrawMode)EditorPrefs.GetInt("GeneralDrawerConfig.QuaternionDrawMode", (int)QuaternionDrawMode.Eulers); }
            set { EditorPrefs.SetInt("GeneralDrawerConfig.QuaternionDrawMode", (int)value); }
        }

        /// <summary>
        /// Specify weather or not a list should hide the foldout triangle when the list is empty.
        /// </summary>
        [Title("Lists")]
        [Indent]
        [Tooltip("Specifies weather or not a list should hide the foldout triangle when the list is empty.")]
        public bool HideFoldoutWhileEmpty = true;

        /// <summary>
        /// Specify weather or not lists should be expanded or collapsed by default.
        /// </summary>
        [Indent]
        [Tooltip("Specify weather or not lists should be expanded or collapsed by default.")]
        public bool OpenListsByDefault = true;

        /// <summary>
        /// Specify weather or not lists should show item count.
        /// </summary>
        [Indent]
        [Tooltip("Specify weather or not lists should show item count.")]
        public bool ShowItemCount = true;

        /// <summary>
        /// Specify the number of elements drawn per page.
        /// </summary>
        [OnValueChanged("ResizeExampleList"), MaxValue(500), MinValue(2)]
        [Indent]
        [Tooltip("Specify the number of elements drawn per page.")]
        public int NumberOfItemsPrPage = 15;

        /// <summary>
        /// Specify weather or not lists should hide the paging buttons when the list is collapsed.
        /// </summary>
        [Indent]
        [Tooltip("Specify weather or not lists should hide the paging buttons when the list is collapsed.")]
        public bool HidePagingWhileCollapsed = true;

        /// <summary>
        /// Specify weather or not lists should hide the paging buttons when there is only one page.
        /// </summary>
        [Indent]
        public bool HidePagingWhileOnlyOnePage = true;

        /// <summary>
        /// Specify weather or not to include a button which expands the list, showing all pages at once.
        /// </summary>
        [Indent]
        [Tooltip("Specify weather or not to include a button which expands the list, showing all pages at once")]
        public bool ShowExpandButton = true;

#pragma warning disable 0414

        [Indent]
        [NonSerialized, ShowInInspector, PropertyOrder(2)]
        private List<int> exampleList = new List<int>();

#pragma warning restore 0414

        private void ResizeExampleList()
        {
            this.exampleList = Enumerable.Range(0, Math.Max(10, (int)(this.NumberOfItemsPrPage * Mathf.PI))).ToList();
        }
    }
}
#endif