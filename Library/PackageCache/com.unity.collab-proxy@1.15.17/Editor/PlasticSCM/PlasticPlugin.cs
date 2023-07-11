using System;

using UnityEngine;
using UnityEditor;

using Codice.CM.Common;
using Unity.PlasticSCM.Editor.AssetMenu;
using Unity.PlasticSCM.Editor.AssetUtils.Processor;
using Unity.PlasticSCM.Editor.AssetsOverlays;
using Unity.PlasticSCM.Editor.AssetsOverlays.Cache;
using Unity.PlasticSCM.Editor.CollabMigration;
using Unity.PlasticSCM.Editor.Inspector;
using Unity.PlasticSCM.Editor.ProjectDownloader;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.SceneView;

namespace Unity.PlasticSCM.Editor
{
    /// <summary>
    /// The Plastic SCM plugin for Unity editor.
    /// </summary>
    [InitializeOnLoad]
    public static class PlasticPlugin
    {
        /// <summary>
        /// Invoked when notification status changed.
        /// </summary>
        public static event Action OnNotificationUpdated = delegate { };

        internal static IAssetStatusCache AssetStatusCache { get; private set; }

        static PlasticPlugin()
        {
            CloudProjectDownloader.Initialize();
            MigrateCollabProject.Initialize();
            EditorDispatcher.Initialize();

            CooldownWindowDelayer cooldownInitializeAction = new CooldownWindowDelayer(
                Enable, UnityConstants.PLUGIN_DELAYED_INITIALIZE_INTERVAL);
            cooldownInitializeAction.Ping();
        }

        /// <summary>
        /// Get the plugin icon.
        /// </summary>
        public static Texture GetPluginIcon()
        {
            return PlasticNotification.GetIcon(sNotificationStatus);
        }

        internal static void Enable()
        {
            if (sIsEnabled)
                return;

            sIsEnabled = true;

            PlasticApp.InitializeIfNeeded();

            if (!FindWorkspace.HasWorkspace(Application.dataPath))
                return;

            EnableForWorkspace();
        }

        internal static void EnableForWorkspace()
        {
            if (sIsEnabledForWorkspace)
                return;

            WorkspaceInfo wkInfo = FindWorkspace.InfoForApplicationPath(
                Application.dataPath,
                PlasticApp.PlasticAPI);

            if (wkInfo == null)
                return;

            sIsEnabledForWorkspace = true;

            PlasticApp.SetWorkspace(wkInfo);

            AssetStatusCache = new AssetStatusCache(
                wkInfo,
                PlasticApp.PlasticAPI.IsGluonWorkspace(wkInfo));

            AssetMenuItems.Enable();
            AssetsProcessors.Enable();
            DrawAssetOverlay.Enable();
            DrawInspectorOperations.Enable();
            DrawSceneOperations.Enable();
        }

        internal static void Disable()
        {
            try
            {
                PlasticApp.Dispose();

                if (!sIsEnabledForWorkspace)
                    return;

                AssetsProcessors.Disable();
                AssetMenuItems.Disable();
                DrawAssetOverlay.Disable();
                DrawInspectorOperations.Disable();
                DrawSceneOperations.Disable();
            }
            finally
            {
                sIsEnabled = false;
                sIsEnabledForWorkspace = false;
            }
        }

        internal static void SetNotificationStatus(
            PlasticWindow plasticWindow,
            PlasticNotification.Status status)
        {
            sNotificationStatus = status;

            plasticWindow.SetupWindowTitle(status);

            if (OnNotificationUpdated!=null) OnNotificationUpdated.Invoke();
        }

        static PlasticNotification.Status sNotificationStatus;

        static bool sIsEnabled;
        static bool sIsEnabledForWorkspace;
    }
}
