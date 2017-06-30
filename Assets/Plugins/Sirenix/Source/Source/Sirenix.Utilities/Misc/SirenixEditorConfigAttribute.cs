//-----------------------------------------------------------------------
// <copyright file="SirenixEditorConfigAttribute.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities
{
    /// <summary>
    /// <para>This attribute is used by classes deriving from GlobalConfig and specifies the menu item path for the preference window and the asset path for the generated config file.</para>
    /// <para>The scriptable object created will be located at the OdinEditorConfigs path unless other is specified.</para>
    /// <para>Classes implementing this attribute will be part of the Odin Preferences window.</para>
    /// </summary>
    /// <seealso cref="SirenixGlobalConfigAttribute"/>
    public class SirenixEditorConfigAttribute : SirenixGlobalConfigAttribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SirenixEditorConfigAttribute"/> class.
        /// </summary>
        /// <param name="menuItem"></param>
        public SirenixEditorConfigAttribute(string menuItem)
            : base(SirenixAssetPaths.OdinEditorConfigsPath, menuItem)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SirenixEditorConfigAttribute"/> class.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="menuItem"></param>
        public SirenixEditorConfigAttribute(string path, string menuItem)
            : base(path, menuItem)
        {
        }
    }
}