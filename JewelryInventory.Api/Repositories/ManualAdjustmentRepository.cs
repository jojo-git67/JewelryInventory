using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Repositories;

public class ManualAdjustmentRepository : IManualAdjustmentRepository
{
    private readonly InventoryDbContext _context;
    
    public ManualAdjustmentRepository(InventoryDbContext context)
    {
        _context = context;
    }
    
    public async Task<ManualAdjustmentDto?> GetManualAdjustmentDtoByIdAsync(int componentId, int adjustmentId)
    {
        return await _context.ManualAdjustments
            .Where(m => m.ComponentId == componentId 
                        && m.Id == adjustmentId)
            .Select(ManualAdjustmentMappings.ToDto)
            .SingleOrDefaultAsync();
    }
}