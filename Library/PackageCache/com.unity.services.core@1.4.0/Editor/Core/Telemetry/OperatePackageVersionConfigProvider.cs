using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Services.Core.Configuration.Editor;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Telemetry.Internal;
using UnityEditor;
using UnityEditor.Build;
using PackageInfo = UnityEditor.PackageManager.PackageInfo;

namespace Unity.Services.Core.Editor
{
    [InitializeOnLoad]
    class OperatePackageVersionConfigProvider : IConfigurationProvider
    {
        static readonly OperatePackageVersionConfigProvider k_EditorInstance;

        static OperatePackageVersionConfigProvider()
        {
            k_EditorInstance = new OperatePackageVersionConfigProvider
            {
                m_OperatePackages = FindAllOperatePackageInProject()
            };
        }

        IEnumerable<PackageConfig> m_OperatePackages;

        int IOrderedCallback.callbackOrder { get; }

        void IConfigurationProvider.OnBuildingConfiguration(ConfigurationBuilder builder)
        {
            var operatePackages = BuildPipeline.isBuildingPlayer
                ? FindAllOperatePackageInProject()
                : k_EditorInstance.m_OperatePackages;

            ProvidePackageConfigs(builder, operatePackages);
        }

        static IEnumerable<PackageConfig> FindAllOperatePackageInProject()
        {
            var allOperatePackagesInProject = TypeCache.GetTypesDerivedFrom<IInitializablePackage>()
                .Select(GetPackageConfigFrom)
                .Where(config => !string.IsNullOrEmpty(config.Name))
                .Distinct()
                .ToList();
            return allOperatePackagesInProject;

            PackageConfig GetPackageConfigFrom(Type type)
            {
                var packageInfo = PackageInfo.FindForAssembly(type.Assembly);
                return new PackageConfig(packageInfo);
            }
        }

        internal static void ProvidePackageConfigs(ConfigurationBuilder builder, IEnumerable<PackageConfig> operatePackages)
        {
            foreach (var packageInfo in operatePackages)
            {
                var configKey = string.Format(FactoryUtils.PackageVersionKeyFormat, packageInfo.Name);
                builder.SetString(configKey, packageInfo.Version, true);
            }
        }
    }
}
