# Changelog

All notable changes to this package will be documented in this file.

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
