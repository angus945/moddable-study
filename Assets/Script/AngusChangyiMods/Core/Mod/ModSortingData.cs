using System;

namespace AngusChangyiMods.Core
{
    [System.Serializable]
    public class ModSortingData
    {
        public string packageId { get; set; }
        public string rootDirectory { get; set; }
        
        [System.Xml.Serialization.XmlIgnore] public ModContentPack relatedContentPack;

        public ModSortingData()
        {
            packageId = string.Empty;
            rootDirectory = string.Empty;
        }
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