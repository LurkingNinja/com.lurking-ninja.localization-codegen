/***
 * Localization Codegen
 * Copyright (c) 2022-2024 Lurking Ninja.
 *
 * MIT License
 * https://github.com/LurkingNinja/com.lurking-ninja.localization-codegen
 */
#if LN_CODEGEN_PRESENT && LN_LOCALIZATION_INSTALLED
using System;
using System.Linq;
using System.Text;
using LurkingNinja.CodeGen.Editor;
using UnityEditor;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;
using Object = UnityEngine.Object;

namespace LurkingNinja.Localization.Editor
{
    [InitializeOnLoad]
    public class OnAssetPostProcessorLocalization : AssetPostprocessor
    {
        static OnAssetPostProcessorLocalization()
        {
            AssemblyReloadEvents.beforeAssemblyReload += OnAssetPostProcessorLocalizationDestructor;
            OnAssetPostProcessor.AddListener(typeof(SharedTableData), ModifyCallback, DeleteCallback);
        }

        private static void OnAssetPostProcessorLocalizationDestructor()
        {
            AssemblyReloadEvents.beforeAssemblyReload -= OnAssetPostProcessorLocalizationDestructor;
            OnAssetPostProcessor.RemoveListener(
                typeof(SharedTableData), ModifyCallback, DeleteCallback);

        }

        private static void ModifyCallback(Object obj, string path)
        {
            if (!LocalizationCodegenSettings.Get.isEnabled) return;
            AssetPostProcessorHelper.WriteFile(path, LocalizationCodegenSettings.Get.path,
                GenerateFileContent(obj as SharedTableData));
        }

        private static void DeleteCallback(Object obj, string path) =>
            AssetPostProcessorHelper.DeleteFile(path, LocalizationCodegenSettings.Get.path);

        private static bool IsSmart(StringTableCollection tableCollection, long id) => 
            tableCollection != null && tableCollection.StringTables
                .Select(stable => stable.GetEntry(id)).Any(tableEntry => tableEntry.IsSmart);

        private static string GenerateFileContent(SharedTableData table)
        {
            var tableCollection = LocalizationEditorSettings
                .GetStringTableCollection(table.TableCollectionNameGuid);
            var sb = new StringBuilder();

            foreach (var entry in table.Entries)
            {
                var key = AssetPostProcessorHelper.KeyToCSharp(entry.Key);
                sb.Append($"\t\t\tpublic static string {key}");
                var isSmart = IsSmart(tableCollection, entry.Id);
                if (isSmart) sb.Append("(List<object> o)");
                sb.Append($" => LocalizationSettings.StringDatabase.GetLocalizedString(NAME, {entry.Id}");
                if(isSmart) sb.Append(", o");
                sb.Append(");");
                sb.Append(Environment.NewLine);
            }

            var settings = LocalizationCodegenSettings.Get;
            var nameSpace = string.IsNullOrEmpty(settings.nameSpace) ? ""
                : $"namespace {settings.nameSpace}";
            return string.Format(settings.template,
                /*{0}*/DateTime.Now,
                /*{1}*/nameSpace,
                /*{2}*/AssetPostProcessorHelper.KeyToCSharp(table.TableCollectionName),
                /*{3}*/table.TableCollectionName,
                /*{4}*/sb);
        }
    }
}
#else
using UnityEditor;
using UnityEditor.PackageManager;

namespace LurkingNinja.Localization.Editor
{
    [InitializeOnLoad]
    public class OnAssetPostProcessorLocalizationInstall
    {
        static OnAssetPostProcessorLocalizationInstall()
        {
            Client.Add("com.unity.localization");
            Client.Add("https://github.com/LurkingNinja/com.lurking-ninja.codegen.git?path=Packages/com.lurking-ninja.codegen");
        }
    }
}
#endif