//-----------------------------------------------------------------------
// <copyright file="FoldoutGroupAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sirenix.OdinInspector
{
    /// <summary>
    /// <para>FoldoutGroup is used on any property, and organizes properties into a foldout.</para>
    /// <para>Use this to organize properties, and to allow the user to hide properties that are not relevant for them at the moment.</para>
    /// </summary>
    /// <example>
    /// <para>The following example shows how FoldoutGroup is used to organize properties into a foldout.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    ///	{
    ///		[FoldoutGroup("MyGroup")]
    ///		public int A;
    ///
    ///		[FoldoutGroup("MyGroup")]
    ///		public int B;
    ///
    ///		[FoldoutGroup("MyGroup")]
    ///		public int C;
    ///	}
    /// </code>
    /// </example>
    /// <example>
    /// <para>The following example shows how properties can be organizes into multiple foldouts.</para>
    /// <code>
    /// public class MyComponent : MonoBehaviour
    ///	{
    ///		[FoldoutGroup("First")]
    ///		public int A;
    ///
    ///		[FoldoutGroup("First")]
    ///		public int B;
    ///
    ///		[FoldoutGroup("Second")]
    ///		public int C;
    ///	}
    /// </code>
    /// </example>
    /// <seealso cref="BoxGroupAttribute"/>
    /// <seealso cref="ButtonGroupAttribute"/>
    /// <seealso cref="TabGroupAttribute"/>
    /// <seealso cref="ToggleGroupAttribute"/>
    /// <seealso cref="PropertyGroupAttribute"/>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class FoldoutGroupAttribute : PropertyGroupAttribute
    {
        /// <summary>
        /// Adds the property to the specified foldout group.
        /// </summary>
        /// <param name="groupName">Name of the foldout group.</param>
        /// <param name="order">The order of the group in the inspector.</param>
        public FoldoutGroupAttribute(string groupName, int order = 0)
            : base(groupName, order)
        {
        }

        /// <summary>
        /// Adds the property to the specified foldout group.
        /// </summary>
        /// <param name="groupName">Name of the foldout group.</param>
        /// <param name="order">The order of the group in the inspector.</param>
        public FoldoutGroupAttribute(string groupName, string titleStringMemberName, int order = 0)
            : base(groupName, order)
        {
            this.TitleStringMemberName = titleStringMemberName;
        }

        /// <summary>
        /// Combines the foldout property with another.
        /// </summary>
        protected override void CombineValuesWith(PropertyGroupAttribute other)
        {
            var attr = other as FoldoutGroupAttribute;

            if (this.TitleStringMemberName == null)
            {
                this.TitleStringMemberName = attr.TitleStringMemberName;
            }
            else if (attr.TitleStringMemberName == null)
            {
                attr.TitleStringMemberName = this.TitleStringMemberName;
            }
        }

        /// <summary>
        /// Name of any string field, property or function, to title the foldout in the inspector.
        /// </summary>
        public string TitleStringMemberName { get; private set; }
    }
}