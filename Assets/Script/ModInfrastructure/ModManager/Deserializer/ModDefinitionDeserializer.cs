using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using ModArchitecture.Definition;
using ModArchitecture.Definition.Deserializers;
using ModArchitecture.Logger;

namespace ModArchitecture
{
    /// <summary>
    /// Handles deserialization of mod definitions from XML documents.
    /// </summary>
    public class ModDefinitionDeserializer
    {
        private readonly Dictionary<string, IDefinitionDeserializer> deserializers = new Dictionary<string, IDefinitionDeserializer>();



        public void RegisterDeserializers()
        {
            var deserializerTypes = AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(assembly => assembly.GetTypes())
                .Where(t => typeof(IDefinitionDeserializer).IsAssignableFrom(t) && !t.IsInterface && !t.IsAbstract);

            foreach (var type in deserializerTypes)
            {
                try
                {
                    IDefinitionDeserializer deserializer = (IDefinitionDeserializer)Activator.CreateInstance(type);
                    if (!deserializers.ContainsKey(deserializer.HandlesNode))
                    {
                        deserializers.Add(deserializer.HandlesNode, deserializer);
                        ModLogger.Log($"Registered deserializer: {type.Name} for node <{deserializer.HandlesNode}>");
                    }
                }
                catch (Exception ex)
                {
                    ModLogger.LogError($"Failed to register deserializer {type.Name}: {ex.Message}");
                }
            }
        }

        public Dictionary<Type, List<Definition.Definition>> InstanceDefinitions(XDocument xmlDocument)
        {
            var definitions = new Dictionary<Type, List<Definition.Definition>>();

            foreach (XElement element in xmlDocument.Root.Elements())
            {
                string nodeName = element.Name.LocalName;
                if (deserializers.TryGetValue(nodeName, out var deserializer))
                {
                    try
                    {
                        Definition.Definition def = deserializer.Deserialize(element);
                        if (def != null)
                        {
                            Type defType = def.GetType();
                            if (!definitions.ContainsKey(defType))
                            {
                                definitions[defType] = new List<Definition.Definition>();
                            }
                            definitions[defType].Add(def);
                            ModLogger.Log($"Instantiated definition: {def.defID} of type {defType.Name}");
                        }
                    }
                    catch (Exception ex)
                    {
                        ModLogger.LogError($"Error deserializing element <{nodeName}> with {deserializer.GetType().Name}: {ex.Message}");
                    }
                }
                else
                {
                    ModLogger.LogWarning($"No deserializer found for node type: <{nodeName}>");
                }
            }

            return definitions;
        }
    }

}