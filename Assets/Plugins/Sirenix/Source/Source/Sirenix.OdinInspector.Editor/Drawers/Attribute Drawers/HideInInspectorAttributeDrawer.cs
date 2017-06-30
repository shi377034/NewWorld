#if UNITY_EDITOR
//-----------------------------------------------------------------------
// <copyright file="HideInInspectorAttributeDrawer.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

namespace Sirenix.OdinInspector.Editor.Drawers
{
    using UnityEngine;

    /// <summary>
    /// Draws properties marked with <see cref="HideInInspector"/>
    /// </summary>
    /// <seealso cref="HideInInspector"/>
    /// <seealso cref="ShowIfAttribute"/>
    /// <seealso cref="HideIfAttribute"/>
    /// <seealso cref="ReadOnlyAttribute"/>
    /// <seealso cref="EnableIfAttribute"/>
    /// <seealso cref="DisableIfAttribute"/>
    /// <seealso cref="DisableInEditorAttribute"/>
    /// <seealso cref="DisableInPlayModeAttribute"/>
    [OdinDrawer]
    [DrawerPriority(1000, 0, 0)]
    public sealed class HideInInspectorAttributeDrawer : OdinAttributeDrawer<HideInInspector>
    {
        /// <summary>
        /// Does not draw the property.
        /// </summary>
        protected override void DrawPropertyLayout(InspectorProperty property, HideInInspector attribute, GUIContent label)
        {
        }
    }
}
#endif