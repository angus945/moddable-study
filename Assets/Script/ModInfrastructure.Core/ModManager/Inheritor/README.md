# ModDefinitionInheritor 使用說明

## 概述

`ModDefinitionInheritor` 是一個基於反射的定義繼承處理器系統，參考了 `ModDefinitionDeserializer` 的設計模式。它能夠自動發現並註冊繼承處理器，並且為模組定義提供繼承關係處理。

## 核心組件

### 1. IDefinitionInheritor 介面

```csharp
public interface IDefinitionInheritor
{
    Type HandlesType { get; }
    IEnumerable<Definition> ProcessInheritance(IEnumerable<Definition> definitions);
}
```

### 2. ModDefinitionInheritor 主要類別

負責：

- 透過反射自動發現所有實作 `IDefinitionInheritor` 的類別
- 註冊繼承處理器
- 處理定義間的繼承關係
- 提供預設繼承邏輯處理

### 3. 具體繼承處理器實作

- `CharacterDefInheritor` - 處理 CharacterDef 的繼承
- `ThingDefInheritor` - 處理 ThingDef 的繼承

## 使用方式

### 1. 註冊繼承處理器

```csharp
var inheritor = new ModDefinitionInheritor();
inheritor.RegisterInheritors(); // 自動發現並註冊所有處理器
```

### 2. 處理繼承關係

```csharp
// 假設已經有解析好的定義字典
Dictionary<Type, List<Definition>> definitions = deserializer.InstanceDefinitions(xmlDocument);

// 處理繼承關係
Dictionary<Type, List<Definition>> processedDefinitions = inheritor.ProcessInheritance(definitions);

// 將處理過的定義設置到資料庫
DefinitionDatabase.SetDefinitions(processedDefinitions);
```

## 創建自定義繼承處理器

### 步驟 1: 實作 IDefinitionInheritor

```csharp
public class MyCustomDefInheritor : IDefinitionInheritor
{
    public Type HandlesType => typeof(MyCustomDef);

    public IEnumerable<Definition> ProcessInheritance(IEnumerable<Definition> definitions)
    {
        var customDefs = definitions.Cast<MyCustomDef>().ToList();
        // 實作自定義的繼承邏輯
        return ProcessCustomInheritance(customDefs).Cast<Definition>();
    }

    private IEnumerable<MyCustomDef> ProcessCustomInheritance(List<MyCustomDef> definitions)
    {
        // 自定義繼承處理邏輯
        // ...
    }
}
```

### 步驟 2: 系統自動發現

系統會透過反射自動發現並註冊新的繼承處理器，無需手動註冊。

## 繼承邏輯

### 預設繼承邏輯

對於沒有特定處理器的類型，系統提供預設繼承處理：

1. **基本欄位繼承**: label, description
2. **組件繼承**: components 列表合併
3. **擴充功能繼承**: extensions 列表合併

### 具體類型繼承邏輯

- **CharacterDef**: 繼承 health, speed 屬性
- **ThingDef**: 繼承 damage, stack, tags, weaponProps 屬性

## 設計原則

### 符合 SOLID 原則

1. **單一職責原則**: 每個繼承處理器只負責一種類型的定義
2. **開放封閉原則**: 可以輕鬆擴充新的繼承處理器而無需修改現有程式碼
3. **依賴反轉原則**: 依賴於抽象的 IDefinitionInheritor 介面

### 參考 Deserializer 設計模式

- 相同的反射發現機制
- 類似的註冊與管理流程
- 一致的錯誤處理與日誌記錄

## 日誌與偵錯

系統會記錄以下資訊：

- 繼承處理器註冊過程
- 繼承處理的成功與失敗
- 具體的繼承操作細節

標籤：`"DefinitionInheritor"`

## 整合到 ModManager

建議在 `ModManager` 中加入繼承處理步驟：

```csharp
// 在 ModManager 中添加
ModDefinitionInheritor inheritor;

// 在建構函式中初始化
public ModManager(...)
{
    // 其他初始化...
    this.inheritor = new ModDefinitionInheritor();
}

// 在適當的時機註冊和處理
public void ProcessDefinitions()
{
    // 註冊繼承處理器
    inheritor.RegisterInheritors();

    // 反序列化定義
    var definitions = deserializer.InstanceDefinitions(loadingRecord.definitionDoc);

    // 處理繼承關係
    var processedDefinitions = inheritor.ProcessInheritance(definitions);

    // 設置到資料庫
    DefinitionDatabase.SetDefinitions(processedDefinitions);
}
```
