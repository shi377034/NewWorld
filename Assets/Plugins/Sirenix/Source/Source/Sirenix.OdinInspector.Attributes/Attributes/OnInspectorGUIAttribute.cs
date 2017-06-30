//-----------------------------------------------------------------------
// <copyright file="OnInspectorGUIAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>OnInspectorGUI is used on any property, and will call the specified function whenever the inspector code is running.</para>
    /// <para>Use this to create custom inspector GUI for an object.</para>
    /// </summary>
	/// <example>
    /// <para></para>
    /// <code>
    /// public MyComponent : MonoBehaviour
	/// {
	///		[OnInspectorGUI]
	///		private void MyInspectorGUI()
	///		{
	///			GUILayout.Label("Label drawn from callback");
	///		}
	/// }
    /// </code>
    /// </example>
	/// <example>
    ///	<para>The following example shows how a callback can be set before another property.</para>
    /// <code>
    /// public MyComponent : MonoBehaviour
	/// {
	///		[OnInspectorGUI("MyInspectorGUI", false)]
	///		public int MyField;
	///
	///		private void MyInspectorGUI()
	///		{
	///			GUILayout.Label("Label before My Field property");
	///		}
	/// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    [DontApplyToListElements]
    public sealed class OnInspectorGUIAttribute : ShowInInspectorAttribute
    {
        /// <summary>
        /// Calls the function when the inspector is being drawn.
        /// </summary>
        public OnInspectorGUIAttribute()
        { }

		/// <summary>
		/// Adds callback to the specified method when the property is being drawn.
		/// </summary>
		/// <param name="methodName">The name of the member function.</param>
		/// <param name="append">If <c>true</c> the method will be called after the property has been drawn. Otherwise the method will be called before.</param>
        public OnInspectorGUIAttribute(string methodName, bool append = true)
        {
            this.MethodName = methodName;
            this.Append = append;
        }

        /// <summary>
        /// If <c>true</c> the method will be called after the property has been drawn. Otherwise the method will be called before.
        /// </summary>
        public bool Append { get; private set; }

        /// <summary>
        /// The name of the method to be called.
        /// </summary>
        public string MethodName { get; private set; }
    }
}