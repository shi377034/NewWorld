//-----------------------------------------------------------------------
// <copyright file="HideIfAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>HideIf is used on any property and can hide the property in the inspector.</para>
    /// <para>Use this to hide irrelevant properties based on the current state of the object.</para>
    /// </summary>
    /// <example>
    /// <para>This example shows a component with fields hidden by the state of another field.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    ///		public bool ShowProperties;
    ///
    ///		[HideIf("showProperties")]
    ///		public int MyInt;
    ///
    ///		[HideIf("showProperties")]
    ///		public string MyString;
    /// }
    /// </code>
    /// </example>
    /// <example>
    /// <para>This example shows a component with a field that is hidden when the game object is inactive.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    /// {
    ///		[HideIf("MyVisibleFunction")]
    ///		public int MyHideableField;
    ///
    ///		private bool MyVisibleFunction()
    ///		{
    ///			return this.gameObject.activeInHierarchy;
    ///		}
    /// }
    /// </code>
    /// </example>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="ShowIfAttribute"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    [DontApplyToListElements]
    public sealed class HideIfAttribute : Attribute
    {
        /// <summary>
        /// Name of a bool field, property or function to show or hide the property.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HideIfAttribute"/> class.
        /// </summary>
        /// <param name="memberName">Name of a bool field, property or function to show or hide the property.</param>
        public HideIfAttribute(string memberName)
        {
            this.MemberName = memberName;
        }
    }
}