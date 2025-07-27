🔧 Prompt：針對模組化框架的測試架構重構

你是一名精通 C#、Unity、模組化系統架構與測試驅動開發（TDD）的高階工程師。
請協助我重構現有模組化載入框架的測試架構，提升可讀性、可維護性與覆蓋率。以下是具體的專案背景與重構目標。
🧭【專案背景】

此專案為 Unity + C# 撰寫的模組化系統，目標是支援模組資料的：

    定義（Def）載入

    XML Patch 操作

    定義繼承（Inheritance）

    模組排序與組裝

    執行時熱插拔（Hot-swap）與反射自動注冊

資料夾結構精簡如下（完整見附檔）：

```bash
moddable-architecture-study/
├── Assets/
│ ├── Script/AngusChangyiMods/
│ │ ├── Core/... ← 模組核心邏輯與 Inheritor / Patcher 系統
│ │ ├── Test/... ← 測試案例，分類良好
│ │ ├── Unity/... ← Unity API 封裝
│ │ ├── Example/... ← 範例模組與擴充 Def
│ ├── StreamingAssets/Mods/ ← 模組資料與 XML
├── \*.csproj / .sln 檔案齊全
```

🎯【重構目標】

請依據以下邏輯重構測試系統：

    針對模組關鍵元件撰寫高品質單元測試，包含：

        ModDefinitionInheritor.cs

        DefinitionPatcher.cs

        ModDefinitionProcessor.cs

        ModDefinitionLoader.cs

    抽離重複邏輯與測資：

        移除重複的 inline XML 定義

        將 XML 測資移出 function，集中於 TestData 目錄或用 [TestCaseSource] 實作參數化

    加強邊界與錯誤處理測試，例如：

        Patch 與 Inherit 執行順序變化的影響

        欄位類型包含：數值、字串、布林、陣列、巢狀物件

        Patch 對應失敗與跳過邏輯（已實作 ModError.cs）

    測試 Logger 與輸出行為：

        已有 MockLogger 與 TestResultLogger.cs

        確認錯誤與警告被正確記錄

    保持與現有測試結構一致，例如：

        AngusChangyiMods.Tests.asmdef 中分類邏輯

        TestDefinition.cs 與 TestDefinition.xml 對應

📌【附加提示】

    可優先重構 ModDefinitionInheritorTests.cs 為測試入口範本

    如需拆出測資，請建立 TestCaseSources/ 或 TestData/ 分類資料夾

    目標為：減少硬編碼、提高測試覆蓋率與擴充性
