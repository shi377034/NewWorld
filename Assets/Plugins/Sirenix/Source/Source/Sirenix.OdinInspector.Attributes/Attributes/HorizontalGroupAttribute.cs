//-----------------------------------------------------------------------
// <copyright file="HorizontalGroupAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>HorizontalGroup is used group multiple properties horizontally in the inspector.</para>
    /// <para>The width can either be specified as percentage or pixels.</para>
    /// <para>All values between 0 and 1 will be treated as a percentage.</para>
    /// <para>If the width is 0 the column will be automatically sized.</para>
    /// <para>Margin-left and right can only be specified in pixels.</para>
    /// </summary>
    /// <example>
    /// <para>The following example shows how three properties have been grouped together horizontally.</para>
    /// <code>
    ///    // Group 1
    ///    [HorizontalGroup("Group 1")]
    ///    public int A;
    ///
    ///    [HorizontalGroup("Group 1")]
    ///    public int B;
    ///
    ///    [HorizontalGroup("Group 1")]
    ///    public int C;
    ///
    ///    // Group 2
    ///    [HorizontalGroup("Group 2")]
    ///    [HideLabel]
    ///    public int D;
    ///
    ///    [HorizontalGroup("Group 2", width: 0.6f)] // Percentage
    ///    [HideLabel]
    ///    public int E;
    ///
    ///    // Group 3
    ///    [HorizontalGroup("Group 3", width: 90)] // Pixels
    ///    [HideLabel]
    ///    public int F;
    ///
    ///    [HorizontalGroup("Group 3", marginLeft: 20)] // Margin
    ///    [HideLabel]
    ///    public int G;
    ///
    ///    // My Group
    ///    [HorizontalGroup("My Group")]
    ///    [AssetList(AssetNamePrefix = "Rock")]
    ///    public GameObject[] Left;
    ///
    ///    [HorizontalGroup("My Group")]
    ///    [AssetList(AssetNamePrefix = "Rock")]
    ///    public GameObject[] Right;
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class HorizontalGroupAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// An optional width for the property.
        /// </summary>
        public float Width { get; private set; }

        /// <summary>
        /// Gets the margin left.
        /// </summary>
        public int MarginLeft { get; private set; }

        /// <summary>
        /// Gets the margin right.
        /// </summary>
        public int MarginRight { get; private set; }

        /// <summary>
        /// Organizes the property in a horizontal group.
        /// </summary>
        /// <param name="group">The group for the property.</param>
        /// <param name="width">The width of the property. Values between 0 and 1 are interpolated as a percentage, otherwise pixels.</param>
        /// <param name="marginLeft">The left margin in pixels.</param>
        /// <param name="marginRight">The right margin in pixels.</param>
        public HorizontalGroupAttribute(string group, float width = 0, int marginLeft = 0, int marginRight = 0)
            : base(group, 0)
        {
            this.Width = width;
            this.MarginLeft = marginLeft;
            this.MarginRight = marginRight;
        }

        /// <summary>
        /// Organizes the property in a horizontal group.
        /// </summary>
        /// <param name="width">The width of the property. Values between 0 and 1 are interpolated as a percentage, otherwise pixels.</param>
        /// <param name="marginLeft">The left margin in pixels.</param>
        /// <param name="marginRight">The right margin in pixels.</param>
        public HorizontalGroupAttribute(float width = 0, int marginLeft = 0, int marginRight = 0)
            : this("_DefaultHorizontalGroup", width, marginLeft, marginRight)
        {
        }
    }
}