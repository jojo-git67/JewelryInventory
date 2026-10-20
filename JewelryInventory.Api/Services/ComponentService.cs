using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
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
}