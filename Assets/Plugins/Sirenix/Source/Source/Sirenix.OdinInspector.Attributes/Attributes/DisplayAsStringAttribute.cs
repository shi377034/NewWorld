//-----------------------------------------------------------------------
// <copyright file="DisplayAsStringAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
	using System;

	/// <summary>
	/// <para>DisplayAsString is used on any property, and displays a string in the inspector as text.</para>
	/// <para>Use this for when you want to show a string in the inspector, but not allow for any editing.</para>
	/// </summary>
	/// <remarks>
	/// <para>DisplayAsString uses the property's ToString method to display the property as a string.</para>
	/// </remarks>
	/// <example>
	/// <para>The following example shows how DisplayAsString is used to display a string property as text in the inspector.</para>
	/// <code>
	/// public class MyComponent : MonoBehaviour
	///	{
	///		[DisplayAsString]
	///		public string MyInt = 5;
	///
	///		// You can combine with <see cref="HideLabelAttribute"/> to display a message in the inspector.
	///		[DisplayAsString, HideLabel]
	///		public string MyMessage = "This string will be displayed as text in the inspector";
	///	}
	/// </code>
	/// </example>
	/// <seealso cref="TitleAttribute"/>
	/// <seealso cref="MultiLinePropertyAttribute"/>
	[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class DisplayAsStringAttribute : Attribute
	{ }
}