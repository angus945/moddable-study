using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ReloadDomainInEditor
{
    private static FileSystemWatcher fileWatcher;
    private static Dictionary<string, DateTime> lastWriteTimes = new Dictionary<string, DateTime>();
    private static bool reloadPending = false;

    static ReloadDomainInEditor()
    {
        InitializeFileWatcher();
        EditorApplication.update += CheckForReload;
    }

    [MenuItem("Reload/Reload Domain")]
    static void Reload()
    {
        EditorUtility.RequestScriptReload();
    }

    private static void InitializeFileWatcher()
    {
        string streamingAssetsPath = Path.Combine(Application.dataPath, "StreamingAssets", "Mods");

        if (!Directory.Exists(streamingAssetsPath))
        {
            Debug.LogWarning($"Mods directory not found: {streamingAssetsPath}");
            return;
        }

        try
        {
            fileWatcher = new FileSystemWatcher(streamingAssetsPath, "*.dll")
            {
                IncludeSubdirectories = true,
                EnableRaisingEvents = true
            };

            fileWatcher.Changed += OnAssemblyChanged;
            fileWatcher.Created += OnAssemblyChanged;
            fileWatcher.Deleted += OnAssemblyChanged;
            fileWatcher.Renamed += OnAssemblyRenamed;

            Debug.Log($"File watcher initialized for mod assemblies: {streamingAssetsPath}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Failed to initialize file watcher: {ex.Message}");
        }
    }

    private static void OnAssemblyChanged(object sender, FileSystemEventArgs e)
    {
        // 確保是在 Assemblies 資料夾中的 .dll 檔案
        if (!e.FullPath.Contains("Assemblies"))
            return;

        string filePath = e.FullPath;
        DateTime currentTime = DateTime.Now;

        // 防止重複觸發（檔案系統事件可能會觸發多次）
        if (lastWriteTimes.ContainsKey(filePath))
        {
            if ((currentTime - lastWriteTimes[filePath]).TotalMilliseconds < 1000)
                return;
        }

        lastWriteTimes[filePath] = currentTime;

        Debug.Log($"Mod assembly changed: {Path.GetFileName(filePath)} ({e.ChangeType})");
        ScheduleReload();
    }

    private static void OnAssemblyRenamed(object sender, RenamedEventArgs e)
    {
        if (!e.FullPath.Contains("Assemblies"))
            return;

        Debug.Log($"Mod assembly renamed: {Path.GetFileName(e.OldName)} -> {Path.GetFileName(e.Name)}");
        ScheduleReload();
    }

    private static void ScheduleReload()
    {
        if (!reloadPending)
        {
            reloadPending = true;
            Debug.Log("Domain reload scheduled due to mod assembly changes...");
        }
    }

    private static void CheckForReload()
    {
        if (reloadPending && !EditorApplication.isCompiling && !EditorApplication.isUpdating)
        {
            reloadPending = false;
            Debug.Log("Reloading domain due to mod assembly changes...");
            EditorUtility.RequestScriptReload();
        }
    }

    // 清理資源
    [InitializeOnLoadMethod]
    private static void OnEditorShutdown()
    {
        EditorApplication.quitting += () =>
        {
            if (fileWatcher != null)
            {
                fileWatcher.Dispose();
                fileWatcher = null;
            }
        };
    }
}
