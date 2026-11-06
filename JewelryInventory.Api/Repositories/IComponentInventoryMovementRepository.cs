using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Repositories;

public interface IComponentInventoryMovementRepository
{
    Task<List<ComponentInventoryMovementDto>> GetComponentInventoryMovementsAsync(int componentId,
        string? source = null);

    Task<decimal> GetCurrentStockAsync(int componentId);
    
    void Add(ComponentInventoryMovement movement);
}