# Basic Logging Demo

This sample shows the minimal category logger pattern used by Deucarian Unity packages.

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

The message appears in the Unity console as:

```text
[Deucarian.Inventory] Inventory example started.
```
