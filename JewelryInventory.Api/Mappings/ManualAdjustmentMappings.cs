using System.Linq.Expressions;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Mappings;

public static class ManualAdjustmentMappings
{
    public static readonly Expression<Func<ManualAdjustment, ManualAdjustmentDto>> ToDto =
        m => new ManualAdjustmentDto
        {
            Id = m.Id,
            ComponentId = m.ComponentId,
            QuantityChange = m.QuantityChange,
            Reason = m.Reason,
            CreatedAt = m.CreatedAt
        };
}