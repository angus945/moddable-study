using UnityEditor;
using UnityEngine;

public class OpenFileExplorer
{
    [MenuItem("File Explorer/Mods Settings Directory")]
    public static void OpenModsSettingsDirectory()
    {
        string path = $"{Application.persistentDataPath}/ModSettings/";

        if (!System.IO.Directory.Exists(path))
        {
            Debug.LogWarning($"Directory does not exist: {path}");
            return;
        }

        EditorUtility.RevealInFinder(path);
        Debug.Log($"Opened file explorer at: {path}");
    }


}