# 繼承處理器測試檔案說明

這個目錄包含用於測試繼承處理器功能的定義檔案。

## 檔案結構

### Characters.xml

測試 `CharacterDef` 的繼承關係：

**繼承層次結構：**

```
BaseCharacter (抽象)
├── BaseWarrior (抽象)
│   └── Knight (具體)
├── BaseMage (抽象)
│   └── Wizard (具體)
└── Archer (具體)

Peasant (獨立，無繼承)
```

**測試要點：**

- 抽象定義不會出現在最終結果中
- `Knight` 應該繼承 `BaseWarrior` 的 health (150)，但覆蓋 speed (3)
- `Wizard` 應該覆蓋 health (70)，但繼承 `BaseMage` 的 speed (6)
- `Archer` 直接從 `BaseCharacter` 繼承，並覆蓋所有屬性
- `Peasant` 沒有繼承關係，保持原始值

### Things.xml

測試 `ThingDef` 的複雜繼承關係：

**繼承層次結構：**

```
BaseItem (抽象)
├── BaseWeapon (抽象)
│   ├── BaseMeleeWeapon (抽象)
│   │   └── IronSword (具體)
│   ├── BaseRangedWeapon (抽象)
│   │   └── WoodenBow (具體)
│   └── MagicStaff (具體)
└── BaseConsumable (抽象)
    └── HealthPotion (具體)

Coin (獨立，無繼承)
```

**測試要點：**

- 標籤合併：子定義的標籤會與父定義的標籤合併
- 複雜物件繼承：`weaponProps` 的部分屬性繼承，部分覆蓋
- 多層繼承：`IronSword` 通過 `BaseMeleeWeapon` 間接繼承 `BaseWeapon` 和 `BaseItem`
- 堆疊數量繼承和覆蓋

## 預期結果

運行繼承處理器後，應該看到：

1. **日誌輸出**：

   - 註冊了 `CharacterDefInheritor` 和 `ThingDefInheritor`
   - 處理了多個定義的繼承關係
   - 只有具體定義（非抽象）出現在最終結果中

2. **最終定義數量**：

   - CharacterDef: 4 個具體定義 (Knight, Wizard, Archer, Peasant)
   - ThingDef: 5 個具體定義 (IronSword, WoodenBow, MagicStaff, HealthPotion, Coin)

3. **繼承效果驗證**：
   - 檢查 `Knight` 的 health 是否為 150 (從 BaseWarrior 繼承)
   - 檢查 `IronSword` 的標籤是否包含 "weapon", "equipment", "melee", "metal"
   - 檢查 `WoodenBow` 的 weaponProps.type 是否為 "Ranged" (繼承)

## 使用方式

```csharp
// 在模組管理器中，繼承處理器會自動處理這些定義
var inheritor = new ModDefinitionInheritor();
inheritor.RegisterInheritors();
var processedDefinitions = inheritor.ProcessInheritance(rawDefinitions);

// 驗證結果
var knight = DefinitionDatabase.GetDefinition<CharacterDef>("Knight");
Debug.Log($"Knight health: {knight.health}"); // 應該是 150

var ironSword = DefinitionDatabase.GetDefinition<ThingDef>("IronSword");
Debug.Log($"IronSword tags: {string.Join(", ", ironSword.tags)}");
```
