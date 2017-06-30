#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="ToggleAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using Utilities.Editor;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// Draws properties marked with <see cref="ToggleAttribute"/>.
    /// </summary>
    /// <seealso cref="ToggleAttribute"/>
    [OdinDrawer]
    public class ToggleAttributeDrawer : OdinAttributeDrawer<ToggleAttribute>
    {
        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, ToggleAttribute attribute, GUIContent label)
        {
            var toggleProperty = property.Children.Get(attribute.ToggleMemberName);

            if (toggleProperty == null)
            {
                SirenixEditorGUI.ErrorMessageBox(attribute.ToggleMemberName + " is not a member of " + property.Info.MemberInfo.GetNiceName() + ".");
            }
            else if (toggleProperty.ValueEntry.TypeOfValue != typeof(bool))
            {
                SirenixEditorGUI.ErrorMessageBox(attribute.ToggleMemberName + " on " + property.Info.MemberInfo.GetNiceName() + "  must be a boolean.");
            }
            else
            {
                bool isEnabled = (bool)toggleProperty.ValueEntry.WeakSmartValue;
                var isVisibleConfig = property.Context.Get(this, "isVisible", false);
                bool isVisible = isVisibleConfig.Value;

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
                        isVisible = false;
                    }
                }

                bool prev = isVisible;
                if (SirenixEditorGUI.BeginToggleGroup(UniqueDrawerKey.Create(property, this), ref isEnabled, ref isVisible, property.Info.MemberInfo.GetNiceName(), GeneralDrawerConfig.Instance.GUIFoldoutAnimationDuration))
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
                SirenixEditorGUI.EndToggleGroup();

                if (openGroup != null && prev != isVisible && isVisible)
                {
                    openGroup.Value = property.Path;
                }

                toggleProperty.ValueEntry.WeakSmartValue = isEnabled;
                isVisibleConfig.Value = isVisible;
            }
        }
    }
}
#endif