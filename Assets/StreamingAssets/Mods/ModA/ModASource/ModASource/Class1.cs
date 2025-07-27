using AngusChangyiMods;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace AuthorA
{
    public class ModAEntry : IModEntry
    {
        public void Initialize()
        {
            Debug.Log("ModA initialized");
        }
        public void OnGameStart()
        {
            Debug.Log("ModA game started");

            var texture = ModAssetsDatabase.GetAsset<Texture2D>("ModA/Avator.png");
            if (texture != null)
            {
                Debug.Log("Texture loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load texture");
            }

            var sound = ModAssetsDatabase.GetAsset<AudioClip>("TestSound.mp3");
            if (sound != null)
            {
                Debug.Log("Sound loaded successfully");
            }
            else
            {
                Debug.LogError("Failed to load sound");
            }
        }
        public void OnGameEnd()
        {
            Debug.Log("ModA game ended");
        }
    }

    /// <summary>
    /// 範例：帶有 StaticConstructorOnStartup attribute 的靜態類別
    /// 用於自動初始化 Harmony patches
    /// </summary>
    [StaticConstructorOnStartup]
    static class HarmonyInit
    {
        static HarmonyInit()
        {
            // 創建 Harmony 執行個體，並以本模組唯一ID（例如作者_模組名稱）初始化
            // var harmony = new Harmony("com.authora.moda.harmonytest");
            // 自動尋找並套用當前程序集中的所有 HarmonyPatch
            // harmony.PatchAll();
            Debug.Log("[ModA] Harmony Patches 已初始化 (靜態建構子已觸發)");
        }
    }

}
