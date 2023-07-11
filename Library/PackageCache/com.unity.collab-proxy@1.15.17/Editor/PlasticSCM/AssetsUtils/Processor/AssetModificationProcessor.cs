using System.IO;

using UnityEditor;

using AssetOverlays = Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;

namespace Unity.PlasticSCM.Editor.AssetUtils.Processor
{
    class AssetModificationProcessor : UnityEditor.AssetModificationProcessor
    {
        internal static bool IsEnabled { get; set; }
        internal static bool ForceCheckout { get; private set; }

        /*We need to do a checkout, verifying that the content/date or size has changed. In order
          to do this checkout we need the changes to have reached the disk. That's why we save the
          changed files in this array, and when they are reloaded in 
        AssetPostprocessor.OnPostprocessAllAssets we process them. */
        internal static string[] ModifiedAssets { get; set; }

        static AssetModificationProcessor()
        {
            ForceCheckout = EditorPrefs.GetBool("forceCheckoutPlasticSCM");
        }

        internal static void RegisterAssetStatusCache(
            IAssetStatusCache assetStatusCache)
        {
            mAssetStatusCache = assetStatusCache;
        }

        internal static void SetForceCheckoutOption(bool isEnabled)
        {
            ForceCheckout = isEnabled;
            EditorPrefs.SetBool(
                "forceCheckoutPlasticSCM",
                isEnabled);
        }

        static string[] OnWillSaveAssets(string[] paths)
        {
            if (!IsEnabled)
                return paths;

            ModifiedAssets = (string[])paths.Clone();

            return paths;
        }

        static bool IsOpenForEdit(string assetPath, out string message)
        {
            message = string.Empty;

            if (!IsEnabled)
                return true;

            if (assetPath.StartsWith("ProjectSettings/"))
                return true;

            if (!ForceCheckout)
                return true;

            if (MetaPath.IsMetaPath(assetPath))
                assetPath = MetaPath.GetPathFromMetaPath(assetPath);

            AssetOverlays.AssetStatus status = mAssetStatusCache.GetStatusForPath(
                Path.GetFullPath(assetPath));

            if (AssetOverlays.ClassifyAssetStatus.IsAdded(status) ||
                AssetOverlays.ClassifyAssetStatus.IsCheckedOut(status))
                return true;

            return !AssetOverlays.ClassifyAssetStatus.IsControlled(status);
        }

        static IAssetStatusCache mAssetStatusCache;
    }
}
