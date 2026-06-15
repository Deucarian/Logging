# Changelog

All notable changes to this package will be documented in this file.

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
