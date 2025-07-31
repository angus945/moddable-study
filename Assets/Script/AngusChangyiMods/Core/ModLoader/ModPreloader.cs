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
        private ILogger logger;
        
        public List<ModContentPack> PreLoadedMods { get; private set; } = new();
        
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
                try
                {
                    ModMetaData meta = CreateModMetadata(modDir);
                    ModContentPack contentPack = new(meta);
                    PreLoadedMods.Add(contentPack);

                    logger.LogInfo($"Loaded mod: {meta.Name} ({meta.PackageId})");
                }
                catch (Exception ex)
                {
                    logger.LogWarning($"Failed to load mod at {modDir}: {ex.Message}");
                }
            }
        }
        
        public ModMetaData CreateModMetadata(string modFolder)
        {
            string aboutPath = Path.Combine(modFolder, ModDirectory.About);
            if (!File.Exists(aboutPath))
            {
                throw new FileNotFoundException("About.xml not found in " + aboutPath);
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
                ModValidator.Validate(meta);

                foreach (XElement ver in root.Elements(Mod.SupportedVersions).Elements(Mod.Li))
                {
                    meta.SupportedVersions.Add(ver.Value.Trim());
                }
                
                return meta;
            }
            catch (ArgumentNullException)
            {
                throw;
            }
            catch (FormatException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to load mod metadata from {aboutPath}: {ex.Message}", ex);
            }
        }
    }
}