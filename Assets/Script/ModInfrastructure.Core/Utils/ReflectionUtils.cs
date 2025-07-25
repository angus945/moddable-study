using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ModArchitecture.Logger;

namespace ModArchitecture.Utils
{
    /// <summary>
    /// 統一的反射載入工具類，提供安全的類型掃描和載入功能
    /// </summary>
    public static class ReflectionUtils
    {
        /// <summary>
        /// 安全地獲取所有實現指定介面或繼承指定類別的類型
        /// </summary>
        /// <typeparam name="T">要搜尋的基底類型或介面</typeparam>
        /// <param name="includeAbstract">是否包含抽象類別</param>
        /// <param name="includeInterface">是否包含介面</param>
        /// <returns>符合條件的類型列表</returns>
        public static IEnumerable<Type> GetTypesAssignableFrom<T>(bool includeAbstract = false, bool includeInterface = false)
        {
            return GetTypesAssignableFrom(typeof(T), includeAbstract, includeInterface);
        }

        /// <summary>
        /// 安全地獲取所有實現指定介面或繼承指定類別的類型
        /// </summary>
        /// <param name="baseType">要搜尋的基底類型或介面</param>
        /// <param name="includeAbstract">是否包含抽象類別</param>
        /// <param name="includeInterface">是否包含介面</param>
        /// <returns>符合條件的類型列表</returns>
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
        /// <param name="typeName">類型名稱</param>
        /// <param name="baseType">可選的基底類型過濾器</param>
        /// <returns>找到的類型，如果沒找到則返回 null</returns>
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
        /// 安全地創建類型實例
        /// </summary>
        /// <typeparam name="T">目標類型</typeparam>
        /// <param name="type">要實例化的類型</param>
        /// <param name="args">構造函數參數</param>
        /// <returns>創建的實例，如果失敗則返回 default(T)</returns>
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
        /// 獲取安全的組件列表，排除系統組件
        /// </summary>
        /// <returns>安全的組件列表</returns>
        public static IEnumerable<Assembly> GetSafeAssemblies()
        {
            var allAssemblies = AppDomain.CurrentDomain.GetAssemblies();
            var safeAssemblies = allAssemblies.Where(assembly => !ShouldSkipAssembly(assembly)).ToList();

            ModLogger.Log($"Total assemblies: {allAssemblies.Length}, Safe assemblies: {safeAssemblies.Count}");

            // 列出所有安全的組件名稱
            var safeAssemblyNames = safeAssemblies.Select(a => a.GetName().Name).ToList();
            ModLogger.Log($"Safe assemblies: [{string.Join(", ", safeAssemblyNames)}]");

            return safeAssemblies;
        }

        /// <summary>
        /// 安全地從組件中獲取類型
        /// </summary>
        /// <param name="assembly">目標組件</param>
        /// <returns>組件中的類型列表</returns>
        public static IEnumerable<Type> GetTypesFromAssembly(Assembly assembly)
        {
            try
            {
                return assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                // 處理部分類型載入失敗的情況
                var assemblyName = assembly.GetName().Name;
                ModLogger.LogWarning($"Some types in assembly {assemblyName} could not be loaded. Using available types.");

                // 記錄具體的載入錯誤以便除錯
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

                // 返回成功載入的類型
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
        /// 判斷是否應該跳過某個組件
        /// </summary>
        /// <param name="assembly">要檢查的組件</param>
        /// <returns>是否應該跳過</returns>
        public static bool ShouldSkipAssembly(Assembly assembly)
        {
            var assemblyName = assembly.FullName;
            var assemblySimpleName = assembly.GetName().Name;

            // 特別保留的 Mod 相關組件（包括動態載入的 Mod）
            string[] keepPrefixes = {
                "ModInfrastructure",
                "Implement",
                "Assembly-CSharp",
                "Mod"  // 添加 "Mod" 前綴來保留所有 ModA, ModB 等組件
            };

            // 如果是 Mod 相關組件，不跳過
            if (keepPrefixes.Any(prefix => assemblySimpleName.StartsWith(prefix)))
            {
                // ModLogger.Log($"Keeping mod-related assembly: {assemblySimpleName}");
                return false;
            }

            // 跳過系統組件和已知會造成問題的組件
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

            // 添加調試資訊
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
