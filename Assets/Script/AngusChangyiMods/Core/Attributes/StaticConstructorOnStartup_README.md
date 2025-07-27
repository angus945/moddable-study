# StaticConstructorOnStartup 功能說明

## 概述

`StaticConstructorOnStartup` attribute 允許標記靜態類別在模組程序集載入完成後自動觸發其靜態建構子。這特別適用於 Harmony patch 的初始化。

## 使用方式

### 1. 標記靜態類別

```csharp
using AngusChangyiMods.Core;

[StaticConstructorOnStartup]
static class HarmonyInit
{
    static HarmonyInit()
    {
        // 靜態建構子會在 LoadModsAssemblies() 完成後自動執行
        var harmony = new Harmony("com.yourname.modname.harmonypatches");
        harmony.PatchAll();
        Debug.Log("[ModName] Harmony Patches 已初始化");
    }
}
```

### 2. Harmony Patch 範例

```csharp
[HarmonyPatch(typeof(SomeClass), "SomeMethod")]
static class SomeMethodPatch
{
    static bool Prefix(ref bool __result)
    {
        // 前綴邏輯
        return true; // 返回 true 繼續執行原方法，false 阻止原方法執行
    }

    static void Postfix(ref bool __result)
    {
        // 後綴邏輯
    }
}
```

## 執行時機

- `LoadModsAssemblies()` 方法完成後自動觸發
- 在 `ModsInitialization()` 之前執行
- 適合用於需要在模組實例化前完成的初始化工作

## 技術細節

### ModManager.TriggerStaticConstructors()

1. 掃描所有已載入的程序集
2. 過濾系統程序集，只處理模組程序集
3. 尋找帶有 `StaticConstructorOnStartup` attribute 的靜態類別
4. 使用 `RuntimeHelpers.RunClassConstructor()` 觸發靜態建構子

### 安全考量

- 只處理非系統程序集，避免影響框架程序集
- 包含完整的例外處理
- 提供詳細的日誌記錄

## 使用範例

參考 `ModA` 中的 `HarmonyInitExample` 類別，展示了完整的使用方式。

## 注意事項

1. 靜態建構子只會執行一次
2. 確保 Harmony 庫已正確引用
3. 使用唯一的 Harmony ID 避免衝突
4. 適當的錯誤處理是必要的
