# DefinitionInheritanceUtils - 繼承處理工具類

## 🎯 **解決的問題**

原先的繼承處理器存在以下問題：

1. **遞歸繼承缺失**: 只處理單層繼承，無法處理多層繼承鏈 (如 `IronSword` → `BaseMeleeWeapon` → `BaseWeapon` → `BaseItem`)
2. **數值繼承判斷不準確**: 使用 `== 0` 判斷是否繼承，對於可能合法的 0 值會誤判
3. **代碼重複**: 每個 Inheritor 都需要重複實現相同的繼承邏輯
4. **複雜物件處理繁瑣**: 對於 `WeaponProperties` 等複雜物件，需要手動處理每個欄位
5. **可讀性問題**: 大量 `Func<>` 和 `Action<>` 降低代碼可讀性

## 🔧 **新工具類功能**

### **語意化 Delegate 定義**

```csharp
// 繼承邏輯處理器
public delegate void InheritanceAction<T>(T child, T parent);

// 屬性存取器
public delegate TValue PropertyGetter<T, TValue>(T definition);
public delegate void PropertySetter<T, TValue>(T definition, TValue value);

// 繼承判斷器
public delegate bool InheritancePredicate<TValue>(TValue childValue, TValue parentValue);

// 物件建立器與欄位繼承器
public delegate TObject ObjectCreator<TObject>(TObject parent);
public delegate void FieldInheritor<TObject>(TObject child, TObject parent);
```

### **核心方法**

```csharp
// 自動處理完整繼承鏈，包含遞歸解析
DefinitionInheritanceUtils.ProcessInheritance<T>(definitions, customLogic)

// 字串屬性繼承 (空值時繼承)
DefinitionInheritanceUtils.InheritStringProperty(child, parent, getter, setter)

// 數值屬性繼承 (預設值時繼承，使用 IComparable<T> 精確比較)
DefinitionInheritanceUtils.InheritNumericProperty(child, parent, getter, setter)

// 列表合併 (避免重複項目)
DefinitionInheritanceUtils.MergeListProperty(child, parent, getter, setter)

// 複雜物件繼承 (部分欄位繼承)
DefinitionInheritanceUtils.InheritComplexProperty(child, parent, getter, setter, createNew, inheritFields)
```

### **改進後的 CharacterDefInheritor**

```csharp
public IEnumerable<Definition> ProcessInheritance(IEnumerable<Definition> definitions)
{
    var characterDefs = definitions.Cast<CharacterDef>().ToList();
    var processedDefs = DefinitionInheritanceUtils.ProcessInheritance(characterDefs, ApplyCharacterDefSpecificInheritance);
    return processedDefs.Cast<Definition>();
}

private void ApplyCharacterDefSpecificInheritance(CharacterDef child, CharacterDef parent)
{
    // 簡潔的數值繼承，使用語意化 delegate
    DefinitionInheritanceUtils.InheritNumericProperty(child, parent,
        c => c.health, (c, v) => c.health = v);

    DefinitionInheritanceUtils.InheritNumericProperty(child, parent,
        c => c.speed, (c, v) => c.speed = v);
```

}

````

## 🚀 **重要改進**

### **1. 遞歸繼承處理**

- 自動建立完整的繼承鏈：`BaseCharacter` → `BaseWarrior` → `Knight`
- 從根節點到葉節點依序套用繼承
- 防止循環繼承 (Circular inheritance detection)

### **2. 更精確的數值判斷**

- 使用 `default(T)` 和 `IComparable<T>` 進行比較
- 支援所有數值類型 (int, float, double 等)
- 避免誤判合法的 0 值

### **3. 標籤智能合併**

```csharp
// Before: 手動處理標籤合併，代碼冗長
if (definition.tags == null && parentDefinition.tags != null) { ... }
else if (definition.tags != null && parentDefinition.tags != null) { ... }

// After: 一行搞定，自動避免重複
DefinitionInheritanceUtils.MergeListProperty(child, parent, c => c.tags, (c, v) => c.tags = v);
````

### **4. 複雜物件繼承**

```csharp
// 自動處理 WeaponProperties 的部分欄位繼承
DefinitionInheritanceUtils.InheritComplexProperty(child, parent,
    c => c.weaponProps,
    (c, v) => c.weaponProps = v,
    parentWeapon => new WeaponProperties { /* 複製建構 */ },
    (childWeapon, parentWeapon) => { /* 欄位繼承邏輯 */ });
```

## 📊 **測試案例修復**

### **修復前的問題**

- `Knight` 的 health = 0 (應該從 `BaseWarrior` 繼承 150)
- `Wizard` 的 speed = 0 (應該從 `BaseMage` 繼承 6)
- `IronSword` 缺少從多層父類繼承的標籤

### **修復後的預期結果**

```csharp
// Knight 應該繼承完整鏈：BaseCharacter → BaseWarrior → Knight
Knight.health = 150 (從 BaseWarrior 繼承)
Knight.speed = 3 (自己定義，覆蓋父類)
Knight.label = "騎士" (自己定義)

// IronSword 多層繼承：BaseItem → BaseWeapon → BaseMeleeWeapon → IronSword
IronSword.tags = ["item", "weapon", "equipment", "melee", "metal"]
IronSword.weaponProps.type = "Melee" (從 BaseMeleeWeapon 繼承)
IronSword.weaponProps.damage = 30 (自己覆蓋)
IronSword.weaponProps.range = 1.2 (從 BaseMeleeWeapon 繼承)
```

## 💡 **使用建議**

1. **簡單屬性**: 使用 `InheritStringProperty` 和 `InheritNumericProperty`
2. **列表屬性**: 使用 `MergeListProperty` 進行智能合併
3. **複雜物件**: 使用 `InheritComplexProperty` 處理部分繼承
4. **自訂邏輯**: 在 `ApplySpecificInheritance` 中實現特殊需求

## 🔄 **最新更新：語意化 Delegate**

為了進一步提升代碼可讀性，將所有 `Func<>` 和 `Action<>` 改為具有明確語意的 delegate：

```csharp
// 1. 繼承邏輯處理器
InheritanceAction<T> - 更清楚地表達「繼承動作」的意圖

// 2. 屬性存取器
PropertyGetter<T, TValue> - 明確表示「屬性取得器」
PropertySetter<T, TValue> - 明確表示「屬性設定器」

// 3. 繼承判斷器
InheritancePredicate<TValue> - 明確表示「繼承條件判斷」

// 4. 物件處理器
ObjectCreator<TObject> - 明確表示「物件建立器」
FieldInheritor<TObject> - 明確表示「欄位繼承器」
```

**優點**：

- 降低認知負擔，程式碼意圖更清晰
- 型別簽名更容易理解和維護
- 符合 Clean Code 原則，提升專業性

這個工具類大幅簡化了繼承處理器的實現複雜度，同時提供了更強大、準確且易讀的繼承功能！
