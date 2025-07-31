using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AngusChangyiMods.Core
{
    public class ModContentPack
    {
        public ModMetaData Meta { get; }
        public string RootDir => Meta.RootDirectory;

        public string AssembliesDir => Path.Combine(RootDir, ModDirectory.Assemblies);
        public string CustomDir => Path.Combine(RootDir, ModDirectory.Custom);
        public string DefsDir => Path.Combine(RootDir, ModDirectory.Defs);
        public string ScriptsDir => Path.Combine(RootDir, ModDirectory.Scripts);
        public string SoundsDir => Path.Combine(RootDir, ModDirectory.Sounds);
        public string TexturesDir => Path.Combine(RootDir, "Textures");
        public string PatchesDir => Path.Combine(RootDir, ModDirectory.Patches);
        public string LanguagesDir => Path.Combine(RootDir, ModDirectory.Languages);

        public List<Assembly> LoadedAssemblies { get; } = new();
        public List<string> LoadedDefFiles { get; } = new();

        public ModContentPack(ModMetaData meta)
        {
            Meta = meta ?? throw new ArgumentNullException(nameof(meta));
        }
        
        public override string ToString()
        {
            return $"ModContentPack: {Meta.Name} ({Meta.PackageId})";
        }
    }
}