using UnityEditor;

using Codice.Client.Common;
using Codice.Client.Common.Connection;
using PlasticGui;
using Unity.PlasticSCM.Editor.UI;
using Unity.PlasticSCM.Editor.Configuration.CloudEdition.Welcome;

namespace Unity.PlasticSCM.Editor.Configuration
{
    internal class CredentialsUiImpl : AskCredentialsToUser.IGui
    {
        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForCredentials(string servername)
        {
            AskCredentialsToUser.DialogData result = null;

            GUIActionRunner.RunGUIAction(delegate
            {
                result = CredentialsDialog.RequestCredentials(
                        servername, ParentWindow.Get());
            });

            return result;
        }

        void AskCredentialsToUser.IGui.ShowSaveProfileErrorMessage(string message)
        {
            GUIActionRunner.RunGUIAction(delegate
            {
                GuiMessage.ShowError(string.Format(
                    PlasticLocalization.GetString(
                        PlasticLocalization.Name.CredentialsErrorSavingProfile),
                    message));
            });
        }

        AskCredentialsToUser.DialogData AskCredentialsToUser.IGui.AskUserForSSOCredentials(string cloudServer)
        {
            AskCredentialsToUser.DialogData result = null;

            // Check SSO auto login here
            GUIActionRunner.RunGUIAction(delegate
            {
                result = RunCredentialsRequest(cloudServer);
            });

            return result;
        }

        private AskCredentialsToUser.DialogData RunCredentialsRequest(string cloudServer)
        {
            AutoLogin autoLogin = new AutoLogin();
            var response = autoLogin.Run();

            if (response != ResponseType.None)
            {
                return autoLogin.BuildCredentialsDialogData(response);
            }
            else
            {
                return SSOCredentialsDialog.RequestCredentials(cloudServer, ParentWindow.Get());
            }
        }
    }
}
