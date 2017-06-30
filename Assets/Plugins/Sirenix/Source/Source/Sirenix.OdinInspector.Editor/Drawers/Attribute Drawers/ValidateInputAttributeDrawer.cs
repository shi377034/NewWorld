#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ValidateInputAttributeDrawer.cs" company="Sirenix IVS">
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
    /// Draws properties marked with <see cref="ValidateInputAttribute"/>.
    /// </summary>
    /// <seealso cref="ValidateInputAttribute"/>
    [OdinDrawer]
    [DrawerPriority(0, 10000, 0)]
    public sealed class ValidateInputAttributeDrawer<T> : OdinAttributeDrawer<ValidateInputAttribute, T>
    {
        private class PropertyConfig
        {
            public string ErrorMessage;
            public Func<object, T, bool> InstanceValidationMethodCaller;
            public Func<T, bool> StaticValidationMethodCaller;
            public string ValidationErrorMessage;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(IPropertyValueEntry<T> entry, ValidateInputAttribute attribute, GUIContent label)
        {
            var config = entry.Property.Context.Get(this, "ValidateInputAttributeDrawer", (PropertyConfig)null);

            if (config.Value == null)
            {
                config.Value = new PropertyConfig();
                MethodInfo methodInfo = entry.ParentType.FindMember()
                    .IsMethod()
                    .HasReturnType<bool>()
                    .HasParameters(entry.BaseValueType)
                    .IsNamed(attribute.MemberName)
                    .GetMember<MethodInfo>(out config.Value.ErrorMessage);

                if (config.Value.ErrorMessage == null)
                {
                    if (methodInfo.IsStatic())
                    {
                        config.Value.StaticValidationMethodCaller = (Func<T, bool>)Delegate.CreateDelegate(typeof(Func<T, bool>), methodInfo);
                    }
                    else
                    {
                        config.Value.InstanceValidationMethodCaller = EmitUtilities.CreateWeakInstanceMethodCaller<bool, T>(methodInfo);
                    }

                    RunValidation(entry, attribute, config);
                }
            }

            if (config.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(config.Value.ErrorMessage);
            }

            bool didShowMessage = false;

            if (config.Value.ValidationErrorMessage != null)
            {
                if (attribute.MessageType == InfoMessageType.Error)
                {
                    didShowMessage = true;
                    SirenixEditorGUI.ErrorMessageBox(attribute.Message);
                }
                else if (attribute.MessageType == InfoMessageType.Warning)
                {
                    didShowMessage = true;
                    SirenixEditorGUI.WarningMessageBox(attribute.Message);
                }
                else if (attribute.MessageType == InfoMessageType.Info)
                {
                    didShowMessage = true;
                    SirenixEditorGUI.InfoMessageBox(attribute.Message);
                }
            }

            if (didShowMessage == false)
            {
                GUIHelper.BeginDrawToNothing();
                {
                    SirenixEditorGUI.InfoMessageBox("");
                }
                GUIHelper.EndDrawToNothing();
            }

            GUIUtility.GetControlID(12938712, FocusType.Passive);
            object key = UniqueDrawerKey.Create(entry.Property, this);
            SirenixEditorGUI.BeginShakeableGroup(key);
            this.CallNextDrawer(entry.Property, label);
            SirenixEditorGUI.EndShakeableGroup(key);

            if (entry.Values.AreDirty)
            {
                if (!RunValidation(entry, attribute, config))
                {
                    SirenixEditorGUI.StartShakingGroup(key, GeneralDrawerConfig.Instance.ShakingAnimationDuration);
                }
            }
        }

        private static bool RunValidation(IPropertyValueEntry<T> entry, ValidateInputAttribute attribute, PropertyContext<PropertyConfig> context)
        {
            bool hasError = true;

            for (int i = 0; i < entry.Property.ParentValues.Count; i++)
            {
                try
                {
                    if (context.Value.StaticValidationMethodCaller != null)
                    {
                        hasError = context.Value.StaticValidationMethodCaller(entry.Values[i]) == false;
                    }
                    else
                    {
                        hasError = context.Value.InstanceValidationMethodCaller(entry.Property.ParentValues[i], entry.Values[i]) == false;
                    }
                }
                catch (System.Exception ex)
                {
                    Debug.LogException(ex);
                }

                if (hasError)
                {
                    if (attribute.RejectInvalidInput)
                    {
                        entry.Values.RevertUnappliedValues();
                    }

                    context.Value.ValidationErrorMessage = attribute.Message;
                }
                else
                {
                    context.Value.ValidationErrorMessage = null;
                }
            }

            return !hasError;
        }
    }
}
#endif