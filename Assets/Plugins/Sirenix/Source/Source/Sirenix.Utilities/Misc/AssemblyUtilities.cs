//-----------------------------------------------------------------------
// <copyright file="AssemblyUtilities.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------
namespace Sirenix.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using UnityEngine;
    using Utilities;

    /// <summary>
    /// TODO: Document.
    /// </summary>
#if UNITY_EDITOR

    [UnityEditor.InitializeOnLoad]
#endif
    public static class AssemblyUtilities
    {
        private static string[] userAssemblyPrefixes = new string[]
        {
            "Assembly-CSharp",
            "Assembly-UnityScript",
            "Assembly-Boo",
            "Assembly-CSharp-Editor",
            "Assembly-UnityScript-Editor",
            "Assembly-Boo-Editor",
        };

        private static string[] pluginAssemblyPrefixes = new string[]
        {
            "Assembly-CSharp-firstpass",
            "Assembly-CSharp-Editor-firstpass",
            "Assembly-UnityScript-firstpass",
            "Assembly-UnityScript-Editor-firstpass",
            "Assembly-Boo-firstpass",
            "Assembly-Boo-Editor-firstpass",
        };

        private static Assembly unityEngineAssembly;

#if UNITY_EDITOR
        private static Assembly unityEditorAssembly;
#endif
        private static DirectoryInfo projectFolderDirectory;
        private static Dictionary<string, Type> stringTypeLookup;
        private static Dictionary<Assembly, AssemblyTypeFlags> assemblyTypeFlagLookup;

        private static ImmutableList<Assembly> allAssemblies;

        private static List<Assembly> userAssemblies;
        private static List<Assembly> userEditorAssemblies;
        private static List<Assembly> pluginAssemblies;
        private static List<Assembly> pluginEditorAssemblies;
        private static List<Assembly> unityAssemblies;
        private static List<Assembly> unityEditorAssemblies;
        private static List<Assembly> otherAssemblies;
        private static Type[] userTypes;
        private static Type[] userEditorTypes;
        private static Type[] pluginTypes;
        private static Type[] pluginEditorTypes;
        private static Type[] unityTypes;
        private static Type[] unityEditorTypes;
        private static Type[] otherTypes;

        /// <summary>
        /// Re-scans the entire assembly.
        /// </summary>
        /// <exception cref="System.NotImplementedException"></exception>
        public static void Reload()
        {
            userAssemblies = new List<Assembly>();
            userEditorAssemblies = new List<Assembly>();
            pluginAssemblies = new List<Assembly>();
            pluginEditorAssemblies = new List<Assembly>();
            unityAssemblies = new List<Assembly>();
            unityEditorAssemblies = new List<Assembly>();
            otherAssemblies = new List<Assembly>();
            userTypes = new Type[0];
            userEditorTypes = new Type[0];
            pluginTypes = new Type[0];
            pluginEditorTypes = new Type[0];
            unityTypes = new Type[0];
            unityEditorTypes = new Type[0];
            otherTypes = new Type[0];
            stringTypeLookup = new Dictionary<string, Type>();
            assemblyTypeFlagLookup = new Dictionary<Assembly, AssemblyTypeFlags>();
            unityEngineAssembly = typeof(Vector3).Assembly;

#if UNITY_EDITOR
            unityEditorAssembly = typeof(UnityEditor.EditorWindow).Assembly;
#endif

            projectFolderDirectory = new DirectoryInfo(Application.dataPath);

            allAssemblies = new ImmutableList<Assembly>(AppDomain.CurrentDomain.GetAssemblies());
            for (int i = 0; i < allAssemblies.Count; i++)
            {
                try
                {
                    var assemblyType = GetAssemblyTypeFlag(allAssemblies[i]);

                    Type[] types = allAssemblies[i].GetTypes();
                    for (int j = 0; j < types.Length; j++)
                    {
                        Type type = types[j];

                        if (type.Namespace != null)
                        {
                            stringTypeLookup[type.Namespace + "." + type.Name] = type;
                        }
                        else
                        {
                            stringTypeLookup[type.Name] = type;
                        }
                    }

                    if (assemblyType == AssemblyTypeFlags.UserTypes)
                    {
                        userAssemblies.Add(allAssemblies[i]);
                    }
                    else if (assemblyType == AssemblyTypeFlags.UserEditorTypes)
                    {
                        userEditorAssemblies.Add(allAssemblies[i]);
                    }
                    else if (assemblyType == AssemblyTypeFlags.PluginTypes)
                    {
                        pluginAssemblies.Add(allAssemblies[i]);
                    }
                    else if (assemblyType == AssemblyTypeFlags.PluginEditorTypes)
                    {
                        pluginEditorAssemblies.Add(allAssemblies[i]);
                    }
                    else if (assemblyType == AssemblyTypeFlags.UnityTypes)
                    {
                        unityAssemblies.Add(allAssemblies[i]);
                    }
                    else if (assemblyType == AssemblyTypeFlags.UnityEditorTypes)
                    {
                        unityEditorAssemblies.Add(allAssemblies[i]);
                    }
                    else if (assemblyType == AssemblyTypeFlags.OtherTypes)
                    {
                        otherAssemblies.Add(allAssemblies[i]);
                    }
                    else
                    {
                        throw new NotImplementedException();
                    }
                }
                catch (ReflectionTypeLoadException)
                {
                    // This happens in builds if people are compiling with a subset of .NET
                    // It means we simply skip this assembly and its types completely when scanning for formatter types
                }
            }

            userTypes = userAssemblies.SelectMany(x => x.GetTypes()).ToArray();
            userEditorTypes = userEditorAssemblies.SelectMany(x => x.GetTypes()).ToArray();
            pluginTypes = pluginAssemblies.SelectMany(x => x.GetTypes()).ToArray();
            pluginEditorTypes = pluginEditorAssemblies.SelectMany(x => x.GetTypes()).ToArray();
            unityTypes = unityAssemblies.SelectMany(x => x.GetTypes()).ToArray();
            unityEditorTypes = unityEditorAssemblies.SelectMany(x => x.GetTypes()).ToArray();
            otherTypes = otherAssemblies.SelectMany(x => x.GetTypes()).ToArray();
        }

        /// <summary>
        /// Initializes the <see cref="AssemblyUtilities"/> class.
        /// </summary>
        static AssemblyUtilities()
        {
            Reload();
        }

        /// <summary>
        /// Gets an <see cref="ImmutableList"/> of all assemblies in the current <see cref="System.AppDomain"/>.
        /// </summary>
        /// <returns>An <see cref="ImmutableList"/> of all assemblies in the current <see cref="AppDomain"/>.</returns>
        public static ImmutableList<Assembly> GetAllAssemblies()
        {
            return allAssemblies;
        }

        /// <summary>
        /// Gets the <see cref="Sirenix.Utilities.AssemblyTypeFlags"/> for a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The <see cref="Sirenix.Utilities.AssemblyTypeFlags"/> for a given assembly.</returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is null.</exception>
        public static AssemblyTypeFlags GetAssemblyTypeFlag(this Assembly assembly)
        {
            if (assembly == null) throw new NullReferenceException("assembly");

            AssemblyTypeFlags result;

            if (assemblyTypeFlagLookup.TryGetValue(assembly, out result) == false)
            {
                result = GetAssemblyTypeFlagNoLookup(assembly);

                assemblyTypeFlagLookup[assembly] = result;
            }

            return result;
        }

        private static AssemblyTypeFlags GetAssemblyTypeFlagNoLookup(Assembly assembly)
        {
            AssemblyTypeFlags result;
            string path = assembly.GetAssemblyDirectory();
            string name = assembly.FullName.ToLower(CultureInfo.InvariantCulture);
            bool isPluginScriptAssembly = name.StartsWithAnyOf(pluginAssemblyPrefixes, StringComparison.InvariantCultureIgnoreCase);
            bool isGame = assembly.IsDependentOn(unityEngineAssembly);
            bool isPlugin = isPluginScriptAssembly || (path != null && Directory.Exists(path) && projectFolderDirectory.IsSubDirectoryOf(new DirectoryInfo(path)));
            bool isUser = !isPlugin && name.StartsWithAnyOf(userAssemblyPrefixes, StringComparison.InvariantCultureIgnoreCase);

#if UNITY_EDITOR
            bool isEditor = isUser ? name.Contains("-editor") : assembly.IsDependentOn(unityEditorAssembly);
            if (isUser)
            {
                isEditor = name.Contains("-editor");
            }
            else
            {
                if (isPlugin && path != null && (isPluginScriptAssembly == false || (isEditor && name.Contains("-editor")) == false))
                {
                    isEditor = ("/" + path
                        .Substring(Application.dataPath.Length, path.Length - Application.dataPath.Length) + "/")
                        .Contains("/editor/", StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    isEditor = assembly.IsDependentOn(unityEditorAssembly);
                }
            }

#else
                bool isEditor = false;
#endif
            if (!isGame && !isEditor && !isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.OtherTypes;
            }
            else if (isEditor && !isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.UnityEditorTypes;
            }
            else if (isGame && !isEditor && !isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.UnityTypes;
            }
            else if (isEditor && isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.PluginEditorTypes;
            }
            else if (!isEditor && isPlugin && !isUser)
            {
                result = AssemblyTypeFlags.PluginTypes;
            }
            else if (isEditor && isUser)
            {
                result = AssemblyTypeFlags.UserEditorTypes;
            }
            else if (!isEditor && isUser)
            {
                result = AssemblyTypeFlags.UserTypes;
            }
            else
            {
                result = AssemblyTypeFlags.OtherTypes;
            }

            return result;
        }

        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <param name="fullName">The full name of the type without any assembly information.</param>
        /// <returns></returns>
        public static Type GetType(string fullName)
        {
            Type type;
            if (stringTypeLookup.TryGetValue(fullName, out type))
            {
                return type;
            }
            return null;
        }

        /// <summary>
        /// Determines whether an assembly is depended on another assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <param name="otherAssembly">The other assembly.</param>
        /// <returns>
        ///   <c>true</c> if <paramref name="assembly"/> has a reference in <paramref name="otherAssembly"/> or <paramref name="assembly"/> is the same as <paramref name="otherAssembly"/>.
        /// </returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is null.</exception>
        /// <exception cref="System.NullReferenceException"><paramref name="otherAssembly"/> is null.</exception>
        public static bool IsDependentOn(this Assembly assembly, Assembly otherAssembly)
        {
            if (assembly == null) throw new NullReferenceException("assembly");
            if (otherAssembly == null) throw new NullReferenceException("otherAssembly");

            if (assembly == otherAssembly)
            {
                return true;
            }

            var otherName = otherAssembly.GetName().ToString();

            var referencedAsssemblies = assembly.GetReferencedAssemblies();

            for (int i = 0; i < referencedAsssemblies.Length; i++)
            {
                if (otherName == referencedAsssemblies[i].ToString())
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Determines whether the assembly module is a of type <see cref="System.Reflection.Emit.ModuleBuilder"/>.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>
        ///   <c>true</c> if the specified assembly of type <see cref="System.Reflection.Emit.ModuleBuilder"/>; otherwise, <c>false</c>.
        /// </returns>
        /// <exception cref="ArgumentNullException">assembly</exception>
        public static bool IsDynamic(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");
            return assembly.ManifestModule is System.Reflection.Emit.ModuleBuilder;
        }

        /// <summary>
        /// Gets the full file path to a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The full file path to a given assembly, or <c>Null</c> if no file path was found.</returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is Null.</exception>
        public static string GetAssemblyDirectory(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            var path = assembly.GetAssemblyFilePath();
            if (path == null)
            {
                return null;
            }

            return Path.GetDirectoryName(path);
        }

        /// <summary>
        /// Gets the full directory path to a given assembly.
        /// </summary>
        /// <param name="assembly">The assembly.</param>
        /// <returns>The full directory path in which a given assembly is located, or <c>Null</c> if no file path was found.</returns>
        /// <exception cref="System.NullReferenceException"><paramref name="assembly"/> is Null.</exception>
        public static string GetAssemblyFilePath(this Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            if (assembly.IsDynamic())
            {
                return null;
            }

            if (string.IsNullOrEmpty(assembly.CodeBase))
            {
                return null;
            }
            else
            {
                UriBuilder uri = new UriBuilder(assembly.CodeBase);
                return Uri.UnescapeDataString(uri.Path);
            }
        }

        /// <summary>
        /// Get types from the current AppDomain with a specified <see cref="Sirenix.Utilities.AssemblyTypeFlags"/> filter.
        /// </summary>
        /// <param name="assemblyTypeFlags">The <see cref="Sirenix.Utilities.AssemblyTypeFlags"/> filters.</param>
        /// <returns>Types from the current AppDomain with the specified <see cref="Sirenix.Utilities.AssemblyTypeFlags"/> filters.</returns>
        public static IEnumerable<Type> GetTypes(AssemblyTypeFlags assemblyTypeFlags)
        {
            bool includeUserTypes = (assemblyTypeFlags & AssemblyTypeFlags.UserTypes) == AssemblyTypeFlags.UserTypes;
            bool includeUserEditorTypes = (assemblyTypeFlags & AssemblyTypeFlags.UserEditorTypes) == AssemblyTypeFlags.UserEditorTypes;
            bool includePluginTypes = (assemblyTypeFlags & AssemblyTypeFlags.PluginTypes) == AssemblyTypeFlags.PluginTypes;
            bool includePluginEditorTypes = (assemblyTypeFlags & AssemblyTypeFlags.PluginEditorTypes) == AssemblyTypeFlags.PluginEditorTypes;
            bool includeUnityTypes = (assemblyTypeFlags & AssemblyTypeFlags.UnityTypes) == AssemblyTypeFlags.UnityTypes;
            bool includeUnityEditorTypes = (assemblyTypeFlags & AssemblyTypeFlags.UnityEditorTypes) == AssemblyTypeFlags.UnityEditorTypes;
            bool includeOtherTypes = (assemblyTypeFlags & AssemblyTypeFlags.OtherTypes) == AssemblyTypeFlags.OtherTypes;

            if (includeUserTypes) for (int i = 0; i < userTypes.Length; i++) yield return userTypes[i];
            if (includeUserEditorTypes) for (int i = 0; i < userEditorTypes.Length; i++) yield return userEditorTypes[i];
            if (includePluginTypes) for (int i = 0; i < pluginTypes.Length; i++) yield return pluginTypes[i];
            if (includePluginEditorTypes) for (int i = 0; i < pluginEditorTypes.Length; i++) yield return pluginEditorTypes[i];
            if (includeUnityTypes) for (int i = 0; i < unityTypes.Length; i++) yield return unityTypes[i];
            if (includeUnityEditorTypes) for (int i = 0; i < unityEditorTypes.Length; i++) yield return unityEditorTypes[i];
            if (includeOtherTypes) for (int i = 0; i < otherTypes.Length; i++) yield return otherTypes[i];
        }

        private static bool StartsWithAnyOf(this string str, IEnumerable<string> values, StringComparison comparisonType = StringComparison.CurrentCulture)
        {
            var iList = values as IList<string>;

            if (iList != null)
            {
                int count = iList.Count;
                for (int i = 0; i < count; i++)
                {
                    if (str.StartsWith(iList[i], comparisonType))
                    {
                        return true;
                    }
                }
            }
            else
            {
                foreach (var value in values)
                {
                    if (str.StartsWith(value, comparisonType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}