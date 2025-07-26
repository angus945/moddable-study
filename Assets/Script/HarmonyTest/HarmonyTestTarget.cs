// using HarmonyLib;
// using UnityEngine;

// public interface ILogicHook<in T>
// {
//     void OnEvent(T data);
// }

// public class HarmonyTestTarget : MonoBehaviour
// {
//     void Awake()
//     {
//         var harmony = new Harmony("com.yourname.moddablestudy.harmonytest");
//         harmony.PatchAll();
//         // 自動尋找並套用當前程序集中的所有 HarmonyPatch (所以每個程序集 asmdf 都要獨立實例並調用 PatchAll)
//         // 每個模組也是獨立實例並調用 PatchAll

//     }
//     public void Start()
//     {
//         InitNewGame("預設的遊戲初始化邏輯。");
//     }

//     // 這個類別可以是任何需要被 Harmony Patch 的目標類別
//     public void InitNewGame(string message)
//     {
//         // 模擬遊戲初始化邏輯
//         Debug.Log(message);
//     }
// }