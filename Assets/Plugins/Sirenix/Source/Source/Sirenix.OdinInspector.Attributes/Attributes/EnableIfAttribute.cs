//-----------------------------------------------------------------------
// <copyright file="EnableIfAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>EnableIf is used on any property, and can enable or disable the property in the inspector.</para>
    /// <para>Use this to enable properties when they are relevant.</para>
    /// </summary>
	/// <example>
    /// <para>The following example shows how a property can be enabled by the state of a field.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
	/// {
	///		public bool EnableProperty;
	///
	///		[EnableIf("EnableProperty")]
	///		public int MyInt;
	/// }
    /// </code>
    /// </example>
	/// <example>
    /// <para>The following examples show how a property can be enabled by a function.</para>
    /// <code>
	/// public class MyComponent : MonoBehaviour
	/// {
	///		[EnableIf("MyEnableFunction")]
	///		public int MyInt;
	///
	///		private bool MyEnableFunction()
	///		{
	///			// ...
	///		}
	/// }
    /// </code>
    /// </example>
	/// <seealso cref="DisableIfAttribute"/>
	/// <seealso cref="ShowIfAttribute"/>
	/// <seealso cref="HideIfAttribute"/>
	/// <seealso cref="DisableInEditorModeAttribute"/>
	/// <seealso cref="DisableInPlayModeAttribute"/>
    [DontApplyToListElements]
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
    public sealed class EnableIfAttribute : Attribute
    {
        /// <summary>
        /// The name of a bool member field, property or method.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Enables a property in the inspector, based on the state of a member.
        /// </summary>
        public EnableIfAttribute(string memberName)
        {
            this.MemberName = memberName;
        }
    }
}