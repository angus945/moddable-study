
using System;

namespace ModArchitecture
{
    public class ModAssetLoaderUtils
    {
        public static string GetRelativePath(string folderPath, string file)
        {
            if (string.IsNullOrEmpty(folderPath) || string.IsNullOrEmpty(file))
                throw new ArgumentException("Folder path and file cannot be null or empty.");

            return file.Substring(folderPath.Length + 1).Replace("\\", "/");
        }
    }
}