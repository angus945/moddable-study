namespace AngusChangyiMods.Core
{
    using System;

    /// <summary>
    /// Interface for mod initializers.
    /// name rules: ModID + Entry
    /// </summary>
    public interface IModEntry
    {
        void Initialize();
        void OnGameStart();
        void OnGameEnd();
    }
}
