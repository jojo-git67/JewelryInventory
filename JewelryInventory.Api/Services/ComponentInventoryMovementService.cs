using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Repositories;

namespace JewelryInventory.Api.Services;

public class ComponentInventoryMovementService
{
    private readonly IComponentRepository _componentRepository;
    private readonly IComponentInventoryMovementRepository _movementRepository;

    public ComponentInventoryMovementService(
        IComponentRepository componentRepository,
        IComponentInventoryMovementRepository movementRepository)
    {
        _componentRepository = componentRepository;
        _movementRepository = movementRepository;
    }

    public async Task<List<ComponentInventoryMovementDto>?> GetComponentInventoryMovementsAsync(
        int componentId,
        string? source = null)
    {
        if (!await _componentRepository.ComponentExistsAsync(componentId))
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(source))
        {
            var normalizedSource = source.ToLowerInvariant();

            if (normalizedSource != "manualadjustment" &&
                normalizedSource != "receiveorder")
            {
                throw new ArgumentException("Invalid movement source");
            }    
        }
        
        return await _movementRepository.GetComponentInventoryMovementsAsync(componentId, source);
    }
}