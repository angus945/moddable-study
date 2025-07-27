namespace AngusChangyiMods.Core
{
    /// <summary>
    /// Define error types that may occur during mod loading process
    /// </summary>
    public enum ModErrorType
    {
        /// <summary>
        /// Mod instantiation error
        /// </summary>
        Instancing,

        /// <summary>
        /// Mod initialization error
        /// </summary>
        Initialization,

        /// <summary>
        /// Game start error
        /// </summary>
        GameStart,

        /// <summary>
        /// Asset loading error
        /// </summary>
        AssetLoading
    }
}
