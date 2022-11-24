using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace LurkingNinja.Localization.Editor
{
    internal static class AssetPostProcessorHelper
    {
        private static string GetFullPath(string fileName, string path) =>
            $"{Application.dataPath}/../{path}{fileName}.cs";

        private static string GetPath(string fileName, string path) =>
            $"{path}{fileName}.cs";

        internal static string KeyToCSharpWithoutAt(string key) => KeyToCSharp(key, false);
		
        internal static string KeyToCSharp(string key, bool addAt = true)
        {
            if(string.IsNullOrEmpty(key))
                throw new ArgumentOutOfRangeException(nameof(key), "Key cannot be empty or null.");
            if (char.IsNumber(key[0])) key = $"_{key}";
            key = key.Replace(" ", "_");
            return addAt ? $"@{key}" : key;
        }

        private static string GetFileName(string fileName) =>
            Path.GetFileNameWithoutExtension(fileName).Replace(" ", "_");

        internal static void DeleteFile(string fileName, string path) =>
            AssetDatabase.DeleteAsset(GetPath(GetFileName(fileName), path));

        internal static void WriteFile(string fileName, string path, string content)
        {
            fileName = GetFileName(fileName);
            var genPath = GetFullPath(fileName, path);
            var folderOnly = Path.GetDirectoryName(genPath);
            if (!Directory.Exists(folderOnly) && folderOnly != null)
                Directory.CreateDirectory(folderOnly);
            using var writer = new StreamWriter(genPath, false);
            writer.WriteLine(content);
            AssetDatabase.ImportAsset($"{path}{fileName}.cs");
        }
    }
}