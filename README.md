# Deucarian Logging

Deucarian Logging is a small, dependency-light wrapper around Unity's built-in `UnityEngine.Debug` logging. It gives Deucarian Unity packages a shared category logger, consistent formatting, configurable filtering, and a simple sink extension point.

It exists so package code can use one tiny logging API without bringing in a large logging framework. This package intentionally does not depend on `com.unity.logging`.

## Installation

Install the package from Git:

```text
https://github.com/JorisHoef/Logging.git
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

## API Overview

- `DLog.For(string category)` creates a category logger.
- `Trace`, `Debug`, `Info`, `Warning`, `Error`, and `Exception` write log entries.
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

## Ring Buffer Sink

Use `RingBufferLogSink` when you need recent log history without adding UI, file logging, or telemetry:

```csharp
var ringBuffer = new RingBufferLogSink(200);
DeucarianLog.RegisterSink(ringBuffer);

foreach (DeucarianLogEntry entry in ringBuffer.Entries)
{
    // Show recent entries in a debug console or include them in a bug report.
}
```

Entries are exposed in oldest-to-newest order. When capacity is reached, the oldest entry is evicted.

## Samples

Import the **Basic Logging Demo** sample from Unity's Package Manager to see a minimal MonoBehaviour using `DLog`.
