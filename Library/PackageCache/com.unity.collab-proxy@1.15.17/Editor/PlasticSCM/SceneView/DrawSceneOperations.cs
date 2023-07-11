using System.Collections.Generic;

using UnityEngine;
using UnityEditor.VersionControl;
using UnityEditor;

using Codice.Client.Common;
using GluonGui;
using PlasticGui;
using PlasticGui.WorkspaceWindow;
using PlasticGui.WorkspaceWindow.Items;
using Unity.PlasticSCM.Editor.AssetUtils;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.AssetMenu;

using GluonCheckoutOperation = GluonGui.WorkspaceWindow.Views.WorkspaceExplorer.Explorer.Operations.CheckoutOperation;

namespace Unity.PlasticSCM.Editor.SceneView
{
    static class DrawSceneOperations
    {
        internal static void Enable()
        {
            if (sIsEnabled)
                return;

            sIsEnabled = true;

            Provider.preCheckoutCallback += Provider_preCheckoutCallback;
        }

        internal static void Disable()
        {
            sIsEnabled = false;

            Provider.preCheckoutCallback -= Provider_preCheckoutCallback;
        }

        internal static void Initialize(
            WorkspaceWindow workspaceWindow,
            ViewHost viewHost,
            NewIncomingChangesUpdater incomingChangesUpdater,
            bool isGluonMode)
        {
            if (!sIsEnabled)
                Enable();

            sWorkspaceWindow = workspaceWindow;
            sViewHost = viewHost;
            sNewIncomingChangesUpdater = incomingChangesUpdater;
            sIsGluonMode = isGluonMode;

            sGuiMessage = new UnityPlasticGuiMessage();
            sProgressControls = new EditorProgressControls(sGuiMessage);
        }

        static bool Provider_preCheckoutCallback(
            AssetList list,
            ref string changesetID,
            ref string changesetDescription)
        {
            if (!sIsEnabled)
                return true;

            if (!FindWorkspace.HasWorkspace(Application.dataPath))
            { 
                Disable();
                return true;
            }

            if (sWorkspaceWindow == null)
                EditorWindow.GetWindow<PlasticWindow>();

            List<string> selectedPaths = GetSelectedPaths.ForOperation(
                list,
                PlasticPlugin.AssetStatusCache,
                AssetMenuOperations.Checkout);

            if (sIsGluonMode)
            {
                GluonCheckoutOperation.Checkout(
                    sViewHost,
                    sProgressControls,
                    sGuiMessage,
                    selectedPaths.ToArray(),
                    false,
                    RefreshAsset.VersionControlCache);

                return true;
            }

            CheckoutOperation.Checkout(
                sWorkspaceWindow,
                null,
                sProgressControls,
                selectedPaths,
                sNewIncomingChangesUpdater,
                RefreshAsset.VersionControlCache);

            return true;
        }

        static bool sIsEnabled;

        static bool sIsGluonMode;
        static ViewHost sViewHost;
        static GuiMessage.IGuiMessage sGuiMessage;
        static EditorProgressControls sProgressControls;
        static IWorkspaceWindow sWorkspaceWindow;
        static NewIncomingChangesUpdater sNewIncomingChangesUpdater;
    }
}
