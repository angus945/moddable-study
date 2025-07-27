using System;
using System.Collections.Generic;
using System.Linq;

namespace AngusChangyiMods.Core
{
    /// <summary>
    /// Base class for definition inheritance processors.
    /// </summary>
    /// <typeparam name="T">The type of definition this inheritor handles</typeparam>
    public abstract class DefinitionInheritorBase<T> : IDefinitionInheritor where T : DefBase
    {
        /// <summary>
        /// The type this inheritor handles.
        /// </summary>
        public virtual Type HandlesType => typeof(T);

        /// <summary>
        /// Process inheritance for a collection of definitions.
        /// </summary>
        /// <param name="definitions">Definitions to process inheritance for</param>
        /// <returns>Processed definitions with inheritance applied</returns>
        public IEnumerable<DefBase> ProcessInheritance(IEnumerable<DefBase> definitions)
        {
            var typedDefinitions = definitions.Cast<T>().ToList();
            return ProcessTypedInheritance(typedDefinitions).Cast<DefBase>();
        }

        /// <summary>
        /// Process inheritance for typed definitions.
        /// </summary>
        /// <param name="definitions">Typed definitions to process</param>
        /// <returns>Processed definitions</returns>
        protected virtual IEnumerable<T> ProcessTypedInheritance(List<T> definitions)
        {
            // Create a map for quick lookup
            var definitionMap = definitions.ToDictionary(def => def.defID, def => def);
            var processedDefinitions = new List<T>();

            // First, collect all abstract definitions
            var abstractDefinitions = definitions.Where(def => def.IsAbstract).ToList();
            var concreteDefinitions = definitions.Where(def => !def.IsAbstract).ToList();

            // Process concrete definitions that may inherit from abstract ones
            foreach (var concreteDef in concreteDefinitions)
            {
                var processedDef = ProcessSingleInheritance(concreteDef, definitionMap);
                processedDefinitions.Add(processedDef);
            }

            return processedDefinitions;
        }

        /// <summary>
        /// Process inheritance for a single definition.
        /// Derived classes can override this to implement specific inheritance logic.
        /// </summary>
        /// <param name="definition">Definition to process</param>
        /// <param name="definitionMap">Map of all available definitions for lookup</param>
        /// <returns>Processed definition</returns>
        protected virtual T ProcessSingleInheritance(T definition, Dictionary<string, T> definitionMap)
        {
            if (string.IsNullOrEmpty(definition.inheritsFrom))
            {
                return definition; // No inheritance to process
            }

            if (definitionMap.TryGetValue(definition.inheritsFrom, out var parentDef))
            {
                // Apply common inheritance
                ApplyCommonInheritance(definition, parentDef);

                // Apply specific inheritance logic
                ApplySpecificInheritance(definition, parentDef);

                ModLogger.Log($"Applied inheritance from {definition.inheritsFrom} to {definition.defID} ({typeof(T).Name})", "DefinitionInheritor");
            }
            else
            {
                ModLogger.LogWarning($"Parent definition '{definition.inheritsFrom}' not found for {definition.defID} ({typeof(T).Name})", "DefinitionInheritor");
            }

            return definition;
        }

        /// <summary>
        /// Apply common inheritance logic that applies to all definition types.
        /// </summary>
        /// <param name="definition">Child definition</param>
        /// <param name="parentDefinition">Parent definition</param>
        protected virtual void ApplyCommonInheritance(T definition, T parentDefinition)
        {
            // Apply basic field inheritance
            if (string.IsNullOrEmpty(definition.label) && !string.IsNullOrEmpty(parentDefinition.label))
            {
                definition.label = parentDefinition.label;
            }

            if (string.IsNullOrEmpty(definition.description) && !string.IsNullOrEmpty(parentDefinition.description))
            {
                definition.description = parentDefinition.description;
            }

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
        /// Apply specific inheritance logic for the concrete definition type.
        /// Derived classes should override this to implement type-specific inheritance behavior.
        /// </summary>
        /// <param name="definition">Child definition</param>
        /// <param name="parentDefinition">Parent definition</param>
        protected abstract void ApplySpecificInheritance(T definition, T parentDefinition);
    }
}
