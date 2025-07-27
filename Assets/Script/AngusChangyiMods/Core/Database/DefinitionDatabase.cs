using System;
using System.Collections.Generic;


namespace AngusChangyiMods.Core
{

    public class DefinitionDatabase
    {
        static Dictionary<Type, List<DefBase>> thingDefs = new Dictionary<Type, List<DefBase>>();

        public static void Clear()
        {
            thingDefs.Clear();
        }
        public static void SetDefinitions(Dictionary<Type, List<DefBase>> definitions)
        {
            foreach (var kvp in definitions)
            {
                if (!thingDefs.ContainsKey(kvp.Key))
                {
                    thingDefs[kvp.Key] = new List<DefBase>();
                }
                thingDefs[kvp.Key].AddRange(kvp.Value);
            }
        }
        public static T GetDefinition<T>(string defID) where T : DefBase
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

        public static Dictionary<Type, List<DefBase>> GetDefinitions()
        {
            return new Dictionary<Type, List<DefBase>>(thingDefs);
        }
    }

}
