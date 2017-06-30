//-----------------------------------------------------------------------
// <copyright file="TabGroupAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    //using Utilities;

    /// <summary>
    /// <para>TabGroup is used on any property, and organizes properties into different tabs.</para>
    /// <para>Use this to organize different value to make a clean and easy to use inspector.</para>
    /// </summary>
	/// <remarks>
    /// <para>Use the group to create multiple tab groups, each with multiple tabs.</para>
    /// <note type="note">Currently there is no label drawn for properties in tab groups, that only contain a single property.</note>
    /// </remarks>
	/// <example>
	/// <para>The following example shows how to create a tab group with two tabs.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
	///	{
	///		[TabGroup("First")]
	///		public int MyFirstInt;
	///
	///		[TabGroup("First")]
	///		public int AnotherInt;
	///
	///		[TabGroup("Second")]
	///		public int MySecondInt;
	///	}
    /// </code>
    /// </example>
	/// <example>
	/// <para>The following example shows how multiple groups of tabs can be created.</para>
    /// <code>
	///	public class MyComponent : MonoBehaviour
	///	{
	///		[TabGroup("A", "FirstGroup")]
	///		public int FirstGroupA;
	///
	///		[TabGroup("B", "FirstGroup")]
	///		public int FirstGroupB;
	///
	///		// The second tab group has been configured to have constant height across all tabs.
	///		[TabGroup("A", "SecondGroup", true)]
	///		public int SecondgroupA;
	///
	///		[TabGroup("B", "SecondGroup")]
	///		public int SecondGroupB;
	///
	///		[TabGroup("B", "SecondGroup")]
	///		public int AnotherInt;
	///	}
	/// </code>
    /// </example>
	/// <seealso cref="TabListAttribute"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TabGroupAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// Name of the tab.
        /// </summary>
        public string TabName { get; private set; }

        /// <summary>
        /// Name of all tabs in this group.
        /// </summary>
        public List<string> Tabs { get; private set; }

        /// <summary>
        /// Should this tab be the same height as the rest of the tab group.
        /// </summary>
        public bool UseFixedHeight { get; private set; }

        /// <summary>
        /// Organizes the property into the specified tab in the default group.
        /// </summary>
        /// <param name="tab">The tab.</param>
        /// <param name="useFixedHeight">if set to <c>true</c> [use fixed height].</param>
        /// <param name="order">The order.</param>
        public TabGroupAttribute(string tab, bool useFixedHeight = false, int order = 0)
            : this("_DefaultTabGroup", tab, useFixedHeight, order)
        { }

        /// <summary>
        /// Organizes the property into the specified tab in the specified group.
        /// </summary>
        /// <param name="group">The group to attach the tab to.</param>
        /// <param name="tab">The name of the tab.</param>
        /// <param name="useFixedHeight">Set to true to have a constant height across the entire tab group.</param>
        /// <param name="order">The order of the group.</param>
        public TabGroupAttribute(string group, string tab, bool useFixedHeight = false, int order = 0)
            : base(group, order)
        {
            this.TabName = tab;
            this.UseFixedHeight = useFixedHeight;

            this.Tabs = new List<string>();
            if (tab != null)
            {
                this.Tabs.Add(tab);
            }
            this.Tabs = new List<string>(this.Tabs);
        }

        /// <summary>
        /// Combines the tab group with another group.
        /// </summary>
        /// <param name="other">The other group.</param>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            base.CombineValuesWith(other);

            var otherTab = other as TabGroupAttribute;
            if (otherTab.TabName != null)
            {
                this.UseFixedHeight = this.UseFixedHeight || otherTab.UseFixedHeight;
                if (this.Tabs.Contains(otherTab.TabName) == false)
                {
                    this.Tabs.Add(otherTab.TabName);
                }
            }
        }
    }
}