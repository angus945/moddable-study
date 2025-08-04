using System.Collections.Generic;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{
    public static class Def
    {
        public const string Root = "Defs";
        public const string DefName ="defName";  
        public const string Label = "label";
        public const string Description = "description";
        public const string Parent = "parent";
        public const string IsAbstract = "abstract";
        public const string Components = "components";
        public const string Extensions = "extensions";

        public const string Li = "li";
        public const string Class = "class";
            
        public const string SourceMod = "sourceMod"; // the mod that defines this def, used for mod compatibility
        public const string SourceFile = "sourceFile";
        
        public const string DefNamePattern = @"^[A-Za-z][A-Za-z0-9]*\.[A-Za-z][A-Za-z0-9]*$"; // OOO.OOO, first letter must be alphabet, only alphanumeric characters allowed
    }
    
    [System.Serializable]
    public abstract class DefBase
    {
        public string defName;
        public string label;
        public string description;

        public string parent;
        public bool isAbstract;
        
        public string sourceMod;
        public string sourceFile;

        public List<ComponentProperty> components = new List<ComponentProperty>(); // every instance will have individual compProperties
        public List<DefExtension> extensions = new List<DefExtension>(); // every instance will share the same extensions
    }
}
