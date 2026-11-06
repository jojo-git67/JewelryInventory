using JewelryInventory.Api.DTOs;

namespace JewelryInventory.Api.Repositories;

public interface IManualAdjustmentRepository
{
    Task<ManualAdjustmentDto?> GetManualAdjustmentDtoByIdAsync(
        int componentId,
        int adjustmentId);
}