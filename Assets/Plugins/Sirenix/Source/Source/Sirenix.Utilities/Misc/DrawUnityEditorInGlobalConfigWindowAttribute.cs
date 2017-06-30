namespace Sirenix.Utilities
{
    using System;

    // TODO: Remove this class, and don't use only use PropertyTree at all in the Odin Preferences window.

    /// <summary>
    /// <para>This attribute is used by classes deriving from GlobalConfig and tells the advanced preferences to
    /// draw it by creating a UnityEditor.Editor instead of a PropertyTree.</para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class DrawUnityEditorInGlobalConfigWindowAttribute : Attribute
    {
    }
}