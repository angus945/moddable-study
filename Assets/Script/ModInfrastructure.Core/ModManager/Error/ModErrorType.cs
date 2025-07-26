namespace ModArchitecture
{
    /// <summary>
    /// 定義模組載入過程中可能發生的錯誤類型
    /// </summary>
    public enum ModErrorType
    {
        /// <summary>
        /// 模組實例化錯誤
        /// </summary>
        Instancing,

        /// <summary>
        /// 模組初始化錯誤
        /// </summary>
        Initialization,

        /// <summary>
        /// 遊戲啟動錯誤
        /// </summary>
        GameStart,

        /// <summary>
        /// 資源載入錯誤
        /// </summary>
        AssetLoading
    }
}
