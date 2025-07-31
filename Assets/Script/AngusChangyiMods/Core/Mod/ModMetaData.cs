using System;
using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    public class ModMetaData
    {
        public readonly string Name;
        public readonly string PackageId;
        public readonly string Author;
        public readonly string Description;
        public readonly string RootDirectory;
        public readonly List<string> SupportedVersions;

        public ModMetaData(string name, string packageId, string author, string description, string rootDirectory)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            PackageId = packageId ?? throw new ArgumentNullException(nameof(packageId));
            Author = author ?? throw new ArgumentNullException(nameof(author));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            RootDirectory = rootDirectory ?? throw new ArgumentNullException(nameof(rootDirectory));
            SupportedVersions = new List<string>();
        }

        public override string ToString()
        {
            return $"{Name} ({PackageId}) by {Author}";
        }
    }
}