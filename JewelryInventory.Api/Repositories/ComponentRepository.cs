using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using JewelryInventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Repositories;

public class ComponentRepository : IComponentRepository
{
    private readonly InventoryDbContext _context;
    
    public ComponentRepository(InventoryDbContext context)
    {
        _context = context;
    }
    
    public async Task<List<ComponentDto>> GetAllComponentDtoAsync()
    {
        return await _context.Components
            .Select(ComponentMappings.ToDto)
            .ToListAsync();
    }

    public async Task<List<ComponentDto>> GetAllActiveComponentDtoAsync()
    {
        return await _context.Components
            .Where(c => c.IsActive)
            .Select(ComponentMappings.ToDto)
            .ToListAsync();
    }

    public async Task<ComponentDto?> GetComponentDtoByIdAsync(int id)
    {
        return await  _context.Components
            .Where(c => c.Id == id)
            .Select(ComponentMappings.ToDto)
            .SingleOrDefaultAsync();
    }

    public async Task<Component?> GetEntityByIdAsync(int id)
    {
        return await _context.Components
            .SingleOrDefaultAsync(c => c.Id == id);
    }

    public async Task<ComponentDto?> GetComponentByBarcodeAsync(string barcode)
    {
        return await _context.Components
            .Where(c => c.Barcode == barcode)
            .Select(ComponentMappings.ToDto)
            .SingleOrDefaultAsync();
    }

    public async Task<bool> ComponentExistsAsync(int id)
    {
        return await _context.Components.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> AllComponentsExistAsync(
        IEnumerable<int> componentIds)
    {
        var ids = componentIds.Distinct().ToList();
        
        var existingCount = await _context.Components
            .CountAsync(c => ids.Contains(c.Id));

        return existingCount == ids.Count;
    }

    public async Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null)
    {
        return await _context.Components
            .AnyAsync(c => c.Barcode == barcode
            && (!excludeId.HasValue || c.Id != excludeId.Value));
    }

    public void Add(Component component)
    {
        _context.Components.Add(component);
    }
}