using System;

namespace AngusChangyiMods.Core
{
    public class ModLoadingData
    {
        public bool active;
        
        public int order;
        public bool raceId;
        public readonly string packageId;
        public readonly  string rootDirectory;

        public ModLoadingData(string packageId, string rootDirectory)
        {
            this.packageId = packageId;
            this.rootDirectory = rootDirectory;
        }

        public bool Matches(ModContentPack contentPack)
        {
            if (contentPack == null) return false;
            
            return contentPack.Meta.PackageId == packageId && contentPack.Meta.RootDirectory == rootDirectory;
        }
    }

}