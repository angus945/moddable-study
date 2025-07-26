using System;
using System.Collections.Generic;
using System.Linq;
using ModArchitecture;
using ModArchitecture.Definition;

namespace Angus
{
    /// <summary>
    /// Inheritor for ThingDef definitions.
    /// </summary>
    /// <remarks>
    /// This class handles the inheritance processing of ThingDef definitions.
    /// </remarks>
    public class ThingDefInheritor : IDefinitionInheritor
    {
        /// <summary>
        /// The type this inheritor handles.
        /// </summary>
        public Type HandlesType => typeof(ThingDef);

        /// <summary>
        /// Process inheritance for a collection of ThingDef definitions.
        /// </summary>
        /// <param name="definitions">Definitions to process inheritance for</param>
        /// <returns>Processed definitions with inheritance applied</returns>
        public IEnumerable<Definition> ProcessInheritance(IEnumerable<Definition> definitions)
        {
            var thingDefs = definitions.Cast<ThingDef>().ToList();
            return ProcessThingDefInheritance(thingDefs).Cast<Definition>();
        }

        /// <summary>
        /// Process inheritance for typed ThingDef definitions.
        /// </summary>
        /// <param name="definitions">ThingDef definitions to process</param>
        /// <returns>Processed ThingDef definitions</returns>
        private IEnumerable<ThingDef> ProcessThingDefInheritance(List<ThingDef> definitions)
        {
            // Create a map for quick lookup
            var definitionMap = definitions.ToDictionary(def => def.defID, def => def);
            var processedDefinitions = new List<ThingDef>();

            // Filter out abstract definitions from final results
            var concreteDefinitions = definitions.Where(def => !def.IsAbstract).ToList();

            // Process concrete definitions that may inherit from abstract ones
            foreach (var concreteDef in concreteDefinitions)
            {
                var processedDef = ProcessSingleThingDefInheritance(concreteDef, definitionMap);
                processedDefinitions.Add(processedDef);
            }

            return processedDefinitions;
        }

        /// <summary>
        /// Process inheritance for a single ThingDef definition.
        /// </summary>
        /// <param name="definition">ThingDef to process</param>
        /// <param name="definitionMap">Map of all available ThingDef definitions for lookup</param>
        /// <returns>Processed ThingDef</returns>
        private ThingDef ProcessSingleThingDefInheritance(ThingDef definition, Dictionary<string, ThingDef> definitionMap)
        {
            if (string.IsNullOrEmpty(definition.inheritsFrom))
            {
                return definition; // No inheritance to process
            }

            if (definitionMap.TryGetValue(definition.inheritsFrom, out var parentDef))
            {
                // Apply common inheritance (label, description, components, extensions)
                ApplyCommonInheritance(definition, parentDef);

                // Apply ThingDef-specific inheritance
                ApplyThingDefSpecificInheritance(definition, parentDef);
            }

            return definition;
        }

        /// <summary>
        /// Apply common inheritance logic that applies to all definition types.
        /// </summary>
        /// <param name="definition">Child ThingDef</param>
        /// <param name="parentDefinition">Parent ThingDef</param>
        private void ApplyCommonInheritance(ThingDef definition, ThingDef parentDefinition)
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
        /// Apply specific inheritance logic for ThingDef.
        /// </summary>
        /// <param name="definition">Child ThingDef</param>
        /// <param name="parentDefinition">Parent ThingDef</param>
        private void ApplyThingDefSpecificInheritance(ThingDef definition, ThingDef parentDefinition)
        {
            // Inherit damage if not specifically set (default value is 0)
            if (definition.damage == 0 && parentDefinition.damage > 0)
            {
                definition.damage = parentDefinition.damage;
            }

            // Inherit stack if not specifically set (default value is 0)
            if (definition.stack == 0 && parentDefinition.stack > 0)
            {
                definition.stack = parentDefinition.stack;
            }

            // Inherit tags if child has none
            if ((definition.tags == null || definition.tags.Count == 0) &&
                parentDefinition.tags != null && parentDefinition.tags.Count > 0)
            {
                definition.tags = new List<string>(parentDefinition.tags);
            }
            else if (definition.tags != null && parentDefinition.tags != null)
            {
                // Merge tags, avoiding duplicates
                foreach (var parentTag in parentDefinition.tags)
                {
                    if (!definition.tags.Contains(parentTag))
                    {
                        definition.tags.Add(parentTag);
                    }
                }
            }

            // Inherit weapon properties if child doesn't have them
            if (definition.weaponProps == null && parentDefinition.weaponProps != null)
            {
                definition.weaponProps = new WeaponProperties
                {
                    type = parentDefinition.weaponProps.type,
                    damage = parentDefinition.weaponProps.damage,
                    range = parentDefinition.weaponProps.range
                };
            }
            else if (definition.weaponProps != null && parentDefinition.weaponProps != null)
            {
                // Inherit individual weapon properties if not set
                if (string.IsNullOrEmpty(definition.weaponProps.type) && !string.IsNullOrEmpty(parentDefinition.weaponProps.type))
                {
                    definition.weaponProps.type = parentDefinition.weaponProps.type;
                }

                if (definition.weaponProps.damage == 0 && parentDefinition.weaponProps.damage > 0)
                {
                    definition.weaponProps.damage = parentDefinition.weaponProps.damage;
                }

                if (definition.weaponProps.range == 0 && parentDefinition.weaponProps.range > 0)
                {
                    definition.weaponProps.range = parentDefinition.weaponProps.range;
                }
            }
        }
    }
}
