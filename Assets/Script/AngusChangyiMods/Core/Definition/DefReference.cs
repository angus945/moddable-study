namespace AngusChangyiMods.Core
{
    public class DefReference<T>
    {
        public string defName;
        public T value;

        public DefReference(string defName, T value)
        {
            this.defName = defName;
            this.value = value;
        }
    }
}