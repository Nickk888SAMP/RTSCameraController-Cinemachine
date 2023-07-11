# Changelog

All notable changes to this package will be documented in this file.

The format is based on [Keep a Changelog](http://keepachangelog.com/en/1.0.0/)
and this project adheres to [Semantic Versioning](http://semver.org/spec/v2.0.0.html).

## [1.4.0] - 2022-04-29

### Added

- Add Vivox public interfaces: `IVivox`, `IVivoxTokenProviderInternal`, to enable interactions with the Vivox service.

## [1.3.2] - 2022-04-14

### Fixed

- Crash on Switch when initializing telemetry persistence. Now telemetry won't persist anything on Switch.
- NullReferenceException while linking the project
- Issue with user roles and service flags

## [1.3.1] - 2022-03-29

### Changed

- Newtonsoft package dependency update to 3.0.2. 


## [1.3.0] - 2022-03-21

### Added

- Add QoS public interface: `IQosResults` and return type `QosResult`, to provide QoS functionality to other
  packages

### Fixed

- Code stripping when core package is not used
- Retrying to initialize all services after a first attempt failed.

## [1.2.0] - 2022-02-23

### Added

- Add Wire public interfaces: `IWire`, `IChannel`, `IChannelTokenProvider`, and their dependencies, to enable
  interactions with the Wire service.
- The `IUnityThreadUtils` component to simplify working with the Unity thread.

### Changed

- Newtonsoft dependency to use the latest major Newtonsoft version, 13.0.1.

## [1.1.0-pre.69] - 2022-02-17

### Added

- Add `IEnvironmentId` component to provide the environment ID from the Access Token to other packages
- `OrganizationProvider` & `IOrganizationHandler` to enable package developers to access Organization Key.

## [1.1.0-pre.41] - 2021-12-08

### Added

- `IDiagnosticsFactory` component & `IDiagnostics` to enable package developers to send diagnostics for their package.
- Add `AnalyticsOptionsExtensions` with `SetAnalyticsUserId(string identifier)` to set a custom analytics user id.
- `IMetricsFactory` component & `IMetrics` to enable package developers to send metrics for their package.

### Fixed

- Calling `UnityServices.InitializeAsync(null)` throwing a null reference exception.

## [1.1.0-pre.11] - 2021-10-25

### Added

- Getter methods for `ConfigurationBuilder`.

### Fixed

- Fix layout for Project Bind Redirect Popup for Light theme

## [1.1.0-pre.10] - 2021-10-08

### Added

- `IActionScheduler` component to schedule actions at runtime.
- `ICloudProjectId` component to access cloudProjectId.

### Removed

- Removed the Service Activation Popup

### Fixed

- Fix define check bug on Android and WebGL

## [1.1.0-pre.9] - 2021-09-24

### Added

- New common error codes: `ApiMissing`, `RequestRejected`, `NotFound`, `InvalidRequest`.
- Link project pop-up dialog

### Fixed

- Core registry throwing exceptions when domain reloads are disabled

## [1.1.0-pre.8] - 2021-08-06

### Added

- Added base exception type for other Operate SDKs to derive from. Consistent error handling experience.

## [1.1.0-pre.7] - 2021-08-06

### Added

- `UnityServices` class at runtime. It is the entry point to initialize unity services with `InitializeAsync()`
  or `InitializeAsync(InitializationOptions)`.
- `InitializationOptions` to enable services initialization customization through code.
- `IInstallationId` component to access the Unity Installation Identifier.
- `IEnvironments` component to get the environment currently used by services.
- `SetEnvironmentName` initialization option to set the environment services should use.
- MiniJson.
- `IProjectConfiguration` component to access services settings at runtime.
- `IConfigurationProvider` to provide configuration values that need to be available at runtime.

## [1.0.1] - 2021-06-28

### Added

- DevEx integration into the editor.
- Service Activation popup.

### This is the first release of *com.unity.services.core*.
