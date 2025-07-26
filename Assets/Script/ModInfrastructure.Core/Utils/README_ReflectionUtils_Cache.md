# ReflectionUtils 快取機制說明

## 概述

ReflectionUtils 現在具備完整的快取機制，可顯著提升反射操作的效能。

## 主要改進

### 1. 快取結構

- **Assembly Types Cache**: 快取每個 Assembly 中的所有 Type
- **Assignable Types Cache**: 快取 `GetTypesAssignableFrom` 的查詢結果
- **Type Name Cache**: 快取 `FindTypeByName` 的查詢結果

### 2. 線程安全

- 使用 `lock(_cacheLock)` 確保多線程環境下的安全性
- 雙重檢查鎖定模式用於初始化

### 3. 延遲載入

- 首次呼叫時才進行 Assembly 掃描
- 後續呼叫直接使用快取結果

## API 使用

### 清除快取

```csharp
ReflectionUtils.ClearCache();
```

### 重新整理快取

```csharp
ReflectionUtils.RefreshCache();
```

### 正常使用（自動快取）

```csharp
// 這些方法現在會自動使用快取
var types = ReflectionUtils.GetTypesAssignableFrom<Definition>();
var type = ReflectionUtils.FindTypeByName("TestDefinition");
```

## 效能提升

### 預期改進

- 首次呼叫：與原版相同（需要初始化）
- 後續呼叫：提升 90%+ 的效能
- 相同參數的重複查詢：近乎即時回應

### 測試方法

在 Test.cs 中新增了 `TestReflectionCachePerformance()` 方法，可透過 Inspector 按鈕測試效能。

## 設計原則遵循

- **SOLID**: 單一職責（快取管理獨立）、依賴反轉
- **KISS**: 介面簡單，向後相容
- **效能優化**: 避免重複掃描
- **線程安全**: 支援多線程環境

## 注意事項

1. **記憶體使用**: 快取會增加記憶體使用量，但相對於效能提升是值得的
2. **動態載入**: 如果有新的 Assembly 動態載入，需要手動呼叫 `RefreshCache()`
3. **清理時機**: 在適當時機（如遊戲結束）可呼叫 `ClearCache()` 釋放記憶體
