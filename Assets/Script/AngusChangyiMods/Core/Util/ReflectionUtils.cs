using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace AngusChangyiMods.Core.Utils
{
    /// <summary>
    /// Unified reflection loading utility class, providing safe type scanning and loading functionality
    /// </summary>
    public class ReflectionUtils
    {
        // Cache for assembly types - maps assembly to its types
        private static readonly Dictionary<Assembly, List<Type>> _assemblyTypesCache = new Dictionary<Assembly, List<Type>>();

        // Cache for assignable types - maps (baseType, includeAbstract, includeInterface) to result
        private static readonly Dictionary<string, List<Type>> _assignableTypesCache = new Dictionary<string, List<Type>>();

        // Cache for type name lookups - maps (typeName, baseTypeName) to result
        private static readonly Dictionary<string, Type> _typeNameCache = new Dictionary<string, Type>();

        // Lock for thread safety
        private static readonly object _cacheLock = new object();

        // Flag to track if initial scan has been performed
        private static bool _isInitialized = false;

        /// <summary>
        /// Static constructor - automatically clears cache during class initialization
        /// </summary>
        static ReflectionUtils()
        {
            ClearCache();
        }

        /// <summary>
        /// Clear all caches and reset initialization state
        /// </summary>
        public static void ClearCache()
        {
            lock (_cacheLock)
            {
                _assemblyTypesCache.Clear();
                _assignableTypesCache.Clear();
                _typeNameCache.Clear();
                _isInitialized = false;
            }
        }

        /// <summary>
        /// Clear cache for testing purposes - ensures clean state between tests
        /// </summary>
        [System.Diagnostics.Conditional("UNITY_EDITOR")]
        public static void ClearCacheForTesting()
        {
            ClearCache();
        }

        /// <summary>
        /// Refresh cache by clearing and re-initializing
        /// </summary>
        public static void RefreshCache()
        {
            lock (_cacheLock)
            {
                ClearCache();
                EnsureInitialized();
            }
        }

        /// <summary>
        /// Initialize cache if not already done
        /// </summary>
        private static void EnsureInitialized()
        {
            if (!_isInitialized)
            {
                lock (_cacheLock)
                {
                    if (!_isInitialized)
                    {
                        InitializeCache();
                        _isInitialized = true;
                    }
                }
            }
        }

        /// <summary>
        /// Initialize the assembly types cache
        /// </summary>
        private static void InitializeCache()
        {
            var safeAssemblies = GetSafeAssembliesInternal();
            int totalTypes = 0;

            foreach (var assembly in safeAssemblies)
            {
                try
                {
                    var types = GetTypesFromAssemblyInternal(assembly);
                    _assemblyTypesCache[assembly] = types.ToList();
                    totalTypes += types.Count();
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException($"Failed to cache types from assembly {assembly.FullName}", ex);
                }
            }
        }
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
            EnsureInitialized();

            // Create cache key
            var cacheKey = $"{baseType.FullName}|{includeAbstract}|{includeInterface}";

            lock (_cacheLock)
            {
                // Check if result is already cached
                if (_assignableTypesCache.TryGetValue(cacheKey, out var cachedResult))
                {
                    return cachedResult;
                }

                // Compute result and cache it
                var allTypes = new List<Type>();
                int assembliesWithMatchingTypes = 0;

                foreach (var assembly in _assemblyTypesCache.Keys)
                {
                    try
                    {
                        var types = _assemblyTypesCache[assembly]
                            .Where(t => baseType.IsAssignableFrom(t) &&
                                      (includeInterface || !t.IsInterface) &&
                                      (includeAbstract || !t.IsAbstract))
                            .ToList();

                        if (types.Count > 0)
                        {
                            assembliesWithMatchingTypes++;
                            allTypes.AddRange(types);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to scan types from assembly {assembly.FullName}", ex);
                    }
                }

                // Cache the result
                _assignableTypesCache[cacheKey] = allTypes;

                return allTypes;
            }
        }

        /// <summary>
        /// 根據類型名稱搜尋類型
        /// </summary>
        /// <param name="typeName">Type name</param>
        /// <param name="baseType">Optional base type filter</param>
        /// <returns>Found type, returns null if not found</returns>
        public static Type FindTypeByName(string typeName, Type baseType = null)
        {
            EnsureInitialized();

            // Create cache key
            var baseTypeName = baseType?.FullName ?? "null";
            var cacheKey = $"{typeName}|{baseTypeName}";

            lock (_cacheLock)
            {
                // Check if result is already cached
                if (_typeNameCache.TryGetValue(cacheKey, out var cachedResult))
                {
                    return cachedResult;
                }

                // Compute result and cache it
                Type foundType = null;

                foreach (var assembly in _assemblyTypesCache.Keys)
                {
                    try
                    {
                        var types = _assemblyTypesCache[assembly];
                        foundType = types.FirstOrDefault(t =>
                            t.Name == typeName &&
                            (baseType == null || baseType.IsAssignableFrom(t)));

                        if (foundType != null)
                            break;
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to search type in assembly {assembly.FullName}", ex);
                    }
                }

                // Cache the result (including null results)
                _typeNameCache[cacheKey] = foundType;

                return foundType;
            }
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
                throw new InvalidOperationException($"Failed to create instance of type {type.FullName}", ex);
            }
        }

        /// <summary>
        /// Get safe assembly list, excluding system assemblies
        /// </summary>
        /// <returns>Safe assembly list</returns>
        public static IEnumerable<Assembly> GetSafeAssemblies()
        {
            EnsureInitialized();

            lock (_cacheLock)
            {
                return _assemblyTypesCache.Keys.ToList();
            }
        }

        /// <summary>
        /// Safely get types from assembly (using cache)
        /// </summary>
        /// <param name="assembly">Target assembly</param>
        /// <returns>List of types in assembly</returns>
        public static IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            EnsureInitialized();

            lock (_cacheLock)
            {
                if (_assemblyTypesCache.TryGetValue(assembly, out var types))
                {
                    return types;
                }

                // If assembly not in cache, try to load it
                var loadedTypes = GetTypesFromAssemblyInternal(assembly).ToList();
                _assemblyTypesCache[assembly] = loadedTypes;
                return loadedTypes;
            }
        }

        /// <summary>
        /// Get safe assembly list, excluding system assemblies (internal version)
        /// </summary>
        /// <returns>Safe assembly list</returns>
        private static IEnumerable<Assembly> GetSafeAssembliesInternal()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var safeAssemblies = allAssemblies.Where(assembly => !ShouldSkipAssembly(assembly)).ToList();

            // List all safe assembly names
            var safeAssemblyNames = safeAssemblies.Select(a => a.GetName().Name).ToList();

            return safeAssemblies;
        }

        /// <summary>
        /// Safely get types from assembly (internal version)
        /// </summary>
        /// <param name="assembly">Target assembly</param>
        /// <returns>List of types in assembly</returns>
        private static IEnumerable<Type> GetTypesFromAssemblyInternal(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // Handle partial type loading failure
                var assemblyName = assembly.GetName().Name;

                // Return successfully loaded types
                var loadedTypes = ex.Types.Where(t => t != null).ToList();

                return loadedTypes;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Unexpected error loading types from assembly {assembly.GetName().Name}", ex);
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
