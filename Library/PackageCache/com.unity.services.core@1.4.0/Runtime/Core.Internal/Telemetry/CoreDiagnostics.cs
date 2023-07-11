using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core.Telemetry.Internal;

namespace Unity.Services.Core.Internal
{
    class CoreDiagnostics
    {
        internal const string CorePackageName = "com.unity.services.core";

        internal const string CircularDependencyDiagnosticName = "circular_dependency";

        internal const string CorePackageInitDiagnosticName = "core_package_init";

        internal const string OperateServicesInitDiagnosticName = "operate_services_init";

        internal const string ProjectConfigTagName = "project_config";

        public IDictionary<string, string> CoreTags { get; internal set; }

        public static CoreDiagnostics Instance { get; internal set; }

        internal IDiagnosticsComponentProvider DiagnosticsComponentProvider { get; set; }

        internal IDiagnostics Diagnostics { get; set; }

        async Task<IDiagnostics> GetOrCreateDiagnostics()
        {
            if (Diagnostics is null)
            {
                var diagnosticFactory = await DiagnosticsComponentProvider.CreateDiagnosticsComponents();
                Diagnostics = diagnosticFactory.Create(CorePackageName);
                SetProjectConfiguration(await DiagnosticsComponentProvider.GetSerializedProjectConfigurationAsync());
            }
            return Diagnostics;
        }

        public void SetProjectConfiguration(string serializedProjectConfig)
        {
            CoreTags = new Dictionary<string, string>();
            CoreTags[ProjectConfigTagName] = serializedProjectConfig;
        }

        public void SendCircularDependencyDiagnostics(Exception exception)
        {
            var sendTask = SendCoreDiagnostics(CircularDependencyDiagnosticName, exception);
            sendTask.ContinueWith(OnSendFailed, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SendCorePackageInitDiagnostics(Exception exception)
        {
            var sendTask = SendCoreDiagnostics(CorePackageInitDiagnosticName, exception);
            sendTask.ContinueWith(OnSendFailed, TaskContinuationOptions.OnlyOnFaulted);
        }

        public void SendOperateServicesInitDiagnostics(Exception exception)
        {
            var sendTask = SendCoreDiagnostics(OperateServicesInitDiagnosticName, exception);
            sendTask.ContinueWith(OnSendFailed, TaskContinuationOptions.OnlyOnFaulted);
        }

        static void OnSendFailed(Task failedSendTask)
        {
            CoreLogger.LogException(failedSendTask.Exception);
        }

        async Task SendCoreDiagnostics(string diagnosticName, Exception exception)
        {
            var diagnostics = await GetOrCreateDiagnostics();
            diagnostics.SendDiagnostic(diagnosticName, exception.ToString(), CoreTags);
        }
    }
}
