using System;
using System.Collections.Generic;
using System.Xml.Linq;

using AngusChangyiMods.Core.Utils;

namespace AngusChangyiMods.Core
{
    /// <summary>
    /// Handles deserialization of mod definitions from XML documents.
    /// </summary>
    public class ModDefinitionDeserializer
    {
        private readonly Dictionary<string, IDefinitionDeserializer> deserializers = new Dictionary<string, IDefinitionDeserializer>();



        public void RegisterDeserializers()
        {
            ModLogger.Log("Starting to register definition deserializers...", "DefinitionDeserializer");
            var deserializerTypes = ReflectionUtils.GetTypesAssignableFrom<IDefinitionDeserializer>();
            int successCount = 0;

            foreach (var type in deserializerTypes)
            {
                var deserializer = ReflectionUtils.SafeCreateInstance<IDefinitionDeserializer>(type);
                if (deserializer != null && !deserializers.ContainsKey(deserializer.HandlesNode))
                {
                    deserializers.Add(deserializer.HandlesNode, deserializer);
                    ModLogger.Log($"Registered deserializer successfully: {type.Name} for node <{deserializer.HandlesNode}>", "DefinitionDeserializer");
                    successCount++;
                }
                else if (deserializer != null)
                {
                    ModLogger.LogWarning($"Deserializer for node <{deserializer.HandlesNode}> already registered, skipping {type.Name}", "DefinitionDeserializer");
                }
            }

            ModLogger.Log($"Definition deserializers registration completed, success: {successCount} deserializers", "DefinitionDeserializer");
        }
        public Dictionary<Type, List<DefBase>> InstanceDefinitions(XDocument xmlDocument)
        {
            ModLogger.Log("Starting to instantiate definitions from XML document...", "DefinitionDeserializer");
            var definitions = new Dictionary<Type, List<DefBase>>();
            int successCount = 0;
            int errorCount = 0;
            int skipCount = 0;

            foreach (XElement element in xmlDocument.Root.Elements())
            {
                string nodeName = element.Name.LocalName;
                if (deserializers.TryGetValue(nodeName, out var deserializer))
                {
                    try
                    {
                        DefBase def = deserializer.Deserialize(element);
                        if (def != null)
                        {
                            Type defType = def.GetType();
                            if (!definitions.ContainsKey(defType))
                            {
                                definitions[defType] = new List<DefBase>();
                            }
                            definitions[defType].Add(def);
                            ModLogger.Log($"Instantiated definition successfully: {def.defID} of type {defType.Name}", "DefinitionDeserializer");
                            successCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        ModLogger.LogError($"Error deserializing element <{nodeName}> with {deserializer.GetType().Name}: {ex.Message}", "DefinitionDeserializer");
                        errorCount++;
                    }
                }
                else
                {
                    ModLogger.LogWarning($"No deserializer found for node type: <{nodeName}>", "DefinitionDeserializer");
                    skipCount++;
                }
            }

            ModLogger.Log($"Definition instantiation completed, success: {successCount}, errors: {errorCount}, skipped: {skipCount}", "DefinitionDeserializer");
            return definitions;
        }
    }

}