using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;

namespace JewelryInventory.Api.Services;

public class ComponentService
{
    private readonly IComponentRepository _componentRepository;
    private readonly IComponentInventoryMovementRepository _movementRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ComponentService(
        IComponentRepository componentRepository,
        IComponentInventoryMovementRepository movementRepository,
        IUnitOfWork unitOfWork)
    {
        _componentRepository = componentRepository;
        _movementRepository = movementRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<List<ComponentDto>> GetAllActiveComponentsAsync()
    {
        return await _componentRepository.GetAllActiveComponentDtoAsync();
    }
    public async Task<List<ComponentDto>> GetAllComponentsAsync()
    {
        return await _componentRepository.GetAllComponentDtoAsync();
    }

    public async Task<ComponentDto?> GetComponentByIdAsync(int id)
    {
        return await _componentRepository.GetComponentDtoByIdAsync(id);
    }
    
    public async Task<ComponentDto?> GetComponentByBarcodeAsync(string barcode)
    {
        return await _componentRepository.GetComponentByBarcodeAsync(barcode);
    }

    public async Task<ComponentDto> CreateComponentAsync(CreateComponentRequest request)
    {
        var name = request.Name?.Trim();
        var barcode = request.Barcode?.Trim();
        var unitOfMeasurement = request.UnitOfMeasurement?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Component name is required");
        }
        
        if (string.IsNullOrWhiteSpace(unitOfMeasurement))
        {
            throw new ArgumentException("Unit of measurement is required");
        }

        if (string.IsNullOrEmpty(barcode))
        {
            barcode = null;
        }

        if (request.InitialStock < 0)
        {
            throw new ArgumentException("Initial stock cannot be negative");
        }

        if (request.UnitCost < 0)
        {
            throw new ArgumentException("Unit cost cannot be negative");
        }

        if (request.LowStockThreshold < 0)
        {
            throw new ArgumentException("Low stock threshold cannot be negative");
        }
        
        if (barcode is not null &&
            await _componentRepository.BarcodeExistsAsync(barcode))
        {
            throw new DuplicateBarcodeException(barcode);
        }

        var component = new Component
        {
            Name = name,
            Color = request.Color,
            Category = request.Category,
            UnitOfMeasurement = unitOfMeasurement,
            UnitCost = request.UnitCost,
            LowStockThreshold = request.LowStockThreshold,
            StorageLocation = request.StorageLocation,
            Barcode = barcode,
            IsActive = true,
        };

        var now = DateTime.UtcNow;

        var adjustment = new ManualAdjustment
        {
            Component = component,
            QuantityChange = request.InitialStock,
            Reason = "Initial stock",
            CreatedAt = now,
        };

        var movement = new ComponentInventoryMovement
        {
            Component = component,
            ManualAdjustment = adjustment,
            QuantityChange = request.InitialStock,
            CreatedAt = now,
        };
        
        _movementRepository.Add(movement);

        await SaveChangesWithBarcodeValidationAsync(barcode);

        return await _componentRepository
                   .GetComponentDtoByIdAsync(component.Id)
               ?? throw new InvalidOperationException(
                   "The component could not be retrieved after saving");
    }

    public async Task<ComponentDto?> UpdateComponentAsync(
        int id,
        UpdateComponentRequest request)
    {
        var component = await _componentRepository.GetEntityByIdAsync(id);

        if (component is null)
        {
            return null;
        }

        var name = request.Name?.Trim();
        var barcode = request.Barcode?.Trim();
        var unitOfMeasurement = request.UnitOfMeasurement?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            throw new ArgumentException("Component name is required");
        }
        
        if (string.IsNullOrWhiteSpace(unitOfMeasurement))
        {
            throw new ArgumentException("Unit of measurement is required");
        }

        if (string.IsNullOrEmpty(barcode))
        {
            barcode = null;
        }
        
        if (request.UnitCost < 0)
        {
            throw new ArgumentException("Unit cost cannot be negative");
        }

        if (request.LowStockThreshold < 0)
        {
            throw new ArgumentException("Low stock threshold cannot be negative");
        }
        
        if (barcode is not null &&
            await _componentRepository.BarcodeExistsAsync(barcode, id))
        {
            throw new DuplicateBarcodeException(barcode);
        }

        component.Name = name;
        component.Color = request.Color;
        component.Category = request.Category;
        component.UnitOfMeasurement = unitOfMeasurement;
        component.UnitCost = request.UnitCost;
        component.LowStockThreshold = request.LowStockThreshold;
        component.StorageLocation = request.StorageLocation;
        component.Barcode = barcode;

        await SaveChangesWithBarcodeValidationAsync(barcode);

        return await _componentRepository.GetComponentDtoByIdAsync(id);

    }

    public async Task<ComponentDto?> SetComponentActiveAsync(
        int id,
        bool isActive)
    {
        var component = await _componentRepository.GetEntityByIdAsync(id);

        if (component is null)
        {
            return null;
        }

        component.IsActive = isActive;

        await _unitOfWork.SaveChangesAsync();

        return await _componentRepository.GetComponentDtoByIdAsync(id);
    }
    
    private async Task SaveChangesWithBarcodeValidationAsync(string? barcode)
    {
        try
        {
            await _unitOfWork.SaveChangesAsync();
        }
        catch (UniqueConstraintViolationException) when (barcode is not null)
        {
            throw new DuplicateBarcodeException(barcode);
        }
    }
}