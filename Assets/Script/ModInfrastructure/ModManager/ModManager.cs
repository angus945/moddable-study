using System.Collections.Generic;
using System.Xml.Linq;
using ModdableArchitecture;

public class ModManager
{
    Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

    ModFinder finder;
    ModSorter sorter;
    ModDefinitionLoader definitionLoader;
    ModDefinitionPatcher definitionPatcher;
    ModDefinitionDeserializer deserializer;

    XDocument definitionDocument = new XDocument(new XElement("Defs"));

    public ModManager(ModFinder finder, ModSorter sorter, ModDefinitionLoader definitionLoader, ModDefinitionPatcher patcher, ModDefinitionDeserializer deserializer)
    {
        this.finder = finder;
        this.sorter = sorter;
        this.definitionLoader = definitionLoader;
        this.definitionPatcher = patcher;
        this.deserializer = deserializer;
    }

    public void FindMods()
    {
        ModMetaData[] mods = finder.FindMods();
        foreach (var mod in mods)
        {
            if (!modMap.ContainsKey(mod.id))
            {
                modMap[mod.id] = mod;
            }
        }
    }
    public void SetModsOrder(List<string> order)
    {
        sorter.SetModsOrder(order, modMap);
    }
    public void LoadModsDefinition()
    {
        List<string> order = sorter.modOrder;
        foreach (var mod in order)
        {
            ModMetaData modData = modMap[mod];
            definitionLoader.LoadDefinitions(modData.definitions, definitionDocument);
            definitionPatcher.ApplyPatches(modData.patches, definitionDocument);
            // TODO 應該要每個 mod 做完自己的 patch? 還是先合併完最後再一次 patch?
        }

        var definitions = deserializer.InstanceDefinitions(definitionDocument);
        DefinitionDatabase.SetDefinitions(definitions);
    }


    //
    public Dictionary<string, ModMetaData> GetModsMap()
    {
        return modMap;
    }
    public XDocument GetDefinitionDocument()
    {
        return definitionDocument;
    }
}
