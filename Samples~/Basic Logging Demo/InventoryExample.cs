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
