## 命名規則

### About.xml

<ModMetaData>
    <id>AuthorA.ModA</id> ← 模組 ID，格式為 `{作者名}.{模組名}` 不得有空白
    <name>Mod A</name>
    <description>This is Mod A</description>
    <author>AuthorA</author>  不得有空白
</ModMetaData>

### IModEntry

模組組程式入口為 {模組名}Entry，需實現 `IModEntry` 介面，命名空間為作者名稱，需要完全一致。

```csharp
namespace AuthorA
{
    public class ModAEntry : IModEntry
    {
        public void Initialize() { }

        public void OnGameStart() { }
    }
}

```

## 模組資料夾結構

```bash
Mods/
 └─ MyModFolder/              ← 模組根目錄 (名稱隨意)
     ├─ About/               ← 模組資訊（必需）
     │   ├─ About.xml        ← 模組描述與定義檔案
     │   └─ Preview.png      ← 模組預覽圖片
     ├─ Assemblies/          ← 編譯後的 DLL 程式碼
     ├─ Custom/              ← 自定義的資源（可選）
     ├─ Defs/                ← 定義(Def)檔案（XML 等）
     ├─ Sounds/              ← 聲音資源
     ├─ Textures/            ← 圖片資源
     ├─ Patches/             ← Def 修改檔(XML Patch，可選)
     └─ Languages/           ← 語言翻譯檔（可選
```

## Assemblies

### Unity

C:\Program Files\Unity\Hub\Editor
C:\Program Files\Unity\Hub\Editor\6000.0.34f1\Editor\Data\Managed

Assembly-CSharp.dll
Assembly-CSharp-firstpass.dll
UnityEngine.dll
UnityEngine.CoreModule.dll
UnityEngine.IMGUIModule.dll

### Project

P:\MainProjects\moddable-architecture-study\Library\ScriptAssemblies\Assembly-CSharp.dll
P:\MainProjects\moddable-architecture-study\Library\ScriptAssemblies\ModInfrastructure.Core.dll
P:\MainProjects\moddable-architecture-study\Library\ScriptAssemblies\ModInfrastructure.Unity.dll
P:\MainProjects\moddable-architecture-study\Library\ScriptAssemblies\Implement.Core.dll

## 未完成項目

- [ ] UnitTest

### 第一章

- [ ] Definition 的依賴
- [ ] Definition 的繼承
- [ ] About.xml 的格式驗證 (ID 格式, 是否重複 ID)

### 第二章

- [ ] 兼容 Lua 方案
- [ ] async Load 的方案
- [ ] lazy load 的方案
- [ ] addressable 的方案
- [ ] 修正 async load 未正確載入

### 待學

- [ ] 怎麼擴展 class 內容?
- [ ] 怎麼擴展 enum ?

## 參考資料

https://github.com/pardeike/Harmony/releases

### 其他模組框架

- [BepInEx]

  - https://www.bilibili.com/read/readlist/rl355806?spm_id_from=333.1369.opus.module_collection.click

- [UMM]
