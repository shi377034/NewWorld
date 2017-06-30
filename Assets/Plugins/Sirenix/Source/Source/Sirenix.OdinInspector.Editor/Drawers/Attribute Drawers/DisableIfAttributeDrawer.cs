#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="DisableIfAttributeDrawer.cs" company="Sirenix IVS">
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
    /// Draws properties marked with <see cref="DisableIfAttribute"/>.
    /// </summary>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableInEditorModeAttribute"/>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    [OdinDrawer]
    [DrawerPriority(DrawerPriorityLevel.SuperPriority)]
    public class DisableIfAttributeDrawer : OdinAttributeDrawer<DisableIfAttribute>
    {
        private class DisableIfConfig
        {
            public Func<bool> StaticMemberGetter;
            public Func<object, bool> InstanceMemberGetter;
            public string ErrorMessage;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, DisableIfAttribute attribute, GUIContent label)
        {
            if (GUI.enabled == false)
            {
                this.CallNextDrawer(property, label);
                return;
            }

            var disableIfConfig = property.Context.Get(this, "EnabledIfConfig", (DisableIfConfig)null);

            if (disableIfConfig.Value == null)
            {
                disableIfConfig.Value = new DisableIfConfig();
                MemberInfo memberInfo = property.ParentType
                    .FindMember()
                    .IsNamed(attribute.MemberName)
                    .HasReturnType(typeof(bool))
                    .HasNoParameters()
                    .GetMember(out disableIfConfig.Value.ErrorMessage);

                if (memberInfo != null)
                {
                    string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

                    if (memberInfo.IsStatic())
                    {
                        disableIfConfig.Value.StaticMemberGetter = DeepReflection.CreateValueGetter<bool>(property.ParentType, name);
                    }
                    else
                    {
                        disableIfConfig.Value.InstanceMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<bool>(property.ParentType, name);
                    }
                }
            }

            if (disableIfConfig.Value.ErrorMessage != null || (disableIfConfig.Value.InstanceMemberGetter == null && disableIfConfig.Value.StaticMemberGetter == null))
            {
                SirenixEditorGUI.ErrorMessageBox(disableIfConfig.Value.ErrorMessage ?? "There should really be an error message here.");
                this.CallNextDrawer(property, label);
            }
            else
            {
                bool enabled = true;
                if (disableIfConfig.Value.InstanceMemberGetter != null)
                {
                    for (int i = 0; i < property.ParentValues.Count; i++)
                    {
                        if (disableIfConfig.Value.InstanceMemberGetter(property.ParentValues[i]) == false)
                        {
                            enabled = false;
                            break;
                        }
                    }
                }
                else
                {
                    enabled = disableIfConfig.Value.StaticMemberGetter();
                }

                GUIHelper.PushGUIEnabled(!enabled);
                this.CallNextDrawer(property, label);
                GUIHelper.PopGUIEnabled();
            }
        }
    }
}
#endif