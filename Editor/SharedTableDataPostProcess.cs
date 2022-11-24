using System;
using System.Linq;
using System.Text;
using UnityEditor.Localization;
using UnityEngine.Localization.Tables;

namespace LurkingNinja.Localization.Editor
{
    internal static class SharedTableDataPostProcess
    {
		internal static void GenerateFile(SharedTableData table, string fileName) =>
            AssetPostProcessorHelper
                .WriteFile(fileName, LocalizationCodegenSettings.Get.path, GenerateFileContent(table));
	
        internal static void DeleteFile(string filename) =>
            AssetPostProcessorHelper.DeleteFile(filename, LocalizationCodegenSettings.Get.path);
        
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
