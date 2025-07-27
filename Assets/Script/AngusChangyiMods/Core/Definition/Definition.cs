using System.Collections.Generic;

namespace AngusChangyiMods.Core
{
    [System.Serializable]
    public abstract class DefBase
    {
        public string defID;
        public string label;
        public string description;

        public string inheritsFrom;
        public bool IsAbstract;

        public List<ComponentProperty> components = new List<ComponentProperty>(); // every instance will have individual compProperties
        public List<DefExtension> extensions = new List<DefExtension>(); // every instance will share the same extensions
    }

    public abstract class DefExtension { }

    public abstract class ComponentProperty { }

    public class DefReference<T>
    {
        public string defID;
        public T value;

        public DefReference(string defID, T value)
        {
            this.defID = defID;
            this.value = value;
        }
    }


}
