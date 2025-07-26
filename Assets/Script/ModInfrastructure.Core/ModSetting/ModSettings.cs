using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using ModArchitecture.Logger;
using ModArchitecture.Utils;

namespace ModArchitecture
{
    public class ModSettings
    {
        Dictionary<string, ModSettingContext> contextMap = new Dictionary<string, ModSettingContext>();
        Dictionary<string, IModeSettings> settingsMap = new Dictionary<string, IModeSettings>();
        string savePath;

        public ModSettings(string savePath)
        {
            this.savePath = savePath;
        }
        public void RegisterDeserializers()
        {
            var deserializerTypes = ReflectionUtils.GetTypesAssignableFrom<IModeSettings>();

            foreach (var type in deserializerTypes)
            {
                IModeSettings settings = ReflectionUtils.SafeCreateInstance<IModeSettings>(type);
                if (settings != null)
                {
                    // 從設定類別名稱和命名空間推導完整的 modId
                    string settingsClassName = type.Name;
                    string namespaceName = type.Namespace;

                    if (settingsClassName.EndsWith("Settings"))
                    {
                        // 從類別名稱取得模組名稱 (移除 Settings 後綴)
                        string modName = settingsClassName.Substring(0, settingsClassName.Length - "Settings".Length);

                        // 組合完整的 modId: {命名空間}.{模組名稱}
                        string modId = $"{namespaceName}.{modName}";

                        if (!settingsMap.ContainsKey(modId))
                        {
                            settingsMap.Add(modId, settings);
                            ModLogger.Log($"Registered settings: {type.Name} for modId: {modId}");
                        }
                        else
                        {
                            ModLogger.LogWarning($"Settings for modId '{modId}' already registered. Skipping {type.Name}");
                        }
                    }
                    else
                    {
                        ModLogger.LogWarning($"Settings class {type.Name} does not follow naming convention (should end with 'Settings')");
                    }
                }
            }
        }
        public void LoadExistingSettings()
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string[] saveFiles = Directory.GetFiles(savePath, $"*.xml");
            foreach (var file in saveFiles)
            {
                XmlDocument doc = new XmlDocument();

                try
                {
                    doc.Load(file);
                }
                catch (XmlException ex)
                {
                    ModLogger.LogError($"Failed to load settings from {file}: {ex.Message}");
                    continue;
                }

                string modId = Path.GetFileNameWithoutExtension(file);
                ModSettingContext context = new ModSettingContext(modId, doc);
                contextMap[context.modId] = context;

                ModLogger.Log($"Loaded settings for mod: {context.modId} from {file}");
            }


        }
        public void ReadSettings(string modId)
        {
            if (!contextMap.TryGetValue(modId, out ModSettingContext context))
            {
                context = CreateSettingContext(modId);
            }

            if (!settingsMap.TryGetValue(modId, out IModeSettings settings))
            {
                ModLogger.LogError($"No settings registered for modId: {modId}");
                return;
            }

            settings.Load(context);
        }
        public void WriteSettings(string modId)
        {
            if (!contextMap.TryGetValue(modId, out ModSettingContext context))
            {
                context = CreateSettingContext(modId);
            }

            if (!settingsMap.TryGetValue(modId, out IModeSettings settings))
            {
                ModLogger.LogError($"No settings registered for modId: {modId}");
                return;
            }

            settings.Save(context);

            // Save the XML document to file
            string filePath = Path.Combine(savePath, $"{modId}.xml");
            context.doc.Save(filePath);
        }

        ModSettingContext CreateSettingContext(string modId)
        {
            XmlDocument doc = new XmlDocument();
            XmlElement root = doc.CreateElement("ModSettings");
            doc.AppendChild(root);

            ModSettingContext context = new ModSettingContext(modId, doc);
            contextMap[modId] = context;

            return context;
        }

        public Dictionary<string, IModeSettings> GetSettings()
        {
            return settingsMap;
        }
    }
}

