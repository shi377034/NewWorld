#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ShowIfAttributeDrawer.cs" company="Sirenix IVS">
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
    /// Draws properties marked with <see cref="ShowIfAttribute"/>.
    /// </summary>
    [OdinDrawer]
    [DrawerPriority(100, 0, 0)]
    public sealed class ShowIfAttributeDrawer : OdinAttributeDrawer<ShowIfAttribute>
    {
        private class ShowIfConfig
        {
            public Func<bool> StaticMemberGetter;
            public Func<object, bool> InstanceMemberGetter;
            public string ErrorMessage;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, ShowIfAttribute attribute, GUIContent label)
        {
            var config = property.Context.Get(this, "ShowIfConfig", (ShowIfConfig)null);

            if (config.Value == null)
            {
                config.Value = new ShowIfConfig();
                var memberInfo = property.ParentType
                    .FindMember()
                    .IsNamed(attribute.MemberName)
                    .HasReturnType<bool>()
                    .HasNoParameters()
                    .GetMember(out config.Value.ErrorMessage);

                if (memberInfo != null)
                {
                    string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

                    if (memberInfo.IsStatic())
                    {
                        config.Value.StaticMemberGetter = DeepReflection.CreateValueGetter<bool>(property.ParentType, name);
                    }
                    else
                    {
                        config.Value.InstanceMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<bool>(property.ParentType, name);
                    }
                }
            }

            if (config.Value.ErrorMessage != null)
            {
                this.CallNextDrawer(property, label);
                SirenixEditorGUI.ErrorMessageBox(config.Value.ErrorMessage);
            }
            else
            {
                bool drawProperty = true;

                if (config.Value.InstanceMemberGetter != null)
                {
                    for (int i = 0; i < property.ParentValues.Count; i++)
                    {
                        if (config.Value.InstanceMemberGetter(property.ParentValues[i]) == false)
                        {
                            drawProperty = false;
                            break;
                        }
                    }
                }
                else if (config.Value.StaticMemberGetter != null)
                {
                    for (int i = 0; i < property.ParentValues.Count; i++)
                    {
                        if (config.Value.StaticMemberGetter() == false)
                        {
                            drawProperty = false;
                            break;
                        }
                    }
                }

                if (SirenixEditorGUI.BeginFadeGroup(UniqueDrawerKey.Create(property, this), drawProperty, GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
                {
                    this.CallNextDrawer(property, label);
                }
                SirenixEditorGUI.EndFadeGroup();
            }
        }
    }
}
#endif