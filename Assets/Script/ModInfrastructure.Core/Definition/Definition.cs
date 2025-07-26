using System.Collections.Generic;

namespace ModArchitecture.Definition
{
    [System.Serializable]
    public abstract class Definition
    {
        public string defID;
        public string label;
        public string description;

        public string inheritsFrom;
        public bool IsAbstract;

        public List<ComponentProperty> components = new List<ComponentProperty>(); // every instance will have individual compProperties
        public List<DefinitionExtension> extensions = new List<DefinitionExtension>(); // every instance will share the same extensions
    }

    public abstract class DefinitionExtension { }

    public abstract class ComponentProperty { }

    public class DefinitionReference<T>
    {
        public string defID;
        public T value;

        public DefinitionReference(string defID, T value)
        {
            this.defID = defID;
            this.value = value;
        }
    }


}
