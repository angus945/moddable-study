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

        public bool HasError { get; private set; }
        public string ErrorReason { get; private set; }

        public ModMetaData(string name, string packageId, string author, string description, string rootDirectory)
        {
            Name = name;
            PackageId = packageId;
            Author = author;
            Description = description;
            RootDirectory = rootDirectory;
            SupportedVersions = new List<string>();
        }
        
        public void SetError(string reason)
        {
            HasError = true;
            ErrorReason = reason;
        }
        public override string ToString()
        {
            return $"{Name} ({PackageId}) by {Author}";
        }
    }
}