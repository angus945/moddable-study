using ModArchitecture;
using UnityEngine;
// 注意：實際使用時需要引用 Harmony 庫
// using HarmonyLib;

namespace AuthorA
{
    /// <summary>
    /// 完整的 Harmony 初始化範例
    /// 此類別會在模組程序集載入後自動執行靜態建構子
    /// </summary>
    [StaticConstructorOnStartup]
    static class HarmonyInitExample
    {
        static HarmonyInitExample()
        {
            // 實際使用時的 Harmony 初始化代碼（目前註解避免編譯錯誤）
            /*
            try
            {
                // 創建 Harmony 執行個體，使用唯一的 ID
                var harmony = new Harmony("com.authora.moda.harmonypatches");
                
                // 自動掃描並套用所有帶有 [HarmonyPatch] attribute 的方法
                harmony.PatchAll();
                
                Debug.Log("[ModA] Harmony patches 已成功載入");
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"[ModA] Harmony 初始化失敗: {ex.Message}");
            }
            */

            // 目前的測試輸出
            Debug.Log("[ModA] HarmonyInitExample 靜態建構子已執行");
        }
    }

    /// <summary>
    /// Harmony Patch 範例類別
    /// 實際使用時需要取消註解並引用 Harmony 庫
    /// </summary>
    /*
    [HarmonyPatch(typeof(UnityEngine.Debug), "Log", typeof(object))]
    static class DebugLogPatch
    {
        // 前綴方法：在原方法執行前執行
        static bool Prefix(object message)
        {
            // 修改所有 Debug.Log 的輸出，添加前綴
            UnityEngine.Debug.Log($"[ModA Patched] {message}");
            return false; // 返回 false 阻止原方法執行
        }
        
        // 後綴方法：在原方法執行後執行
        static void Postfix(object message)
        {
            // 在原方法執行後執行的邏輯
        }
    }
    */
}
