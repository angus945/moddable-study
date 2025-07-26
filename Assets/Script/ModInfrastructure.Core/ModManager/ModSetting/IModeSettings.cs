namespace ModArchitecture
{
    public interface IModeSettings
    {
        void Save(IModSettingContext context);
        void Load(IModSettingContext context);
    }
}

