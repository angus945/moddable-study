// using System;
// using System.Collections.Generic;
// using System.Linq;

// using AngusChangyiMods.Core.Utils;

// namespace AngusChangyiMods.Core
// {
//     /// <summary>
//     /// Handles inheritance processing for mod definitions.
//     /// </summary>
//     public class ModDefinitionInheritor
//     {
//         private readonly Dictionary<Type, IDefinitionInheritor> inheritors = new Dictionary<Type, IDefinitionInheritor>();

//         /// <summary>
//         /// Register all available definition inheritors using reflection.
//         /// </summary>
//         public void RegisterInheritors()
//         {
//             ModLogger.Log("Starting to register definition inheritors...", "DefinitionInheritor");
//             var inheritorTypes = ReflectionUtils.GetTypesAssignableFrom<IDefinitionInheritor>();
//             int successCount = 0;

//             foreach (var type in inheritorTypes)
//             {
//                 var inheritor = ReflectionUtils.SafeCreateInstance<IDefinitionInheritor>(type);
//                 if (inheritor != null && !inheritors.ContainsKey(inheritor.HandlesType))
//                 {
//                     inheritors.Add(inheritor.HandlesType, inheritor);
//                     ModLogger.Log($"Registered inheritor successfully: {type.Name} for type {inheritor.HandlesType.Name}", "DefinitionInheritor");
//                     successCount++;
//                 }
//                 else if (inheritor != null)
//                 {
//                     ModLogger.LogWarning($"Inheritor for type {inheritor.HandlesType.Name} already registered, skipping {type.Name}", "DefinitionInheritor");
//                 }
//             }

//             ModLogger.Log($"Definition inheritors registration completed, success: {successCount} inheritors", "DefinitionInheritor");
//         }

//         /// <summary>
//         /// Process inheritance for all definitions.
//         /// </summary>
//         /// <param name="definitions">Dictionary of definitions grouped by type</param>
//         /// <returns>Processed definitions with inheritance applied</returns>
//         public Dictionary<Type, List<DefBase>> ProcessInheritance(Dictionary<Type, List<DefBase>> definitions)
//         {
//             ModLogger.Log("Starting inheritance processing...", "DefinitionInheritor");
//             var processedDefinitions = new Dictionary<Type, List<DefBase>>();
//             int totalProcessed = 0;
//             int totalErrors = 0;

//             foreach (var kvp in definitions)
//             {
//                 Type defType = kvp.Key;
//                 List<DefBase> defList = kvp.Value;

//                 if (inheritors.TryGetValue(defType, out var inheritor))
//                 {
//                     try
//                     {
//                         var processed = inheritor.ProcessInheritance(defList);
//                         processedDefinitions[defType] = processed.ToList();
//                         totalProcessed += processed.Count();
//                         ModLogger.Log($"Processed {processed.Count()} definitions of type {defType.Name}", "DefinitionInheritor");
//                     }
//                     catch (Exception ex)
//                     {
//                         ModLogger.LogError($"Error processing inheritance for type {defType.Name}: {ex.Message}", "DefinitionInheritor");
//                         // Fall back to original definitions if processing fails
//                         processedDefinitions[defType] = defList;
//                         totalErrors++;
//                     }
//                 }
//                 else
//                 {
//                     // No specific inheritor found, use default processing
//                     var processed = ProcessDefaultInheritance(defList);
//                     processedDefinitions[defType] = processed.ToList();
//                     totalProcessed += processed.Count();
//                     ModLogger.Log($"Applied default inheritance processing for {processed.Count()} definitions of type {defType.Name}", "DefinitionInheritor");
//                 }
//             }

//             ModLogger.Log($"Inheritance processing completed, processed: {totalProcessed}, errors: {totalErrors}", "DefinitionInheritor");
//             return processedDefinitions;
//         }

//         /// <summary>
//         /// Default inheritance processing for types without specific inheritors.
//         /// </summary>
//         /// <param name="definitions">List of definitions to process</param>
//         /// <returns>Processed definitions</returns>
//         private IEnumerable<DefBase> ProcessDefaultInheritance(List<DefBase> definitions)
//         {
//             // Create a map for quick lookup
//             var definitionMap = definitions.ToDictionary(def => def.defID, def => def);
//             var processedDefinitions = new List<DefBase>();

//             // First, collect all abstract definitions
//             var abstractDefinitions = definitions.Where(def => def.IsAbstract).ToList();
//             var concreteDefinitions = definitions.Where(def => !def.IsAbstract).ToList();

//             // Process concrete definitions that may inherit from abstract ones
//             foreach (var concreteDef in concreteDefinitions)
//             {
//                 var processedDef = ProcessSingleInheritance(concreteDef, definitionMap);
//                 processedDefinitions.Add(processedDef);
//             }

//             return processedDefinitions;
//         }

//         /// <summary>
//         /// Process inheritance for a single definition using default logic.
//         /// </summary>
//         /// <param name="definition">Definition to process</param>
//         /// <param name="definitionMap">Map of all available definitions</param>
//         /// <returns>Processed definition</returns>
//         private DefBase ProcessSingleInheritance(DefBase definition, Dictionary<string, DefBase> definitionMap)
//         {
//             if (string.IsNullOrEmpty(definition.inheritsFrom))
//             {
//                 return definition; // No inheritance to process
//             }

//             if (definitionMap.TryGetValue(definition.inheritsFrom, out var parentDef))
//             {
//                 // Apply basic field inheritance
//                 if (string.IsNullOrEmpty(definition.label) && !string.IsNullOrEmpty(parentDef.label))
//                 {
//                     definition.label = parentDef.label;
//                 }

//                 if (string.IsNullOrEmpty(definition.description) && !string.IsNullOrEmpty(parentDef.description))
//                 {
//                     definition.description = parentDef.description;
//                 }

//                 // Inherit components and extensions if not already present
//                 foreach (var parentComponent in parentDef.components)
//                 {
//                     if (!definition.components.Any(c => c.GetType() == parentComponent.GetType()))
//                     {
//                         definition.components.Add(parentComponent);
//                     }
//                 }

//                 foreach (var parentExtension in parentDef.extensions)
//                 {
//                     if (!definition.extensions.Any(e => e.GetType() == parentExtension.GetType()))
//                     {
//                         definition.extensions.Add(parentExtension);
//                     }
//                 }

//                 ModLogger.Log($"Applied default inheritance from {definition.inheritsFrom} to {definition.defID}", "DefinitionInheritor");
//             }
//             else
//             {
//                 ModLogger.LogWarning($"Parent definition '{definition.inheritsFrom}' not found for {definition.defID}", "DefinitionInheritor");
//             }

//             return definition;
//         }
//     }
// }
