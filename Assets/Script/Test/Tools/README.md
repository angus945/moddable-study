# Test Tools - 獨立測試輸出工具

## 概述

這個工具包提供完全獨立的測試結果記錄和輸出功能，不依賴任何特定的測試框架或日誌系統。

## 檔案結構

```
Assets/Script/Test/Tools/
├── TestResultWriter.cs    # 核心輸出工具
├── SimpleTestRunner.cs    # 簡單測試執行器
└── README.md              # 本說明文件
```

## 主要組件

### 1. TestResultWriter

負責記錄和輸出測試結果到文件。

**命名空間**: `Test.Tools`  
**主要方法**:

- `LogTestResult()` - 記錄測試結果
- `LogTestStep()` - 記錄測試步驟
- `WriteAllResults()` - 輸出所有結果到文件
- `ClearResults()` - 清除記錄
- `GetSummary()` - 獲取測試統計

### 2. SimpleTestRunner

簡單的測試執行器，提供基本的斷言和測試組織功能。

**命名空間**: `Test.Tools`  
**主要方法**:

- `ExecuteTest()` - 執行單個測試
- `VerifyEqual()` - 驗證相等
- `VerifyNotNull()` - 驗證非空
- `VerifyTrue()` / `VerifyFalse()` - 驗證布爾條件
- `RunTestSuite()` - 運行測試套件

## 使用範例

### 基本使用

```csharp
using Test.Tools;

// 運行測試套件
SimpleTestRunner.RunTestSuite("MyTestSuite", _ => {

    SimpleTestRunner.ExecuteTest("Test1", () => {
        SimpleTestRunner.VerifyEqual("Test1", "數值檢查", 100, 100);
        SimpleTestRunner.VerifyTrue("Test1", "條件檢查", true);
    });

    SimpleTestRunner.ExecuteTest("Test2", () => {
        var obj = new MyClass();
        SimpleTestRunner.VerifyNotNull("Test2", "物件檢查", obj);
    });

});
```

### 手動記錄

```csharp
using Test.Tools;

// 清除舊記錄
TestResultWriter.ClearResults();

// 手動記錄結果
TestResultWriter.LogTestResult("MyTest", true, "成功", 15.5);
TestResultWriter.LogTestStep("MyTest", "步驟1", 100, 100, true);

// 輸出到文件
TestResultWriter.WriteAllResults("MyTestSession");
```

## 輸出格式

### 檔案位置

所有測試結果保存在：`[ProjectRoot]/UnitTestLog/`

### 輸出文件

- `{TestName}_yyyyMMdd_HHmmss.txt` - 純文字報告
- `{TestName}_yyyyMMdd_HHmmss.json` - JSON 格式報告

### 純文字報告範例

```
================================================================================
測試結果報告
生成時間: 2025-07-26 23:30:45
================================================================================

測試摘要:
  總測試數: 3
  通過: 2
  失敗: 1
  總執行時間: 25.75ms
  成功率: 66.7%

詳細結果:
--------------------------------------------------------------------------------
[PASS] Test1
        ✓ 數值檢查: Expected 100, Got 100
        執行時間: 12.5ms
        時間戳記: 23:30:45.123

[FAIL] Test2
        ✗ 失敗檢查: Expected 100, Got 95
        執行時間: 8.25ms
        時間戳記: 23:30:45.456
```

### JSON 報告範例

```json
{
  "metadata": {
    "generatedAt": "2025-07-26T23:30:45.789Z",
    "totalTests": 3,
    "passedTests": 2,
    "failedTests": 1,
    "totalDurationMs": 25.75,
    "successRate": 66.7
  },
  "results": [
    {
      "testName": "Test1",
      "passed": true,
      "message": "✓ 數值檢查: Expected 100, Got 100",
      "durationMs": 12.5,
      "timestamp": "2025-07-26T23:30:45.123Z"
    }
  ]
}
```

## 與 NUnit 測試整合

在 NUnit 測試中使用這些工具：

```csharp
using NUnit.Framework;
using Test.Tools;

[TestFixture]
public class MyNUnitTests
{
    [OneTimeSetUp]
    public void Setup()
    {
        TestResultWriter.ClearResults();
    }

    [Test]
    public void MyTest()
    {
        var testName = TestContext.CurrentContext.Test.Name;

        SimpleTestRunner.ExecuteTest(testName, () => {
            // 您的測試邏輯
            SimpleTestRunner.VerifyEqual(testName, "檢查", expected, actual);
        });
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        TestResultWriter.WriteAllResults("NUnitResults");
    }
}
```

## 特點

- ✅ 完全獨立，無外部依賴
- ✅ 不受 Assembly Definition 限制
- ✅ 自動計算執行時間
- ✅ 支援多種輸出格式
- ✅ 詳細的錯誤資訊記錄
- ✅ 可與任何測試框架整合
- ✅ 簡單易用的 API
