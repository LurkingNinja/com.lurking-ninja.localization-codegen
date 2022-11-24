using UnityEditor;
using UnityEngine;
using UnityEngine.Localization.Tables;
using Object = UnityEngine.Object;

namespace LurkingNinja.Localization.Editor
{
    // To detect creation and saving an asset. We do not care about moving.
    public class OnAssetPostProcessorLocalization : AssetPostprocessor
    {
        private static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets,
            string[] movedAssets, string[] movedFromAssetPaths)
        {
            if (!LocalizationCodegenSettings.Get.isEnabled) return;
            foreach (var path in importedAssets)
            {
                if (AssetDatabase.LoadAssetAtPath<Object>(path) is SharedTableData tableData)
                    SharedTableDataPostProcess.GenerateFile(tableData, path);
                
            }
        }
    }

    // To detect asset removal.
    public class CustomAssetModificationProcessor : AssetModificationProcessor
    {
        private static AssetDeleteResult OnWillDeleteAsset(string path, RemoveAssetOptions rao)
        {
            if (AssetDatabase.LoadAssetAtPath<Object>(path) is SharedTableData)
                SharedTableDataPostProcess.DeleteFile(path);
            return AssetDeleteResult.DidNotDelete;
        }
    }
}
