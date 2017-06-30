#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="TabGroupAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.OdinInspector.Editor.Drawers
{
    using System.Collections.Generic;
    using Utilities.Editor;
    using UnityEngine;

    /// <summary>
    /// Draws all properties grouped together with the <see cref="TabGroupAttribute"/>
    /// </summary>
    /// <seealso cref="TabGroupAttribute"/>
    [OdinDrawer]
    public class TabGroupAttributeDrawer : OdinGroupDrawer<TabGroupAttribute>
    {
        private class Tab
        {
            public string TabName;
            public List<InspectorProperty> InspectorProperties;
        }

        /// <summary>
        /// Draws the property.
        /// </summary>
        protected override void DrawPropertyGroupLayout(InspectorProperty property, TabGroupAttribute attribute, GUIContent label)
        {
            var tabGroup = SirenixEditorGUI.CreateAnimatedTabGroup(property);
            tabGroup.AnimationSpeed = 1 / GeneralDrawerConfig.Instance.TabPageSlideAnimationDuration;
            tabGroup.FixedHeight = attribute.UseFixedHeight;

            var tabs = property.Context.Get(this, "attribute", (List<Tab>)null);
            if (tabs.Value == null)
            {
                tabs.Value = new List<Tab>();
                for (int i = 0; i < attribute.Tabs.Count; i++)
                {
                    var tab = new Tab();
                    tab.TabName = attribute.Tabs[i];
                    tab.InspectorProperties = new List<InspectorProperty>();

                    for (int j = 0; j < property.Children.Count; j++)
                    {
                        if (property.Children[j].Info.GetAttribute<TabGroupAttribute>().TabName == tab.TabName)
                        {
                            tab.InspectorProperties.Add(property.Children[j]);
                        }
                    }

                    tabs.Value.Add(tab);
                }

                for (int j = 0; j < property.Children.Count; j++)
                {
                    if (property.Children[j].Info.GetAttribute<TabGroupAttribute>().TabName == null)
                    {
                        var tab = new Tab();
                        tab.TabName = property.Children[j].NiceName;
                        tab.InspectorProperties = new List<InspectorProperty>();
                        tab.InspectorProperties.Add(property.Children[j]);
                        tabs.Value.Add(tab);
                    }
                }
            }

            for (int i = 0; i < tabs.Value.Count; i++)
            {
                tabGroup.RegisterTab(tabs.Value[i].TabName);
            }

            SirenixEditorGUI.BeginIndentedVertical(SirenixGUIStyles.PropertyPadding);
            tabGroup.BeginGroup();

            for (int i = 0; i < tabs.Value.Count; i++)
            {
                var page = tabGroup.RegisterTab(tabs.Value[i].TabName);

                if (page.BeginPage())
                {
                    int pageCount = tabs.Value[i].InspectorProperties.Count;
                    for (int j = 0; j < pageCount; j++)
                    {
                        InspectorUtilities.DrawProperty(tabs.Value[i].InspectorProperties[j]);
                    }
                }
                page.EndPage();
            }

            tabGroup.EndGroup();
            SirenixEditorGUI.EndIndentedVertical();
        }
    }
}
#endif