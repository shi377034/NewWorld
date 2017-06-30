#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ColorPaletteManager.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using System;
    using System.Collections.Generic;
    using Utilities;
    using UnityEngine;

    /// <summary>
    /// <para>Add, Edit or remove custom color palettes used by the <see cref="ColorPaletteAttribute"/>.</para>
    /// <para>
    /// You can modify the configuration in the Odin Preferences window found in 'Window -> Odin Preferences -> Drawers -> Color Palettes',
    /// or by locating the configuration file stored as a serialized object in the Sirenix folder under 'Odin Inspector/Config/Editor/ColorPaletteManager'.
    /// </para>
    /// </summary>
    [SirenixEditorConfig("Odin Inspector/Drawers/Color Palettes")]
    public class ColorPaletteManager : GlobalConfig<ColorPaletteManager>
    {
        [Tooltip("Specify the amount of spacing between each color in a color palette.")]
        [SerializeField, Range(0, 7)]
        private int swatchSpacing = 0;

        [Tooltip("Specify the width of each color in a color palette. If StretchPalette is set to true, this will become the min-width.")]
        [SerializeField]
        private int swatchSize = 20;

        [Tooltip("If true, all color in a color palette is stretch so that the entire color-palette area is filled.")]
        [SerializeField]
        private bool stretchPalette = true;

        [Tooltip("If true, a tool-bar with the name of the color palette is shown above each color palette.")]
        [SerializeField]
        private bool showPaletteName = false;

        [PropertyOrder(2)]
        [SerializeField]
        [ListDrawerSettings(DraggableItems = true, ShowPaging = false, ShowItemCount = false, Expanded = true)]
        [DisableContextMenu(true, false)]
        private List<ColorPalette> colorPalettes = new List<ColorPalette>()
        {
            new ColorPalette(){ Name = "Country",  Colors = new List<Color>() { new Color(0.776f, 0.651f, 0.349f, 1f), new Color(0.863f, 0.761f, 0.631f, 1f), new Color(0.91f, 0.831f, 0.686f, 1f), new Color(0.961f, 0.902f, 0.788f, 1f), new Color(0.753f, 0.714f, 0.667f, 1f), new Color(0.478f, 0.573f, 0.431f, 1f), new Color(0.314f, 0.427f, 0.31f, 1f), new Color(0.596f, 0.345f, 0.235f, 1f), new Color(0.545f, 0.329f, 0.318f, 1f), new Color(0.647f, 0.204f, 0.227f, 1f), new Color(0.435f, 0.161f, 0.063f, 1f), new Color(0.357f, 0.333f, 0.278f, 1f), new Color(0.976f, 0.98f, 0.961f, 1f), new Color(0.165f, 0.271f, 0.11f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Beach",  Colors = new List<Color>() { new Color(0.996f, 0.906f, 0.459f, 1f), new Color(0.314f, 0.592f, 0.035f, 1f), new Color(0.486f, 0.953f, 0.875f, 1f), new Color(0.996f, 0.82f, 0.212f, 1f), new Color(1f, 0.769f, 0.165f, 1f), new Color(0.804f, 0.835f, 0.753f, 1f), new Color(1f, 0.769f, 0.165f, 1f), new Color(1f, 0.702f, 0.063f, 1f), new Color(1f, 0.898f, 0.569f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Fall",  Colors = new List<Color>() { new Color(0.82f, 0.722f, 0.318f, 1f), new Color(0.537f, 0.192f, 0.153f, 1f), new Color(0.996f, 0.812f, 0.012f, 1f), new Color(1f, 0.431f, 0.02f, 1f), new Color(0.937f, 0.267f, 0.094f, 1f), new Color(0.42f, 0.212f, 0.18f, 1f), new Color(0.992f, 0.651f, 0.004f, 1f), new Color(0.89f, 0.353f, 0.086f, 1f), new Color(1f, 0.443f, 0.004f, 1f), new Color(0.682f, 0.275f, 0.137f, 1f), new Color(0.306f, 0.231f, 0.114f, 1f), new Color(0.384f, 0.416f, 0.082f, 1f), new Color(0.165f, 0.157f, 0.008f, 1f), new Color(0.906f, 0.635f, 0.227f, 1f), new Color(0.82f, 0.722f, 0.318f, 1f), new Color(0.745f, 0.435f, 0.031f, 1f), new Color(0.765f, 0.682f, 0.569f, 1f), new Color(0.18f, 0.149f, 0.075f, 1f), new Color(0.702f, 0.451f, 0.059f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Passion",  Colors = new List<Color>() { new Color(0.925f, 0.682f, 0.624f, 1f), new Color(0.188f, 0.114f, 0.224f, 1f), new Color(0.349f, 0.11f, 0.231f, 1f), new Color(0.435f, 0.267f, 0.357f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Sepia",  Colors = new List<Color>() { new Color(0.353f, 0.098f, 0.02f, 1f), new Color(0.663f, 0.188f, 0.114f, 1f), new Color(0.906f, 0.643f, 0.082f, 1f), new Color(0.996f, 0.839f, 0.322f, 1f), new Color(0.486f, 0.392f, 0.02f, 1f), new Color(0.294f, 0.235f, 0.012f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Floral",  Colors = new List<Color>() { new Color(0.855f, 0.518f, 0.412f, 1f), new Color(0.827f, 0.294f, 0.333f, 1f), new Color(0.737f, 0.118f, 0.208f, 1f), new Color(0.549f, 0.149f, 0.235f, 1f), new Color(0.949f, 0.925f, 0.784f, 1f), new Color(0.945f, 0.882f, 0.69f, 1f), new Color(0.871f, 0.812f, 0.698f, 1f), new Color(0.4f, 0.196f, 0.243f, 1f), new Color(0.271f, 0.157f, 0.227f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Underwater",  Colors = new List<Color>() { new Color(0.663f, 0.416f, 0.733f, 1f), new Color(0.2f, 0.6f, 0.698f, 1f), new Color(0.11f, 0.49f, 0.698f, 1f), new Color(0.439f, 0.627f, 0.227f, 1f), new Color(0f, 0.357f, 0.604f, 1f), new Color(0.067f, 0.271f, 0.353f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Breeze",  Colors = new List<Color>() { new Color(0.706f, 1f, 0f, 1f), new Color(0.651f, 1f, 0.404f, 1f), new Color(0.122f, 1f, 0.514f, 1f), new Color(0.216f, 0.894f, 0.961f, 1f), new Color(0.4f, 1f, 0.882f, 1f), new Color(0.027f, 0.792f, 0.8f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Clovers",  Colors = new List<Color>() { new Color(0.431f, 0.549f, 0.102f, 1f), new Color(0.671f, 0.714f, 0.071f, 1f), new Color(0.969f, 0.949f, 0.831f, 1f), new Color(0.886f, 0.902f, 0.702f, 1f), new Color(0.753f, 0.824f, 0.627f, 1f), new Color(0.404f, 0.6f, 0.4f, 1f) }, ShowAlpha = false },
            new ColorPalette(){ Name = "Tropical",  Colors = new List<Color>() { new Color(0.953f, 0.647f, 0.804f, 1f), new Color(0.965f, 0.741f, 0.871f, 1f), new Color(0.949f, 0.549f, 0.643f, 1f), new Color(0.992f, 0.659f, 0.498f, 1f), new Color(0.976f, 0.792f, 0.729f, 1f), new Color(0.984f, 0.855f, 0.725f, 1f), new Color(0.259f, 0.882f, 0.663f, 1f), new Color(0.349f, 0.753f, 0.78f, 1f), new Color(0.725f, 0.976f, 0.91f, 1f), new Color(0.647f, 0.745f, 0.957f, 1f), new Color(0.725f, 0.863f, 0.973f, 1f), new Color(0.89f, 0.945f, 0.996f, 1f) }, ShowAlpha = false },
        };

        internal ColorPalette GetColorPalette(string colorPaletteName)
        {
            for (int i = 0; i < this.colorPalettes.Count; i++)
            {
                if (this.colorPalettes[i].Name == colorPaletteName)
                {
                    return this.colorPalettes[i];
                }
            }
            return null;
        }

        internal void CreateColorPalette(string colorPaletName)
        {
            this.colorPalettes.Add(new ColorPalette() { Name = colorPaletName ?? "My Color Palette" });
        }

        private string[] colorPaletteNameBuffer = new string[] { "None" };

        private string[] emptyColorPaletNameBuffer = new string[] { "None" };

        /// <summary>
        /// Specify the amount of spacing between each color in a color palette.
        /// </summary>
        public int SwatchSpacing { get { return this.swatchSpacing; } }

        /// <summary>
        /// Specify the width of each color in a color palette. If StretchPalette is set to true, this will become the min-width.
        /// </summary>
        public int SwatchSize { get { return this.swatchSize; } }

        /// <summary>
        /// If true, all color in a color palette is stretch so that the entire color-palette area is filled.
        /// </summary>
        public bool StretchPalette { get { return this.stretchPalette; } }

        /// <summary>
        /// If true, a tool-bar with the name of the color palette is shown above each color palette.
        /// </summary>
        public bool ShowPaletteName { get { return this.showPaletteName; } }

        internal string[] GetColorPaletteNames()
        {
            if (this.colorPalettes.Count == 0)
            {
                return this.emptyColorPaletNameBuffer;
            }

            if (this.colorPaletteNameBuffer.Length != this.colorPalettes.Count)
            {
                Array.Resize(ref this.colorPaletteNameBuffer, this.colorPalettes.Count);
            }

            for (int i = 0; i < this.colorPalettes.Count; i++)
            {
                this.colorPaletteNameBuffer[i] = this.colorPalettes[i].Name;
            }

            return this.colorPaletteNameBuffer;
        }
    }
}
#endif