//-----------------------------------------------------------------------
// <copyright file="KeyframeFormatter.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    using UnityEngine;

    /// <summary>
    /// Custom formatter for the <see cref="Keyframe"/> type.
    /// </summary>
    /// <seealso cref="Sirenix.Serialization.MinimalBaseFormatter{UnityEngine.Keyframe}" />
    [CustomFormatter]
    public class KeyframeFormatter : MinimalBaseFormatter<Keyframe>
    {
        private static readonly Serializer<float> FloatSerializer = Serializer.Get<float>();
        private static readonly Serializer<int> IntSerializer = Serializer.Get<int>();

        /// <summary>
        /// Reads into the specified value using the specified reader.
        /// </summary>
        /// <param name="value">The value to read into.</param>
        /// <param name="reader">The reader to use.</param>
        protected override void Read(ref Keyframe value, IDataReader reader)
        {
            value.inTangent = KeyframeFormatter.FloatSerializer.ReadValue(reader);
            value.outTangent = KeyframeFormatter.FloatSerializer.ReadValue(reader);
            value.time = KeyframeFormatter.FloatSerializer.ReadValue(reader);
            value.value = KeyframeFormatter.FloatSerializer.ReadValue(reader);
            value.tangentMode = KeyframeFormatter.IntSerializer.ReadValue(reader);
        }

        /// <summary>
        /// Writes from the specified value using the specified writer.
        /// </summary>
        /// <param name="value">The value to write from.</param>
        /// <param name="writer">The writer to use.</param>
        protected override void Write(ref Keyframe value, IDataWriter writer)
        {
            KeyframeFormatter.FloatSerializer.WriteValue(value.inTangent, writer);
            KeyframeFormatter.FloatSerializer.WriteValue(value.outTangent, writer);
            KeyframeFormatter.FloatSerializer.WriteValue(value.time, writer);
            KeyframeFormatter.FloatSerializer.WriteValue(value.value, writer);
            KeyframeFormatter.IntSerializer.WriteValue(value.tangentMode, writer);
        }
    }
}