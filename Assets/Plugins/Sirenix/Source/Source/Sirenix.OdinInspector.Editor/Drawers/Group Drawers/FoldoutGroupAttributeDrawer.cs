#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="FoldoutGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System;
    using Utilities;
    using Utilities.Editor;
    using UnityEngine;
    using System.Reflection;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="FoldoutGroupAttribute"/>
    /// </summary>
    /// <seealso cref="FoldoutGroupAttribute"/>
    [OdinDrawer]
    public class FoldoutGroupAttributeDrawer : OdinGroupDrawer<FoldoutGroupAttribute>
    {
        private class FoldoutGroupConfig
        {
            public Func<string> StaticTitleMemberGetter;
            public Func<object, string> InstanceTitleMemberGetter;
            public string ErrorMessage;
            public bool IsVisible = false;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, FoldoutGroupAttribute attribute, GUIContent label)
        {
            var config = property.Context.Get<FoldoutGroupConfig>(this, "ToggleGroupConfig", (FoldoutGroupConfig)null);
            if (config.Value == null)
            {
                config.Value = new FoldoutGroupConfig();

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

            if (config.Value.ErrorMessage != null)
            {
                SirenixEditorGUI.ErrorMessageBox(config.Value.ErrorMessage);
            }

            SirenixEditorGUI.BeginBox();
            {
                SirenixEditorGUI.BeginBoxHeader();

                string title = null;

                if (config.Value.InstanceTitleMemberGetter != null)
                {
                    title = config.Value.InstanceTitleMemberGetter(property.ParentValues[0]);
                }
                else if (config.Value.StaticTitleMemberGetter != null)
                {
                    title = config.Value.StaticTitleMemberGetter();
                }

                if (config.Value.InstanceTitleMemberGetter == null && config.Value.StaticTitleMemberGetter == null)
                {
                    config.Value.IsVisible = SirenixEditorGUI.Foldout(config.Value.IsVisible, label);
                }
                else
                {
                    config.Value.IsVisible = SirenixEditorGUI.Foldout(config.Value.IsVisible, title);
                }

                SirenixEditorGUI.EndBoxHeader();

                if (SirenixEditorGUI.BeginFadeGroup(config, config.Value.IsVisible, GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
                {
                    for (int i = 0; i < property.Children.Count; i++)
                    {
                        try
                        {
                            InspectorUtilities.DrawProperty(property.Children[i]);
                        }
                        catch (Exception ex)
                        {
                            if (ex is ExitGUIException || ex.InnerException is ExitGUIException)
                            {
                                throw ex;
                            }
                            else
                            {
                                Debug.Log("The following exception was thrown while drawing property at path " + property.Children.GetPath(i) + ".");
                                Debug.LogException(ex);
                            }
                        }
                    }
                }

                SirenixEditorGUI.EndFadeGroup();
            }
            SirenixEditorGUI.EndBox();
        }
    }
}
#endif