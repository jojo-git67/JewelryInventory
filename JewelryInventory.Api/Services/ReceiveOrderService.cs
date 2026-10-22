using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using JewelryInventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Services;

public class ReceiveOrderService
{
    private readonly InventoryDbContext _context;
    private readonly InventoryLockService _inventoryLockService;

    public ReceiveOrderService(
        InventoryDbContext context,
        InventoryLockService inventoryLockService)
    {
        _context = context;
        _inventoryLockService = inventoryLockService;
    }

    public async Task<ReceiveOrderDto?> GetReceiveOrderByIdAsync(int orderId)
    {
        return await _context.ReceiveOrders
            .AsNoTracking()
            .Where(o => o.Id == orderId)
            .Select(ReceiveOrderMappings.ToDto)
            .SingleOrDefaultAsync();
    }

    public async Task<ReceiveOrderDto> CreateReceiveOrderAsync(
        CreateReceiveOrderRequest request)
    {
        if (request.Rows is null || request.Rows.Count == 0)
        {
            throw new ArgumentException("A receiving order must contain at least one row");
        }

        foreach (var row in request.Rows)
        {
            if (row is null)
                throw new ArgumentException("Receiving order rows cannot be null.");

            if (row.QuantityReceived <= 0)
                throw new ArgumentException("Quantity must be greater than zero");

            if (row.UnitPurchasePrice < 0)
                throw new ArgumentException("Unit price cannot be negative");
        }
            
        var duplicateComponentIds = request.Rows
            .GroupBy(r => r.ComponentId)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();

        if (duplicateComponentIds.Count > 0)
        {
            throw new ArgumentException("A component cannot appear more than once in a receiving order.");
        }
        
        var componentIds = request.Rows
            .Select(r => r.ComponentId)
            .ToList();

        

        var acquiredLocks = new List<SemaphoreSlim>();

        try
        {
            foreach (var componentId in componentIds.OrderBy(id => id))
            {
                var componentLock = _inventoryLockService.GetComponentLock(componentId);
                
                await componentLock.WaitAsync();
                acquiredLocks.Add(componentLock);
            }
            
            var existingComponentIds = await _context.Components
                .Where(c => componentIds.Contains(c.Id))
                .Select(c => c.Id)
                .ToListAsync();

            if (existingComponentIds.Count != componentIds.Count)
            {
                throw new ArgumentException(
                    "One or more components do not exist.");
            }
            
            var now = DateTime.UtcNow;

            var receiveOrder = new ReceiveOrder
            {
                CreatedAt = now
            };
        
            foreach (var row in request.Rows)
            {
                var receiveOrderRow = new ReceiveOrderRow
                {
                    ComponentId = row.ComponentId,
                    QuantityReceived = row.QuantityReceived,
                    UnitPurchasePrice = row.UnitPurchasePrice
                };

                var movement = new ComponentInventoryMovement
                {
                    ComponentId = row.ComponentId,
                    ReceiveOrderRow = receiveOrderRow,
                    QuantityChange = row.QuantityReceived,
                    CreatedAt = now
                };

                receiveOrder.ReceiveOrderRows.Add(receiveOrderRow);

                _context.ComponentInventoryMovements.Add(movement);
            }
        
            _context.ReceiveOrders.Add(receiveOrder);

            await _context.SaveChangesAsync();
        
            return (await GetReceiveOrderByIdAsync(receiveOrder.Id))!;
        }
        finally
        {
            foreach (var componentLock in acquiredLocks)
            {
                componentLock.Release();
            }
        }
        
        
    }
}