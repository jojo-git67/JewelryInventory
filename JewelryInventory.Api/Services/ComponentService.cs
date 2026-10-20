using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Services;

public class ComponentService
{
    private readonly InventoryDbContext _context;

    public ComponentService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<List<ComponentDto>> GetAllActiveComponentsAsync()
    {
        return await _context.Components
            .AsNoTracking()
            .Where(c => c.IsActive)
            .Select(c => new ComponentDto()
            {
                Id = c.Id,
                Name = c.Name,
                Color = c.Color,
                Category = c.Category,
                UnitOfMeasurement = c.UnitOfMeasurement,
                UnitCost = c.UnitCost,
                LowStockThreshold = c.LowStockThreshold,
                StorageLocation = c.StorageLocation,
                IsActive = c.IsActive,
                Barcode = c.Barcode,
                
                CurrentStock = c.InventoryMovements.Sum(m => m.QuantityChange)
                
            })
            .ToListAsync();
    }
}