//-----------------------------------------------------------------------
// <copyright file="ListDrawerSettingsAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;
    using UnityEngine;

    /// <summary>
    /// Customize the behavior for lists and arrays in the inspector.
    /// </summary>
    /// <remarks>
    /// This attribute is scheduled for refactoring.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ListDrawerSettingsAttribute : Attribute
    {
        private string onTitleBarGUI = null;

        private bool paging;
        private bool draggable;
        private bool isReadOnly;
        private bool showItemCount;

        private bool pagingHasValue = false;
        private bool draggableHasValue = false;
        private bool isReadOnlyHasValue = false;
        private bool showItemCountHasValue = false;
        private bool expanded;
        private bool expandedHasValue;

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell weather paging should be enabled or not.
        /// </summary>
        public bool ShowPaging
        {
            get { return this.paging; }
            set
            {
                this.paging = value;
                this.pagingHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell weather items should be draggable or not.
        /// </summary>
        public bool DraggableItems
        {
            get { return this.draggable; }
            set
            {
                this.draggable = value;
                this.draggableHasValue = true;
            }
        }

        /// <summary>
        /// Mark a list as read-only. This removes all editing capabilities from the list such as Add, Drag and delete,
        /// but without disabling GUI for each element drawn as other wise would be the case if the <see cref="ReadOnlyAttribute"/> was used.
        /// </summary>
        public bool IsReadOnly
        {
            get { return this.isReadOnly; }
            set
            {
                this.isReadOnly = value;
                this.isReadOnlyHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell weather or not item count should be shown.
        /// </summary>
        public bool ShowItemCount
        {
            get { return this.showItemCount; }
            set
            {
                this.showItemCount = value;
                this.showItemCountHasValue = true;
            }
        }

        /// <summary>
        /// Override the default setting specified in the Advanced Odin Preferences window and explicitly tell weather or not the list should be expanded or collapsed by default.
        /// </summary>
        public bool Expanded
        {
            get { return this.expanded; }
            set
            {
                this.expanded = value;
                this.expandedHasValue = true;
            }
        }

        /// <summary>
        /// Use this to inject custom GUI into the title-bar of the list.
        /// </summary>
        public string OnTitleBarGUI
        {
            get { return this.onTitleBarGUI; }
            set { this.onTitleBarGUI = value; }
        }

        /// <summary>
        /// If true, a label is drawn for each element which shows the index of the element.
        /// </summary>
        public bool ShowIndexLabels { get; set; }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public bool PagingHasValue { get { return this.pagingHasValue; } }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public bool ShowItemCountHasValue { get { return this.showItemCountHasValue; } }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public bool DraggableHasValue { get { return this.draggableHasValue; } }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public bool IsReadOnlyHasValue { get { return this.isReadOnlyHasValue; } }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public bool ExpandedHasValue
        {
            get
            {
                return this.expandedHasValue;
            }
        }
    }
}