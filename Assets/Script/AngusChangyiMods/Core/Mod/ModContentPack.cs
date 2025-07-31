using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace AngusChangyiMods.Core
{
    public class ModContentPack
    {
        public ModMetaData Meta { get; }
        public bool IsValid => !Meta.HasError;
        public string RootDir => Meta.RootDirectory;

        public string AssembliesDir => Path.Combine(RootDir, ModPath.AssembliesDir);
        public string CustomDir => Path.Combine(RootDir, ModPath.CustomDir);
        public string DefsDir => Path.Combine(RootDir, ModPath.DefsDir);
        public string ScriptsDir => Path.Combine(RootDir, ModPath.ScriptsDir);
        public string SoundsDir => Path.Combine(RootDir, ModPath.SoundsDir);
        public string TexturesDir => Path.Combine(RootDir, "Textures");
        public string PatchesDir => Path.Combine(RootDir, ModPath.PatchesDir);
        public string LanguagesDir => Path.Combine(RootDir, ModPath.LanguagesDir);

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