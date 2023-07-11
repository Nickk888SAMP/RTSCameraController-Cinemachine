using UnityEditor;
using UnityEngine;

using Codice.CM.Common;
using PlasticGui;
using PlasticGui.Gluon;
using PlasticGui.WorkspaceWindow.Topbar;
using PlasticGui.WorkspaceWindow.PendingChanges;

using GluonShowIncomingChanges = PlasticGui.Gluon.WorkspaceWindow.ShowIncomingChanges;

namespace Unity.PlasticSCM.Editor.UI
{
    internal class IncomingChangesNotification
    {
        internal string InfoText;
        internal string ActionText;
        internal string TooltipText;
        internal bool HasUpdateAction;
        internal PlasticNotification.Status Status;

        internal void Clear()
        {
            InfoText = string.Empty;
            ActionText = string.Empty;
            TooltipText = string.Empty;
            HasUpdateAction = false;
            Status = PlasticNotification.Status.None;
        }
    }

    interface IIncomingChangesNotifier
    {
        bool HasNotification { get; }
        IncomingChangesNotification Notification { get; }
    }

    internal class StatusBar
    {
        internal StatusBar()
        {
            mCooldownNotificationClearAction = new CooldownWindowDelayer(
                DelayedClearNotification,
                UnityConstants.NOTIFICATION_CLEAR_INTERVAL);
        }

        internal void Notify(string message, MessageType type, Texture2D image)
        {
            mNotification = new Notification(message, type, image);
            mCooldownNotificationClearAction.Ping();
        }

        internal void OnGUI(
            WorkspaceInfo wkInfo,
            WorkspaceWindow workspaceWindow,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher,
            IIncomingChangesNotifier incomingChangesNotifier,
            bool isGluonMode)
        {
            EditorGUILayout.BeginVertical(
                GetBarStyle(),
                GUILayout.Height(UnityConstants.STATUS_BAR_HEIGHT));
            GUILayout.FlexibleSpace();
            EditorGUILayout.BeginHorizontal();

            if (incomingChangesNotifier.HasNotification)
            {
                DrawIncomingChangesNotification(
                    wkInfo,
                    workspaceWindow,
                    mergeViewLauncher,
                    gluonViewSwitcher,
                    incomingChangesNotifier.Notification,
                    isGluonMode);
            }

            if (mNotification != null)
                DrawNotification(mNotification);

            GUILayout.FlexibleSpace();

            DrawWorkspaceStatus(workspaceWindow.WorkspaceStatus);

            EditorGUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndVertical();
        }

        static void DrawIncomingChangesNotification(
            WorkspaceInfo wkInfo,
            WorkspaceWindow workspaceWindow,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonViewSwitcher,
            IncomingChangesNotification notification,
            bool isGluonMode)
        {
            Texture2D icon = notification.Status == PlasticNotification.Status.Conflicts ?
                Images.GetConflictedIcon() :
                Images.GetOutOfSyncIcon();

            DrawIcon(icon);

            DrawNotificationLabel(notification.InfoText);

            if (DrawButton(notification.ActionText, notification.TooltipText))
            {
                if (notification.HasUpdateAction)
                {
                    workspaceWindow.UpdateWorkspace();
                    return;
                }

                ShowIncomingChangesForMode(
                    wkInfo,
                    mergeViewLauncher,
                    gluonViewSwitcher,
                    isGluonMode);
            }
        }

        static void DrawNotification(Notification notification)
        {
            DrawIcon(notification.Image);
            DrawNotificationLabel(notification.Message);
        }

        static void DrawWorkspaceStatus(WorkspaceStatusString.Data status)
        {
            DrawIcon(Images.GetBranchIcon());

            if (status != null)
            {
                DrawLabel(string.Format(
                    "{0}@{1}@{2}",
                    status.ObjectSpec,
                    status.RepositoryName,
                    status.Server));
            }
        }

        static void DrawIcon(Texture2D icon)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                icon,
                UnityStyles.StatusBar.Icon,
                GUILayout.Height(UnityConstants.STATUS_BAR_ICON_SIZE),
                GUILayout.Width(UnityConstants.STATUS_BAR_ICON_SIZE));

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static void DrawLabel(string label)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                label,
                UnityStyles.StatusBar.Label);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static void DrawNotificationLabel(string label)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            GUILayout.Label(
                label,
                UnityStyles.StatusBar.NotificationLabel);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        static bool DrawButton(string label, string tooltip)
        {
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            bool buttonClicked = GUILayout.Button(
                new GUIContent(label, tooltip),
                UnityStyles.StatusBar.Button);

            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();

            return buttonClicked;
        }

        static void ShowIncomingChangesForMode(
            WorkspaceInfo workspaceInfo,
            IMergeViewLauncher mergeViewLauncher,
            IGluonViewSwitcher gluonSwitcher,
            bool isGluonMode)
        {
            if (isGluonMode)
            {
                GluonShowIncomingChanges.FromNotificationBar(
                    workspaceInfo, gluonSwitcher);
                return;
            }

            ShowIncomingChanges.FromNotificationBar(
                workspaceInfo, mergeViewLauncher);
        }

        void DelayedClearNotification()
        {
            mNotification = null;
        }

        GUIStyle GetBarStyle()
        {
            if (mBarTexture == null)
                mBarTexture = new Texture2D(1, 1);

            if (mBarStyle == null)
                mBarStyle = new GUIStyle();

            mBarTexture.SetPixel(0, 0, UnityStyles.Colors.BackgroundBar);
            mBarTexture.Apply();
            mBarStyle.normal.background = mBarTexture;

            return mBarStyle;
        }

        class Notification
        {
            internal string Message { get; private set; }
            internal MessageType MessageType { get; private set; }
            internal Texture2D Image { get; private set; }

            internal Notification(string message, MessageType messageType, Texture2D image)
            {
                Message = message;
                MessageType = messageType;
                Image = image;
            }
        }

        Texture2D mBarTexture;
        GUIStyle mBarStyle;

        Notification mNotification;

        readonly CooldownWindowDelayer mCooldownNotificationClearAction;
    }
}