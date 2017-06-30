#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValueDropdownAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using Utilities.Editor;
    using UnityEngine;
    using Utilities;
	using System.Linq;

	/// <summary>
	/// Draws properties marked with <see cref="ValueDropdownAttribute"/>.
	/// </summary>
	/// <seealso cref="ValueDropdownAttribute"/>
	[OdinDrawer]
    public sealed class ValueDropdownAttributeDrawer<T> : OdinAttributeDrawer<ValueDropdownAttribute, T>
    {
        private class PropertyConfig
        {
            public bool IsValueDropdown;
            public string ErrorMessage;
            public Func<object, IList<T>> InstanceValueDropdownGetter;
            public Func<IList<T>> StaticValueDropdownGetter;
            public Func<object, IList<ValueDropdownItem<T>>> ValueDropdownInstanceValueDropdownGetter;
            public Func<IList<ValueDropdownItem<T>>> ValueDropdownStaticValueDropdownGetter;
        }

        private static List<int> selectedValuesBuffer = new List<int>();

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, ValueDropdownAttribute attribute, GUIContent label)
        {
            var config = entry.Property.Context.Get(this, "Config", (PropertyConfig)null);

            if (config.Value == null)
            {
                config.Value = new PropertyConfig();

                MemberInfo memberInfo = entry.ParentType.FindMember()
                    .HasReturnType<IList<T>>(true)
                    .HasNoParameters()
                    .IsNamed(attribute.MemberName)
                    .GetMember<MemberInfo>(out config.Value.ErrorMessage);

                if (config.Value.ErrorMessage == null)
                {
                    string memberName = attribute.MemberName + ((memberInfo is MethodInfo) ? "()" : "");
                    if (memberInfo.IsStatic())
                    {
                        config.Value.StaticValueDropdownGetter = DeepReflection.CreateValueGetter<IList<T>>(entry.Property.ParentType, memberName);
                    }
                    else
                    {
                        config.Value.InstanceValueDropdownGetter = DeepReflection.CreateWeakInstanceValueGetter<IList<T>>(entry.Property.ParentType, memberName);
                    }
                    config.Value.IsValueDropdown = false;
                }
                else
                {
                    string errorMessage;

                    memberInfo = entry.ParentType.FindMember()
                       .HasReturnType<IList<ValueDropdownItem<T>>>(true)
                       .HasNoParameters()
                       .IsNamed(attribute.MemberName)
                       .GetMember<MemberInfo>(out errorMessage);

                    if (errorMessage == null)
                    {
                        string memberName = attribute.MemberName + ((memberInfo is MethodInfo) ? "()" : "");
                        if (memberInfo.IsStatic())
                        {
                            config.Value.ValueDropdownStaticValueDropdownGetter = DeepReflection.CreateValueGetter<IList<ValueDropdownItem<T>>>(entry.Property.ParentType, memberName);
                        }
                        else
                        {
                            config.Value.ValueDropdownInstanceValueDropdownGetter = DeepReflection.CreateWeakInstanceValueGetter<IList<ValueDropdownItem<T>>>(entry.Property.ParentType, memberName);
                        }
                        config.Value.ErrorMessage = null;
                        config.Value.IsValueDropdown = true;
                    }
                    else
                    {
                        if (config.Value.ErrorMessage != errorMessage)
                        {
                            config.Value.ErrorMessage += " or\n " + errorMessage;
                        }
                    }
                }
            }

            if (config.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(config.Value.ErrorMessage);
            }
            else
            {
                if (config.Value.IsValueDropdown)
                {
                    IList<ValueDropdownItem<T>> selectList = config.Value.ValueDropdownStaticValueDropdownGetter != null ?
                    config.Value.ValueDropdownStaticValueDropdownGetter() :
                    config.Value.ValueDropdownInstanceValueDropdownGetter(entry.Property.ParentValues[0]);

                    selectedValuesBuffer.Clear();

                    if (selectList != null && selectList.Count > 0)
                    {
                        for (int i = 0; i < entry.Values.Count; i++)
                        {
                            var val = entry.Values[i];
                            for (int j = 0; j < selectList.Count; j++)
                            {
                                if (EqualityComparer<T>.Default.Equals(val, selectList[j].Value))
                                {
                                    selectedValuesBuffer.Add(j);
                                }
                            }
                        }

                        if (selectList.Count > 0 && selectedValuesBuffer.Count == 0 && entry.Values.Count == 1)
                        {
                            entry.SmartValue = selectList[0].Value;
                        }
                    }

					if (SirenixEditorFields.Dropdown<ValueDropdownItem<T>>(label, selectedValuesBuffer, selectList, false))
                    {
                        if (selectedValuesBuffer.Count > 0)
                        {
                            entry.SmartValue = selectList[selectedValuesBuffer[0]].Value;
                        }
                    }
                }
                else
                {
                    IList<T> selectList = config.Value.StaticValueDropdownGetter != null ?
                    config.Value.StaticValueDropdownGetter() :
                    config.Value.InstanceValueDropdownGetter(entry.Property.ParentValues[0]);

                    selectedValuesBuffer.Clear();

                    if (selectList != null && selectList.Count > 0)
                    {
                        for (int i = 0; i < entry.Values.Count; i++)
                        {
                            var val = entry.Values[i];
                            for (int j = 0; j < selectList.Count; j++)
                            {
                                if (EqualityComparer<T>.Default.Equals(val, selectList[j]))
                                {
                                    selectedValuesBuffer.Add(j);
                                }
                            }
                        }

                        if (selectList.Count > 0 && selectedValuesBuffer.Count == 0 && entry.Values.Count == 1)
                        {
                            entry.SmartValue = selectList[0];
                        }
                    }

					if (SirenixEditorFields.Dropdown<T>(label, selectedValuesBuffer, selectList, false))
                    {
                        if (selectedValuesBuffer.Count > 0)
                        {
                            entry.SmartValue = selectList[selectedValuesBuffer[0]];
                        }
                    }
                }
                //GUILayout.Space(5);
            }
        }
    }
}
#endif