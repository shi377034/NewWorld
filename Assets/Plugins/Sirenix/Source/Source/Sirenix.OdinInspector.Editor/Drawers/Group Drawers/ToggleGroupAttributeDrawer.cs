#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ToggleGroupAttributeDrawer.cs" company="Sirenix IVS">
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
    /// Draws all properties grouped together with the <see cref="ToggleGroupAttribute"/>
    /// </summary>
    /// <seealso cref="ToggleGroupAttribute"/>
    [OdinDrawer]
    public class ToggleGroupAttributeDrawer : OdinGroupDrawer<ToggleGroupAttribute>
    {
        private class ToggleGroupConfig
        {
            public Func<string> StaticTitleMemberGetter;
            public Func<object, string> InstanceTitleMemberGetter;
            public bool IsVisible = false;
            public string ErrorMessage;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, ToggleGroupAttribute attribute, GUIContent label)
        {
            var toggleProperty = property.Children.Get(attribute.ToggleMemberName);

            var config = property.Context.Get<ToggleGroupConfig>(this, "ToggleGroupConfig", (ToggleGroupConfig)null);

            if (config.Value == null)
            {
                config.Value = new ToggleGroupConfig();
                if (toggleProperty == null)
                {
                    config.Value.ErrorMessage = "No property or field named " + attribute.ToggleMemberName + " found. Make sure the property is part of the inspector and the group.";
                }
                else
                {
                    if (attribute.TitleStringMemberName != null)
                    {
                        MemberInfo memberInfo = property.ParentType
                            .FindMember()
                            .IsNamed(attribute.TitleStringMemberName)
                            .HasReturnType(typeof(string))
                            .HasNoParameters()
                            .GetMember(out config.Value.ErrorMessage);

                        if (memberInfo != null)
                        {
                            string name = (memberInfo is MethodInfo) ? memberInfo.Name + "()" : memberInfo.Name;

                            if (memberInfo.IsStatic())
                            {
                                config.Value.StaticTitleMemberGetter = DeepReflection.CreateValueGetter<string>(property.ParentType, name);
                            }
                            else
                            {
                                config.Value.InstanceTitleMemberGetter = DeepReflection.CreateWeakInstanceValueGetter<string>(property.ParentType, name);
                            }
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
                PropertyContext<string> openGroup = null;

                if (attribute.CollapseOthersOnExpand)
                {
                    if (property.Parent == null)
                    {
                        openGroup = GUIHelper.GetTemporaryContext<PropertyContext<string>>(property.Tree);
                    }
                    else
                    {
                        var parent = property.Parent.ValueEntry.ValueCategory == PropertyValueCategory.Member ? property.Parent : property.Parent.Parent;
                        openGroup = parent.Context.GetGlobal<string>("OpenFoldoutToggleGroup", (string)null);
                    }

                    if (openGroup.Value != null && openGroup.Value != property.Path)
                    {
                        config.Value.IsVisible = false;
                    }
                }

                var parentInstance = property.ParentValues[0];
                bool isEnabled = (bool)toggleProperty.ValueEntry.WeakSmartValue;

                string title = attribute.GroupID;

                if (config.Value.InstanceTitleMemberGetter != null)
                {
                    title = config.Value.InstanceTitleMemberGetter(parentInstance);
                }
                else if (config.Value.StaticTitleMemberGetter != null)
                {
                    title = config.Value.StaticTitleMemberGetter();
                }
                else if (attribute.ToggleGroupTitle != null)
                {
                    title = attribute.ToggleGroupTitle;
                }
                bool prev = config.Value.IsVisible;
                if (SirenixEditorGUI.BeginToggleGroup(UniqueDrawerKey.Create(property, this), ref isEnabled, ref config.Value.IsVisible, title, GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
                {
                    for (int i = 0; i < property.Children.Count; i++)
                    {
                        var child = property.Children[i];
                        if (child != toggleProperty)
                        {
                            InspectorUtilities.DrawProperty(child);
                        }
                    }
                }
                else
                {
                    // OnValueChanged is not fired if property is not drawn.
                    GUIHelper.BeginDrawToNothing();
                    InspectorUtilities.DrawProperty(toggleProperty);
                    GUIHelper.EndDrawToNothing();
                }
                SirenixEditorGUI.EndToggleGroup();

                if (openGroup != null && prev != config.Value.IsVisible && config.Value.IsVisible)
                {
                    openGroup.Value = property.Path;
                }

                toggleProperty.ValueEntry.WeakSmartValue = isEnabled;
                toggleProperty.ValueEntry.ApplyChanges();
            }
        }
    }
}
#endif