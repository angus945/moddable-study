namespace AngusChangyiMods.Core
{
    public interface IModeSettings
    {
        void Save(IModSettingContext context);
        void Load(IModSettingContext context);
    }
}

