//-----------------------------------------------------------------------
// <copyright file="DirectoryInfoExtensions.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities
{
    using System.IO;

    /// <summary>
    /// DirectoryInfo method extensions.
    /// </summary>
    public static class DirectoryInfoExtensions
    {
        /// <summary>
        /// Determines whether [is sub directory of] [the specified dir2].
        /// </summary>
        /// <param name="dir1">The dir1.</param>
        /// <param name="dir2">The dir2.</param>
        /// <returns>
        ///   <c>true</c> if [is sub directory of] [the specified dir2]; otherwise, <c>false</c>.
        /// </returns>
        public static bool IsSubDirectoryOf(this DirectoryInfo dir1, DirectoryInfo dir2)
        {
            while (dir2 != null)
            {
                if (dir2.FullName.TrimEnd('\\') == dir1.FullName.TrimEnd('\\'))
                {
                    return true;
                }
                else dir2 = dir2.Parent;
            }
            return false;
        }

        /// <summary>
        /// Finds the name of the parent directory with.
        /// </summary>
        /// <param name="dir">The directory info.</param>
        /// <param name="folderName">Name of the folder.</param>
        /// <returns></returns>
        public static DirectoryInfo FindParentDirectoryWithName(this DirectoryInfo dir, string folderName)
        {
            if (dir.Parent == null)
            {
                return null;
            }

            if (string.Equals(dir.Name, folderName, System.StringComparison.InvariantCultureIgnoreCase))
            {
                return dir;
            }

            return dir.Parent.FindParentDirectoryWithName(folderName);
        }
    }
}