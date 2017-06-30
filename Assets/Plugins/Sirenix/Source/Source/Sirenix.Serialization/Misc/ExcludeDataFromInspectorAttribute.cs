//-----------------------------------------------------------------------
// <copyright file="ExcludeDataFromInspectorAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Serialization
{
    using System;

    /// <summary>
    /// <para>Write 1 max 2 paragraphs in summery, they are supposed to be a breaf description and overview.</para>
    /// <para>Remember to use the para tag, it is equivalent of writing a 'p' tag in html.</para>
    /// </summary>
    /// <remarks>
    /// <para>If more clarifycation is needed write it here</para>
    /// <note type="note">Note that you can add notes!</note>
    /// </remarks>
    /// <example>
    /// Write an example description
    /// <code>
    /// // And write the code within code tags.
    /// </code>
    /// </example>
    /// <example>
    /// Write multiple examples!
    /// <code>
    /// // More code
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class ExcludeDataFromInspectorAttribute : Attribute
    {
    }
}