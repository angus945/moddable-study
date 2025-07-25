namespace ModArchitecture
{
    using System;

    /// <summary>
    /// Interface for mod initializers.
    /// </summary>
    public interface IModInstance
    {
        void Initialize();
        void OnGameStart();
        void OnGameEnd();
    }
}
