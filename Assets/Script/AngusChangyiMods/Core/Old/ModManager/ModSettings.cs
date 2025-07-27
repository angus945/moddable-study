using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using AngusChangyiMods.Core.Utils;

namespace AngusChangyiMods.Core
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
            ModLogger.Log("Starting to register mod settings deserializers...", "ModSettings");
            var deserializerTypes = ReflectionUtils.GetTypesAssignableFrom<IModeSettings>();
            int successCount = 0;

            foreach (var type in deserializerTypes)
            {
                IModeSettings settings = ReflectionUtils.SafeCreateInstance<IModeSettings>(type);
                if (settings != null)
                {
                    // Derive complete modId from settings class name and namespace
                    string settingsClassName = type.Name;
                    string namespaceName = type.Namespace;

                    if (settingsClassName.EndsWith("Settings"))
                    {
                        // Get mod name from class name (remove Settings suffix)
                        string modName = settingsClassName.Substring(0, settingsClassName.Length - "Settings".Length);

                        // Combine complete modId: {namespace}.{modName}
                        string modId = $"{namespaceName}.{modName}";

                        if (!settingsMap.ContainsKey(modId))
                        {
                            settingsMap.Add(modId, settings);
                            ModLogger.Log($"Registered settings successfully: {type.Name} for modId: {modId}", "ModSettings");
                            successCount++;
                        }
                        else
                        {
                            ModLogger.LogWarning($"Settings for modId '{modId}' already registered. Skipping {type.Name}", "ModSettings");
                        }
                    }
                    else
                    {
                        ModLogger.LogWarning($"Settings class {type.Name} does not follow naming convention (should end with 'Settings')", "ModSettings");
                    }
                }
            }

            ModLogger.Log($"Settings deserializers registration completed, success: {successCount}/{deserializerTypes.Count()}", "ModSettings");
        }
        public void LoadExistingSettings()
        {
            ModLogger.Log("Starting to load existing settings files...", "ModSettings");

            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
                ModLogger.Log($"Created settings directory: {savePath}", "ModSettings");
            }

            string[] saveFiles = Directory.GetFiles(savePath, $"*.xml");
            int successCount = 0;

            foreach (var file in saveFiles)
            {
                XmlDocument doc = new XmlDocument();

                try
                {
                    doc.Load(file);
                    string modId = Path.GetFileNameWithoutExtension(file);
                    ModSettingContext context = new ModSettingContext(modId, doc);
                    contextMap[context.modId] = context;

                    ModLogger.Log($"Loaded settings successfully for mod: {context.modId} from {Path.GetFileName(file)}", "ModSettings");
                    successCount++;
                }
                catch (XmlException ex)
                {
                    ModLogger.LogError($"Failed to load settings from {Path.GetFileName(file)}: {ex.Message}", "ModSettings");
                    continue;
                }
            }

            ModLogger.Log($"Existing settings loading completed, success: {successCount}/{saveFiles.Length} files", "ModSettings");
        }
        public void ReadSettings(string modId)
        {
            if (!contextMap.TryGetValue(modId, out ModSettingContext context))
            {
                context = CreateSettingContext(modId);
            }

            if (!settingsMap.TryGetValue(modId, out IModeSettings settings))
            {
                ModLogger.LogError($"No settings registered for modId: {modId}", "ModSettings");
                return;
            }

            try
            {
                settings.Load(context);
                ModLogger.Log($"Settings loaded successfully for mod: {modId}", "ModSettings");
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Failed to load settings for mod {modId}: {ex.Message}", "ModSettings");
            }
        }
        public void WriteSettings(string modId)
        {
            if (!contextMap.TryGetValue(modId, out ModSettingContext context))
            {
                context = CreateSettingContext(modId);
            }

            if (!settingsMap.TryGetValue(modId, out IModeSettings settings))
            {
                ModLogger.LogError($"No settings registered for modId: {modId}", "ModSettings");
                return;
            }

            try
            {
                settings.Save(context);

                // Save the XML document to file
                string filePath = Path.Combine(savePath, $"{modId}.xml");
                context.doc.Save(filePath);
                ModLogger.Log($"Settings saved successfully for mod: {modId}", "ModSettings");
            }
            catch (Exception ex)
            {
                ModLogger.LogError($"Failed to save settings for mod {modId}: {ex.Message}", "ModSettings");
            }
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

