#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="InspectorPropertyInfo.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor
{
    using Serialization;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using UnityEditor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// Contains meta-data information about a property in the inspector.
    /// </summary>
    public abstract class InspectorPropertyInfo : IValueGetterSetter
    {
        private static readonly DoubleLookupDictionary<Type, bool, InspectorPropertyInfo[]> PropertyInfoCache = new DoubleLookupDictionary<Type, bool, InspectorPropertyInfo[]>();

        private static readonly HashSet<string> AlwaysSkipUnityProperties = new HashSet<string>()
        {
            "m_PathID",
            "m_FileID",
            "m_ObjectHideFlags",
            "m_PrefabParentObject",
            "m_PrefabInternal",
            "m_PrefabInternal",
            "m_GameObject",
            "m_Enabled",
            "m_Script",
            "m_EditorHideFlags",
            "m_EditorClassIdentifier",
        };

        private static readonly HashSet<string> AlwaysSkipUnityPropertiesForComponents = new HashSet<string>()
        {
            "m_Name",
        };

        private static readonly DoubleLookupDictionary<Type, string, string> UnityPropertyMemberNameReplacements = new DoubleLookupDictionary<Type, string, string>()
        {
            { typeof(Bounds), new Dictionary<string, string>() {
                { "m_Extent", "m_Extents" }
            } }
        };

        private static readonly HashSet<Type> NeverProcessUnityPropertiesFor = new HashSet<Type>()
        {
            typeof(Matrix4x4)
        };

        private static readonly HashSet<Type> AlwaysSkipUnityPropertiesDeclaredBy = new HashSet<Type>()
        {
            typeof(UnityEngine.Object),
            typeof(ScriptableObject),
            typeof(Component),
            typeof(Behaviour),
            typeof(MonoBehaviour),
            typeof(StateMachineBehaviour),
        };

        private readonly MemberInfo[] memberInfos;
        private Attribute[] attributes;
        private Type typeOfOwner;
        private Type typeOfValue;

        /// <summary>
        /// The name of the property.
        /// </summary>
        public string PropertyName { get; private set; }

        /// <summary>
        /// The member info of the property. If the property has many member infos, such as if it is a group property, the first member info of <see cref="MemberInfos"/> is returned.
        /// </summary>
        public MemberInfo MemberInfo { get { return this.memberInfos.Length == 0 ? null : this.memberInfos[0]; } }

        /// <summary>
        /// Indicates which type of property it is.
        /// </summary>
        public PropertyType PropertyType { get; private set; }

        /// <summary>
        /// The serialization backend for this property.
        /// </summary>
        public SerializationBackend SerializationBackend { get; private set; }

        /// <summary>
        /// The type on which this property is declared.
        /// </summary>
        public Type TypeOfOwner { get { return this.typeOfOwner; } }

        /// <summary>
        /// The base type of the value which this property represents.
        /// </summary>
        public Type TypeOfValue { get { return this.typeOfValue; } }

        /// <summary>
        /// Whether this property is editable or not.
        /// </summary>
        public bool IsEditable { get; private set; }

        /// <summary>
        /// All member infos of the property. There will only be more than one member if it is an <see cref="InspectorPropertyGroupInfo"/>.
        /// </summary>
        public MemberInfo[] MemberInfos { get { return this.memberInfos; } }

        /// <summary>
        /// The order value of this property. Properties are ordered by ascending order, IE, lower order values are shown first in the inspector.
        /// </summary>
        public int Order { get; private set; }

        /// <summary>
        /// The attributes associated with this property.
        /// </summary>
        public Attribute[] Attributes { get { return this.attributes; } }

        /// <summary>
        /// Whether this property only exists as a Unity <see cref="SerializedProperty"/>, and has no associated managed member to represent it.
        /// </summary>
        public virtual bool IsUnityPropertyOnly { get { return false; } } // Return false to remove warning?

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPropertyInfo"/> class.
        /// </summary>
        /// <param name="memberInfo">The member to represent.</param>
        /// <param name="propertyType">Type of the property.</param>
        /// <param name="serializationBackend">The serialization backend.</param>
        /// <param name="allowEditable">Whether the property is editable.</param>
        /// <exception cref="System.ArgumentNullException">memberInfo is null</exception>
        /// <exception cref="System.ArgumentException">Cannot greate a property group for only one member.</exception>
        protected internal InspectorPropertyInfo(MemberInfo memberInfo, PropertyType propertyType, SerializationBackend serializationBackend, bool allowEditable)
        {
            if (memberInfo == null)
            {
                throw new ArgumentNullException("memberInfo");
            }

            if (propertyType == PropertyType.Group)
            {
                throw new ArgumentException("Cannot create a property group for only one member.");
            }

            this.memberInfos = new MemberInfo[] { memberInfo };
            this.PropertyName = memberInfo.Name;
            this.PropertyType = propertyType;
            this.SerializationBackend = serializationBackend;

            this.typeOfOwner = memberInfo.DeclaringType;

            if (memberInfo is FieldInfo || memberInfo is PropertyInfo)
            {
                this.typeOfValue = memberInfo.GetReturnType();
            }

            var propertyInfo = memberInfo as PropertyInfo;
            this.IsEditable = memberInfo.IsDefined<ReadOnlyAttribute>(true) == false &&
                              (propertyInfo == null || propertyInfo.CanWrite);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPropertyInfo"/> class.
        /// </summary>
        /// <param name="groupId">The group identifier.</param>
        /// <param name="infos">The member infos.</param>
        /// <exception cref="System.ArgumentNullException">
        /// groupId is null
        /// or
        /// infos is null
        /// </exception>
        protected internal InspectorPropertyInfo(string groupId, IList<InspectorPropertyInfo> infos)
        {
            if (groupId == null)
            {
                throw new ArgumentNullException("groupId");
            }

            if (infos == null)
            {
                throw new ArgumentNullException("infos");
            }

            this.memberInfos = infos.SelectMany(n => n.MemberInfos).ToArray();

            for (int i = 0; i < infos.Count; i++)
            {
                int order = infos[i].Order;

                if (order > this.Order)
                {
                    this.Order = order;
                }
            }

            this.typeOfOwner = this.memberInfos[0].DeclaringType;

            this.PropertyName = groupId;
            this.PropertyType = PropertyType.Group;
            this.SerializationBackend = SerializationBackend.None;
            this.IsEditable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InspectorPropertyInfo"/> class.
        /// </summary>
        /// <param name="unityPropertyName">Name of the unity property.</param>
        /// <param name="typeOfOwner">The type of owner.</param>
        /// <param name="typeOfValue">The type of value.</param>
        /// <param name="isEditable">Whether the property is editable.</param>
        /// <exception cref="System.ArgumentNullException">
        /// unityPropertyName is null
        /// or
        /// ownerType is null
        /// or
        /// valueType is null
        /// </exception>
        protected internal InspectorPropertyInfo(string unityPropertyName, Type typeOfOwner, Type typeOfValue, bool isEditable)
        {
            if (unityPropertyName == null)
            {
                throw new ArgumentNullException("unityPropertyName");
            }

            if (typeOfOwner == null)
            {
                throw new ArgumentNullException("ownerType");
            }

            if (typeOfValue == null)
            {
                throw new ArgumentNullException("valueType");
            }

            this.memberInfos = new MemberInfo[0];

            this.typeOfOwner = typeOfOwner;
            this.typeOfValue = typeOfValue;

            this.PropertyName = unityPropertyName;
            this.PropertyType = typeOfValue.IsValueType ? PropertyType.ValueType : PropertyType.ReferenceType;
            this.SerializationBackend = SerializationBackend.Unity;
            this.IsEditable = isEditable;
        }

        /// <summary>
        /// Sets the value of this property on the given owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="value">The value.</param>
        public abstract void SetValue(object owner, object value);

        /// <summary>
        /// Gets the value of this property from the given owner.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public abstract object GetValue(object owner);

        /// <summary>
        /// <para>Tries to convert this property to a strongly typed <see cref="IValueGetterSetter{TOwner, TValue}" />.</para>
        /// <para>A polymorphic alias <see cref="AliasGetterSetter{TOwner, TValue, TPropertyOwner, TPropertyValue}" /> will be created if necessary.</para>
        /// </summary>
        /// <typeparam name="TOwner">The type of the owner.</typeparam>
        /// <typeparam name="TValue">The type of the value.</typeparam>
        /// <param name="getterSetter">The converted getter setter.</param>
        /// <returns>True if the conversion succeeded, otherwise false.</returns>
        public abstract bool TryConvertToGetterSetter<TOwner, TValue>(out IValueGetterSetter<TOwner, TValue> getterSetter);

        /// <summary>
        /// Gets the first attribute of a given type on this property.
        /// </summary>
        public T GetAttribute<T>() where T : Attribute
        {
            if (this.attributes != null)
            {
                T result;

                for (int i = 0; i < this.attributes.Length; i++)
                {
                    result = this.attributes[i] as T;

                    if (result != null)
                    {
                        return result;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the first attribute of a given type on this property, which is not contained in a given hashset.
        /// </summary>
        /// <param name="exclude">The attributes to exclude.</param>
        public T GetAttribute<T>(HashSet<T> exclude) where T : Attribute
        {
            if (this.attributes != null)
            {
                for (int i = 0; i < this.attributes.Length; i++)
                {
                    T attr = this.attributes[i] as T;

                    if (attr != null && (exclude == null || !exclude.Contains(attr)))
                    {
                        return attr;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Gets all attributes of a given type on the property.
        /// </summary>
        public IEnumerable<T> GetAttributes<T>() where T : Attribute
        {
            if (this.attributes != null)
            {
                T result;

                for (int i = 0; i < this.attributes.Length; i++)
                {
                    result = this.attributes[i] as T;

                    if (result != null)
                    {
                        yield return result;
                    }
                }
            }
        }

        private static bool TryCreate(MemberInfo member, bool includeSpeciallySerializedMembers, out InspectorPropertyInfo result)
        {
            SerializationBackend? backEnd = null;

            if (member.IsDefined<ExcludeDataFromInspectorAttribute>(true))
            {
                result = null;
                return false;
            }

            bool unityWillSerialize = UnitySerializationUtility.GuessIfUnityWillSerialize(member);

            if (member.IsDefined<OdinSerializeAttribute>(true))
            {
                backEnd = SerializationBackend.Odin;
            }
            else if (unityWillSerialize)
            {
                backEnd = SerializationBackend.Unity;
            }
            else if (member is FieldInfo && ((member as FieldInfo).IsPublic || member.IsDefined<SerializeField>(true)) && !member.IsDefined<NonSerializedAttribute>(true))
            {
                backEnd = SerializationBackend.Odin;
            }
            else if (member.IsDefined<ShowInInspectorAttribute>(true))
            {
                backEnd = SerializationBackend.None;
            }

            if (backEnd == null || (backEnd == SerializationBackend.Odin && !includeSpeciallySerializedMembers))
            {
                if (unityWillSerialize)
                {
                    backEnd = SerializationBackend.Unity;
                }
                else if (member.IsDefined<ShowInInspectorAttribute>(true))
                {
                    backEnd = SerializationBackend.None;
                }
                else
                {
                    result = null;
                    return false;
                }
            }

            return TryCreate(member, backEnd.Value, true, out result);
        }

        private static bool TryCreate(MemberInfo member, SerializationBackend backEnd, bool allowEditable, out InspectorPropertyInfo result)
        {
            result = null;

            if (member is FieldInfo)
            {
                Type genericInfoType = typeof(InspectorValuePropertyInfo<,>).MakeGenericType(member.DeclaringType, (member as FieldInfo).FieldType);
                result = (InspectorPropertyInfo)Activator.CreateInstance(genericInfoType, member, backEnd, allowEditable);
            }
            else if (member is PropertyInfo)
            {
                PropertyInfo propInfo = member as PropertyInfo;
                PropertyInfo nonAliasedPropInfo = propInfo.DeAliasProperty();

                bool valid = false;

                if (backEnd == SerializationBackend.Odin)
                {
                    if (propInfo.IsDefined<ShowInInspectorAttribute>() || nonAliasedPropInfo.IsAutoProperty())
                    {
                        valid = true;
                    }
                }
                else if (propInfo.CanRead)
                {
                    valid = true;
                }

                if (valid)
                {
                    Type genericInfoType = typeof(InspectorValuePropertyInfo<,>).MakeGenericType(member.DeclaringType, propInfo.PropertyType);
                    result = (InspectorPropertyInfo)Activator.CreateInstance(genericInfoType, member, backEnd, allowEditable);
                }
            }
            else if (member is MethodInfo)
            {
                Type genericInfoType = typeof(InspectorMethodPropertyInfo<>).MakeGenericType(member.DeclaringType);
                result = (InspectorPropertyInfo)Activator.CreateInstance(genericInfoType, member);
            }

            if (result != null)
            {
                result.attributes = member.GetAttributes(true);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Gets all <see cref="InspectorPropertyInfo" />s for a given type.
        /// </summary>
        /// <param name="type">The type to get infos for.</param>
        /// <param name="includeSpeciallySerializedMembers">if set to <c>true</c> members that are serialized by Odin will be included.</param>
        /// <exception cref="System.ArgumentNullException">type is null</exception>
        public static InspectorPropertyInfo[] Get(Type type, bool includeSpeciallySerializedMembers)
        {
            if (type == null)
            {
                throw new ArgumentNullException("type");
            }

            InspectorPropertyInfo[] result;

            if (PropertyInfoCache.TryGetInnerValue(type, includeSpeciallySerializedMembers, out result) == false)
            {
                result = CreateInspectorProperties(type, includeSpeciallySerializedMembers);
                PropertyInfoCache.AddInner(type, includeSpeciallySerializedMembers, result);
            }

            return result;
        }

        private static int GetMemberOrder(MemberInfo member)
        {
            if (member == null) return 0; // This tends to happen for aliased Unity properties - pretend they're a field
            if (member is FieldInfo) return 0;
            if (member is PropertyInfo) return 1;
            if (member is MethodInfo) return 2;
            return 3;
        }

        private static InspectorPropertyInfo[] CreateInspectorProperties(Type type, bool includeSpeciallySerializedMembers)
        {
            List<InspectorPropertyInfo> result = new List<InspectorPropertyInfo>();

            var assemblyFlag = AssemblyUtilities.GetAssemblyTypeFlag(type.Assembly);

            if ((assemblyFlag == AssemblyTypeFlags.UnityEditorTypes || assemblyFlag == AssemblyTypeFlags.UnityTypes) && !NeverProcessUnityPropertiesFor.Contains(type))
            {
                // It's a Unity type - we do weird stuff for those
                PopulateUnityProperties(type, result);
            }
            else
            {
                PopulateMemberInspectorProperties(type, includeSpeciallySerializedMembers, result);
            }

            Dictionary<InspectorPropertyInfo, int> originalOrder = new Dictionary<InspectorPropertyInfo, int>();

            for (int i = 0; i < result.Count; i++)
            {
                originalOrder.Add(result[i], i);
            }

            // Sort all members by type first, to ensure consistent behaviour
            result = result.OrderBy(n => GetMemberOrder(n.MemberInfo))
                           .ThenBy(n => originalOrder[n])
                           .ToList();

            Dictionary<string, List<InspectorPropertyInfo>> groups = new Dictionary<string, List<InspectorPropertyInfo>>();

            for (int i = 0; i < result.Count; i++)
            {
                var info = result[i];

                var groupAttr = info.GetAttribute<PropertyGroupAttribute>();

                if (groupAttr != null && groupAttr.GroupID != null)
                {
                    info.Order = groupAttr.Order;

                    List<InspectorPropertyInfo> group;

                    if (!groups.TryGetValue(groupAttr.GroupID, out group))
                    {
                        group = new List<InspectorPropertyInfo>();
                        groups.Add(groupAttr.GroupID, group);
                    }

                    if (group.Count > 0 && group[0].GetAttribute<PropertyGroupAttribute>().GetType() != groupAttr.GetType())
                    {
                        Debug.LogWarning("Cannot have group attributes of different types with the same group name, on the same type (or its inherited types)! Expected attributes of type '" + group[0].GetAttribute<PropertyGroupAttribute>().GetType().GetNiceName() + "' for group '" + group[0].MemberInfo.DeclaringType.GetNiceName() + "." + groupAttr.GroupID + "', but got a group attribute of type '" + groupAttr.GetType().GetNiceName() + "' on member '" + info.MemberInfo.DeclaringType.GetNiceName() + "." + info.MemberInfo.Name + "'.");
                    }
                    else
                    {
                        result.RemoveAt(i--);
                        group.Add(info);
                    }
                }
                else
                {
                    var orderAttr = info.GetAttribute<PropertyOrderAttribute>();

                    if (orderAttr != null)
                    {
                        info.Order = orderAttr.Order;
                    }
                }
            }

            originalOrder.Clear();

            var groupsResult = new List<InspectorPropertyInfo>(groups.Count);

            foreach (var group in groups)
            {
                bool continueToNextGroup = false;

                for (int i = 0; i < result.Count; i++)
                {
                    if (result[i].PropertyName == group.Key)
                    {
                        var hiddenProperty = result[i];

                        if (hiddenProperty.PropertyType == PropertyType.Group)
                        {
                            var groupName = group.Value[0].MemberInfo.DeclaringType.GetNiceName() + "." + group.Key;
                            var otherGroupName = hiddenProperty.MemberInfo.DeclaringType.GetNiceName() + "." + hiddenProperty.PropertyName;

                            Debug.LogWarning("Property group '" + groupName + "' hides already existing group property '" + otherGroupName + "'. Group property '" + groupName + "' will be removed from the property tree.");
                            continueToNextGroup = true;
                        }
                        else
                        {
                            var alias = FormatterUtilities.GetPrivateMemberAlias(hiddenProperty.MemberInfo, hiddenProperty.MemberInfo.DeclaringType.GetNiceName(), " -> ");

                            var aliasName = alias.Name;
                            var groupName = group.Value[0].MemberInfo.DeclaringType.GetNiceName() + "." + group.Key;
                            var hiddenPropertyName = hiddenProperty.MemberInfo.DeclaringType.GetNiceName() + "." + hiddenProperty.PropertyName;

                            InspectorPropertyInfo aliasPropertyInfo;

                            if (InspectorPropertyInfo.TryCreate(alias, includeSpeciallySerializedMembers, out aliasPropertyInfo))
                            {
                                Debug.LogWarning("Property group '" + groupName + "' hides member property '" + hiddenPropertyName + "'. Alias property '" + aliasName + "' created for member property '" + hiddenPropertyName + "'.");
                                result[i] = aliasPropertyInfo;
                            }
                            else
                            {
                                Debug.LogWarning("Property group '" + groupName + "' hides member property '" + hiddenPropertyName + "'. Failed to create alias property '" + aliasName + "' for member property '" + hiddenPropertyName + "'; group property '" + groupName + "' will be removed.");
                                continueToNextGroup = true;
                            }
                        }
                    }
                }

                if (continueToNextGroup)
                {
                    continue;
                }

                originalOrder.Clear();

                int counter = 0;

                foreach (var propInfo in group.Value)
                {
                    originalOrder.Add(propInfo, counter++);
                }

                var sortedGroup = group.Value.OrderBy(n =>
                                              {
                                                  var orderAttr = n.GetAttribute<PropertyOrderAttribute>();

                                                  if (orderAttr != null)
                                                  {
                                                      return orderAttr.Order;
                                                  }

                                                  return 0;
                                              })
                                              .ThenBy(n => originalOrder[n])
                                              .ToList();

                var combinedPropertyGroupAttribute = sortedGroup[0].GetAttribute<PropertyGroupAttribute>();

                for (int i = 1; i < sortedGroup.Count; i++)
                {
                    combinedPropertyGroupAttribute = combinedPropertyGroupAttribute.Combine(sortedGroup[i].GetAttribute<PropertyGroupAttribute>());
                }

                var info = new InspectorPropertyGroupInfo(group.Key, sortedGroup);

                info.Order = combinedPropertyGroupAttribute.Order;
                info.attributes = new Attribute[] { combinedPropertyGroupAttribute }; // Property group info only have their own group attribute

                groupsResult.Add(info);
            }

            result.InsertRange(0, groupsResult);

            originalOrder.Clear();

            for (int i = 0; i < result.Count; i++)
            {
                originalOrder.Add(result[i], i);
            }

            return result.OrderBy(n => n.Order)
                         .ThenBy(n => n.MemberInfo is MethodInfo ? 1 : 0)
                         .ThenBy(n => originalOrder[n])
                         .ToArray();
        }

        private static void PopulateUnityProperties(Type type, List<InspectorPropertyInfo> result)
        {
            // Steal the properties from Unity; we have no way of knowing what Unity is going to do with this type
            SerializedProperty prop;

            if (type.IsAbstract || type.IsInterface || type.IsArray || type == typeof(AnimationCurve)) return;

            UnityEngine.Object toDestroy = null;

            if (typeof(Component).IsAssignableFrom(type))
            {
                GameObject go = new GameObject("temp");
                Component component = go.AddComponent(type);

                SerializedObject obj = new SerializedObject(component);
                prop = obj.GetIterator();

                toDestroy = go;
            }
            else if (typeof(ScriptableObject).IsAssignableFrom(type))
            {
                ScriptableObject scriptableObject = ScriptableObject.CreateInstance(type);

                SerializedObject obj = new SerializedObject(scriptableObject);
                prop = obj.GetIterator();

                toDestroy = scriptableObject;
            }
            else if (UnityVersion.IsVersionOrGreater(2017, 1))
            {
                // Unity broke creation of emitted scriptable objects in 2017.1, but emitting
                // MonoBehaviours still works.

                GameObject go = new GameObject();
                var handle = UnityPropertyEmitter.CreateEmittedMonoBehaviourProperty("InspectorPropertyInfo_UnityPropertyExtractor", type, 1, ref go);
                prop = handle.UnityProperty;
                toDestroy = go;
            }
            else
            {
                prop = UnityPropertyEmitter.CreateEmittedScriptableObjectProperty("InspectorPropertyInfo_UnityPropertyExtractor", type, 1);

                if (prop != null)
                {
                    toDestroy = prop.serializedObject.targetObject;
                }
            }

            try
            {
                if (prop == null)
                {
                    Debug.LogWarning("Could not get serialized property for type " + type.GetNiceName() + "; this type will not be shown in the inspector.");
                    return;
                }

                // Enter children if there are any
                if (prop.Next(true))
                {
                    var members = type.GetAllMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                                      .Where(n => (n is FieldInfo || n is PropertyInfo) && !AlwaysSkipUnityPropertiesDeclaredBy.Contains(n.DeclaringType))
                                      .ToList();

                    // Iterate through children (but not sub-children)
                    do
                    {
                        if (AlwaysSkipUnityProperties.Contains(prop.name)) continue;
                        if (typeof(Component).IsAssignableFrom(type) && AlwaysSkipUnityPropertiesForComponents.Contains(prop.name)) continue;

                        string memberName = prop.name;

                        if (UnityPropertyMemberNameReplacements.ContainsKeys(type, memberName))
                        {
                            memberName = UnityPropertyMemberNameReplacements[type][memberName];
                        }

                        MemberInfo member = members.FirstOrDefault(n => n.Name == memberName || n.Name == prop.name);

                        if (member == null)
                        {
                            // Try to find a member that matches the display name
                            var propName = prop.displayName.Replace(" ", "");
                            bool changedPropName = false;

                            if (string.Equals(propName, "material", StringComparison.InvariantCultureIgnoreCase))
                            {
                                changedPropName = true;
                                propName = "sharedMaterial";
                            }
                            else if (string.Equals(propName, "mesh", StringComparison.InvariantCultureIgnoreCase))
                            {
                                changedPropName = true;
                                propName = "sharedMesh";
                            }

                            member = members.FirstOrDefault(n => string.Equals(n.Name, propName, StringComparison.InvariantCultureIgnoreCase) && prop.IsCompatibleWithType(n.GetReturnType()));

                            if (changedPropName && member == null)
                            {
                                // Try again with the old name
                                propName = prop.displayName.Replace(" ", "");
                                member = members.FirstOrDefault(n => string.Equals(n.Name, propName, StringComparison.InvariantCultureIgnoreCase) && prop.IsCompatibleWithType(n.GetReturnType()));
                            }
                        }

                        if (member == null)
                        {
                            // Now we are truly getting desperate.
                            // Look away, kids - this code is rated M for Monstrous

                            var propName = prop.displayName;
                            //string typeName = prop.GetProperTypeName();

                            var possibles = members.Where(n => (propName.Contains(n.Name, StringComparison.InvariantCultureIgnoreCase) || n.Name.Contains(propName, StringComparison.InvariantCultureIgnoreCase)) && prop.IsCompatibleWithType(n.GetReturnType())).ToList();

                            if (possibles.Count == 1)
                            {
                                // We found only one possibly compatible member
                                // It's... *probably* this one
                                member = possibles[0];
                            }
                        }

                        if (member == null)
                        {
                            // If we can alias this Unity property as a "virtual member", do that
                            var valueType = prop.GuessContainedType();

                            if (valueType != null && SerializedPropertyUtilities.CanSetGetValue(valueType))
                            {
                                result.Add(new UnityOnlyPropertyInfo(prop.name, type, valueType, prop.editable));
                                continue;
                            }
                        }

                        if (member == null)
                        {
                            Debug.LogWarning("Failed to find corresponding member for Unity property '" + prop.name + "/" + prop.displayName + "' on type " + type.GetNiceName() + ", and cannot alias a Unity property of type '" + prop.propertyType + "/" + prop.type + "'. This property will be missing in the inspector.");
                            continue;
                        }

                        // Makes things easier if we can only find the same member once
                        members.Remove(member);

                        InspectorPropertyInfo info;

                        // Add Unity's found property member as an info
                        if (TryCreate(member, SerializationBackend.Unity, prop.editable, out info))
                        {
                            // Make sure the names match - that way, we can find the property again
                            // when we create a Unity property path from the names
                            info.PropertyName = prop.name;

                            result.Add(info);
                        }
                    } while (prop.Next(false));
                }
            }
            catch (InvalidOperationException)
            {
                // Ignore; it just means we've reached the end of the property
            }
            finally
            {
                if (toDestroy != null)
                {
                    UnityEngine.Object.DestroyImmediate(toDestroy);
                }
            }
        }

        private static void PopulateMemberInspectorProperties(Type type, bool includeSpeciallySerializedMembers, List<InspectorPropertyInfo> properties)
        {
            if (type.BaseType != typeof(object) && type.BaseType != null)
            {
                PopulateMemberInspectorProperties(type.BaseType, includeSpeciallySerializedMembers, properties);
            }

            foreach (var member in type.GetMembers(Flags.InstanceAnyDeclaredOnly))
            {
                InspectorPropertyInfo info;

                if (InspectorPropertyInfo.TryCreate(member, includeSpeciallySerializedMembers, out info))
                {
                    InspectorPropertyInfo previousPropertyWithName = null;
                    int previousPropertyIndex = -1;

                    for (int j = 0; j < properties.Count; j++)
                    {
                        if (properties[j].MemberInfo.Name == info.MemberInfo.Name)
                        {
                            previousPropertyIndex = j;
                            previousPropertyWithName = properties[j];
                            break;
                        }
                    }

                    if (previousPropertyWithName != null)
                    {
                        bool createAlias = true;

                        if (previousPropertyWithName.PropertyType == PropertyType.Method && info.PropertyType == PropertyType.Method)
                        {
                            var oldMethod = (MethodInfo)previousPropertyWithName.MemberInfo;
                            var newMethod = (MethodInfo)member;

                            if (oldMethod.GetBaseDefinition() == newMethod.GetBaseDefinition())
                            {
                                // We have encountered an override of a method that is already a property
                                // This is a special case; we remove the base method property, and keep
                                // only the override method property.

                                createAlias = false;
                                properties.RemoveAt(previousPropertyIndex);
                            }
                        }

                        if (createAlias)
                        {
                            var alias = FormatterUtilities.GetPrivateMemberAlias(previousPropertyWithName.MemberInfo, previousPropertyWithName.MemberInfo.DeclaringType.GetNiceName(), " -> ");

                            var aliasName = alias.Name;
                            var hidden = info.MemberInfo.DeclaringType.GetNiceName() + "." + info.MemberInfo.Name;
                            var inherited = previousPropertyWithName.MemberInfo.DeclaringType.GetNiceName() + "." + previousPropertyWithName.MemberInfo.Name;

                            if (InspectorPropertyInfo.TryCreate(alias, includeSpeciallySerializedMembers, out previousPropertyWithName))
                            {
                                //Debug.LogWarning("The inspector property '" + hidden + "' hides inherited property '" + inherited + "'. Alias property '" + aliasName + "' created for inherited property '" + inherited + "'.");
                                properties[previousPropertyIndex] = previousPropertyWithName;
                            }
                            else
                            {
                                Debug.LogWarning("The inspector property '" + hidden + "' hides inherited property '" + inherited + "'. Failed to create alias property '" + aliasName + "' for inherited property '" + inherited + "'; removing inherited property instead.");
                                properties.RemoveAt(previousPropertyIndex);
                            }
                        }
                    }

                    properties.Add(info);
                }
            }
        }
    }
}
#endif