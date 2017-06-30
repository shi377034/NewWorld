//-----------------------------------------------------------------------
// <copyright file="InspectorSerializationUtility.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    using System.Globalization;
    using System;
    using System.Collections.Generic;
    using Utilities;

    /// <summary>
    /// Provides utility methods meant only for the inspector, but which nevertheless also have to be accessible to the serialization system.
    /// </summary>
    public static class InspectorSerializationUtility
    {
        private static readonly HashSet<Type> SupportedDictionaryKeyTypes = new HashSet<Type>()
        {
            typeof(string),
            typeof(char),
            typeof(byte),
            typeof(sbyte),
            typeof(ushort),
            typeof(short),
            typeof(uint),
            typeof(int),
            typeof(ulong),
            typeof(long),
            typeof(float),
            typeof(double),
            typeof(decimal),
        };

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static IEnumerable<Type> GetSupportedDictionaryKeyTypes()
        {
            foreach (var type in SupportedDictionaryKeyTypes)
            {
                yield return type;
            }
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static bool IsSupportedDictionaryKeyType(Type type)
        {
            return SupportedDictionaryKeyTypes.Contains(type);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static string GetDictionaryKeyString(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException("key");
            }

            Type type = key.GetType();

            if (!IsSupportedDictionaryKeyType(type))
            {
                throw new NotSupportedException("The type " + type.GetNiceName() + " is not a supported dictionary key type!");
            }

            if (type == typeof(string)) return "{\"" + key + "\"}";
            if (type == typeof(char)) return "{'" + ((char)key).ToString(CultureInfo.InvariantCulture) + "'}";
            if (type == typeof(byte)) return "{" + ((byte)key).ToString("D", CultureInfo.InvariantCulture) + "ub}";
            if (type == typeof(sbyte)) return "{" + ((sbyte)key).ToString("D", CultureInfo.InvariantCulture) + "sb}";
            if (type == typeof(ushort)) return "{" + ((ushort)key).ToString("D", CultureInfo.InvariantCulture) + "us}";
            if (type == typeof(short)) return "{" + ((short)key).ToString("D", CultureInfo.InvariantCulture) + "ss}";
            if (type == typeof(uint)) return "{" + ((uint)key).ToString("D", CultureInfo.InvariantCulture) + "ui}";
            if (type == typeof(int)) return "{" + ((int)key).ToString("D", CultureInfo.InvariantCulture) + "si}";
            if (type == typeof(ulong)) return "{" + ((ulong)key).ToString("D", CultureInfo.InvariantCulture) + "ul}";
            if (type == typeof(long)) return "{" + ((long)key).ToString("D", CultureInfo.InvariantCulture) + "sl}";
            if (type == typeof(float)) return "{" + ((float)key).ToString("R", CultureInfo.InvariantCulture) + "fl}";
            if (type == typeof(double)) return "{" + ((double)key).ToString("R", CultureInfo.InvariantCulture) + "dl}";
            if (type == typeof(decimal)) return "{" + ((decimal)key).ToString("G", CultureInfo.InvariantCulture) + "dc}";

            throw new NotImplementedException("Support has not been implemented for the supported dictionary key type '" + type.GetNiceName() + "'.");
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static object GetDictionaryKeyValue(string keyStr)
        {
            const string InvalidKeyString = "Invalid key string: ";

            if (keyStr == null) throw new ArgumentNullException("keyStr");
            if (keyStr.Length < 4 || keyStr[0] != '{' || keyStr[keyStr.Length - 1] != '}') throw new ArgumentException(InvalidKeyString + keyStr);

            if (keyStr[1] == '"')
            {
                if (keyStr[keyStr.Length - 2] != '"') throw new ArgumentException(InvalidKeyString + keyStr);
                return keyStr.Substring(2, keyStr.Length - 4);
            }

            if (keyStr[1] == '\'')
            {
                if (keyStr.Length != 5 || keyStr[keyStr.Length - 2] != '\'') throw new ArgumentException(InvalidKeyString + keyStr);
                return keyStr[2];
            }

            if (keyStr.EndsWith("ub}")) return byte.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("sb}")) return sbyte.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("us}")) return ushort.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("ss}")) return short.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("ui}")) return uint.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("si}")) return int.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("ul}")) return ulong.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("sl}")) return long.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("fl}")) return float.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("dl}")) return double.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);
            if (keyStr.EndsWith("dc}")) return decimal.Parse(keyStr.Substring(1, keyStr.Length - 4), NumberStyles.Any);

            throw new ArgumentException(InvalidKeyString + keyStr);
        }
    }
}