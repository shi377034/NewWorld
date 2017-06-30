//-----------------------------------------------------------------------
// <copyright file="GradientFormatter.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    using UnityEngine;

    /// <summary>
    /// Custom formatter for the <see cref="Gradient"/> type.
    /// </summary>
    /// <seealso cref="Sirenix.Serialization.MinimalBaseFormatter{UnityEngine.Gradient}" />
    [CustomFormatter]
    public class GradientFormatter : MinimalBaseFormatter<Gradient>
    {
        private static readonly Serializer<GradientAlphaKey[]> AlphaKeysSerializer = Serializer.Get<GradientAlphaKey[]>();
        private static readonly Serializer<GradientColorKey[]> ColorKeysSerializer = Serializer.Get<GradientColorKey[]>();

        /// <summary>
        /// Reads into the specified value using the specified reader.
        /// </summary>
        /// <param name="value">The value to read into.</param>
        /// <param name="reader">The reader to use.</param>
        protected override void Read(ref Gradient value, IDataReader reader)
        {
            value.alphaKeys = GradientFormatter.AlphaKeysSerializer.ReadValue(reader);
            value.colorKeys = GradientFormatter.ColorKeysSerializer.ReadValue(reader);
        }

        /// <summary>
        /// Writes from the specified value using the specified writer.
        /// </summary>
        /// <param name="value">The value to write from.</param>
        /// <param name="writer">The writer to use.</param>
        protected override void Write(ref Gradient value, IDataWriter writer)
        {
            GradientFormatter.AlphaKeysSerializer.WriteValue(value.alphaKeys, writer);
            GradientFormatter.ColorKeysSerializer.WriteValue(value.colorKeys, writer);
        }
    }
}