using UnityEditor;
using UnityEngine.UIElements;

namespace Unity.Services.Core.Editor.ProjectBindRedirect
{
    class ProjectBindRedirectContentUI
    {
        public ProjectBindRedirectContentUI(VisualElement parentElement)
        {
            SetupUxml(parentElement);
        }

        static void SetupUxml(VisualElement containerElement)
        {
            var visualAsset = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(ProjectBindRedirectUiConstants.UxmlPath.Content);
            if (visualAsset != null)
            {
                visualAsset.CloneTree(containerElement);
            }
        }
    }
}
