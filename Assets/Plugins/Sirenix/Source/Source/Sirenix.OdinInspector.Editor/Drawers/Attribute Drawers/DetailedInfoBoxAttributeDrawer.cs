#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DetailedInfoBoxAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using System.Reflection;
    using Utilities.Editor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    ///	Draws properties marked with <see cref="DetailedInfoBoxAttribute"/>.
    /// </summary>
	/// <seealso cref="DetailedInfoBoxAttribute"/>
	/// <seealso cref="InfoBoxAttribute"/>
	/// <seealso cref="RequiredAttribute"/>
	/// <seealso cref="OnInspectorGUIAttribute"/>
    [OdinDrawer]
    [DrawerPriority(0, 100, 0)]
    public sealed class DetailedInfoBoxAttributeDrawer<T> : OdinAttributeDrawer<DetailedInfoBoxAttribute, T>
    {
        private class InfoBoxContext
        {
            public string ErrorMessage;
            public Func<object, T, bool> InstanceValidationParameterMethodCaller;
            public Func<T, bool> StaticValidationParameterMethodCaller;
            public Func<object, bool> InstanceValidationMethodCaller;
            public WeakValueGetter InstanceValueGetter;
            public Func<bool> StaticValidationCaller;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, DetailedInfoBoxAttribute attribute, GUIContent label)
        {
            PropertyContext<InfoBoxContext> context = null;

            if (attribute.VisibleIf != null)
            {
                context = entry.Property.Context.Get(this, "Context", (InfoBoxContext)null);
                if (context.Value == null)
                {
                    context.Value = new InfoBoxContext();

                    MemberInfo memberInfo;

                    // Parameter functions
                    if (entry.ParentType.FindMember()
                        .IsMethod()
                        .HasReturnType<bool>()
                        .HasParameters<T>()
                        .IsNamed(attribute.VisibleIf)
                        .TryGetMember(out memberInfo, out context.Value.ErrorMessage))
                    {
                        if (context.Value.ErrorMessage == null)
                        {
                            if (memberInfo is MethodInfo)
                            {
                                if (memberInfo.IsStatic())
                                {
                                    context.Value.StaticValidationParameterMethodCaller = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), memberInfo as MethodInfo);
                                }
                                else
                                {
                                    context.Value.InstanceValidationParameterMethodCaller = EmitUtilities.CreateWeakInstanceMethodCaller<bool, T>(memberInfo as MethodInfo);
                                }
                            }
                            else
                            {
                                context.Value.ErrorMessage = "Invalid member type!";
                            }
                        }
                    }
                    // Fields, properties, and no-parameter functions.
                    else if (entry.ParentType.FindMember()
                        .HasReturnType<bool>()
                        .HasNoParameters()
                        .IsNamed(attribute.VisibleIf)
                        .TryGetMember(out memberInfo, out context.Value.ErrorMessage))
                    {
                        if (context.Value.ErrorMessage == null)
                        {
                            if (memberInfo is FieldInfo)
                            {
                                if (memberInfo.IsStatic())
                                {
                                    context.Value.StaticValidationCaller = EmitUtilities.CreateStaticFieldGetter<bool>(memberInfo as FieldInfo);
                                }
                                else
                                {
                                    context.Value.InstanceValueGetter = EmitUtilities.CreateWeakInstanceFieldGetter(entry.ParentType, memberInfo as FieldInfo);
                                }
                            }
                            else if (memberInfo is PropertyInfo)
                            {
                                if (memberInfo.IsStatic())
                                {
                                    context.Value.StaticValidationCaller = EmitUtilities.CreateStaticPropertyGetter<bool>(memberInfo as PropertyInfo);
                                }
                                else
                                {
                                    context.Value.InstanceValueGetter = EmitUtilities.CreateWeakInstancePropertyGetter(entry.ParentType, memberInfo as PropertyInfo);
                                }
                            }
                            else if (memberInfo is MethodInfo)
                            {
                                if (memberInfo.IsStatic())
                                {
                                    context.Value.StaticValidationCaller = (Func<bool>)Delegate.CreateDelegate(typeof(Func<bool>), memberInfo as MethodInfo);
                                }
                                else
                                {
                                    context.Value.InstanceValidationMethodCaller = EmitUtilities.CreateWeakInstanceMethodCallerFunc<bool>(memberInfo as MethodInfo);
                                }
                            }
                            else
                            {
                                context.Value.ErrorMessage = "Invalid member type!";
                            }
                        }
                    }
                }
            }

            if (attribute.VisibleIf != null && context.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
            }
            else
            {
                var parentValue = entry.Property.ParentValues[0];

                bool drawMessageBox =
                    attribute.VisibleIf == null ||
                    (context.Value.StaticValidationParameterMethodCaller != null && context.Value.StaticValidationParameterMethodCaller(entry.SmartValue)) ||
                    (context.Value.InstanceValidationParameterMethodCaller != null && context.Value.InstanceValidationParameterMethodCaller(entry.Property.ParentValues[0], entry.SmartValue)) ||
                    (context.Value.InstanceValidationMethodCaller != null && context.Value.InstanceValidationMethodCaller(entry.Property.ParentValues[0])) ||
                    (context.Value.InstanceValueGetter != null && (bool)context.Value.InstanceValueGetter(ref parentValue)) ||
                    (context.Value.StaticValidationCaller != null && context.Value.StaticValidationCaller());

                if (drawMessageBox)
                {
                    var foldedConfig = entry.Property.Context.Get<bool>(this, "IsFolded", true);

                    switch (attribute.InfoMessageType)
                    {
                        case InfoMessageType.None:
                            foldedConfig.Value = SirenixEditorGUI.DetailedMessageBox(attribute.Message, attribute.Details, UnityEditor.MessageType.None, foldedConfig.Value);
                            break;

                        case InfoMessageType.Info:
                            foldedConfig.Value = SirenixEditorGUI.DetailedMessageBox(attribute.Message, attribute.Details, UnityEditor.MessageType.Info, foldedConfig.Value);
                            break;

                        case InfoMessageType.Warning:
                            foldedConfig.Value = SirenixEditorGUI.DetailedMessageBox(attribute.Message, attribute.Details, UnityEditor.MessageType.Warning, foldedConfig.Value);
                            break;

                        case InfoMessageType.Error:
                            foldedConfig.Value = SirenixEditorGUI.DetailedMessageBox(attribute.Message, attribute.Details, UnityEditor.MessageType.Error, foldedConfig.Value);
                            break;

                        default:
                            SirenixEditorGUI.ErrorMessageBox("Unknown InfoBoxType: " + attribute.InfoMessageType.ToString());
                            break;
                    }
                }
            }

            this.CallNextDrawer(entry.Property, label);
        }
    }
}
#endif