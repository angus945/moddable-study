using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModArchitecture.Logger;

namespace ModArchitecture.Utils
{
    /// <summary>
    /// Unified reflection loading utility class, providing safe type scanning and loading functionality
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// Safely get all types that implement a specified interface or inherit from a specified class
        /// </summary>
        /// <typeparam name="T">The base type or interface to search for</typeparam>
        /// <param name="includeAbstract">Whether to include abstract classes</param>
        /// <param name="includeInterface">Whether to include interfaces</param>
        /// <returns>List of types that meet the criteria</returns>
        public static IEnumerable<Type> GetTypesAssignableFrom<T>(bool includeAbstract = false, bool includeInterface = false)
        {
            return GetTypesAssignableFrom(typeof(T), includeAbstract, includeInterface);
        }

        /// <summary>
        /// Safely get all types that implement a specified interface or inherit from a specified class
        /// </summary>
        /// <param name="baseType">The base type or interface to search for</param>
        /// <param name="includeAbstract">Whether to include abstract classes</param>
        /// <param name="includeInterface">Whether to include interfaces</param>
        /// <returns>List of types that meet the criteria</returns>
        public static IEnumerable<Type> GetTypesAssignableFrom(Type baseType, bool includeAbstract = false, bool includeInterface = false)
        {
            var allTypes = new List<Type>();
            int totalAssembliesChecked = 0;
            int assembliesWithMatchingTypes = 0;

            foreach (var assembly in GetSafeAssemblies())
            {
                totalAssembliesChecked++;
                try
                {
                    var types = GetTypesFromAssembly(assembly)
                        .Where(t => baseType.IsAssignableFrom(t) &&
                                  (includeInterface || !t.IsInterface) &&
                                  (includeAbstract || !t.IsAbstract))
                        .ToList();

                    if (types.Count > 0)
                    {
                        assembliesWithMatchingTypes++;
                        ModLogger.Log($"Found {types.Count} types implementing {baseType.Name} in assembly {assembly.GetName().Name}");
                        allTypes.AddRange(types);
                    }
                }
                catch (Exception ex)
                {
                    ModLogger.LogError($"Failed to scan types from assembly {assembly.FullName}: {ex.Message}");
                }
            }

            ModLogger.Log($"Scanned {totalAssembliesChecked} assemblies, found {baseType.Name} implementations in {assembliesWithMatchingTypes} assemblies, total types: {allTypes.Count}");
            return allTypes;
        }

        /// <summary>
        /// 根據類型名稱搜尋類型
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <param name="baseType">Optional base type filter</param>
        /// <returns>Found type, returns null if not found</returns>
        public static Type FindTypeByName(string typeName, Type baseType = null)
        {
            foreach (var assembly in GetSafeAssemblies())
            {
                try
                {
                    var types = GetTypesFromAssembly(assembly);
                    var foundType = types.FirstOrDefault(t =>
                        t.Name == typeName &&
                        (baseType == null || baseType.IsAssignableFrom(t)));

                    if (foundType != null)
                        return foundType;
                }
                catch (Exception ex)
                {
                    ModLogger.LogError($"Failed to search type in assembly {assembly.FullName}: {ex.Message}");
                }
            }

            return null;
        }

        /// <summary>
        /// Safely create type instance
        /// </summary>
        /// <typeparam name="T">Target type</typeparam>
        /// <param name="type">Type to instantiate</param>
        /// <param name="args">Constructor parameters</param>
        /// <returns>Created instance, returns default(T) if failed</returns>
        public static T SafeCreateInstance<T>(Type type, params object[] args)
        {
            try
            {
                return (T)Activator.CreateInstance(type, args);
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Failed to create instance of type {type.FullName}: {ex.Message}");
                return default(T);
            }
        }

        /// <summary>
        /// Get safe assembly list, excluding system assemblies
        /// </summary>
        /// <returns>Safe assembly list</returns>
        public static IEnumerable<Assembly> GetSafeAssemblies()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var safeAssemblies = allAssemblies.Where(assembly => !ShouldSkipAssembly(assembly)).ToList();

            ModLogger.Log($"Total assemblies: {allAssemblies.Length}, Safe assemblies: {safeAssemblies.Count}");

            // List all safe assembly names
            var safeAssemblyNames = safeAssemblies.Select(a => a.GetName().Name).ToList();
            ModLogger.Log($"Safe assemblies: [{string.Join(", ", safeAssemblyNames)}]");

            return safeAssemblies;
        }

        /// <summary>
        /// Safely get types from assembly
        /// </summary>
        /// <param name="assembly">Target assembly</param>
        /// <returns>List of types in assembly</returns>
        public static IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Handle partial type loading failure
                var assemblyName = assembly.GetName().Name;
                ModLogger.LogWarning($"Some types in assembly {assemblyName} could not be loaded. Using available types.");

                // Log specific loading errors for debugging
                if (ex.LoaderExceptions != null && ex.LoaderExceptions.Length > 0)
                {
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        if (loaderException != null)
                        {
                            ModLogger.LogWarning($"  Loader exception: {loaderException.Message}");
                        }
                    }
                }

                // Return successfully loaded types
                var loadedTypes = ex.Types.Where(t => t != null).ToList();
                ModLogger.Log($"  Successfully loaded {loadedTypes.Count} types from {assemblyName}");

                return loadedTypes;
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Unexpected error loading types from assembly {assembly.GetName().Name}: {ex.Message}");
                return new Type[0];
            }
        }

        /// <summary>
        /// Determine whether to skip an assembly
        /// </summary>
        /// <param name="assembly">Assembly to check</param>
        /// <returns>是否應該跳過</returns>
        public static bool ShouldSkipAssembly(Assembly assembly)
        {
            var assemblyName = assembly.FullName;
            var assemblySimpleName = assembly.GetName().Name;

            // Specially reserved Mod-related assemblies (including dynamically loaded Mods)
            string[] keepPrefixes = {
                "ModInfrastructure",
                "Implement",
                "Assembly-CSharp",
                "Mod"  // Add "Mod" prefix to preserve all ModA, ModB etc. assemblies
            };

            // If it's a Mod-related assembly, don't skip
            if (keepPrefixes.Any(prefix => assemblySimpleName.StartsWith(prefix)))
            {
                // ModLogger.Log($"Keeping mod-related assembly: {assemblySimpleName}");
                return false;
            }

            // Skip system assemblies and known problematic assemblies
            string[] skipPrefixes = {
                "System.",
                "Microsoft.",
                "UnityEngine.",
                "UnityEditor.",
                "mscorlib",
                "netstandard",
                "Mono.",
                "nunit.",
                "Newtonsoft.",
                "log4net"
            };

            bool shouldSkip = skipPrefixes.Any(prefix => assemblyName.StartsWith(prefix));

            // Add debug information
            if (shouldSkip)
            {
                // ModLogger.Log($"Skipping system assembly: {assemblySimpleName}");
            }
            else
            {
                // ModLogger.Log($"Including assembly: {assemblySimpleName}");
            }

            return shouldSkip;
        }
    }
}
