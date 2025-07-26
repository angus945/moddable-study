using System;
using System.Collections.Generic;
using System.Linq;
using ModArchitecture;
using ModArchitecture.Definition;

namespace Angus
{
    /// <summary>
    /// Inheritor for CharacterDef definitions.
    /// </summary>
    /// <remarks>
    /// This class handles the inheritance processing of CharacterDef definitions.
    /// </remarks>
    public class CharacterDefInheritor : IDefinitionInheritor
    {
        /// <summary>
        /// The type this inheritor handles.
        /// </summary>
        public Type HandlesType => typeof(CharacterDef);

        /// <summary>
        /// Process inheritance for a collection of CharacterDef definitions.
        /// </summary>
        /// <param name="definitions">Definitions to process inheritance for</param>
        /// <returns>Processed definitions with inheritance applied</returns>
        public IEnumerable<Definition> ProcessInheritance(IEnumerable<Definition> definitions)
        {
            var characterDefs = definitions.Cast<CharacterDef>().ToList();
            return ProcessCharacterDefInheritance(characterDefs).Cast<Definition>();
        }

        /// <summary>
        /// Process inheritance for typed CharacterDef definitions.
        /// </summary>
        /// <param name="definitions">CharacterDef definitions to process</param>
        /// <returns>Processed CharacterDef definitions</returns>
        private IEnumerable<CharacterDef> ProcessCharacterDefInheritance(List<CharacterDef> definitions)
        {
            // Create a map for quick lookup
            var definitionMap = definitions.ToDictionary(def => def.defID, def => def);
            var processedDefinitions = new List<CharacterDef>();

            // Filter out abstract definitions from final results
            var concreteDefinitions = definitions.Where(def => !def.IsAbstract).ToList();

            // Process concrete definitions that may inherit from abstract ones
            foreach (var concreteDef in concreteDefinitions)
            {
                var processedDef = ProcessSingleCharacterDefInheritance(concreteDef, definitionMap);
                processedDefinitions.Add(processedDef);
            }

            return processedDefinitions;
        }

        /// <summary>
        /// Process inheritance for a single CharacterDef definition.
        /// </summary>
        /// <param name="definition">CharacterDef to process</param>
        /// <param name="definitionMap">Map of all available CharacterDef definitions for lookup</param>
        /// <returns>Processed CharacterDef</returns>
        private CharacterDef ProcessSingleCharacterDefInheritance(CharacterDef definition, Dictionary<string, CharacterDef> definitionMap)
        {
            if (string.IsNullOrEmpty(definition.inheritsFrom))
            {
                return definition; // No inheritance to process
            }

            if (definitionMap.TryGetValue(definition.inheritsFrom, out var parentDef))
            {
                // Apply common inheritance (label, description, components, extensions)
                ApplyCommonInheritance(definition, parentDef);

                // Apply CharacterDef-specific inheritance
                ApplyCharacterDefSpecificInheritance(definition, parentDef);
            }

            return definition;
        }

        /// <summary>
        /// Apply common inheritance logic that applies to all definition types.
        /// </summary>
        /// <param name="definition">Child CharacterDef</param>
        /// <param name="parentDefinition">Parent CharacterDef</param>
        private void ApplyCommonInheritance(CharacterDef definition, CharacterDef parentDefinition)
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
        /// Apply specific inheritance logic for CharacterDef.
        /// </summary>
        /// <param name="definition">Child CharacterDef</param>
        /// <param name="parentDefinition">Parent CharacterDef</param>
        private void ApplyCharacterDefSpecificInheritance(CharacterDef definition, CharacterDef parentDefinition)
        {
            // Inherit health if not specifically set (default value is 0)
            if (definition.health == 0 && parentDefinition.health > 0)
            {
                definition.health = parentDefinition.health;
            }

            // Inherit speed if not specifically set (default value is 0)
            if (definition.speed == 0 && parentDefinition.speed > 0)
            {
                definition.speed = parentDefinition.speed;
            }
        }
    }
}
