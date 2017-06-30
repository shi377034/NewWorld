#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="InfoBoxAttributeDrawer.cs" company="Sirenix IVS">
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
    /// Draws properties marked with <see cref="InfoBoxAttribute"/>.
	/// Draws an info box above the property. Error and warning info boxes can be tracked by Odin Scene Validator.
    /// </summary>
	/// <seealso cref="InfoBoxAttribute"/>
	/// <seealso cref="DetailedInfoBoxAttribute"/>
	/// <seealso cref="RequiredAttribute"/>
	/// <seealso cref="ValidateInputAttribute"/>
    [OdinDrawer]
    [DrawerPriority(0, 100, 0)]
    public sealed class InfoBoxAttributeDrawer<T> : OdinAttributeDrawer<InfoBoxAttribute, T>
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
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, InfoBoxAttribute attribute, GUIContent label)
        {
            PropertyContext<InfoBoxContext> context = null;

            if (attribute.VisibleIf != null)
            {
                context = entry.Property.Context.Get(this, "Config", (InfoBoxContext)null);
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

                    //if (config.Value.ErrorMessage == null)
                    //{
                    //if(memberInfo is FieldInfo)
                    //{
                    //	if(memberInfo.IsStatic())
                    //	{
                    //		config.Value.StaticValidationMethodCaller = DeepReflection.CreateValueGetter<bool>(entry.ParentType, memberInfo.Name);
                    //	}
                    //	else
                    //	{
                    //		config.Value.InstanceValidationMethodCaller = EmitUtilities.CreateInstanceFieldGetter<T, bool>(entry.ParentType, memberInfo as FieldInfo);
                    //	}
                    //}
                    //else if(memberInfo is PropertyInfo)
                    //{
                    //	if(memberInfo.IsStatic())
                    //	{
                    //	}
                    //	else
                    //	{
                    //	}
                    //}
                    //else if(memberInfo is MethodInfo)
                    //{
                    //	if (memberInfo.IsStatic())
                    //	{
                    //	    config.Value.StaticValidationMethodCaller = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), memberInfo as MethodInfo);
                    //	}
                    //	else
                    //	{
                    //	    config.Value.InstanceValidationMethodCaller = EmitUtilities.CreateWeakInstanceMethodCaller<bool, T>(memberInfo as MethodInfo);
                    //	}
                    //}
                    //else
                    //{
                    //	config.Value.ErrorMessage = "Invalid member type!";
                    //}
                    //}
                }
            }

            if (attribute.VisibleIf != null && context.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(context.Value.ErrorMessage);
            }
            else
            {
                var parentValue = entry.Property.ParentValues[0];

                bool drawMessageBox = true;
                try
                {
                    drawMessageBox =
                        attribute.VisibleIf == null ||
                        (context.Value.StaticValidationParameterMethodCaller != null && context.Value.StaticValidationParameterMethodCaller(entry.SmartValue)) ||
                        (context.Value.InstanceValidationParameterMethodCaller != null && context.Value.InstanceValidationParameterMethodCaller(entry.Property.ParentValues[0], entry.SmartValue)) ||
                        (context.Value.InstanceValidationMethodCaller != null && context.Value.InstanceValidationMethodCaller(entry.Property.ParentValues[0])) ||
                        (context.Value.InstanceValueGetter != null && (bool)context.Value.InstanceValueGetter(ref parentValue)) ||
                        (context.Value.StaticValidationCaller != null && context.Value.StaticValidationCaller());
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }

                if (drawMessageBox)
                {
                    switch (attribute.InfoMessageType)
                    {
                        case InfoMessageType.None:
                            SirenixEditorGUI.MessageBox(attribute.Message);
                            break;

                        case InfoMessageType.Info:
                            SirenixEditorGUI.InfoMessageBox(attribute.Message);
                            break;

                        case InfoMessageType.Warning:
                            SirenixEditorGUI.WarningMessageBox(attribute.Message);
                            break;

                        case InfoMessageType.Error:
                            SirenixEditorGUI.ErrorMessageBox(attribute.Message);
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