using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using AngusChangyiMods.Logger;

namespace AngusChangyiMods.Core.ModLoader
{
    public interface IModPreloader
    {
        void PreloadAllMods(string[] modsRootDirectories);
        List<ModContentPack> PreLoadedMods { get; }
    }

    public class ModPreloader : IModPreloader
    {
        public const string errorAboutNotFound = "About.xml not found in mod folder";
        public const string errorXmlParsingFailed = "Error parsing mod metadata";
        public const string packageIdDuplicateError = "Mod Package ID already registered, please use a unique ID";
        
        private ILogger logger;
        public List<ModContentPack> PreLoadedMods { get; private set; } = new();
        private Dictionary<string, ModMetaData> registeredPackagesId = new();

        public ModPreloader(ILogger logger)
        {
            this.logger = logger;
        }

        public void PreloadAllMods(string[] modsRootDirectories)
        {
            foreach (string modsRootDirectory in modsRootDirectories)
            {
                PreloadAllMods(modsRootDirectory);
            }
        }

        private void PreloadAllMods(string modsRootDirectory)
        {
            if (!Directory.Exists(modsRootDirectory))
                throw new DirectoryNotFoundException($"Mods root not found: {modsRootDirectory}");

            logger.LogInfo($"Preloading mods from: {modsRootDirectory}");

            string[] modFolders = Directory.GetDirectories(modsRootDirectory);

            foreach (string modDir in modFolders)
            {
                ModMetaData meta = CreateModMetadata(modDir);
                ModContentPack contentPack = new(meta);
                PreLoadedMods.Add(contentPack);

                logger.LogInfo($"Loaded mod: {meta.Name} ({meta.PackageId})");
            }
        }
        
        public ModMetaData CreateModMetadata(string modFolder)
        {
            string aboutPath = Path.Combine(modFolder, ModPath.AboutFile);
            if (!File.Exists(aboutPath))
            {
                // 建立基本的 ModMetaData 並標記錯誤
                string folderName = Path.GetFileName(modFolder);
                ModMetaData errorMeta = new ModMetaData(folderName, "Nan", "Unknown", "Failed to load", modFolder);
                errorMeta.SetError($"{errorAboutNotFound} at {modFolder}");
                logger.LogError($"{errorAboutNotFound} at {modFolder}");
                return errorMeta;
            }

            try
            {
                XDocument doc = XDocument.Load(aboutPath);
                XElement root = doc.Root;

                string name = root.Element(Mod.Name)?.Value;
                string packageId = root.Element(Mod.PackageId)?.Value;
                string author = root.Element(Mod.Author)?.Value;
                string description = root.Element(Mod.Description)?.Value;

                ModMetaData meta = new ModMetaData(name, packageId, author, description, modFolder);
                if(registeredPackagesId.TryGetValue(packageId, out ModMetaData existingPack))
                {
                    meta.SetError($"{packageIdDuplicateError}: {packageId}");
                    existingPack.SetError($"{packageIdDuplicateError}: {packageId}");
                    logger.LogError($"{packageIdDuplicateError}: {packageId}");
                }
                
                if (!meta.HasError)
                {
                    foreach (XElement ver in root.Elements(Mod.SupportedVersions).Elements(Mod.Li))
                    {
                        meta.SupportedVersions.Add(ver.Value.Trim());
                    }
                    registeredPackagesId.Add(packageId, meta);
                }

                return meta;
            }
            catch (Exception ex)
            {
                string folderName = Path.GetFileName(modFolder);
                ModMetaData errorMeta = new ModMetaData(folderName, folderName, "Unknown", "Failed to load", modFolder);
                errorMeta.SetError($"{errorXmlParsingFailed} at {modFolder}: {ex.Message}");
                logger.LogError($"{errorXmlParsingFailed} at {modFolder}: {ex.Message}");
                return errorMeta;
            }
        }
    }
}