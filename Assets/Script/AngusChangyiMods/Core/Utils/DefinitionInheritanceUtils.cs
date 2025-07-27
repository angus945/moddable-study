using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;


namespace AngusChangyiMods.Core.Utils
{
    /// <summary>
    /// Utility class to simplify DefinitionInheritor implementation and provide reusable inheritance operations.
    /// </summary>
    public static class DefinitionInheritanceUtils
    {
        // Delegate definitions for better readability and type safety

        /// <summary>
        /// Delegate for custom inheritance logic between parent and child definitions.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="child">Child definition to inherit to</param>
        /// <param name="parent">Parent definition to inherit from</param>
        public delegate void InheritanceAction<T>(T child, T parent) where T : AngusChangyiMods.Core.DefBase;

        /// <summary>
        /// Delegate for getting a property value from a definition.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the property value</typeparam>
        /// <param name="definition">Definition to get value from</param>
        /// <returns>Property value</returns>
        public delegate TValue PropertyGetter<T, TValue>(T definition);

        /// <summary>
        /// Delegate for setting a property value on a definition.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the property value</typeparam>
        /// <param name="definition">Definition to set value on</param>
        /// <param name="value">Value to set</param>
        public delegate void PropertySetter<T, TValue>(T definition, TValue value);

        /// <summary>
        /// Delegate for determining whether inheritance should occur based on child and parent values.
        /// </summary>
        /// <typeparam name="TValue">The type of the values to compare</typeparam>
        /// <param name="childValue">Value from child definition</param>
        /// <param name="parentValue">Value from parent definition</param>
        /// <returns>True if inheritance should occur, false otherwise</returns>
        public delegate bool InheritancePredicate<TValue>(TValue childValue, TValue parentValue);

        /// <summary>
        /// Delegate for creating a new object instance based on a parent object.
        /// </summary>
        /// <typeparam name="TObject">The type of object to create</typeparam>
        /// <param name="parent">Parent object to base creation on</param>
        /// <returns>New object instance</returns>
        public delegate TObject ObjectCreator<TObject>(TObject parent);

        /// <summary>
        /// Delegate for inheriting fields from parent object to child object.
        /// </summary>
        /// <typeparam name="TObject">The type of object</typeparam>
        /// <param name="child">Child object to inherit to</param>
        /// <param name="parent">Parent object to inherit from</param>
        public delegate void FieldInheritor<TObject>(TObject child, TObject parent);

        /// <summary>
        /// Process inheritance for a collection of definitions with recursive parent resolution.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="definitions">Definitions to process</param>
        /// <param name="specificInheritanceAction">Custom action for type-specific inheritance logic</param>
        /// <returns>Processed definitions with inheritance applied</returns>
        public static IEnumerable<T> ProcessInheritance<T>(
            IEnumerable<T> definitions,
            InheritanceAction<T> specificInheritanceAction = null) where T : AngusChangyiMods.Core.DefBase
        {
            var definitionList = definitions.ToList();
            var definitionMap = definitionList.ToDictionary(def => def.defID, def => def);
            var processedDefinitions = new List<T>();

            // First, process all definitions (including abstract ones) to ensure inheritance chains are complete
            var allProcessedMap = new Dictionary<string, T>();

            // Process in dependency order (parents before children)
            var sortedDefinitions = TopologicalSort(definitionList);

            ModLogger.Log($"Processing {sortedDefinitions.Count} definitions in topological order", "DefinitionInheritanceUtils");

            foreach (var def in sortedDefinitions)
            {
                ModLogger.Log($"Processing definition: {def.defID} (abstract: {def.IsAbstract}, inheritsFrom: {def.inheritsFrom ?? "none"})", "DefinitionInheritanceUtils");

                var processedDef = ProcessSingleDefinitionWithRecursion(def, allProcessedMap.Count > 0 ? allProcessedMap : definitionMap, specificInheritanceAction);
                allProcessedMap[def.defID] = processedDef;

                // Only include concrete definitions in final results
                if (!def.IsAbstract)
                {
                    processedDefinitions.Add(processedDef);
                    ModLogger.Log($"Added concrete definition {def.defID} to final results", "DefinitionInheritanceUtils");
                }
                else
                {
                    ModLogger.Log($"Skipped abstract definition {def.defID} from final results", "DefinitionInheritanceUtils");
                }
            }

            ModLogger.Log($"Processed {processedDefinitions.Count} concrete definitions of type {typeof(T).Name}", "DefinitionInheritanceUtils");
            return processedDefinitions;
        }

        /// <summary>
        /// Sort definitions in topological order (parents before children).
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="definitions">Definitions to sort</param>
        /// <returns>Sorted definitions</returns>
        private static List<T> TopologicalSort<T>(List<T> definitions) where T : AngusChangyiMods.Core.DefBase
        {
            var sorted = new List<T>();
            var visited = new HashSet<string>();
            var visiting = new HashSet<string>();

            ModLogger.Log($"Starting topological sort for {definitions.Count} definitions", "DefinitionInheritanceUtils");

            void Visit(T def)
            {
                if (visited.Contains(def.defID)) return;
                if (visiting.Contains(def.defID))
                {
                    ModLogger.LogWarning($"Circular dependency detected for {def.defID}", "DefinitionInheritanceUtils");
                    return;
                }

                visiting.Add(def.defID);

                // Visit parent first
                if (!string.IsNullOrEmpty(def.inheritsFrom))
                {
                    var parent = definitions.FirstOrDefault(d => d.defID == def.inheritsFrom);
                    if (parent != null)
                    {
                        ModLogger.Log($"Visiting parent {parent.defID} before {def.defID}", "DefinitionInheritanceUtils");
                        Visit(parent);
                    }
                }

                visiting.Remove(def.defID);
                visited.Add(def.defID);
                sorted.Add(def);
                ModLogger.Log($"Added {def.defID} to sorted list (position {sorted.Count})", "DefinitionInheritanceUtils");
            }

            foreach (var def in definitions)
            {
                Visit(def);
            }

            ModLogger.Log($"Topological sort completed. Order: {string.Join(" -> ", sorted.Select(d => d.defID))}", "DefinitionInheritanceUtils");
            return sorted;
        }

        /// <summary>
        /// Process inheritance for a single definition with recursive parent resolution.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="definition">Definition to process</param>
        /// <param name="definitionMap">Map of all available definitions</param>
        /// <param name="specificInheritanceAction">Custom action for type-specific inheritance logic</param>
        /// <returns>Processed definition</returns>
        private static T ProcessSingleDefinitionWithRecursion<T>(
            T definition,
            Dictionary<string, T> definitionMap,
            InheritanceAction<T> specificInheritanceAction) where T : AngusChangyiMods.Core.DefBase
        {
            if (string.IsNullOrEmpty(definition.inheritsFrom))
            {
                return definition; // No inheritance to process
            }

            // Get the inheritance chain (from root to immediate parent)
            var inheritanceChain = BuildInheritanceChain(definition, definitionMap);

            // Apply inheritance from root to leaf
            foreach (var parentDef in inheritanceChain)
            {
                ApplyCommonInheritance(definition, parentDef);
                specificInheritanceAction?.Invoke(definition, parentDef);

                ModLogger.Log($"Applied inheritance from {parentDef.defID} to {definition.defID}", "DefinitionInheritanceUtils");
            }

            return definition;
        }

        /// <summary>
        /// Build the inheritance chain from root to immediate parent.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="definition">Definition to build chain for</param>
        /// <param name="definitionMap">Map of all available definitions</param>
        /// <returns>List of parent definitions from root to immediate parent</returns>
        private static List<T> BuildInheritanceChain<T>(T definition, Dictionary<string, T> definitionMap) where T : AngusChangyiMods.Core.DefBase
        {
            var chain = new List<T>();
            var visited = new HashSet<string>(); // Prevent circular inheritance
            var current = definition;

            // Build chain from leaf to root
            var tempChain = new List<T>();
            ModLogger.Log($"Building inheritance chain for {definition.defID}", "DefinitionInheritanceUtils");

            while (!string.IsNullOrEmpty(current.inheritsFrom))
            {
                ModLogger.Log($"  Looking for parent: {current.inheritsFrom}", "DefinitionInheritanceUtils");

                if (visited.Contains(current.inheritsFrom))
                {
                    ModLogger.LogWarning($"Circular inheritance detected: {current.defID} -> {current.inheritsFrom}", "DefinitionInheritanceUtils");
                    break;
                }

                if (definitionMap.TryGetValue(current.inheritsFrom, out var parentDef))
                {
                    ModLogger.Log($"  Found parent: {parentDef.defID}", "DefinitionInheritanceUtils");
                    tempChain.Add(parentDef);
                    visited.Add(current.inheritsFrom);
                    current = parentDef;
                }
                else
                {
                    ModLogger.LogWarning($"Parent definition '{current.inheritsFrom}' not found for {current.defID}", "DefinitionInheritanceUtils");
                    break;
                }
            }

            // Reverse to get chain from root to immediate parent
            tempChain.Reverse();
            ModLogger.Log($"Final chain for {definition.defID}: {string.Join(" -> ", tempChain.Select(d => d.defID))}", "DefinitionInheritanceUtils");
            return tempChain;
        }

        /// <summary>
        /// Apply common inheritance logic that applies to all definition types.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="definition">Child definition</param>
        /// <param name="parentDefinition">Parent definition</param>
        public static void ApplyCommonInheritance<T>(T definition, T parentDefinition) where T : AngusChangyiMods.Core.DefBase
        {
            // Apply basic field inheritance
            InheritStringProperty(definition, parentDefinition, d => d.label, (d, v) => d.label = v);
            InheritStringProperty(definition, parentDefinition, d => d.description, (d, v) => d.description = v);

            // Inherit components and extensions if not already present
            foreach (var parentComponent in parentDefinition.components)
            {
                if (!definition.components.Any(c => c.GetType() == parentComponent.GetType()))
                {
                    definition.components.Add(parentComponent);
                }
            }

            foreach (var parentExtension in parentDefinition.extensions)
            {
                if (!definition.extensions.Any(e => e.GetType() == parentExtension.GetType()))
                {
                    definition.extensions.Add(parentExtension);
                }
            }
        }

        /// <summary>
        /// Inherit a string property if the child's value is null or empty.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <param name="child">Child definition</param>
        /// <param name="parent">Parent definition</param>
        /// <param name="getter">Function to get the property value</param>
        /// <param name="setter">Action to set the property value</param>
        public static void InheritStringProperty<T>(T child, T parent, PropertyGetter<T, string> getter, PropertySetter<T, string> setter)
        {
            var childValue = getter(child);
            var parentValue = getter(parent);

            if (string.IsNullOrEmpty(childValue) && !string.IsNullOrEmpty(parentValue))
            {
                setter(child, parentValue);
            }
        }

        /// <summary>
        /// Inherit a numeric property if the child's value is the default value.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the numeric property</typeparam>
        /// <param name="child">Child definition</param>
        /// <param name="parent">Parent definition</param>
        /// <param name="getter">Function to get the property value</param>
        /// <param name="setter">Action to set the property value</param>
        /// <param name="defaultValue">Default value to check against (optional, defaults to default(TValue))</param>
        public static void InheritNumericProperty<T, TValue>(T child, T parent, PropertyGetter<T, TValue> getter, PropertySetter<T, TValue> setter, TValue defaultValue = default(TValue))
            where TValue : struct, IComparable<TValue>
        {
            var childValue = getter(child);
            var parentValue = getter(parent);

            if (childValue.CompareTo(defaultValue) == 0 && parentValue.CompareTo(defaultValue) != 0)
            {
                setter(child, parentValue);
            }
        }

        /// <summary>
        /// Inherit a property using custom logic to determine if inheritance should occur.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TValue">The type of the property</typeparam>
        /// <param name="child">Child definition</param>
        /// <param name="parent">Parent definition</param>
        /// <param name="getter">Function to get the property value</param>
        /// <param name="setter">Action to set the property value</param>
        /// <param name="shouldInherit">Function to determine if inheritance should occur</param>
        public static void InheritProperty<T, TValue>(T child, T parent, PropertyGetter<T, TValue> getter, PropertySetter<T, TValue> setter, InheritancePredicate<TValue> shouldInherit)
        {
            var childValue = getter(child);
            var parentValue = getter(parent);

            if (shouldInherit(childValue, parentValue))
            {
                setter(child, parentValue);
            }
        }

        /// <summary>
        /// Merge list properties, adding parent items that don't exist in child.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TItem">The type of list items</typeparam>
        /// <param name="child">Child definition</param>
        /// <param name="parent">Parent definition</param>
        /// <param name="getter">Function to get the list property</param>
        /// <param name="setter">Action to set the list property (optional, for initialization)</param>
        /// <param name="comparer">Custom comparer for items (optional)</param>
        public static void MergeListProperty<T, TItem>(T child, T parent, PropertyGetter<T, List<TItem>> getter, PropertySetter<T, List<TItem>> setter = null, IEqualityComparer<TItem> comparer = null)
        {
            var childList = getter(child);
            var parentList = getter(parent);

            if (parentList == null || parentList.Count == 0)
                return;

            if (childList == null)
            {
                childList = new List<TItem>(parentList);
                setter?.Invoke(child, childList);
                return;
            }

            var actualComparer = comparer ?? EqualityComparer<TItem>.Default;

            foreach (var parentItem in parentList)
            {
                if (!childList.Contains(parentItem, actualComparer))
                {
                    childList.Add(parentItem);
                }
            }
        }

        /// <summary>
        /// Inherit complex object properties with partial field inheritance.
        /// </summary>
        /// <typeparam name="T">The type of definition</typeparam>
        /// <typeparam name="TObject">The type of the complex object</typeparam>
        /// <param name="child">Child definition</param>
        /// <param name="parent">Parent definition</param>
        /// <param name="getter">Function to get the object property</param>
        /// <param name="setter">Action to set the object property</param>
        /// <param name="createNew">Function to create a new instance if child doesn't have one</param>
        /// <param name="inheritFields">Action to inherit individual fields of the object</param>
        public static void InheritComplexProperty<T, TObject>(
            T child,
            T parent,
            PropertyGetter<T, TObject> getter,
            PropertySetter<T, TObject> setter,
            ObjectCreator<TObject> createNew,
            FieldInheritor<TObject> inheritFields) where TObject : class
        {
            var childObject = getter(child);
            var parentObject = getter(parent);

            if (parentObject == null)
                return;

            if (childObject == null)
            {
                childObject = createNew(parentObject);
                setter(child, childObject);
            }
            else
            {
                inheritFields(childObject, parentObject);
            }
        }

        /// <summary>
        /// Helper method to copy all fields from parent object to child object using reflection.
        /// Only copies fields that are default/null in the child object.
        /// </summary>
        /// <typeparam name="TObject">The type of the object</typeparam>
        /// <param name="child">Child object</param>
        /// <param name="parent">Parent object</param>
        public static void InheritObjectFields<TObject>(TObject child, TObject parent) where TObject : class
        {
            if (child == null || parent == null) return;

            var type = typeof(TObject);
            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

            foreach (var field in fields)
            {
                var childValue = field.GetValue(child);
                var parentValue = field.GetValue(parent);

                if (ShouldInheritFieldValue(childValue, parentValue, field.FieldType))
                {
                    field.SetValue(child, parentValue);
                }
            }

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var property in properties)
            {
                if (!property.CanRead || !property.CanWrite) continue;

                var childValue = property.GetValue(child);
                var parentValue = property.GetValue(parent);

                if (ShouldInheritFieldValue(childValue, parentValue, property.PropertyType))
                {
                    property.SetValue(child, parentValue);
                }
            }
        }

        /// <summary>
        /// Determine if a field value should be inherited based on its type and current value.
        /// </summary>
        /// <param name="childValue">Current value in child</param>
        /// <param name="parentValue">Value from parent</param>
        /// <param name="fieldType">Type of the field</param>
        /// <returns>True if should inherit, false otherwise</returns>
        private static bool ShouldInheritFieldValue(object childValue, object parentValue, Type fieldType)
        {
            if (parentValue == null) return false;

            // For strings, inherit if child is null or empty
            if (fieldType == typeof(string))
            {
                return string.IsNullOrEmpty(childValue as string);
            }

            // For value types, inherit if child equals default value
            if (fieldType.IsValueType)
            {
                var defaultValue = Activator.CreateInstance(fieldType);
                return Equals(childValue, defaultValue);
            }

            // For reference types, inherit if child is null
            return childValue == null;
        }
    }
}
