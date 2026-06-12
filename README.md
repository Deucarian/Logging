# Deucarian Logging

Deucarian Logging is a small, dependency-light wrapper around Unity's built-in `UnityEngine.Debug` logging. It gives Deucarian Unity packages a shared category logger, consistent formatting, configurable filtering, and a simple sink extension point.

It exists so package code can use one tiny logging API without bringing in a large logging framework. This package intentionally does not depend on `com.unity.logging`.

## Architecture Boundary

This package is local logging only:

- `com.deucarian.logging` provides local logging, filtering, categories, sinks, and a ring buffer.
- A future `com.deucarian.telemetry` package may depend on logging.
- Logging must not depend on telemetry.
- No HTTP clients are included.
- No analytics SDK is included.
- No remote upload is implemented.
- No user or session tracking is implemented.
- No automatic collection of device or user data is implemented.

Future telemetry can plug in by implementing `IDeucarianLogSink`, for example with a `TelemetryLogSink`, without modifying this package.

## Installation

Install the package from Git:

```text
https://github.com/Deucarian/Logging.git
```

Or install it by package name from a configured scoped registry:

```text
com.deucarian.logging
```

The package requires Unity 2022.3 LTS or newer.

## Quick Start

```csharp
using Deucarian.Logging;
using UnityEngine;

public sealed class InventoryExample : MonoBehaviour
{
    private static readonly DLog Log = DLog.For("Inventory");

    private void Start()
    {
        Log.Info("Inventory example started.", this);
    }
}
```

This writes a clean Unity console message like:

```text
[Deucarian.Inventory] Inventory example started.
```

## Category Examples

Use short, stable categories for the system emitting logs:

```csharp
private static readonly DLog InventoryLog = DLog.For("Inventory");
private static readonly DLog SaveLog = DLog.For("SaveSystem");
private static readonly DLog AudioLog = DLog.For("Audio");
private static readonly DLog UiLog = DLog.For("UI");
private static readonly DLog NetworkLog = DLog.For("Networking");
```

Categories are intentionally strings, not enums. Deucarian packages and user projects should be able to add categories freely without modifying this package.

Optional convenience constants are available for common package categories:

```csharp
private static readonly DLog Log = DLog.For(DeucarianLogCategories.ObjectLoading);
```

The built-in constants are strings only:

- `DeucarianLogCategories.PackageInstaller`
- `DeucarianLogCategories.ObjectLoading`
- `DeucarianLogCategories.Theming`
- `DeucarianLogCategories.Selection`
- `DeucarianLogCategories.Session`
- `DeucarianLogCategories.ApiHelper`

Categories are trimmed and normalized. Passing `Deucarian.Inventory` is treated as `Inventory`, so output is not double-prefixed. Passing `null`, an empty string, or only the `Deucarian` prefix falls back to `General`.

## Log Levels

Levels are ordered from most detailed to most severe:

- `Trace`
- `Debug`
- `Info`
- `Warning`
- `Error`
- `Exception`
- `None`

`None` is intended for `DeucarianLogSettings.MinimumLevel` when all logging should be disabled by level.

## Filtering

Filtering happens before entries are sent to sinks:

- `DeucarianLogSettings.Enabled = false` suppresses all logs.
- `DeucarianLogSettings.MinimumLevel` suppresses logs below that level.
- Editor and development builds default to `Debug` and above.
- Release builds default to `Warning` and above.

Production recommendation: keep release builds at warnings/errors only unless you have a clear reason to lower the minimum level.

## Privacy And Redaction

Do not log secrets. Avoid logging tokens, passwords, authorization headers, API keys, or full URLs with query strings.

Log messages are passed through `DeucarianLogUtility.RedactSensitiveData` before they reach sinks. This helper redacts obvious keys such as:

- `token`
- `access_token`
- `password`
- `secret`
- `api_key`
- `authorization`

It also strips query strings from full `http` and `https` URLs. This is a best-effort convenience, not a security-grade sanitizer. Treat it as a last line of defense, not permission to log sensitive data.

Exception objects are preserved for exception logs so tools can inspect them. Do not create or log exceptions whose messages contain secrets.

## API Overview

- `DLog.For(string category)` creates a category logger.
- `Trace`, `Debug`, `Info`, `Warning`, `Error`, and `Exception` write log entries.
- `Scope` and `Measure` time operations with start/end logs and elapsed milliseconds.
- `DeucarianLogSettings` controls global enablement, minimum level, timestamp/frame metadata, and the package prefix.
- `DeucarianLog.RegisterSink` adds custom sinks.
- `RingBufferLogSink` keeps the latest log entries in memory for later in-game consoles or bug report exporters.
- `UnityConsoleLogSink` is registered by default and forwards entries to `UnityEngine.Debug`.

In Unity 2022.3 and newer, trivial logging wrapper methods are marked with `UnityEngine.HideInCallstack` where available to reduce console callstack clutter. This is only a cosmetic cleanup and is not required for correctness.

## Runtime Defaults

- Logging is enabled by default.
- Editor and development builds log `Debug` and above.
- Release builds log `Warning` and above.
- Timestamp and frame metadata are disabled by default to keep Unity console output compact.
- The default prefix is `Deucarian`.

In the Unity Editor, these values can be edited at **Project Settings > Deucarian > Logging**. The v0 editor settings are stored in `EditorPrefs` and applied to runtime static settings while in the Editor.

Useful editor menu items:

- **Tools > Deucarian > Logging > Reset Logging Settings**
- **Tools > Deucarian > Logging > Test Log Messages**

## Custom Sinks

```csharp
using Deucarian.Logging;

public sealed class MySink : IDeucarianLogSink
{
    public void Log(in DeucarianLogEntry entry)
    {
        // Forward to an in-game console, bug report, or test capture.
    }
}
```

Register the sink at startup:

```csharp
DeucarianLog.RegisterSink(new MySink());
```

Sink exceptions are caught by the dispatcher so logging failures do not escape into game code.

A future telemetry package can add a sink such as `TelemetryLogSink : IDeucarianLogSink`. That sink belongs in the telemetry package, not here.

## Scoped Measurement

Use `Scope` or `Measure` to log lightweight operation timings:

```csharp
private static readonly DLog Log = DLog.For(DeucarianLogCategories.ObjectLoading);

public void LoadAssetBundle()
{
    using (Log.Measure("Load asset bundle"))
    {
        // Work being measured.
    }
}
```

The scope logs a start message and a completion message with elapsed milliseconds only when the chosen level is enabled. The default level is `Debug`; pass another level when a workflow should be visible at `Info`, `Warning`, or another severity:

```csharp
using (Log.Measure("Install package", DeucarianLogLevel.Info))
{
    // Work being measured.
}
```

Disposing the scope more than once is safe.

## Ring Buffer Sink

Use `RingBufferLogSink` when you need recent local log history without adding UI, file logging, or telemetry:

```csharp
var ringBuffer = new RingBufferLogSink(200);
DeucarianLog.RegisterSink(ringBuffer);

foreach (DeucarianLogEntry entry in ringBuffer.Entries)
{
    // Show recent entries in a debug console or include them in a bug report.
}
```

Entries are exposed in oldest-to-newest order. When capacity is reached, the oldest entry is evicted.

The ring buffer is the local bridge toward:

- an in-game debug console
- bug report export
- a future telemetry sink in a separate telemetry package

The logging package itself remains local-only.

## Samples

Import the **Basic Logging Demo** sample from Unity's Package Manager to see a minimal MonoBehaviour using `DLog`.
