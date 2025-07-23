

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ModdableArchitecture.Utils;

public class ModStructure
{
    public const string About = "About";
    public const string Defs = "Defs";
    public const string Patches = "Patches";
}

public class ModManager : IModManager
{
    ILogger logger;
    List<ModMetaData> mods = new List<ModMetaData>();
    List<string> modOrder = new List<string>();
    Dictionary<string, ModMetaData> modMap = new Dictionary<string, ModMetaData>();

    // IModManager implementation
    public List<string> modIDs => modOrder;
    public string GetModDirectory(string modId)
    {
        if (modMap.TryGetValue(modId, out var mod))
        {
            return mod.folderPath;
        }

        throw new System.ArgumentException($"Mod with ID {modId} not found.");
    }

    public ModManager(ILogger logger)
    {
        this.logger = logger;
    }

    public void LoadModsInfo(string directoryPath)
    {
        mods.Clear();
        modOrder.Clear();
        modMap.Clear();

        ModMetaData[] modMetadata = GetModMetadata(directoryPath);
        foreach (var mod in modMetadata)
        {
            if (mod != null && !string.IsNullOrEmpty(mod.id))
            {
                mods.Add(mod);
                modOrder.Add(mod.id);
                modMap[mod.id] = mod;
                logger.Log($"Loaded modinfo: {mod.id} - {mod.ToString()}");
            }
            else
            {
                logger.LogWarning("Invalid mod metadata found, skipping.");
            }
        }

        logger.Log($"Loaded {mods.Count} mods from {directoryPath}");
    }
    public void SetModsOrder(List<string> order)
    {
        modOrder.Clear();
        var orderSet = new HashSet<string>(order);

        // Add mods in the order specified by 'order'
        foreach (var modId in order)
        {
            if (modMap.ContainsKey(modId))
            {
                modOrder.Add(modId);
            }
        }

        // Add remaining mods that are not in 'order'
        foreach (var mod in mods)
        {
            if (!orderSet.Contains(mod.id))
            {
                modOrder.Add(mod.id);
            }
        }
    }


    ModMetaData[] GetModMetadata(string directoryPath)
    {
        List<ModMetaData> modList = new List<ModMetaData>();

        foreach (string dir in Directory.GetDirectories(directoryPath))
        {
            string modInfoPath = Path.Combine(dir, "About/About.xml");

            if (!File.Exists(modInfoPath))
            {
                logger.LogWarning($"Mod info file not found in {dir}");
                continue;
            }

            string xmlContent = File.ReadAllText(modInfoPath);
            XmlSerializer serializer = new XmlSerializer(typeof(ModMetaData));
            using (StringReader reader = new StringReader(xmlContent))
            {
                ModMetaData mod = (ModMetaData)serializer.Deserialize(reader);
                mod.folderPath = dir;
                modList.Add(mod);
            }
        }

        return modList.ToArray();
    }


}