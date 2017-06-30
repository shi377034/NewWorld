#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EditorIcon.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities.Editor
{
    using UnityEngine;

    /// <summary>
    /// Icon for using in editor GUI.
    /// </summary>
    public abstract class EditorIcon
    {
        private GUIContent inactiveGUIContent;
        private GUIContent highlightedGUIContent;
        private GUIContent activeGUIContent;

        /// <summary>
        /// Gets the icon's highlighted texture.
        /// </summary>
        public abstract Texture Highlighted { get; }

        /// <summary>
        /// Gets the icon's active texture.
        /// </summary>
        public abstract Texture Active { get; }

        /// <summary>
        /// Gets the icon's inactive texture.
        /// </summary>
        public abstract Texture Inactive { get; }

        /// <summary>
        /// Gets a GUIContent object with the active texture.
        /// </summary>
        public GUIContent ActiveGUIContent
        {
            get
            {
                if (this.activeGUIContent == null || this.activeGUIContent.image == null)
                {
                    this.activeGUIContent = new GUIContent(this.Inactive);
                }
                return this.activeGUIContent;
            }
        }

        /// <summary>
        /// Gets a GUIContent object with the inactive texture.
        /// </summary>
        public GUIContent InactiveGUIContent
        {
            get
            {
                if (this.inactiveGUIContent == null || this.inactiveGUIContent.image == null)
                {
                    this.inactiveGUIContent = new GUIContent(this.Inactive);
                }
                return this.inactiveGUIContent;
            }
        }

        /// <summary>
        /// Gets a GUIContent object with the highlighted texture.
        /// </summary>
        public GUIContent HighlightedGUIContent
        {
            get
            {
                if (this.highlightedGUIContent == null || this.highlightedGUIContent.image == null)
                {
                    this.highlightedGUIContent = new GUIContent(this.Inactive);
                }
                return this.highlightedGUIContent;
            }
        }
    }
}
#endif