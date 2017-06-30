//-----------------------------------------------------------------------
// <copyright file="TitleAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>Title is used to make a bold header above a property.</para>
    /// </summary>
    /// <remarks>
    /// <para>Title is identical to <see cref="UnityEngine.HeaderAttribute"/>, but also supports property and function members.</para>
    /// </remarks>
    /// <example>
    /// The following example shows how Title is used on different properties.
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    ///		[Title("Fields")]
    ///		public int MyIntField;
    ///
    ///		[Title("Properties")]
    ///		public int MyIntProperty { get; set; }
    ///
    ///		[Title("Functions"), Button]
    ///		private void MyButton()
    ///		{
    ///			// ...
    ///		}
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="ButtonAttribute"/>
    /// <seealso cref="LabelTextAttribute"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    [DontApplyToListElements]
    public class TitleAttribute : Attribute
    {
        /// <summary>
        /// The title displayed above the property in the inspector.
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// If <c>true</c> the title will be dispalyed with a bold font.
        /// </summary>
        public bool Bold { get; private set; }

        /// <summary>
        /// Creates a title above any property in the inspector.
        /// </summary>
        /// <param name="title">The title displayed above the property in the inspector.</param>
        /// <param name="bold">If <c>true</c> the title will be drawn with a bold font.</param>
        public TitleAttribute(string title, bool bold = true)
        {
            this.Title = title ?? "null";
            this.Bold = bold;
        }
    }
}