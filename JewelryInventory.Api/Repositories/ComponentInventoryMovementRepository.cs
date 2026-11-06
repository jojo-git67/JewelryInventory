using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using JewelryInventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Repositories;

public class ComponentInventoryMovementRepository : IComponentInventoryMovementRepository
{
    private readonly InventoryDbContext _context;
    
    public ComponentInventoryMovementRepository(InventoryDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ComponentInventoryMovementDto>> GetComponentInventoryMovementsAsync(
        int componentId, string? source = null)
    {
        var query = _context.ComponentInventoryMovements
            .Where(m => m.ComponentId == componentId);

        if (!string.IsNullOrWhiteSpace(source))
        {
            switch (source.ToLowerInvariant())
            {
                case "manualadjustment":
                    query = query.Where(m => m.ManualAdjustmentId != null);
                    break;
                case "receiveorder":
                    query = query.Where(m => m.ReceiveOrderRowId != null);
                    break;
            }
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Select(ComponentInventoryMovementMappings.ToDto)
            .ToListAsync();
    }

    public async Task<decimal> GetCurrentStockAsync(int componentId)
    {
        return await _context.ComponentInventoryMovements
            .Where(m => m.ComponentId == componentId)
            .SumAsync(m => m.QuantityChange);
    }

    public void Add(ComponentInventoryMovement movement)
    {
        _context.ComponentInventoryMovements.Add(movement);
    }
}