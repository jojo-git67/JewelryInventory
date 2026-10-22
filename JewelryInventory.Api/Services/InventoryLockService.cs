using System.Collections.Concurrent;

namespace JewelryInventory.Api.Services;

public class InventoryLockService
{
    private readonly ConcurrentDictionary<int, SemaphoreSlim> _componentLocks = new();

    public SemaphoreSlim GetComponentLock(int componentId)
    {
        return _componentLocks.GetOrAdd(
            componentId,
            _ => new SemaphoreSlim(1, 1));
    }
}