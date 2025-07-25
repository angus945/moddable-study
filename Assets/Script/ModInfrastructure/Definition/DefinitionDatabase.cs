using System;
using System.Collections.Generic;
using ModdableArchitecture.Definition;

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
    public static Dictionary<Type, List<Definition>> GetDefinitions()
    {
        return new Dictionary<Type, List<Definition>>(thingDefs);
    }
}

