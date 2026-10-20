using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using JewelryInventory.Api.Models;
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
            .Select(ComponentMappings.ToDto)
            .ToListAsync();
    }
    public async Task<List<ComponentDto>> GetAllComponentsAsync()
    {
        return await _context.Components
            .AsNoTracking()
            .Select(ComponentMappings.ToDto)
            .ToListAsync();
    }

    public async Task<ComponentDto?> GetComponentById(int id)
    {
        return await _context.Components
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(ComponentMappings.ToDto)
            .SingleOrDefaultAsync();
    }

    public async Task<ComponentDto> CreateComponentAsync(CreateComponentRequest request)
    {
        var component = new Component
        {
            Name = request.Name,
            Color = request.Color,
            Category = request.Category,
            UnitOfMeasurement = request.UnitOfMeasurement,
            UnitCost = request.UnitCost,
            LowStockThreshold = request.LowStockThreshold,
            StorageLocation = request.StorageLocation,
            Barcode = request.Barcode,
            IsActive = true
            
        };

        var now = DateTime.UtcNow;
        
        var adjustment = new ManualAdjustment()
        {
            Component = component,
            QuantityChange = request.InitialStock,
            Reason = "Initial stock",
            CreatedAt = now

        };

        var movement = new ComponentInventoryMovement
        {
            Component = component,
            ManualAdjustment = adjustment,
            QuantityChange = request.InitialStock,
            CreatedAt = now
            
        };

        _context.ComponentInventoryMovements.Add(movement);
        
        await _context.SaveChangesAsync();

        return await GetComponentById(component.Id) 
               ?? throw new InvalidOperationException(
                   "The newly created component could not be retrieved");
    }
}