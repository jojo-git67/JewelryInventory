using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Services;

public class ComponentInventoryMovementService
{
    private readonly  InventoryDbContext _context;
    
    public  ComponentInventoryMovementService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComponentInventoryMovementDto>?> GetComponentInventoryMovementsAsync(
        int componentId,
        string? source = null)
    {
        var componentExists = await _context.Components
            .AnyAsync(c => c.Id == componentId);

        if (!componentExists)
        {
            return null;
        }

        var query = _context.ComponentInventoryMovements
            .AsNoTracking()
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

                default:
                    throw new ArgumentException("Invalid movement source.");
            }
        }

        return await query
            .OrderByDescending(m => m.CreatedAt)
            .ThenByDescending(m => m.Id)
            .Select(ComponentInventoryMovementMappings.ToDto)
            .ToListAsync();
    }
}