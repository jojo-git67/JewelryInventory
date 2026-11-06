using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Exceptions;
using JewelryInventory.Api.Models;
using JewelryInventory.Api.Repositories;

namespace JewelryInventory.Api.Services;

public class ManualAdjustmentService
{
    private readonly IComponentRepository _componentRepository;
    private readonly IComponentInventoryMovementRepository _movementRepository;
    private readonly IManualAdjustmentRepository _manualAdjustmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly InventoryLockService _inventoryLockService;
    
    public ManualAdjustmentService(
        IComponentRepository componentRepository,
        IComponentInventoryMovementRepository movementRepository,
        IManualAdjustmentRepository manualAdjustmentRepository,
        IUnitOfWork unitOfWork,
        InventoryLockService inventoryLockService)
    {
        _componentRepository = componentRepository;
        _movementRepository = movementRepository;
        _manualAdjustmentRepository = manualAdjustmentRepository;
        _unitOfWork = unitOfWork;
        _inventoryLockService = inventoryLockService;
    }

    public async Task<ManualAdjustmentDto?> CreateManualAdjustmentAsync(
        int componentId,
        CreateManualAdjustmentRequest request
        )
    {
        if (componentId <= 0)
        {
            throw new ArgumentException("Invalid component ID");
        }

        if (request.QuantityChange == 0)
        {
            throw new ArgumentException("Quantity change cannot be 0");
        }

        var reason = request.Reason.Trim();

        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException("Reason is required");
        }

        if (!await _componentRepository.ComponentExistsAsync(componentId))
        {
            return null;
        }

        var semaphore = _inventoryLockService.GetComponentLock(componentId);
        await semaphore.WaitAsync();

        try
        {
            var component = await _componentRepository.GetEntityByIdAsync(componentId);

            if (component is null)
            {
                return null;
            }

            var currentStock = await _movementRepository.GetCurrentStockAsync(componentId);

            if (currentStock + request.QuantityChange < 0)
            {
                throw new InsufficientStockException(componentId);
            }

            var now = DateTime.UtcNow;

            var adjustment = new ManualAdjustment
            {
                Component = component,
                QuantityChange = request.QuantityChange,
                Reason = reason,
                CreatedAt = now,
            };

            var movement = new ComponentInventoryMovement
            {
                Component = component,
                ManualAdjustment = adjustment,
                QuantityChange = request.QuantityChange,
                CreatedAt = now,
            };

            _movementRepository.Add(movement);

            await _unitOfWork.SaveChangesAsync();

            return new ManualAdjustmentDto
            {
                Id = adjustment.Id,
                ComponentId = adjustment.ComponentId,
                QuantityChange = adjustment.QuantityChange,
                Reason = adjustment.Reason,
                CreatedAt = adjustment.CreatedAt,
            };
        }
        finally
        {
            semaphore.Release();
        }
    }

    public async Task<ManualAdjustmentDto?> GetManualAdjustmentByIdAsync(
        int componentId,
        int adjustmentId)
    {
        return await _manualAdjustmentRepository
            .GetManualAdjustmentDtoByIdAsync(componentId, adjustmentId);
    }
}