#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="EnabledIfAttributeDrawer.cs" company="Sirenix IVS">
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
    /// Draws properties marked with <see cref="EnableIfAttribute"/>.
    /// </summary>
	/// <seealso cref="EnableIfAttribute"/>
	/// <seealso cref="DisableIfAttribute"/>
	/// <seealso cref="DisableInEditorModeAttribute"/>
	/// <seealso cref="DisableInPlayModeAttribute"/>
	/// <seealso cref="ReadOnlyAttribute"/>
	/// <seealso cref="ShowIfAttribute"/>
	/// <seealso cref="HideIfAttribute"/>
	/// <seealso cref="HideInInspector"/>
    [OdinDrawer]
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public sealed class EnableIfAttributeDrawer : OdinAttributeDrawer<EnableIfAttribute>
    {
        private class EnabledIfConfig
        {
            public Func<bool> StaticMemberGetter;
            public Func<object, bool> InstanceMemberGetter;
            public string ErrorMessage;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, EnableIfAttribute attribute, GUIContent label)
        {
            if (GUI.enabled == false)
            {
                this.CallNextDrawer(property, label);
                return;
            }

            var enabledIfConfig = property.Context.Get(this, "EnabledIfConfig", (EnabledIfConfig)null);

            if (enabledIfConfig.Value == null)
            {
                enabledIfConfig.Value = new EnabledIfConfig();
                MemberInfo memberInfo = property.ParentType
                    .FindMember()
                    .IsNamed(attribute.MemberName)
                    .HasReturnType(typeof(bool))
                    .HasNoParameters()
                    .GetMember(out enabledIfConfig.Value.ErrorMessage);

                if (memberInfo != null)
                {
                    string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

                    if (memberInfo.IsStatic())
                    {
                        enabledIfConfig.Value.StaticMemberGetter = DeepReflection.CreateValueGetter<bool>(property.ParentType, name);
                    }
                    else
                    {
                        enabledIfConfig.Value.InstanceMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<bool>(property.ParentType, name);
                    }
                }
            }

            if (enabledIfConfig.Value.ErrorMessage != null || (enabledIfConfig.Value.InstanceMemberGetter == null && enabledIfConfig.Value.StaticMemberGetter == null))
            {
                this.CallNextDrawer(property, label);
                SirenixEditorGUI.ErrorMessageBox(enabledIfConfig.Value.ErrorMessage ?? "There should really be an error message here.");
            }
            else
            {
                bool enabled = true;
                if (enabledIfConfig.Value.InstanceMemberGetter != null)
                {
                    for (int i = 0; i < property.ParentValues.Count; i++)
                    {
                        if (enabledIfConfig.Value.InstanceMemberGetter(property.ParentValues[i]) == false)
                        {
                            enabled = false;
                            break;
                        }
                    }
                }
                else
                {
                    enabled = enabledIfConfig.Value.StaticMemberGetter();
                }

                GUIHelper.PushGUIEnabled(GUI.enabled && enabled);
                this.CallNextDrawer(property, label);
                GUIHelper.PopGUIEnabled();
            }
        }
    }
}
#endif