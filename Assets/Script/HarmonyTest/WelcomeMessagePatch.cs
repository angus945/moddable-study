// using HarmonyLib;
// using ModArchitecture.Logger;
// using UnityEngine;

// public class StartLogger : ILogicHook<string>
// {
//     public void OnEvent(string origin)
//     {
//         // 在遊戲開局前執行歡迎訊息
//         Debug.Log("歡迎來到 RimWorld 模組世界！ (這是由 Harmony Patch 覆蓋的歡迎訊息)");
//     }
// }

// public class LoggingDecorator : ILogicHook<string>
// {
//     private readonly ILogicHook<string> _inner;

//     public LoggingDecorator(ILogicHook<string> inner)
//     {
//         _inner = inner;
//     }

//     public void OnEvent(string data)
//     {
//         ModLogger.Log($"LoggingDecorator: {data}");
//         _inner.OnEvent(data);
//     }
// }

// [HarmonyPatch(typeof(HarmonyTestTarget))] // 目標類別
// [HarmonyPatch(nameof(HarmonyTestTarget.InitNewGame))] // 或直接寫 "InitNewGame"
// static class WelcomeMessagePatch
// {

//     public static ILogicHook<string> logger = new LoggingDecorator(new StartLogger());

//     static bool Prefix(string message)
//     {
//         // 在遊戲開局前執行歡迎訊息__message
//         logger.OnEvent(message);

//         return false; // 返回 false 以阻止原方法執行
//     }
// }