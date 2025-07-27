

using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace AngusChangyiMods.Core
{
    public class ModFinder
    {
        string[] findDirectories;
        List<ModMetaData> findMods = new List<ModMetaData>();
        HashSet<string> modIDs = new HashSet<string>();

        public ModFinder(params string[] findDirectories)
        {
            this.findDirectories = findDirectories;
        }

        public ModMetaData[] FindMods()
        {
            ModLogger.Log($"Starting mod directory search: {string.Join(", ", findDirectories)}", "ModFinder");
            findMods.Clear();
            modIDs.Clear();

            foreach (var findDirectory in findDirectories)
            {
                FindMods(findDirectory);
            }

            ModLogger.Log($"Mod search completed, found {findMods.Count} mods", "ModFinder");
            return findMods.ToArray();
        }
        void FindMods(string directoryPath)
        {
            if (!Directory.Exists(directoryPath))
            {
                ModLogger.LogWarning($"Mod Root directory does not exist: {directoryPath}", "ModFinder");
                return;
            }

            ModLogger.Log($"搜尋目錄: {directoryPath}", "ModFinder");
            string[] modDirectories = GetModDirectories(directoryPath);
            int foundCount = 0;

            foreach (var modDirectory in modDirectories)
            {
                if (GetModMetadata(modDirectory, out ModMetaData mod))
                {
                    GetModAssetsPath(mod);

                    findMods.Add(mod);
                    ModLogger.Log($"Found mod: {mod.id} - {mod.name}", "ModFinder");
                    foundCount++;
                }
            }

            ModLogger.Log($"Directory {directoryPath} search completed, found {foundCount} mods", "ModFinder");
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
                ModLogger.LogWarning($"About.md file not found in {modDirectory}");
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
                ModLogger.LogWarning($"Mod ID is missing in {modInfoPath}");
                return false;
            }

            if (modIDs.Contains(mod.id))
            {
                ModLogger.LogWarning($"Skipping Duplicate IDs mod: {mod.id}, {modDirectory}.");
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
            mod.textures = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Textures), "*.*");
            mod.sounds = GetAssetsPaths(Path.Combine(mod.directory, ModStructure.Sounds), "*.*");
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
}