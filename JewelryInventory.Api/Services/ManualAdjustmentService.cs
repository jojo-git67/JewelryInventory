using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Services;

public class ManualAdjustmentService
{
    private readonly InventoryDbContext _context;
    
    public ManualAdjustmentService(InventoryDbContext context)
    {
        _context = context;
    }

    public async Task<ManualAdjustmentDto?> CreateManualAdjustmentAsync(
        int componentId,
        CreateManualAdjustmentRequest request
        )
    {
        if (request.QuantityChange == 0)
        {
            throw new ArgumentException("Quantity change cannot be zero.");
        }

        if (string.IsNullOrWhiteSpace(request.Reason))
        {
            throw new ArgumentException("Reason cannot be empty.");
        }
        
        var component = await _context.Components
            .SingleOrDefaultAsync(c => c.Id == componentId);

        if (component is null)
        {
            return null;
        }

        var currentStock = await _context.ComponentInventoryMovements
            .Where(m => m.ComponentId == componentId)
            .SumAsync(m => m.QuantityChange);

        if (currentStock + request.QuantityChange < 0)
        {
            throw new InsufficientStockException(componentId);
        }
        
        var now = DateTime.UtcNow;

        var adjustment = new ManualAdjustment()
        {
            Component = component,
            QuantityChange = request.QuantityChange,
            Reason = request.Reason,
            CreatedAt = now

        };
        
        var movement = new ComponentInventoryMovement
        {
            Component = component,
            ManualAdjustment = adjustment,
            QuantityChange = request.QuantityChange,
            CreatedAt = now
            
        };
        
        _context.ComponentInventoryMovements.Add(movement);

        await _context.SaveChangesAsync();

        return new ManualAdjustmentDto
        {
            Id = adjustment.Id,
            ComponentId = adjustment.ComponentId,
            QuantityChange = adjustment.QuantityChange,
            Reason = adjustment.Reason,
            CreatedAt = adjustment.CreatedAt,
        };

    }
}