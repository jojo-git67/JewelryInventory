using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Mappings;
using JewelryInventory.Api.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Data.Sqlite;
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
        if (request.Barcode is not null)
        {
            var barcodeExists = await _context.Components
                .AnyAsync(c => c.Barcode == request.Barcode);
            
            if (barcodeExists)
            {
                throw new DuplicateBarcodeException(request.Barcode);
            }
        }
        
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
        
        await SaveChangesWithBarcodeValidationAsync(request.Barcode);

        return await GetComponentById(component.Id) 
               ?? throw new InvalidOperationException(
                   "The newly created component could not be retrieved");
    }

    public async Task<ComponentDto?> UpdateComponentAsync(
        int id,
        UpdateComponentRequest request)
    {
        var component = await _context.Components
            .SingleOrDefaultAsync(c => c.Id == id);
        
        if (component is null)
        {
            return null;
        }
        
        if (request.Barcode is not null)
        {
            var barcodeExists = await _context.Components
                .AnyAsync(c => c.Barcode == request.Barcode &&  c.Id != id);

            if (barcodeExists)
            {
                throw new DuplicateBarcodeException(request.Barcode);
            }
        }
        
        component.Name = request.Name;
        component.Color = request.Color;
        component.Category = request.Category;
        component.UnitOfMeasurement = request.UnitOfMeasurement;
        component.UnitCost = request.UnitCost;
        component.LowStockThreshold = request.LowStockThreshold;
        component.StorageLocation = request.StorageLocation;
        component.Barcode = request.Barcode;

        await SaveChangesWithBarcodeValidationAsync(request.Barcode);

        return await GetComponentById(component.Id);
    }

    public async Task<ComponentDto?> SetComponentActiveAsync(
        int id,
        bool isActive)
    {
        var component = await _context.Components
            .SingleOrDefaultAsync(c => c.Id == id);

        if (component is null)
        {
            return null;
        }
        
        component.IsActive = isActive;

        await _context.SaveChangesAsync();
        
        return await GetComponentById(component.Id);
    }
    
    private async Task SaveChangesWithBarcodeValidationAsync(string? barcode)
    {
        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (barcode is not null
                  && ex.InnerException is SqliteException sqliteEx
                  && sqliteEx.SqliteExtendedErrorCode == 2067)
        {
            throw new DuplicateBarcodeException(barcode);
        }
    }
}