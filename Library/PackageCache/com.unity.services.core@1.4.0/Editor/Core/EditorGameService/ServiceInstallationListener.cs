#if UNITY_2020_2_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core.Editor.ProjectBindRedirect;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Unity.Services.Core.Editor
{
    static class ServiceInstallationListener
    {
        static IEditorGameServiceAnalyticsSender s_EditorGameServiceAnalyticsSender;
        static IEditorGameServiceAnalyticsSender EditorGameServiceAnalyticsSender
        {
            get
            {
                if (s_EditorGameServiceAnalyticsSender == null)
                {
                    s_EditorGameServiceAnalyticsSender = new EditorGameServiceAnalyticsSender();
                }
                return s_EditorGameServiceAnalyticsSender;
            }
        }

        [InitializeOnLoadMethod]
        static void RegisterToEvents()
        {
            Events.registeredPackages -= OnPackagesRegistered;
            Events.registeredPackages += OnPackagesRegistered;
        }

        static void OnPackagesRegistered(PackageRegistrationEventArgs args)
        {
            if (args.added.Any())
            {
                OnPackagesAdded(args.added);
            }
        }

        static void OnPackagesAdded(IEnumerable<PackageInfo> packageInfos)
        {
            var newServices = GetNewServices(packageInfos);
            if (newServices.Any())
            {
                var request = new ProjectStateRequest();
                var projectState = request.GetProjectState();
                if (ShouldShowRedirect(projectState))
                {
                    List<string> installedPackages = new List<string>();
                    foreach (var service in newServices)
                    {
                        installedPackages.Add(service.Name);
                    }
                    ProjectBindRedirectPopupWindow.CreateAndShowPopup(installedPackages, EditorGameServiceAnalyticsSender);
                }
            }
        }

        internal static bool ShouldShowRedirect(ProjectState projectState)
        {
            return !projectState.ProjectBound || !projectState.IsLoggedIn;
        }

        static IEnumerable<IEditorGameService> GetNewServices(IEnumerable<PackageInfo> packageInfos)
        {
            var output = new HashSet<IEditorGameService>();
            var serviceTypes = TypeCache.GetTypesDerivedFrom<IEditorGameService>();
            foreach (var serviceType in serviceTypes)
            {
                if (IsTypeDefinedInPackages(serviceType, packageInfos))
                {
                    foreach (var kvp in EditorGameServiceRegistry.Instance.Services)
                    {
                        if (kvp.Value.GetType() == serviceType)
                        {
                            output.Add(kvp.Value);
                        }
                    }
                }
            }

            return output;
        }

        static bool IsTypeDefinedInPackages(Type type, IEnumerable<PackageInfo> packageInfos)
        {
            var output = false;
            foreach (var packageInfo in packageInfos)
            {
                if (IsTypeDefinedInPackage(type, packageInfo))
                {
                    output = true;
                    break;
                }
            }

            return output;
        }

        static bool IsTypeDefinedInPackage(Type type, PackageInfo packageInfo)
        {
            var packageInfoFromAssembly = PackageInfo.FindForAssembly(type.Assembly);

            return ArePackageInfosEqual(packageInfoFromAssembly, packageInfo);
        }

        static bool ArePackageInfosEqual(PackageInfo x, PackageInfo y)
        {
            return x != null && y != null && x.name == y.name;
        }
    }
}
#endif
