//-----------------------------------------------------------------------
// <copyright file="IndentAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector
{
    using System;

    /// <summary>
    /// <para>InlineAttribute is used on any property or field with a type that inherits from UnityEngine.Object. This includes components and assets etc.</para>
    /// </summary>
    /// <example>
    /// <code>
    /// public class InlineExamples : MonoBehaviour
    /// {
    ///     [InlineEditor]
    ///     public Transform InlineComponent;
    ///
    ///     [InlineEditor(InlineEditorModes.All)]
    ///     public Material FullInlineEditor;
    ///
    ///     [InlineEditor(InlineEditorModes.GUIAndHeader)]
    ///     public Material InlineMaterial;
    ///
    ///     [InlineEditor(InlineEditorModes.SmallPreview)]
    ///     public Material[] InlineMaterialList;
    ///
    ///     [InlineEditor(InlineEditorModes.LargePreview)]
    ///     public Mesh InlineMeshPreview;
    /// }
    /// </code>
    /// </example>
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class InlineEditorAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InlineEditorAttribute" /> class.
        /// </summary>
        /// <param name="inlineEditorMode">The inline editor mode.</param>
        public InlineEditorAttribute(InlineEditorModes inlineEditorMode = InlineEditorModes.GUIOnly)
        {
            switch (inlineEditorMode)
            {
                case InlineEditorModes.GUIOnly:
                    this.DrawGUI = true;
                    break;

                case InlineEditorModes.GUIAndHeader:
                    this.DrawGUI = true;
                    this.DrawHeader = true;
                    break;

                case InlineEditorModes.GUIAndPreview:
                    this.DrawGUI = true;
                    this.DrawPreview = true;
                    break;

                case InlineEditorModes.SmallPreview:
                    this.Expanded = true;
                    this.DrawPreview = true;
                    break;

                case InlineEditorModes.LargePreview:
                    this.Expanded = true;
                    this.DrawPreview = true;
                    this.PreviewHeight = 170;
                    break;

                case InlineEditorModes.FullEditor:
                    this.DrawGUI = true;
                    this.DrawHeader = true;
                    this.DrawPreview = true;
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// If true, the inline editor will start expanded.
        /// </summary>
        public bool Expanded;

        /// <summary>
        /// Draw the header editor header inline.
        /// </summary>
        public bool DrawHeader;

        /// <summary>
        /// Draw editor GUI inline.
        /// </summary>
        public bool DrawGUI;

        /// <summary>
        /// Draw editor preview inline.
        /// </summary>
        public bool DrawPreview;

        /// <summary>
        /// The size of the editor preview if drawn together with GUI.
        /// </summary>
        public float PreviewWidth = 100;

        /// <summary>
        /// The size of the editor preview if drawn alone.
        /// </summary>
        public float PreviewHeight = 35;
    }
}