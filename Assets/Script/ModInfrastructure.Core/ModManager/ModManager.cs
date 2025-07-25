using System;
using System.Collections.Generic;
using System.Xml.Linq;
using ModArchitecture;

public class ModLoadingRecord
{
    public XDocument definitionDoc;
}

public class ModManager
{
    Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

    ModFinder finder;
    ModSorter sorter;
    ModAssemblyLoader assemblyLoader;
    ModDefinitionLoader definitionLoader;
    ModDefinitionPatcher definitionPatcher;
    ModDefinitionDeserializer deserializer;
    ModAssetsLoader assetsLoader;
    ModInitializer initializer;

    ModLoadingRecord loadingRecord = new ModLoadingRecord();

    public ModManager(ModFinder finder, ModSorter sorter, ModAssemblyLoader assemblyLoader, ModDefinitionLoader definitionLoader, ModDefinitionPatcher patcher, ModDefinitionDeserializer deserializer, ModAssetsLoader assetsLoader, ModInitializer initializer)
    {
        this.finder = finder;
        this.sorter = sorter;
        this.assemblyLoader = assemblyLoader;
        this.definitionLoader = definitionLoader;
        this.definitionPatcher = patcher;
        this.deserializer = deserializer;
        this.assetsLoader = assetsLoader;
        this.initializer = initializer;
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
    public void LoadModsAssemblies()
    {
        foreach (var mod in sorter.modOrder)
        {
            ModMetaData modData = modMap[mod];
            assemblyLoader.LoadModAssembly(modData.assemblies);
        }
    }
    public void LoadModsDefinition()
    {
        XDocument definitionDocument = new XDocument(new XElement("Defs"));

        List<string> order = sorter.modOrder;
        foreach (var mod in order)
        {
            ModMetaData modData = modMap[mod];
            definitionLoader.LoadDefinitions(modData.definitions, definitionDocument);
            definitionPatcher.ApplyPatches(modData.patches, definitionDocument);
            // TODO 應該要每個 mod 做完自己的 patch? 還是先合併完最後再一次 patch?
        }

        deserializer.RegisterDeserializers();

        var definitions = deserializer.InstanceDefinitions(definitionDocument);
        DefinitionDatabase.SetDefinitions(definitions);

        loadingRecord.definitionDoc = definitionDocument;
    }
    public void LoadModsAssets()
    {
        assetsLoader.RegisterDeserializers();

        foreach (var mod in sorter.modOrder)
        {
            ModMetaData modData = modMap[mod];

            assetsLoader.LoadAssets(modData.textures);
            assetsLoader.LoadAssets(modData.sounds);
            assetsLoader.LoadAssets(modData.custom);
        }
    }
    public void ModsInitialization()
    {
        initializer.RegisterInitializer();
        initializer.InitializeMods(); // TODO: 這裡應該要有個順序，依照 mod 的依賴關係來初始化
    }


    //
    public Dictionary<string, ModMetaData> GetModsMap()
    {
        return modMap;
    }
    public XDocument GetDefinitionDocument()
    {
        return loadingRecord.definitionDoc;
    }


}
