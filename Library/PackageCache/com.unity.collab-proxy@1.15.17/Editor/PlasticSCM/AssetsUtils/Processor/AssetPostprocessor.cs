using Codice.Client.Common.FsNodeReaders.Watcher;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    class AssetPostprocessor : UnityEditor.AssetPostprocessor
    {
        internal static bool IsEnabled { get; set; }

        static void OnPostprocessAllAssets(
            string[] importedAssets,
            string[] deletedAssets,
            string[] movedAssets,
            string[] movedFromAssetPaths)
        {
            if (!IsEnabled)
                return;

            // We need to ensure that the FSWatcher is enabled before processing Plastic operations
            // It fixes the following scenario: 
            // 1. Close PlasticSCM window
            // 2. Create an asset, it appears with the added overlay
            // 3. Open PlasticSCM window, the asset should appear as added instead of deleted locally
            MonoFileSystemWatcher.IsEnabled = true;

            for (int i = 0; i < movedAssets.Length; i++)
            {
                PlasticAssetsProcessor.MoveOnSourceControl(
                    movedFromAssetPaths[i],
                    movedAssets[i]);
            }

            foreach (string deletedAsset in deletedAssets)
            {
                PlasticAssetsProcessor.DeleteFromSourceControl(
                    deletedAsset);
            }

            PlasticAssetsProcessor.AddToSourceControl(importedAssets);

            if (AssetModificationProcessor.ModifiedAssets == null)
                return;

            PlasticAssetsProcessor.CheckoutOnSourceControl(AssetModificationProcessor.ModifiedAssets);
            AssetModificationProcessor.ModifiedAssets = null;
        }
    }
}
