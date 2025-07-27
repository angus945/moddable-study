

using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    public class ModSorter
    {
        public string[] modOrder { get; private set; } = new string[0];

        List<string> order = new List<string>();
        public void SetModsOrder(string[] setOrder, Dictionary<string, ModMetaData> modMap)
        {
            order.Clear();
            HashSet<string> orderSet = new HashSet<string>(setOrder);

            // Add mods in the order specified by 'order'
            foreach (var modId in setOrder)
            {
                if (!modMap.ContainsKey(modId)) continue;

                order.Add(modId);
            }

            // Add remaining mods that are not in 'order'
            foreach (var mod in modMap.Values)
            {
                if (orderSet.Contains(mod.id)) continue;

                order.Add(mod.id);
            }

            modOrder = order.ToArray();
        }
    }

}
