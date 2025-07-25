using System;
using System.Collections.Generic;
using ModArchitecture.Definition;

public class DefinitionDatabase
{
    static Dictionary<Type, List<Definition>> thingDefs = new Dictionary<Type, List<Definition>>();

    public static void Clear()
    {
        thingDefs.Clear();
    }
    public static void SetDefinitions(Dictionary<Type, List<Definition>> definitions)
    {
        foreach (var kvp in definitions)
        {
            if (!thingDefs.ContainsKey(kvp.Key))
            {
                thingDefs[kvp.Key] = new List<Definition>();
            }
            thingDefs[kvp.Key].AddRange(kvp.Value);
        }
    }
    public static T GetDefinition<T>(string defID) where T : Definition
    {
        if (thingDefs.TryGetValue(typeof(T), out var definitions))
        {
            foreach (var def in definitions)
            {
                if (def.defID == defID)
                {
                    return def as T;
                }
            }
        }
        return null;
    }

    public static Dictionary<Type, List<Definition>> GetDefinitions()
    {
        return new Dictionary<Type, List<Definition>>(thingDefs);
    }
}

