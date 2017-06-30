//-----------------------------------------------------------------------
// <copyright file="UnitySerializationUtility.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.Serialization
{
    using System.Globalization;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using Utilities;
    using UnityEngine;
    using UnityEngine.Events;

    /// <summary>
    /// Provides an array of utility wrapper methods for easy serialization and deserialization of Unity objects of any type.
    /// <para />
    /// Note that setting the IndexReferenceResolver on contexts passed into methods on this class will have no effect, as it will always
    /// be set to a UnityReferenceResolver.
    /// </summary>
    public static class UnitySerializationUtility
    {
#if UNITY_EDITOR

        [NonSerialized]
        private static readonly Dictionary<UnityEngine.Object, List<PrefabModification>> RegisteredPrefabModifications = new Dictionary<UnityEngine.Object, List<PrefabModification>>();

        [NonSerialized]
        private static readonly HashSet<UnityEngine.Object> PrefabsWithValuesApplied = new HashSet<UnityEngine.Object>();

#endif

        private static readonly Dictionary<DataFormat, IDataReader> UnityReaders = new Dictionary<DataFormat, IDataReader>();
        private static readonly Dictionary<DataFormat, IDataWriter> UnityWriters = new Dictionary<DataFormat, IDataWriter>();
        private static readonly Dictionary<MemberInfo, WeakValueGetter> UnityMemberGetters = new Dictionary<MemberInfo, WeakValueGetter>();
        private static readonly Dictionary<MemberInfo, WeakValueSetter> UnityMemberSetters = new Dictionary<MemberInfo, WeakValueSetter>();

#if UNITY_EDITOR

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static List<PrefabModification> GetRegisteredPrefabModifications(UnityEngine.Object obj)
        {
            List<PrefabModification> result;
            RegisteredPrefabModifications.TryGetValue(obj, out result);
            return result;
        }

#endif

        /// <summary>
        /// Checks whether Odin will serialize a given member.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <param name="serializeUnityFields">Whether to allow serialization of members that will also be serialized by Unity.</param>
        /// <returns>True if Odin will serialize the member, otherwise false.</returns>
        public static bool OdinWillSerialize(MemberInfo member, bool serializeUnityFields)
        {
            // Enforce serialization of fields with [OdinSerialize], regardless of whether Unity
            // serializes the field or not
            if (member is FieldInfo && member.HasCustomAttribute<OdinSerializeAttribute>())
            {
                return true;
            }

            var willUnitySerialize = GuessIfUnityWillSerialize(member);

            if (willUnitySerialize)
            {
                return serializeUnityFields;
            }

            return SerializationPolicies.Unity.ShouldSerializeMember(member);
        }

        /// <summary>
        /// Guesses whether or not Unity will serialize a given member. This is not completely accurate.
        /// </summary>
        /// <param name="member">The member to check.</param>
        /// <returns>True if it is guessed that Unity will serialize the member, otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">The parameter <paramref name="member"/> is null.</exception>
        public static bool GuessIfUnityWillSerialize(MemberInfo member)
        {
            if (member == null)
            {
                throw new ArgumentNullException("member");
            }

            FieldInfo fieldInfo = member as FieldInfo;

            if (fieldInfo == null)
            {
                return false;
            }

            if (!typeof(UnityEngine.Object).IsAssignableFrom(fieldInfo.FieldType) && fieldInfo.FieldType == fieldInfo.DeclaringType)
            {
                // Unity will not serialize references that are obviously cyclical
                return false;
            }

            if (fieldInfo.IsDefined<NonSerializedAttribute>() || (!fieldInfo.IsPublic && !fieldInfo.IsDefined<SerializeField>()))
            {
                return false;
            }

            return GuessIfUnityWillSerialize(fieldInfo.FieldType);
        }

        /// <summary>
        /// Guesses whether or not Unity will serialize a given type. This is not completely accurate.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns>True if it is guessed that Unity will serialize the type, otherwise false.</returns>
        /// <exception cref="System.ArgumentNullException">The parameter <paramref name="type"/> is null.</exception>
        public static bool GuessIfUnityWillSerialize(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                // Unity will always serialize all of its own special objects
                return true;
            }

            if (type.IsAbstract || type.IsInterface || type == typeof(object))
            {
                return false;
            }

            if (type.IsEnum)
            {
                return Enum.GetUnderlyingType(type) == typeof(int);
            }

            if (type.IsPrimitive || type == typeof(string))
            {
                return true;
            }

            if (typeof(UnityEventBase).IsAssignableFrom(type))
            {
                return !type.IsGenericType;
            }

            if (type.Assembly.FullName.StartsWith("UnityEngine", StringComparison.InvariantCulture) || type.Assembly.FullName.StartsWith("UnityEditor", StringComparison.InvariantCulture))
            {
                // We assume Unity will serialize all of their own structs and classes (many of them are not marked serializable).
                // If not, well, too bad - the user can use the [OdinSerialize] attribute on their field/property in that case to trigger custom serialization.
                return true;
            }

            if (type.IsArray)
            {
                // Unity does not support jagged arrays.
                return type.GetArrayRank() == 1 && !type.GetElementType().IsArray && GuessIfUnityWillSerialize(type.GetElementType());
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                // Unity does not support lists in lists.
                var elementType = type.GetArgumentsOfInheritedOpenGenericClass(typeof(List<>))[0];
                if (elementType.IsArray || elementType.InheritsFrom(typeof(IList<>)))
                {
                    return false;
                }
                return GuessIfUnityWillSerialize(elementType);
            }

            if (type.IsGenericType)
            {
                return false;
            }

            if (type.IsDefined<SerializableAttribute>(false))
            {
                // Before Unity 4.5, Unity did not support serialization of custom structs, only custom classes
                if (UnityVersion.IsVersionOrGreater(4, 5))
                {
                    return true;
                }
                else
                {
                    return type.IsClass;
                }
            }

            return false;
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void SerializeUnityObject(UnityEngine.Object unityObject, ref SerializationData data, bool serializeUnityFields = false, SerializationContext context = null)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

#if UNITY_EDITOR

            {
                bool pretendIsPlayer = Application.isPlaying && !UnityEditor.AssetDatabase.Contains(unityObject);

                //
                // Check if we are currently building a player by looking at a stack trace.
                // This is pretty hacky, but as far as we can tell it's the only way to do it.
                //

                if (!pretendIsPlayer)
                {
                    var stackFrames = new System.Diagnostics.StackTrace().GetFrames();
                    Type buildPipelineType = typeof(UnityEditor.BuildPipeline);

                    for (int i = 0; i < stackFrames.Length; i++)
                    {
                        var frame = stackFrames[i];

                        if (frame.GetMethod().DeclaringType == buildPipelineType)
                        {
                            pretendIsPlayer = true;
                            break;
                        }
                    }
                }

                //
                // Prefab handling
                //
                // If we're not building a player and the Unity object is a prefab instance
                // that supports special prefab serialization, we enter a special bail-out case.
                //

                if (!pretendIsPlayer)
                {
                    UnityEngine.Object prefab = null;
                    var prefabType = UnityEditor.PrefabUtility.GetPrefabType(unityObject);
                    SerializationData prefabData = default(SerializationData);

                    if (prefabType == UnityEditor.PrefabType.PrefabInstance)
                    {
                        prefab = UnityEditor.PrefabUtility.GetPrefabParent(unityObject);

                        if (prefab.SafeIsUnityNull() && !object.ReferenceEquals(data.Prefab, null))
                        {
                            // Sometimes, GetPrefabParent does not return the prefab,
                            // because Unity is just completely unreliable.
                            //
                            // In these cases, we sometimes have a reference to the
                            // prefab in the data. If so, we can use that instead.
                            //
                            // Even though that reference is "fake null".

                            prefab = data.Prefab;
                        }

                        if (!object.ReferenceEquals(prefab, null))
                        {
                            if (prefab is ISupportsPrefabSerialization)
                            {
                                prefabData = (prefab as ISupportsPrefabSerialization).SerializationData;
                            }
                            else
                            {
                                Debug.LogWarning(unityObject.name + " is a prefab instance, but the type " + unityObject.GetType().GetNiceName() + " does not implement the interface " + typeof(ISupportsPrefabSerialization).GetNiceName() + "; non-Unity-serialized data will most likely not be updated properly from the prefab any more.");
                                prefab = null;
                            }
                        }
                    }

                    if (!object.ReferenceEquals(prefab, null))
                    {
                        // We will bail out. But first...

                        if (prefabData.PrefabModifications != null && prefabData.PrefabModifications.Count > 0)
                        {
                            //
                            // This is a special case that can happen after changes to a prefab instance
                            // have been applied to the source prefab using "Apply Changes", thus copying
                            // the instances' applied changes over to the source prefab.
                            //
                            // We re-serialize the prefab, to make sure its data is properly saved.
                            // Though data saved this way will still work, it is quite inefficient.
                            //

                            // TODO: (Tor) This call may be unnecessary, check if SaveAsset always triggers serialization
                            (prefab as ISerializationCallbackReceiver).OnBeforeSerialize();

                            UnityEditor.EditorUtility.SetDirty(prefab);
                            UnityEditor.AssetDatabase.SaveAssets();

                            prefabData = (prefab as ISupportsPrefabSerialization).SerializationData;
                        }

                        // Now we determine the modifications string to keep

                        bool newModifications = false;
                        List<string> modificationsToKeep;
                        List<PrefabModification> modificationsList;
                        List<UnityEngine.Object> modificationsReferencedUnityObjects = data.PrefabModificationsReferencedUnityObjects;

                        if (RegisteredPrefabModifications.TryGetValue(unityObject, out modificationsList))
                        {
                            RegisteredPrefabModifications.Remove(unityObject);

                            // We have to generate a new prefab modification string from the registered changes
                            modificationsToKeep = SerializePrefabModifications(modificationsList, ref modificationsReferencedUnityObjects);

                            //Debug.Log("Setting new modifications: ");

                            //foreach (var mod in modificationsToKeep)
                            //{
                            //    Debug.Log("    " + mod);
                            //}

                            newModifications = true;
                        }
                        else
                        {
                            // Keep the old ones
                            modificationsToKeep = data.PrefabModifications;
                        }

                        // Make sure we have the same base data as the prefab, then change the rest
                        data = prefabData;

                        data.Prefab = prefab;
                        data.PrefabModifications = modificationsToKeep;
                        data.PrefabModificationsReferencedUnityObjects = modificationsReferencedUnityObjects;

                        if (newModifications)
                        {
                            SetUnityObjectModifications(unityObject, ref data, prefab);
                        }

                        return; // Buh bye
                    }
                }

                //
                // We are not dealing with a properly supported prefab instance if we get this far.
                // Serialize as if it isn't a prefab instance.
                //

                // Ensure there is no superfluous data left over after serialization
                // (We will reassign all necessary data.)
                data.Reset();

                DataFormat format;

                // Get the format to serialize as
                {
                    IOverridesSerializationFormat formatOverride = unityObject as IOverridesSerializationFormat;

                    if (formatOverride != null)
                    {
                        format = formatOverride.GetFormatToSerializeAs(pretendIsPlayer);
                    }
                    else if (GlobalSerializationConfig.HasInstanceLoaded)
                    {
                        if (pretendIsPlayer)
                        {
                            format = GlobalSerializationConfig.Instance.BuildSerializationFormat;
                        }
                        else
                        {
                            format = GlobalSerializationConfig.Instance.EditorSerializationFormat;
                        }
                    }
                    else if (pretendIsPlayer)
                    {
                        format = DataFormat.Binary;
                    }
                    else
                    {
                        format = DataFormat.Nodes;
                    }
                }

                if (pretendIsPlayer)
                {
                    // We pretend as though we're serializing outside of the editor
                    if (format == DataFormat.Nodes)
                    {
                        Debug.LogWarning("The serialization format '" + format.ToString() + "' is disabled in play mode, and when building a player. Defaulting to the format '" + DataFormat.Binary.ToString() + "' instead.");
                        format = DataFormat.Binary;
                    }

                    UnitySerializationUtility.SerializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, format, serializeUnityFields);
                    data.SerializedFormat = format;
                }
                else
                {
                    if (format == DataFormat.Nodes)
                    {
                        // Special case for node format
                        if (context == null)
                        {
                            using (var newContext = Cache<SerializationContext>.Claim())
                            using (var writer = new SerializationNodeDataWriter(newContext))
                            using (var resolver = Cache<UnityReferenceResolver>.Claim())
                            {
                                resolver.Value.SetReferencedUnityObjects(data.ReferencedUnityObjects);

                                newContext.Value.Config.SerializationPolicy = SerializationPolicies.Unity;
                                newContext.Value.IndexReferenceResolver = resolver.Value;

                                writer.Context = newContext;

                                UnitySerializationUtility.SerializeUnityObject(unityObject, writer, serializeUnityFields);
                                data.SerializationNodes = writer.Nodes;
                                data.ReferencedUnityObjects = resolver.Value.GetReferencedUnityObjects();
                            }
                        }
                        else
                        {
                            using (var writer = new SerializationNodeDataWriter(context))
                            using (var resolver = Cache<UnityReferenceResolver>.Claim())
                            {
                                resolver.Value.SetReferencedUnityObjects(data.ReferencedUnityObjects);
                                context.IndexReferenceResolver = resolver.Value;

                                UnitySerializationUtility.SerializeUnityObject(unityObject, writer, serializeUnityFields);
                                data.SerializationNodes = writer.Nodes;
                                data.ReferencedUnityObjects = resolver.Value.GetReferencedUnityObjects();
                            }
                        }
                    }
                    else
                    {
                        UnitySerializationUtility.SerializeUnityObject(unityObject, ref data.SerializedBytesString, ref data.ReferencedUnityObjects, format, serializeUnityFields, context);
                    }

                    data.SerializedFormat = format;
                }
            }
#else
            {
                DataFormat format;
                IOverridesSerializationFormat formatOverride = unityObject as IOverridesSerializationFormat;

                if (formatOverride != null)
                {
                    format = formatOverride.GetFormatToSerializeAs(true);
                }
                else if (GlobalSerializationConfig.HasInstanceLoaded)
                {
                    format = GlobalSerializationConfig.Instance.BuildSerializationFormat;
                }
                else
                {
                    format = DataFormat.Binary;
                }

                if (format == DataFormat.Nodes)
                {
                    Debug.LogWarning("The serialization format '" + format.ToString() + "' is disabled outside of the editor. Defaulting to the format '" + DataFormat.Binary.ToString() + "' instead.");
                    format = DataFormat.Binary;
                }

                UnitySerializationUtility.SerializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, format);
                data.SerializedFormat = format;
            }
#endif
        }

#if UNITY_EDITOR

        private static void SetUnityObjectModifications(UnityEngine.Object unityObject, ref SerializationData data, UnityEngine.Object prefab)
        {
            //
            // We need to set the modifications to the prefab instance manually,
            // to ensure that Unity gets it right and doesn't mess with them.
            //

            Type unityObjectType = unityObject.GetType();
            var serializedDataField = unityObjectType.GetAllMembers<FieldInfo>(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                                     .Where(field => field.FieldType == typeof(SerializationData) && UnitySerializationUtility.GuessIfUnityWillSerialize(field))
                                                     .LastOrDefault();

            if (serializedDataField == null)
            {
                Debug.LogError("Could not find a field of type " + typeof(SerializationData).Name + " on the serializing type " + unityObjectType.GetNiceName() + " when trying to manually set prefab modifications. It is possible that prefab instances of this type will be corrupted if changes are ever applied to prefab.", prefab);
            }
            else
            {
                string serializedDataPath = serializedDataField.Name + ".";
                string referencedUnityObjectsPath = serializedDataPath + SerializationData.PrefabModificationsReferencedUnityObjectsFieldName + ".Array.";
                string modificationsPath = serializedDataPath + SerializationData.PrefabModificationsFieldName + ".Array.";
                string prefabPath = serializedDataPath + SerializationData.PrefabFieldName;

                var mods = UnityEditor.PrefabUtility.GetPropertyModifications(unityObject).ToList();

                //
                // Clear all old modifications to serialized data out
                //

                for (int i = 0; i < mods.Count; i++)
                {
                    var mod = mods[i];

                    if (mod.propertyPath.StartsWith(serializedDataPath, StringComparison.InvariantCulture))
                    {
                        mods.RemoveAt(i);
                        i--;
                    }
                }

                //
                // Add the new modifications
                //

                // Array length changes seem to always come first? Let's do that to be sure...
                mods.Insert(0, new UnityEditor.PropertyModification()
                {
                    target = prefab,
                    propertyPath = referencedUnityObjectsPath + "size",
                    value = data.PrefabModificationsReferencedUnityObjects.Count.ToString("D", CultureInfo.InvariantCulture)
                });

                mods.Insert(0, new UnityEditor.PropertyModification()
                {
                    target = prefab,
                    propertyPath = modificationsPath + "size",
                    value = data.PrefabModifications.Count.ToString("D", CultureInfo.InvariantCulture)
                });

                // Then the prefab object reference
                mods.Add(new UnityEditor.PropertyModification()
                {
                    target = prefab,
                    propertyPath = prefabPath,
                    objectReference = prefab
                });

                // Then the actual array values
                for (int i = 0; i < data.PrefabModificationsReferencedUnityObjects.Count; i++)
                {
                    mods.Add(new UnityEditor.PropertyModification()
                    {
                        target = prefab,
                        propertyPath = referencedUnityObjectsPath + "data[" + i.ToString("D", CultureInfo.InvariantCulture) + "]",
                        objectReference = data.PrefabModificationsReferencedUnityObjects[i]
                    });
                }

                for (int i = 0; i < data.PrefabModifications.Count; i++)
                {
                    mods.Add(new UnityEditor.PropertyModification()
                    {
                        target = prefab,
                        propertyPath = modificationsPath + "data[" + i.ToString("D", CultureInfo.InvariantCulture) + "]",
                        value = data.PrefabModifications[i]
                    });
                }

                // Set the Unity property modifications

                // This won't always stick; there is code in the PropertyTree class
                // that keeps checking if the number of custom modifications is correct
                // and, if not, it keeps registering the change until Unity gets it.

                UnityEditor.PrefabUtility.SetPropertyModifications(unityObject, mods.ToArray());
            }
        }

#endif

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void SerializeUnityObject(UnityEngine.Object unityObject, ref string base64Bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, bool serializeUnityFields = false, SerializationContext context = null)
        {
            byte[] bytes = null;
            SerializeUnityObject(unityObject, ref bytes, ref referencedUnityObjects, format, serializeUnityFields, context);
            base64Bytes = Convert.ToBase64String(bytes);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void SerializeUnityObject(UnityEngine.Object unityObject, ref byte[] bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, bool serializeUnityFields = false, SerializationContext context = null)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            if (format == DataFormat.Nodes)
            {
                Debug.LogError("The serialization data format '" + format.ToString() + "' is not supported by this method. You must create your own writer.");
                return;
            }

            if (referencedUnityObjects == null)
            {
                referencedUnityObjects = new List<UnityEngine.Object>();
            }
            else
            {
                referencedUnityObjects.Clear();
            }

            int previousByteCount = 100;

            if (bytes != null)
            {
                previousByteCount = bytes.Length;
            }

            // TODO: Cache and reuse memory streams?
            using (var stream = new MemoryStream((int)(previousByteCount * 1.2f)))
            using (var resolver = Cache<UnityReferenceResolver>.Claim())
            {
                resolver.Value.SetReferencedUnityObjects(referencedUnityObjects);

                if (context != null)
                {
                    context.IndexReferenceResolver = resolver.Value;
                    SerializeUnityObject(unityObject, GetCachedUnityWriter(format, stream, context), serializeUnityFields);
                }
                else
                {
                    using (var con = Cache<SerializationContext>.Claim())
                    {
                        con.Value.Config.SerializationPolicy = SerializationPolicies.Unity;

                        /* If the config instance is not loaded (it should usually be, but in rare cases
                         * it's not), we must not ask for it, as we are not allowed to load from resources
                         * or the asset database during some serialization callbacks.
                         *
                         * (Trying to do that causes internal Unity errors and potentially even crashes.)
                         *
                         * If it's not loaded, we fall back to default values, since there's no other choice.
                         */
                        if (GlobalSerializationConfig.HasInstanceLoaded)
                        {
                            //Debug.Log("Serializing " + unityObject.GetType().Name + " WITH loaded!");
                            con.Value.Config.DebugContext.ErrorHandlingPolicy = GlobalSerializationConfig.Instance.ErrorHandlingPolicy;
                            con.Value.Config.DebugContext.LoggingPolicy = GlobalSerializationConfig.Instance.LoggingPolicy;
                            con.Value.Config.DebugContext.Logger = GlobalSerializationConfig.Instance.Logger;
                        }
                        else
                        {
                            //Debug.Log("Serializing " + unityObject.GetType().Name + " WITHOUT loaded!");
                            con.Value.Config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient;
                            con.Value.Config.DebugContext.LoggingPolicy = LoggingPolicy.LogErrors;
                            con.Value.Config.DebugContext.Logger = DefaultLoggers.UnityLogger;
                        }

                        con.Value.IndexReferenceResolver = resolver.Value;

                        SerializeUnityObject(unityObject, GetCachedUnityWriter(format, stream, con), serializeUnityFields);
                    }
                }

                bytes = stream.ToArray();
            }
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void SerializeUnityObject(UnityEngine.Object unityObject, IDataWriter writer, bool serializeUnityFields = false)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            writer.PrepareNewSerializationSession();

            var members = FormatterUtilities.GetSerializableMembers(unityObject.GetType(), writer.Context.Config.SerializationPolicy);
            object unityObjectInstance = unityObject;

            for (int i = 0; i < members.Length; i++)
            {
                var member = members[i];
                WeakValueGetter getter = null;

                if (!OdinWillSerialize(member, serializeUnityFields) || (getter = GetCachedUnityMemberGetter(member)) == null)
                {
                    continue;
                }

                var value = getter(ref unityObjectInstance);

                bool isNull = object.ReferenceEquals(value, null);

                // Never serialize serialization data. That way lies madness.
                if (!isNull && value.GetType() == typeof(SerializationData))
                {
                    continue;
                }

                Serializer serializer;

                if (!isNull && FormatterUtilities.IsPrimitiveType(value.GetType()) && FormatterUtilities.GetContainedType(member) == typeof(object))
                {
                    // We are writing a boxed primitive type, so pretend it's an object
                    serializer = Serializer.Get<object>();
                }
                else
                {
                    serializer = Serializer.GetForValue(value);
                }

                try
                {
                    serializer.WriteValueWeak(member.Name, value, writer);
                }
                catch (Exception ex)
                {
                    writer.Context.Config.DebugContext.LogException(ex);
                }
            }

            writer.FlushToStream();
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref SerializationData data, DeserializationContext context = null)
        {
#if UNITY_EDITOR
            DeserializeUnityObjectInEditor(unityObject, ref data, context, isPrefabData: false);
#else
            UnitySerializationUtility.DeserializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, data.SerializedFormat, context);
            data = default(SerializationData); // Free all data for GC
#endif
        }

#if UNITY_EDITOR

        private static void DeserializeUnityObjectInEditor(UnityEngine.Object unityObject, ref SerializationData data, DeserializationContext context, bool isPrefabData)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            if (data.SerializedBytes != null && data.SerializedBytes.Length > 0)
            {
                // If it happens that we have bytes in the serialized bytes array
                // then we deserialize from that instead.

                // This happens often in play mode, when instantiating, since we
                // are emulating build behaviour.
                UnitySerializationUtility.DeserializeUnityObject(unityObject, ref data.SerializedBytes, ref data.ReferencedUnityObjects, data.SerializedFormat, context);
            }
            else
            {
                Cache<DeserializationContext> cachedContext = null;

                try
                {
                    if (context == null)
                    {
                        cachedContext = Cache<DeserializationContext>.Claim();
                        context = cachedContext;

                        context.Config.SerializationPolicy = SerializationPolicies.Unity;

                        /* If the config instance is not loaded (it should usually be, but in rare cases
                         * it's not), we must not ask for it, as we are not allowed to load from resources
                         * or the asset database during some serialization callbacks.
                         *
                         * (Trying to do that causes internal Unity errors and potentially even crashes.)
                         *
                         * If it's not loaded, we fall back to default values, since there's no other choice.
                         */
                        if (GlobalSerializationConfig.HasInstanceLoaded)
                        {
                            //Debug.Log("Deserializing " + unityObject.GetType().Name + " WITH loaded!");
                            context.Config.DebugContext.ErrorHandlingPolicy = GlobalSerializationConfig.Instance.ErrorHandlingPolicy;
                            context.Config.DebugContext.LoggingPolicy = GlobalSerializationConfig.Instance.LoggingPolicy;
                            context.Config.DebugContext.Logger = GlobalSerializationConfig.Instance.Logger;
                        }
                        else
                        {
                            //Debug.Log("Deserializing " + unityObject.GetType().Name + " WITHOUT loaded!");
                            context.Config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient;
                            context.Config.DebugContext.LoggingPolicy = LoggingPolicy.LogErrors;
                            context.Config.DebugContext.Logger = DefaultLoggers.UnityLogger;
                        }
                    }

                    if (!isPrefabData && !data.Prefab.SafeIsUnityNull())
                    {
                        if (data.Prefab is ISupportsPrefabSerialization)
                        {
                            if (object.ReferenceEquals(data.Prefab, unityObject) && data.PrefabModifications != null && data.PrefabModifications.Count > 0)
                            {
                                // We are deserializing a prefab, which has *just* had changes applied
                                // from an instance of itself.
                                //
                                // This is the only place, anywhere, where we can detect this happening
                                // so we need to register it, so the prefab instance that just applied
                                // its values knows to wipe all of its modifications clean.

                                PrefabsWithValuesApplied.Add(unityObject);
                            }
                            else
                            {
                                // We are dealing with a prefab instance, which is a special bail-out case
                                SerializationData prefabData = (data.Prefab as ISupportsPrefabSerialization).SerializationData;

                                if (PrefabsWithValuesApplied.Contains(data.Prefab))
                                {
                                    // Our prefab has had values applied; now to check if the object we're
                                    // deserializing was the one to apply those values. If it is, then we
                                    // have to wipe all of this object's prefab modifications clean.
                                    //
                                    // So far, the only way we know how to do that, is checking whether this
                                    // object is currently selected.

                                    if (IsCurrentlySelectedObject(unityObject))
                                    {
                                        PrefabsWithValuesApplied.Remove(data.Prefab);

                                        if (data.PrefabModifications != null)
                                        {
                                            data.PrefabModifications.Clear();
                                        }

                                        if (data.PrefabModificationsReferencedUnityObjects != null)
                                        {
                                            data.PrefabModificationsReferencedUnityObjects.Clear();
                                        }

                                        RegisterPrefabModificationsChange(unityObject, new List<PrefabModification>());
                                    }
                                }

                                if (!prefabData.HasEditorData)
                                {
                                    // Sometimes, the prefab hasn't actually been deserialized yet, because
                                    // Unity doesn't do anything in a sensible way at all.
                                    //
                                    // In this case, we have to deserialize from our own data, and just
                                    // pretend it's the prefab's data. We can just hope Unity hasn't messed
                                    // with the serialized data; it should be the same on this instance as
                                    // it is on the prefab itself.
                                    //
                                    // This case occurs often during editor recompile reloads.

                                    DeserializeUnityObjectInEditor(unityObject, ref data, context, isPrefabData: true);
                                }
                                else
                                {
                                    // Deserialize the current object with the prefab's data
                                    DeserializeUnityObjectInEditor(unityObject, ref prefabData, context, isPrefabData: true);
                                }

                                // Then apply the prefab modifications using the deserialization context
                                ApplyPrefabModifications(unityObject, data.PrefabModifications, data.PrefabModificationsReferencedUnityObjects);

                                return; // Buh bye
                            }
                        }
                        else
                        {
                            Debug.LogWarning("The type " + data.Prefab.GetType().GetNiceName() + " no longer supports special prefab serialization (the interface " + typeof(ISupportsPrefabSerialization).GetNiceName() + ") upon deserialization of an instance of a prefab; prefab data may be lost.");
                        }
                    }

                    if (data.SerializedFormat == DataFormat.Nodes)
                    {
                        // Special case for node format
                        using (var reader = new SerializationNodeDataReader(context))
                        using (var resolver = Cache<UnityReferenceResolver>.Claim())
                        {
                            resolver.Value.SetReferencedUnityObjects(data.ReferencedUnityObjects);
                            context.IndexReferenceResolver = resolver.Value;

                            reader.Nodes = data.SerializationNodes;

                            UnitySerializationUtility.DeserializeUnityObject(unityObject, reader);
                        }
                    }
                    else
                    {
                        UnitySerializationUtility.DeserializeUnityObject(unityObject, ref data.SerializedBytesString, ref data.ReferencedUnityObjects, data.SerializedFormat, context);
                    }

                    if (data.PrefabModifications != null && data.PrefabModifications.Count > 0)
                    {
                        // We may have a prefab that has had changes applied to it; either way, apply the stored modifications.
                        ApplyPrefabModifications(unityObject, data.PrefabModifications, data.PrefabModificationsReferencedUnityObjects);
                    }
                }
                finally
                {
                    if (cachedContext != null)
                    {
                        Cache<DeserializationContext>.Release(cachedContext);
                    }
                }
            }
        }

#endif

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref string base64Bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, DeserializationContext context = null)
        {
            if (string.IsNullOrEmpty(base64Bytes))
            {
                return;
            }

            byte[] bytes = Convert.FromBase64String(base64Bytes);
            DeserializeUnityObject(unityObject, ref bytes, ref referencedUnityObjects, format, context);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void DeserializeUnityObject(UnityEngine.Object unityObject, ref byte[] bytes, ref List<UnityEngine.Object> referencedUnityObjects, DataFormat format, DeserializationContext context = null)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            if (bytes == null || bytes.Length == 0)
            {
                return;
            }

            if (format == DataFormat.Nodes)
            {
                Debug.LogError("The serialization data format '" + format.ToString() + "' is not supported by this method. You must create your own reader.");
                return;
            }

            if (referencedUnityObjects == null)
            {
                referencedUnityObjects = new List<UnityEngine.Object>();
            }

            // TODO: Cache and reuse memory streams?
            using (var stream = new MemoryStream(bytes))
            using (var resolver = Cache<UnityReferenceResolver>.Claim())
            {
                resolver.Value.SetReferencedUnityObjects(referencedUnityObjects);

                if (context != null)
                {
                    context.IndexReferenceResolver = resolver.Value;
                    DeserializeUnityObject(unityObject, GetCachedUnityReader(format, stream, context));
                }
                else
                {
                    using (var con = Cache<DeserializationContext>.Claim())
                    {
                        con.Value.Config.SerializationPolicy = SerializationPolicies.Unity;

                        /* If the config instance is not loaded (it should usually be, but in rare cases
                         * it's not), we must not ask for it, as we are not allowed to load from resources
                         * or the asset database during some serialization callbacks.
                         *
                         * (Trying to do that causes internal Unity errors and potentially even crashes.)
                         *
                         * If it's not loaded, we fall back to default values, since there's no other choice.
                         */
                        if (GlobalSerializationConfig.HasInstanceLoaded)
                        {
                            //Debug.Log("Deserializing " + unityObject.GetType().Name + " WITH loaded!");
                            con.Value.Config.DebugContext.ErrorHandlingPolicy = GlobalSerializationConfig.Instance.ErrorHandlingPolicy;
                            con.Value.Config.DebugContext.LoggingPolicy = GlobalSerializationConfig.Instance.LoggingPolicy;
                            con.Value.Config.DebugContext.Logger = GlobalSerializationConfig.Instance.Logger;
                        }
                        else
                        {
                            //Debug.Log("Deserializing " + unityObject.GetType().Name + " WITHOUT loaded!");
                            con.Value.Config.DebugContext.ErrorHandlingPolicy = ErrorHandlingPolicy.Resilient;
                            con.Value.Config.DebugContext.LoggingPolicy = LoggingPolicy.LogErrors;
                            con.Value.Config.DebugContext.Logger = DefaultLoggers.UnityLogger;
                        }

                        con.Value.IndexReferenceResolver = resolver.Value;

                        DeserializeUnityObject(unityObject, GetCachedUnityReader(format, stream, con));
                    }
                }
            }
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void DeserializeUnityObject(UnityEngine.Object unityObject, IDataReader reader)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            if (reader == null)
            {
                throw new ArgumentNullException("reader");
            }

            reader.PrepareNewSerializationSession();

            var members = FormatterUtilities.GetSerializableMembersMap(unityObject.GetType(), reader.Context.Config.SerializationPolicy);

            int count = 0;
            string name;
            EntryType entryType;
            object unityObjectInstance = unityObject;

            while ((entryType = reader.PeekEntry(out name)) != EntryType.EndOfNode && entryType != EntryType.EndOfArray && entryType != EntryType.EndOfStream)
            {
                MemberInfo member = null;
                WeakValueSetter setter = null;

                bool skip = false;

                if (string.IsNullOrEmpty(name))
                {
                    reader.Context.Config.DebugContext.LogError("Entry of type \"" + entryType + "\" in node \"" + reader.CurrentNodeName + "\" is missing a name.");
                    skip = true;
                }
                else if (members.TryGetValue(name, out member) == false || (setter = GetCachedUnityMemberSetter(member)) == null)
                {
                    skip = true;
                }

                if (skip)
                {
                    reader.SkipEntry();
                    continue;
                }

                {
                    Type expectedType = FormatterUtilities.GetContainedType(member);
                    Serializer serializer = Serializer.Get(expectedType);

                    try
                    {
                        object value = serializer.ReadValueWeak(reader);
                        setter(ref unityObjectInstance, value);
                    }
                    catch (Exception ex)
                    {
                        reader.Context.Config.DebugContext.LogException(ex);
                    }
                }

                count++;

                if (count > 1000)
                {
                    reader.Context.Config.DebugContext.LogError("Breaking out of infinite reading loop! (Read more than a thousand entries for one type!)");
                    break;
                }
            }
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static List<string> SerializePrefabModifications(List<PrefabModification> modifications, ref List<UnityEngine.Object> referencedUnityObjects)
        {
            if (modifications == null || modifications.Count == 0)
            {
                return new List<string>();
            }

            if (referencedUnityObjects == null)
            {
                referencedUnityObjects = new List<UnityEngine.Object>();
            }
            else if (referencedUnityObjects.Count > 0)
            {
                referencedUnityObjects.Clear();
            }

            // Sort modifications alphabetically by path; this will ensure that modifications
            // to child paths are always applied after modifications to the parent paths
            modifications.Sort((a, b) =>
            {
                int compared = a.Path.CompareTo(b.Path);

                if (compared == 0)
                {
                    if ((a.ModificationType == PrefabModificationType.ListLength || a.ModificationType == PrefabModificationType.Dictionary) && b.ModificationType == PrefabModificationType.Value)
                    {
                        return 1;
                    }
                    else if (a.ModificationType == PrefabModificationType.Value && (b.ModificationType == PrefabModificationType.ListLength || b.ModificationType == PrefabModificationType.Dictionary))
                    {
                        return -1;
                    }
                }

                return compared;
            });

            List<string> result = new List<string>();

            using (var context = Cache<SerializationContext>.Claim())
            using (var stream = new MemoryStream())
            using (var writer = (JsonDataWriter)GetCachedUnityWriter(DataFormat.JSON, stream, context))
            using (var resolver = Cache<UnityReferenceResolver>.Claim())
            {
                writer.PrepareNewSerializationSession();
                writer.FormatAsReadable = false;
                writer.EnableTypeOptimization = false;

                resolver.Value.SetReferencedUnityObjects(referencedUnityObjects);
                writer.Context.IndexReferenceResolver = resolver.Value;

                for (int i = 0; i < modifications.Count; i++)
                {
                    var mod = modifications[i];

                    if (mod.ModificationType == PrefabModificationType.ListLength)
                    {
                        writer.MarkJustStarted();
                        writer.WriteString("path", mod.Path);
                        writer.WriteInt32("length", mod.NewLength);

                        writer.FlushToStream();
                        result.Add(GetStringFromStreamAndReset(stream));
                    }
                    else if (mod.ModificationType == PrefabModificationType.Value)
                    {
                        writer.MarkJustStarted();
                        writer.WriteString("path", mod.Path);

                        if (mod.ReferencePaths != null && mod.ReferencePaths.Count > 0)
                        {
                            writer.BeginStructNode("references", null);
                            {
                                for (int j = 0; j < mod.ReferencePaths.Count; j++)
                                {
                                    writer.WriteString(null, mod.ReferencePaths[j]);
                                }
                            }
                            writer.EndNode("references");
                        }

                        var serializer = Serializer.Get<object>();
                        serializer.WriteValueWeak("value", mod.ModifiedValue, writer);

                        writer.FlushToStream();
                        result.Add(GetStringFromStreamAndReset(stream));
                    }
                    else if (mod.ModificationType == PrefabModificationType.Dictionary)
                    {
                        writer.MarkJustStarted();
                        writer.WriteString("path", mod.Path);

                        Serializer.Get<object[]>().WriteValue("add_keys", mod.DictionaryKeysAdded, writer);
                        Serializer.Get<object[]>().WriteValue("remove_keys", mod.DictionaryKeysRemoved, writer);

                        writer.FlushToStream();
                        result.Add(GetStringFromStreamAndReset(stream));
                    }

                    // We don't want modifications to be able to reference each other
                    writer.Context.ResetInternalReferences();
                }
            }

            return result;
        }

        private static string GetStringFromStreamAndReset(Stream stream)
        {
            byte[] bytes = new byte[stream.Position];
            stream.Position = 0;
            stream.Read(bytes, 0, bytes.Length);
            stream.Position = 0;

            return Encoding.UTF8.GetString(bytes);
        }

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static List<PrefabModification> DeserializePrefabModifications(List<string> modifications, List<UnityEngine.Object> referencedUnityObjects)
        {
            if (modifications == null || modifications.Count == 0)
            {
                // Nothing to apply
                return new List<PrefabModification>();
            }

            List<PrefabModification> result = new List<PrefabModification>();

            int longestByteCount = 0;

            for (int i = 0; i < modifications.Count; i++)
            {
                int count = modifications[i].Length * 2;

                if (count > longestByteCount)
                {
                    longestByteCount = count;
                }
            }

            using (var context = Cache<DeserializationContext>.Claim())
            using (var stream = new MemoryStream(longestByteCount))
            using (var reader = (JsonDataReader)GetCachedUnityReader(DataFormat.JSON, stream, context))
            using (var resolver = Cache<UnityReferenceResolver>.Claim())
            {
                resolver.Value.SetReferencedUnityObjects(referencedUnityObjects);
                reader.Context.IndexReferenceResolver = resolver.Value;

                for (int i = 0; i < modifications.Count; i++)
                {
                    string modStr = modifications[i];
                    byte[] bytes = Encoding.UTF8.GetBytes(modStr);

                    stream.SetLength(bytes.Length);
                    stream.Position = 0;
                    stream.Write(bytes, 0, bytes.Length);
                    stream.Position = 0;

                    PrefabModification modification = new PrefabModification();

                    string entryName;
                    EntryType entryType;

                    reader.PrepareNewSerializationSession();

                    entryType = reader.PeekEntry(out entryName);

                    if (entryType == EntryType.EndOfStream)
                    {
                        // We might have reached the end of stream from a prior modification string
                        // If we have, force our way into the first entry in the new string
                        reader.SkipEntry();
                    }

                    while ((entryType = reader.PeekEntry(out entryName)) != EntryType.EndOfNode && entryType != EntryType.EndOfArray && entryType != EntryType.EndOfStream)
                    {
                        if (entryName == null)
                        {
                            Debug.LogError("Unexpected entry of type " + entryType + " without a name.");
                            reader.SkipEntry();
                            continue;
                        }

                        if (entryName.Equals("path", StringComparison.InvariantCultureIgnoreCase))
                        {
                            reader.ReadString(out modification.Path);
                        }
                        else if (entryName.Equals("length", StringComparison.InvariantCultureIgnoreCase))
                        {
                            reader.ReadInt32(out modification.NewLength);
                            modification.ModificationType = PrefabModificationType.ListLength;
                        }
                        else if (entryName.Equals("references", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modification.ReferencePaths = new List<string>();

                            Type dummy;
                            reader.EnterNode(out dummy);
                            {
                                while (reader.PeekEntry(out entryName) == EntryType.String)
                                {
                                    string path;
                                    reader.ReadString(out path);
                                    modification.ReferencePaths.Add(path);
                                }
                            }
                            reader.ExitNode();
                        }
                        else if (entryName.Equals("value", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modification.ModifiedValue = Serializer.Get<object>().ReadValue(reader);
                            modification.ModificationType = PrefabModificationType.Value;
                        }
                        else if (entryName.Equals("add_keys", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modification.DictionaryKeysAdded = Serializer.Get<object[]>().ReadValue(reader);
                            modification.ModificationType = PrefabModificationType.Dictionary;
                        }
                        else if (entryName.Equals("remove_keys", StringComparison.InvariantCultureIgnoreCase))
                        {
                            modification.DictionaryKeysRemoved = Serializer.Get<object[]>().ReadValue(reader);
                            modification.ModificationType = PrefabModificationType.Dictionary;
                        }
                        else
                        {
                            Debug.LogError("Unexpected entry name '" + entryName + "' while deserializing prefab modifications.");
                            reader.SkipEntry();
                        }
                    }

                    if (modification.Path == null)
                    {
                        Debug.LogWarning("Error when deserializing prefab modification; no path found. Modification lost; string was: '" + modStr + "'.");
                        continue;
                    }

                    result.Add(modification);
                }
            }

            return result;
        }

#if UNITY_EDITOR

        /// <summary>
        /// Not yet documented.
        /// </summary>
        public static void RegisterPrefabModificationsChange(UnityEngine.Object unityObject, List<PrefabModification> modifications)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            //Debug.Log((Event.current == null ? "NO EVENT" : Event.current.type.ToString()) + ": Registering " + (modifications == null ? 0 : modifications.Count) + " modifications to " + unityObject.name + ":");

            //for (int i = 0; i < modifications.Count; i++)
            //{
            //    var mod = modifications[i];

            //    if (mod.ModificationType == PrefabModificationType.ListLength)
            //    {
            //        Debug.Log("    LENGTH@" + mod.Path + ": " + mod.NewLength);
            //    }
            //    else if (mod.ModificationType == PrefabModificationType.Dictionary)
            //    {
            //        Debug.Log("    DICT@" + mod.Path + ": (add: " + (mod.DictionaryKeysAdded != null ? mod.DictionaryKeysAdded.Length : 0) + ") -- (remove: " + (mod.DictionaryKeysRemoved != null ? mod.DictionaryKeysRemoved.Length : 0) + ")");
            //    }
            //    else
            //    {
            //        Debug.Log("    VALUE@" + mod.Path + ": " + mod.ModifiedValue);
            //    }
            //}

            RegisteredPrefabModifications[unityObject] = modifications;
        }

#endif

        private static void ApplyPrefabModifications(UnityEngine.Object unityObject, List<string> modificationData, List<UnityEngine.Object> referencedUnityObjects)
        {
            if (unityObject == null)
            {
                throw new ArgumentNullException("unityObject");
            }

            if (modificationData == null || modificationData.Count == 0)
            {
                // Nothing to apply.
                return;
            }

            var modifications = DeserializePrefabModifications(modificationData, referencedUnityObjects);

            for (int i = 0; i < modifications.Count; i++)
            {
                var mod = modifications[i];

                try
                {
                    mod.Apply(unityObject);
                }
                catch (Exception ex)
                {
                    Debug.Log("The following exception was thrown when trying to apply a prefab modification for path '" + mod.Path + "':");
                    Debug.LogException(ex);
                }
            }
        }

        private static WeakValueGetter GetCachedUnityMemberGetter(MemberInfo member)
        {
            WeakValueGetter result;

            if (UnityMemberGetters.TryGetValue(member, out result) == false)
            {
                if (member is FieldInfo)
                {
                    result = EmitUtilities.CreateWeakInstanceFieldGetter(member.DeclaringType, member as FieldInfo);
                }
                else if (member is PropertyInfo)
                {
                    result = EmitUtilities.CreateWeakInstancePropertyGetter(member.DeclaringType, member as PropertyInfo);
                }
                else
                {
                    result = delegate (ref object instance)
                    {
                        return FormatterUtilities.GetMemberValue(member, instance);
                    };
                }

                UnityMemberGetters.Add(member, result);
            }

            return result;
        }

        private static WeakValueSetter GetCachedUnityMemberSetter(MemberInfo member)
        {
            WeakValueSetter result;

            if (UnityMemberSetters.TryGetValue(member, out result) == false)
            {
                if (member is FieldInfo)
                {
                    result = EmitUtilities.CreateWeakInstanceFieldSetter(member.DeclaringType, member as FieldInfo);
                }
                else if (member is PropertyInfo)
                {
                    result = EmitUtilities.CreateWeakInstancePropertySetter(member.DeclaringType, member as PropertyInfo);
                }
                else
                {
                    result = delegate (ref object instance, object value)
                    {
                        FormatterUtilities.SetMemberValue(member, instance, value);
                    };
                }

                UnityMemberSetters.Add(member, result);
            }

            return result;
        }

        private static IDataWriter GetCachedUnityWriter(DataFormat format, Stream stream, SerializationContext context)
        {
            IDataWriter writer;

            if (UnityWriters.TryGetValue(format, out writer) == false)
            {
                writer = SerializationUtility.CreateWriter(stream, context, format);
                UnityWriters.Add(format, writer);
            }
            else
            {
                writer.Context = context;
                writer.Stream = stream;
            }

            return writer;
        }

        private static IDataReader GetCachedUnityReader(DataFormat format, Stream stream, DeserializationContext context)
        {
            IDataReader reader;

            if (UnityReaders.TryGetValue(format, out reader) == false)
            {
                reader = SerializationUtility.CreateReader(stream, context, format);
                UnityReaders.Add(format, reader);
            }
            else
            {
                reader.Context = context;
                reader.Stream = stream;
            }

            return reader;
        }

#if UNITY_EDITOR

        [UnityEditor.InitializeOnLoad]
        private static class SelectionTracker
        {
            public static List<UnityEngine.Object> SelectedObjects { get; private set; }

            static SelectionTracker()
            {
                SelectedObjects = new List<UnityEngine.Object>();
                UnityEditor.Selection.selectionChanged += OnSelectionChanged;
                OnSelectionChanged();
            }

            private static void OnSelectionChanged()
            {
                SelectedObjects.Clear();

                var selection = UnityEditor.Selection.objects;

                SelectedObjects.AddRange(selection);

                for (int i = 0; i < selection.Length; i++)
                {
                    var obj = selection[i];

                    GameObject gameObject = obj as GameObject;

                    if (!gameObject.SafeIsUnityNull())
                    {
                        SelectedObjects.AddRange(gameObject.GetComponents(typeof(Component)));
                    }
                }
            }
        }

        private static bool IsCurrentlySelectedObject(UnityEngine.Object obj)
        {
            // TODO: Support apply prefab from hierarchy parent prefab as well

            var selectedObjects = SelectionTracker.SelectedObjects;

            for (int i = 0; i < selectedObjects.Count; i++)
            {
                if (object.ReferenceEquals(obj, selectedObjects[i]))
                {
                    return true;
                }
            }

            return false;
        }

#endif
    }
}