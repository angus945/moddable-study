using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    [System.Serializable]
    public abstract class DefBase
    {
        public string defName;
        public string label;
        public string description;

        public string inheritsFrom;
        public bool isAbstract;

        public List<ComponentProperty> Components = new List<ComponentProperty>(); // every instance will have individual compProperties
        public List<DefExtension> Extensions = new List<DefExtension>(); // every instance will share the same extensions
    }
}
