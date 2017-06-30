//-----------------------------------------------------------------------
// <copyright file="BoxGroupAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
	/// <para>ButtonGroup is used on any property and organizes the property in a boxed group.</para>
	/// <para>Use this to cleanly organize relevant values together in the inspector.</para>
    /// </summary>
	/// <example>
	/// <para>The following example shows how BoxGroup is used to organize properties together into a box.</para>
    /// <code>
	///	public class MyComponent : MonoBehaviour
	///	{
	///		[BoxGroup("My box")]
	///		public int A;
	///
	///		[BoxGroup("My box")]
	///		public int B;
	///
	///		[BoxGroup("My box")]
	///		public int C;
	///	}
    /// </code>
    /// </example>
	/// <example>
	/// <para>The following example shows how properties can be organized into multiple groups.</para>
    /// <code>
	///	public class MyComponent : MonoBehaviour
	///	{
	///		[BoxGroup("Letters")]
	///		public int A;
	///
	///		[BoxGroup("Letters")]
	///		public int B;
	///
	///		[BoxGroup("Numbers")]
	///		public int One;
	///
	///		[BoxGroup("Numbers")]
	///		public int Two;
	///	}
    /// </code>
    /// </example>
	/// <seealso cref="ButtonGroupAttribute"/>
	/// <seealso cref="FoldoutGroupAttribute"/>
	/// <seealso cref="HorizontalGroupAttribute"/>
	/// <seealso cref="TabGroupAttribute"/>
	/// <seealso cref="ToggleGroupAttribute"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class BoxGroupAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// Header label for the group.
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// If <c>true</c> a label for the group will be drawn on top.
        /// </summary>
        public bool ShowLabel { get; private set; }

        /// <summary>
        /// If <c>true</c> the header label will be places in the center of the group header. Otherwise it will be in left side.
        /// </summary>
        public bool CenterLabel { get; private set; }

        /// <summary>
        /// Adds the property to the specified box group.
        /// </summary>
        /// <param name="group">The box group.</param>
		/// <param name="showLabel">If <c>true</c> a label will be drawn for the group.</param>
        /// <param name="centerLabel">If set to <c>true</c> the header label will be centered.</param>
        /// <param name="order">The order of the group in the inspector.</param>
        public BoxGroupAttribute(string group, bool showLabel = true, bool centerLabel = false, int order = 0)
            : base(group, order)
        {
            this.Label = group;
            this.ShowLabel = showLabel;
            this.CenterLabel = centerLabel;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxGroupAttribute"/> class. Use the other constructor overloads in order to show a header-label on the box group.
        /// </summary>
        public BoxGroupAttribute()
            : this("_DefaultBoxGroup", false)
        {
        }

        /// <summary>
        /// Combines the box group with another group.
        /// </summary>
        /// <param name="other">The other group.</param>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var attr = other as BoxGroupAttribute;

            if (this.Label == null)
            {
                this.Label = attr.Label;
            }

            this.ShowLabel |= attr.ShowLabel;
            this.CenterLabel |= attr.CenterLabel;
        }
    }
}