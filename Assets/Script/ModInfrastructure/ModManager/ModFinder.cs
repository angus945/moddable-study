

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using ModArchitecture.Utils;

public class ModFinder
{
    ILogger logger;

    string[] findDirectories;
    List<ModMetaData> findMods = new List<ModMetaData>();
    HashSet<string> modIDs = new HashSet<string>();

    public ModFinder(ILogger logger, params string[] findDirectories)
    {
        this.logger = logger;
        this.findDirectories = findDirectories;
    }

    public ModMetaData[] FindMods()
    {
        findMods.Clear();
        modIDs.Clear();

        foreach (var findDirectory in findDirectories)
        {
            FindMods(findDirectory);
        }

        return findMods.ToArray();
    }
    void FindMods(string directoryPath)
    {
        if (!Directory.Exists(directoryPath))
        {
            logger.LogWarning($"Mod Root directory does not exist: {directoryPath}");
            return;
        }

        string[] modDirectories = GetModDirectories(directoryPath);
        foreach (var modDirectory in modDirectories)
        {
            if (GetModMetadata(modDirectory, out ModMetaData mod))
            {
                GetModAssetsPath(mod);

                findMods.Add(mod);
                logger.Log($"Loaded modinfo: {mod.id} - {mod.ToString()}");
            }
        }

        logger.Log($"Loaded {findMods.Count} mods from {directoryPath}");
    }
    string[] GetModDirectories(string root)
    {
        return Directory.GetDirectories(root, "*", SearchOption.TopDirectoryOnly);
    }
    bool GetModMetadata(string modDirectory, out ModMetaData mod)
    {
        XmlSerializer serializer = new XmlSerializer(typeof(ModMetaData));

        string modInfoPath = Path.Combine(modDirectory, ModStructure.About);
        string modPreviewPath = Path.Combine(modDirectory, ModStructure.Preview);

        if (!File.Exists(modInfoPath))
        {
            logger.LogWarning($"About.md file not found in {modDirectory}");
            mod = null;
            return false;
        }

        string xmlContent = File.ReadAllText(modInfoPath);
        using (StringReader reader = new StringReader(xmlContent))
        {
            mod = (ModMetaData)serializer.Deserialize(reader);
            mod.directory = modDirectory;
        }

        if (mod.id == null || mod.id.Trim() == "")
        {
            logger.LogWarning($"Mod ID is missing in {modInfoPath}");
            return false;
        }

        if (modIDs.Contains(mod.id))
        {
            logger.LogWarning($"Skipping Duplicate IDs mod: {mod.id}, {modDirectory}.");
            return false;
        }

        modIDs.Add(mod.id);

        return true;
    }
    void GetModAssetsPath(ModMetaData mod)
    {
        mod.assemblies = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Assemblies), "*.dll");
        mod.custom = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Custom), "*.*");
        mod.definitions = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Defs), "*.xml");
        mod.textures = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Textures), "*.png");
        mod.sounds = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Sounds), "*.mp3");
        mod.patches = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Patches), "*.xml");

        string[] GetAssetsPaths(string directoryPath, string searchPattern)
        {
            if (!Directory.Exists(directoryPath)) return new string[0];

            List<string> assets = new List<string>();
            foreach (string file in Directory.GetFiles(directoryPath, searchPattern, SearchOption.AllDirectories))
            {
                assets.Add(file);
            }
            return assets.ToArray();
        }
    }

}