using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ModdableArchitecture.Utils;

public class DefinitionInstanter
{
    readonly Dictionary<string, Type> defTypeMap = new Dictionary<string, Type>
    {
        { "CharacterDef", typeof(CharacterDef) },
        { "ThingDef", typeof(ThingDef) },
        // Add other definition types here
    };

    ILogger logger;
    public DefinitionInstanter(ILogger logger)
    {
        this.logger = logger;
    }
    public Dictionary<Type, List<Definition>> InstanceDefinitions(XDocument xmlDocument)
    {
        Dictionary<Type, List<Definition>> definitions = new Dictionary<Type, List<Definition>>();
        foreach (var defType in defTypeMap.Values)
        {
            definitions[defType] = new List<Definition>();
        }

        foreach (XElement element in xmlDocument.Root.Elements())
        {
            string defTypeName = element.Name.LocalName;
            if (!defTypeMap.TryGetValue(defTypeName, out Type defType))
            {
                logger.LogWarning($"Unknown definition type: {defTypeName}");
                continue;
            }

            Definition definition = (Definition)Activator.CreateInstance(defType);
            using (var reader = element.CreateReader())
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(defType);
                var deserializedDef = (Definition)serializer.Deserialize(reader);
                // Example: Add to definitions if it's a ThingDef
                definitions[defType].Add(deserializedDef);
                logger.Log($"Deserialized definition: {deserializedDef.defID} of type {defTypeName}");
            }
        }

        return definitions;
    }

}

