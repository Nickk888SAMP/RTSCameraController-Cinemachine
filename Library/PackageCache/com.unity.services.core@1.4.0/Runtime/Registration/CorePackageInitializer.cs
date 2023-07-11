using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Unity.Services.Core.Configuration;
using Unity.Services.Core.Configuration.Internal;
using Unity.Services.Core.Device;
using Unity.Services.Core.Device.Internal;
using Unity.Services.Core.Environments;
using Unity.Services.Core.Environments.Internal;
using Unity.Services.Core.Internal;
using Unity.Services.Core.Scheduler.Internal;
using Unity.Services.Core.Telemetry.Internal;
using Unity.Services.Core.Threading.Internal;
using UnityEngine;
using NotNull = JetBrains.Annotations.NotNullAttribute;
using SuppressMessage = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

namespace Unity.Services.Core.Registration
{
    [SuppressMessage("ReSharper", "RedundantTypeArgumentsOfMethod")]
    class CorePackageInitializer : IInitializablePackage, IDiagnosticsComponentProvider
    {
        internal const string CorePackageName = "com.unity.services.core";

        internal ActionScheduler ActionScheduler { get; private set; }

        internal InstallationId InstallationId { get; private set; }

        internal ProjectConfiguration ProjectConfig { get; private set; }

        internal Environments.Internal.Environments Environments { get; private set; }

        internal CloudProjectId CloudProjectId { get; private set; }

        internal IDiagnosticsFactory DiagnosticsFactory { get; private set; }

        internal IMetricsFactory MetricsFactory { get; private set; }

        internal UnityThreadUtilsInternal UnityThreadUtils { get; private set; }

        InitializationOptions m_CurrentInitializationOptions;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Register()
        {
            var corePackageInitializer = new CorePackageInitializer();
            CoreDiagnostics.Instance.DiagnosticsComponentProvider = corePackageInitializer;
            CoreRegistry.Instance.RegisterPackage(corePackageInitializer)
                .ProvidesComponent<IInstallationId>()
                .ProvidesComponent<ICloudProjectId>()
                .ProvidesComponent<IActionScheduler>()
                .ProvidesComponent<IEnvironments>()
                .ProvidesComponent<IProjectConfiguration>()
                .ProvidesComponent<IMetricsFactory>()
                .ProvidesComponent<IDiagnosticsFactory>()
                .ProvidesComponent<IUnityThreadUtils>();
        }

        /// <summary>
        /// This is the Initialize callback that will be triggered by the Core package.
        /// This method will be invoked when the game developer calls UnityServices.InitializeAsync().
        /// </summary>
        /// <param name="registry">
        /// The registry containing components from different packages.
        /// </param>
        /// <returns>
        /// Return a Task representing your initialization.
        /// </returns>
        public async Task Initialize(CoreRegistry registry)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            try
            {
                if (HaveInitOptionsChanged())
                {
                    FreeOptionsDependantComponents();
                }

                // There are potential race conditions with other services we're trying to avoid by calling
                // RegisterInstallationId as the _very first_ thing we do.
                InitializeInstallationId();

                InitializeActionScheduler();

                await InitializeProjectConfigAsync(UnityServices.Instance.Options);

                InitializeEnvironments(ProjectConfig);
                InitializeCloudProjectId();

                InitializeDiagnostics(ActionScheduler, ProjectConfig, CloudProjectId, Environments);
                CoreDiagnostics.Instance.Diagnostics = DiagnosticsFactory.Create(CorePackageName);
                CoreDiagnostics.Instance.SetProjectConfiguration(ProjectConfig.ToJson());

                InitializeMetrics(ActionScheduler, ProjectConfig, CloudProjectId, Environments);
                CoreMetrics.Instance.Metrics = MetricsFactory.Create(CorePackageName);

                InitializeUnityThreadUtils();

                // Register components as late as possible to provide them only when initialization succeeded.
                RegisterProvidedComponents();
            }
            catch (Exception reason)
            {
                CoreDiagnostics.Instance.SendCorePackageInitDiagnostics(reason);
                throw;
            }

            stopwatch.Stop();
            CoreMetrics.Instance.SendCorePackageInitTimeMetric(stopwatch.Elapsed.TotalSeconds);

            void RegisterProvidedComponents()
            {
                registry.RegisterServiceComponent<IInstallationId>(InstallationId);
                registry.RegisterServiceComponent<IActionScheduler>(ActionScheduler);
                registry.RegisterServiceComponent<IProjectConfiguration>(ProjectConfig);
                registry.RegisterServiceComponent<IEnvironments>(Environments);
                registry.RegisterServiceComponent<ICloudProjectId>(CloudProjectId);
                registry.RegisterServiceComponent<IDiagnosticsFactory>(DiagnosticsFactory);
                registry.RegisterServiceComponent<IMetricsFactory>(MetricsFactory);
                registry.RegisterServiceComponent<IUnityThreadUtils>(UnityThreadUtils);
            }
        }

        bool HaveInitOptionsChanged()
        {
            return !(m_CurrentInitializationOptions is null)
                && !m_CurrentInitializationOptions.Values.ValueEquals(UnityServices.Instance.Options.Values);
        }

        void FreeOptionsDependantComponents()
        {
            ProjectConfig = null;
            Environments = null;
            DiagnosticsFactory = null;
            MetricsFactory = null;
        }

        internal void InitializeInstallationId()
        {
            if (!(InstallationId is null))
                return;

            var installationId = new InstallationId();
            installationId.CreateIdentifier();
            InstallationId = installationId;
        }

        internal void InitializeActionScheduler()
        {
            if (!(ActionScheduler is null))
                return;

            var actionScheduler = new ActionScheduler();
            actionScheduler.JoinPlayerLoopSystem();
            ActionScheduler = actionScheduler;
        }

        internal async Task InitializeProjectConfigAsync([NotNull] InitializationOptions options)
        {
            if (!(ProjectConfig is null))
                return;

            ProjectConfig = await GenerateProjectConfigurationAsync(options);

            // Copy options in case only values are changed without changing the reference.
            m_CurrentInitializationOptions = new InitializationOptions(options);
        }

        internal static async Task<ProjectConfiguration> GenerateProjectConfigurationAsync(
            [NotNull] InitializationOptions options)
        {
            var serializedConfig = await GetSerializedConfigOrEmptyAsync();
            if (serializedConfig.Keys is null
                || serializedConfig.Values is null)
            {
                serializedConfig = SerializableProjectConfiguration.Empty;
            }

            var configValues = new Dictionary<string, ConfigurationEntry>(serializedConfig.Keys.Length);
            configValues.FillWith(serializedConfig);
            configValues.FillWith(options);
            return new ProjectConfiguration(configValues);
        }

        internal static async Task<SerializableProjectConfiguration> GetSerializedConfigOrEmptyAsync()
        {
            try
            {
                var config = await ConfigurationUtils.ConfigurationLoader.GetConfigAsync();
                return config;
            }
            catch (Exception e)
            {
                CoreLogger.LogError(
                    "En error occured while trying to get the project configuration for services." +
                    $"\n{e.Message}" +
                    $"\n{e.StackTrace}");
                return SerializableProjectConfiguration.Empty;
            }
        }

        internal void InitializeEnvironments(IProjectConfiguration projectConfiguration)
        {
            if (!(Environments is null))
                return;

            var currentEnvironment = projectConfiguration.GetString(
                EnvironmentsOptionsExtensions.EnvironmentNameKey, "production");
            Environments = new Environments.Internal.Environments
            {
                Current = currentEnvironment,
            };
        }

        internal void InitializeCloudProjectId()
        {
            if (!(CloudProjectId is null))
                return;

            CloudProjectId = new CloudProjectId();
        }

        internal void InitializeDiagnostics(
            IActionScheduler scheduler, IProjectConfiguration projectConfiguration, ICloudProjectId cloudProjectId,
            IEnvironments environments)
        {
            if (!(DiagnosticsFactory is null))
                return;

            DiagnosticsFactory = TelemetryUtils.CreateDiagnosticsFactory(
                scheduler, projectConfiguration, cloudProjectId, environments);
        }

        internal void InitializeMetrics(
            IActionScheduler scheduler, IProjectConfiguration projectConfiguration, ICloudProjectId cloudProjectId,
            IEnvironments environments)
        {
            if (!(MetricsFactory is null))
                return;

            MetricsFactory = TelemetryUtils.CreateMetricsFactory(
                scheduler, projectConfiguration, cloudProjectId, environments);
        }

        internal void InitializeUnityThreadUtils()
        {
            if (!(UnityThreadUtils is null))
                return;

            UnityThreadUtils = new UnityThreadUtilsInternal();
        }

        public async Task<IDiagnosticsFactory> CreateDiagnosticsComponents()
        {
            if (HaveInitOptionsChanged())
            {
                FreeOptionsDependantComponents();
            }

            InitializeActionScheduler();
            await InitializeProjectConfigAsync(UnityServices.Instance.Options);
            InitializeEnvironments(ProjectConfig);
            InitializeCloudProjectId();
            InitializeDiagnostics(ActionScheduler, ProjectConfig, CloudProjectId, Environments);
            return DiagnosticsFactory;
        }

        public async Task<string> GetSerializedProjectConfigurationAsync()
        {
            await InitializeProjectConfigAsync(UnityServices.Instance.Options);
            return ProjectConfig.ToJson();
        }
    }
}
