using System.Collections.Generic;
using System.Xml.Linq;

namespace AngusChangyiMods.Core
{

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
