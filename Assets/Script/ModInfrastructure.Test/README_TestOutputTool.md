# 獨立測試輸出工具使用說明

## 概述

這是一個完全獨立的測試結果輸出工具，不依賴 ModLogger 或任何其他日誌系統。

## 主要功能

### 1. TestResultWriter - 核心輸出工具

```csharp
// 記錄單個測試結果
TestResultWriter.LogTestResult("MyTest", true, "測試通過", 15.5);

// 記錄測試步驟
TestResultWriter.LogTestStep("MyTest", "驗證數值", 100, 100, true);

// 輸出所有結果到文件
TestResultWriter.WriteAllResults("MyTestSession");

// 清除記錄
TestResultWriter.ClearResults();
```

### 2. TestExecutor - 測試執行裝飾器

```csharp
// 自動執行測試並記錄結果（包含執行時間和異常處理）
TestExecutor.ExecuteTest("MyTest", () => {
    // 您的測試邏輯
    var result = SomeFunction();
    Assert.AreEqual(expected, result);
});

// 驗證並自動記錄
TestExecutor.VerifyAndLog("MyTest", "檢查數值", 100, actualValue);
TestExecutor.VerifyNotNullAndLog("MyTest", "檢查物件", myObject);
TestExecutor.VerifyConditionAndLog("MyTest", "檢查條件", someCondition, "說明");
```

### 3. BaseTestWithOutput - 測試類別基底

```csharp
[TestFixture]
public class MyTests : BaseTestWithOutput
{
    [Test]
    public void MyTest()
    {
        var testName = GetCurrentTestName();
        LogTestStart(testName);

        TestExecutor.ExecuteTest(testName, () => {
            // 測試邏輯
        });

        LogTestComplete(testName, "- 額外說明");
    }
}
```

## 輸出格式

### 1. 純文字報告 (.txt)

```
================================================================================
測試結果報告
生成時間: 2025-07-26 10:30:45
================================================================================

測試摘要:
  總測試數: 8
  通過: 7
  失敗: 1
  總執行時間: 125.50ms
  成功率: 87.5%

詳細結果:
--------------------------------------------------------------------------------
[PASS] TestKnightInheritance
        ✓ Knight_Exists: Expected NotNull, Got CharacterDef
        ✓ Knight_Health: Expected 150, Got 150
        ✓ Knight_Speed: Expected 3, Got 3
        執行時間: 15.25ms
        時間戳記: 10:30:45.123

[FAIL] TestSomeFunction
        ✗ SomeCheck: Expected 100, Got 95
        執行時間: 8.75ms
        時間戳記: 10:30:45.456
```

### 2. JSON 報告 (.json)

```json
{
  "metadata": {
    "generatedAt": "2025-07-26T10:30:45.789Z",
    "totalTests": 8,
    "passedTests": 7,
    "failedTests": 1,
    "totalDurationMs": 125.5,
    "successRate": 87.5
  },
  "results": [
    {
      "testName": "TestKnightInheritance",
      "passed": true,
      "message": "",
      "durationMs": 15.25,
      "timestamp": "2025-07-26T10:30:45.123Z"
    }
  ]
}
```

## 檔案位置

所有測試結果檔案會自動保存在：

- **專案根目錄**: `[ProjectRoot]/UnitTestLog/`
- 例如: `p:\MainProjects\moddable-architecture-study\UnitTestLog\`

檔案命名格式：

- `TestResults_yyyyMMdd_HHmmss.txt` - 純文字報告
- `TestResults_yyyyMMdd_HHmmss.json` - JSON 格式報告

## 特點

- ✅ 完全獨立，無外部依賴
- ✅ 自動計算執行時間
- ✅ 支援多種輸出格式
- ✅ 詳細的錯誤資訊記錄
- ✅ 測試摘要統計
- ✅ Unity Console 即時顯示
- ✅ 與現有 NUnit 測試完全相容

## 集成方式

1. 繼承 `BaseTestWithOutput` 類別
2. 使用 `TestExecutor.ExecuteTest()` 包裝測試邏輯
3. 使用 `TestExecutor.VerifyXXXAndLog()` 方法進行驗證
4. 測試完成後自動生成報告檔案
