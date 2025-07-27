// using System;
// using System.Collections.Generic;
// using System.Linq;
// using AngusChangyiMods.Core;
// using AngusChangyiMods.Core.Utils;

// namespace Angus
// {
//     /// <summary>
//     /// Inheritor for ThingDef definitions.
//     /// </summary>
//     /// <remarks>
//     /// This class handles the inheritance processing of ThingDef definitions using DefinitionInheritanceUtils.
//     /// </remarks>
//     public class ThingDefInheritor : IDefinitionInheritor
//     {
//         /// <summary>
//         /// The type this inheritor handles.
//         /// </summary>
//         public Type HandlesType => typeof(ThingDef);

//         /// <summary>
//         /// Process inheritance for a collection of ThingDef definitions.
//         /// </summary>
//         /// <param name="definitions">Definitions to process inheritance for</param>
//         /// <returns>Processed definitions with inheritance applied</returns>
//         public IEnumerable<AngusChangyiMods.Core.DefBase> ProcessInheritance(IEnumerable<AngusChangyiMods.Core.DefBase> definitions)
//         {
//             var thingDefs = definitions.Cast<ThingDef>().ToList();
//             var processedDefs = DefinitionInheritanceUtils.ProcessInheritance(thingDefs, ApplyThingDefSpecificInheritance);
//             return processedDefs.Cast<AngusChangyiMods.Core.DefBase>();
//         }

//         /// <summary>
//         /// Apply ThingDef-specific inheritance logic.
//         /// </summary>
//         /// <param name="child">Child ThingDef</param>
//         /// <param name="parent">Parent ThingDef</param>
//         private void ApplyThingDefSpecificInheritance(ThingDef child, ThingDef parent)
//         {
//             // Inherit damage if child has default value (0)
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent,
//                 c => c.damage,
//                 (c, v) => c.damage = v);

//             // Inherit stack if child has default value (0)
//             DefinitionInheritanceUtils.InheritNumericProperty(child, parent,
//                 c => c.stack,
//                 (c, v) => c.stack = v);

//             // Merge tags - combine parent and child tags
//             DefinitionInheritanceUtils.MergeListProperty(child, parent,
//                 c => c.tags,
//                 (c, v) => c.tags = v);

//             // Inherit weapon properties with partial field inheritance
//             DefinitionInheritanceUtils.InheritComplexProperty(child, parent,
//                 c => c.weaponProps,
//                 (c, v) => c.weaponProps = v,
//                 parentWeapon => new WeaponProperties
//                 {
//                     type = parentWeapon.type,
//                     damage = parentWeapon.damage,
//                     range = parentWeapon.range
//                 },
//                 (childWeapon, parentWeapon) =>
//                 {
//                     // Inherit individual weapon properties if not set
//                     DefinitionInheritanceUtils.InheritStringProperty(childWeapon, parentWeapon,
//                         w => w.type, (w, v) => w.type = v);

//                     DefinitionInheritanceUtils.InheritNumericProperty(childWeapon, parentWeapon,
//                         w => w.damage, (w, v) => w.damage = v);

//                     DefinitionInheritanceUtils.InheritNumericProperty(childWeapon, parentWeapon,
//                         w => w.range, (w, v) => w.range = v);
//                 });
//         }
//     }
// }
