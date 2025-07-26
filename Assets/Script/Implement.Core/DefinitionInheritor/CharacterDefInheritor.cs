using System;
using System.Collections.Generic;
using System.Linq;
using ModArchitecture;
using ModArchitecture.Definition;
using ModArchitecture.Logger;
using ModArchitecture.Utils;

namespace Angus
{
    /// <summary>
    /// Inheritor for CharacterDef definitions.
    /// </summary>
    /// <remarks>
    /// This class handles the inheritance processing of CharacterDef definitions using DefinitionInheritanceUtils.
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
        public IEnumerable<ModArchitecture.Definition.Definition> ProcessInheritance(IEnumerable<ModArchitecture.Definition.Definition> definitions)
        {
            ModLogger.Log("CharacterDefInheritor.ProcessInheritance called", "CharacterDefInheritor");

            var characterDefs = definitions.Cast<CharacterDef>().ToList();
            ModLogger.Log($"Processing {characterDefs.Count} CharacterDef definitions", "CharacterDefInheritor");

            var processedDefs = DefinitionInheritanceUtils.ProcessInheritance(characterDefs, ApplyCharacterDefSpecificInheritance);

            ModLogger.Log("CharacterDefInheritor.ProcessInheritance completed", "CharacterDefInheritor");
            return processedDefs.Cast<ModArchitecture.Definition.Definition>();
        }

        /// <summary>
        /// Apply CharacterDef-specific inheritance logic.
        /// </summary>
        /// <param name="child">Child CharacterDef</param>
        /// <param name="parent">Parent CharacterDef</param>
        private void ApplyCharacterDefSpecificInheritance(CharacterDef child, CharacterDef parent)
        {
            ModLogger.Log($"ApplyCharacterDefSpecificInheritance: {child.defID} <- {parent.defID}", "CharacterDefInheritor");
            ModLogger.Log($"  Before: child.health={child.health}, child.speed={child.speed}", "CharacterDefInheritor");
            ModLogger.Log($"  Parent: parent.health={parent.health}, parent.speed={parent.speed}", "CharacterDefInheritor");

            // Inherit health if child has default value (0)
            DefinitionInheritanceUtils.InheritNumericProperty(child, parent,
                c => c.health,
                (c, v) => c.health = v);

            // Inherit speed if child has default value (0)
            DefinitionInheritanceUtils.InheritNumericProperty(child, parent,
                c => c.speed,
                (c, v) => c.speed = v);

            ModLogger.Log($"  After: child.health={child.health}, child.speed={child.speed}", "CharacterDefInheritor");
        }
    }
}