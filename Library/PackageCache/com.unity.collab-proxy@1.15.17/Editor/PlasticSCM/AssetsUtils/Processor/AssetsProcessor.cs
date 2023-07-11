namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    internal static class AssetsProcessors
    {
        internal static void Enable()
        {
            PlasticAssetsProcessor.RegisterPlasticAPI(PlasticApp.PlasticAPI);
            AssetModificationProcessor.RegisterAssetStatusCache(PlasticPlugin.AssetStatusCache);

            AssetPostprocessor.IsEnabled = true;
            AssetModificationProcessor.IsEnabled = true;
        }

        internal static void Disable()
        {
            AssetPostprocessor.IsEnabled = false;
            AssetModificationProcessor.IsEnabled = false;
        }
    }
}
