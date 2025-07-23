using System;
using System.Collections.Generic;

public class DefinitionDatabase
{
    public Dictionary<Type, List<Definition>> thingDefs = new Dictionary<Type, List<Definition>>();
    public void AddDefinition(Dictionary<Type, List<Definition>> definitions)
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
}

