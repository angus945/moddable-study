

using System.Collections.Generic;

public class ModSorter
{
    public List<string> modOrder = new List<string>();
    public void SetModsOrder(List<string> order, Dictionary<string, ModMetaData> modMap)
    {
        modOrder.Clear();
        HashSet<string> orderSet = new HashSet<string>(order);

        // Add mods in the order specified by 'order'
        foreach (var modId in order)
        {
            if (!modMap.ContainsKey(modId)) continue;

            modOrder.Add(modId);
        }

        // Add remaining mods that are not in 'order'
        foreach (var mod in modMap.Values)
        {
            if (orderSet.Contains(mod.id)) continue;

            modOrder.Add(mod.id);
        }
    }
}
