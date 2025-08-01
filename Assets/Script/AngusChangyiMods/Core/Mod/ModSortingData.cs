using System;

namespace AngusChangyiMods.Core
{
    [System.Serializable]
    public class ModSortingData
    {
        public readonly string packageId;
        public readonly  string rootDirectory;
        
        [NonSerialized] public ModContentPack relatedContentPack;

        public ModSortingData(string packageId, string rootDirectory)
        {
            this.packageId = packageId;
            this.rootDirectory = rootDirectory;
        }

        public bool Matches(ModContentPack contentPack)
        {
            if (contentPack == null) return false;
            
            return contentPack.Meta.PackageId == packageId && contentPack.Meta.RootDirectory == rootDirectory;
        }

        public static bool CheckRelation(ModSortingData sortdata, ModContentPack contentPack)
        {
            if(contentPack.Meta.PackageId != sortdata.packageId) return false;
            if(contentPack.Meta.RootDirectory != sortdata.rootDirectory) return false;

            return true;
        }
    }

}