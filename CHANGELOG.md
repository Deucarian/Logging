# Changelog

All notable changes to this package will be documented in this file.

## [1.0.2] - 2026-07-16

### Changed

- Aligned the package version and its editor-only `com.deucarian.editor` dependency to `1.0.2` for the coordinated dependency version wave.
- Kept the runtime logging assembly independent from Deucarian Editor.

## [1.0.1] - 2026-06-22

### Changed

- Routed the editor Reset Logging Settings confirmation through the package-owned `Logging.Editor` DLog category.
- Kept direct Unity Debug calls limited to the Unity console sink and sink-failure fallback paths.

## [1.0.0] - 2026-06-22

### Changed

- Marked the current package metadata as 1.0.0 for the stable Logging package line.
- Documented that `com.deucarian.editor` is used by the editor settings UI while runtime logging remains independent from editor helpers.

## [0.2.6] - 2026-06-15

### Changed

- Updated the shared editor helper dependency to `com.deucarian.editor` `0.1.2` for npmjs scoped-registry publishing.

## [0.2.5] - 2026-06-15

### Changed

- Lowered the package Unity minimum to 2021.3 LTS now that `UnityEngine.HideInCallstack` usage is guarded for newer Unity versions.
- Updated the shared editor helper dependency to `com.deucarian.editor` `0.1.2` for stable npm publishing.
- Updated package metadata and documentation to match the 2021.3-compatible runtime and editor API surface.

## [0.2.4] - 2026-06-15

### Changed

- Moved logging editor menu items under `Tools > Deucarian > Logging`.
- Updated the shared editor helper dependency to `com.deucarian.editor` `0.1.1`.
- Removed `com.unity.test-framework` from consumer-facing package dependencies.
- Updated README structure with usage, tests, and license sections.

## [0.2.3] - 2026-06-15

### Changed

- Added a dependency on `com.deucarian.editor` for fixed Deucarian editor chrome.
- Updated the logging Project Settings provider to use shared Deucarian editor header, section, and footer helpers.
- Kept runtime logging assemblies independent from `Deucarian.Editor`.

## [0.2.2] - 2026-06-15

### Added

- Added the package-local `Deucarian > Logging > Open Logging Settings` menu item.

### Changed

- Moved logging reset and test-log menu items under `Deucarian > Logging`.
- Documented the Deucarian menu entries and kept logging local-only with no telemetry additions.

## [0.2.1] - 2026-06-15

### Fixed

- Added Unity `.meta` files for package assets so Git UPM installs do not ignore immutable package files.
- Guarded runtime and editor test assemblies with `UNITY_INCLUDE_TESTS`.
- Declared the Unity Test Framework dependency so committed test assemblies resolve their NUnit references when tests are included.

## [0.2.0] - 2026-06-12

### Added

- Documented Unity console stack trace preservation and source hyperlink preservation.
- Documented that context object navigation is preserved by the default Unity console sink.
- Broadened `UnityEngine.HideInCallstack` usage on trivial logging wrappers where Unity supports it.
- Documented ring buffer usage as a local bridge for debug consoles, bug reports, and future telemetry sinks.
- Documented timing scopes for Object Loading, ApiHelper, Package Installer, and similar package workflows.
- Clarified sink architecture as the extension point for future ecosystem integrations.
- Clarified telemetry-ready extension points while keeping this package local-only.

## [0.1.0] - 2026-06-12

### Added

- Initial `com.deucarian.logging` Unity package.
- Explicit local-only logging architecture documentation that keeps telemetry separate.
- Category logger API through `DLog`.
- String-only category constants for common Deucarian package categories.
- Scoped operation timing through `DLog.Scope` and `DLog.Measure`.
- Built-in Unity console sink backed by `UnityEngine.Debug`.
- In-memory `RingBufferLogSink`.
- Best-effort redaction helper for obvious secrets and URL query strings.
- Global runtime settings for enablement, level filtering, prefix, timestamp, and frame metadata.
- Project Settings provider for editor logging settings.
- Editor menu items for resetting settings and emitting test log messages.
- Runtime and editor assembly definitions.
- Runtime and editor test assemblies.
- Basic Logging Demo sample documentation.
