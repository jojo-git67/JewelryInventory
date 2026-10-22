using System.Linq.Expressions;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Mappings;

public class ComponentInventoryMovementMappings
{
    public static readonly Expression<Func<ComponentInventoryMovement,
        ComponentInventoryMovementDto>> ToDto = m => new ComponentInventoryMovementDto
    {
        Id = m.Id,
        ComponentId = m.ComponentId,
        QuantityChange = m.QuantityChange,

        Source = m.ManualAdjustmentId != null
            ? "ManualAdjustment"
            : m.ReceiveOrderRowId != null
                ? "ReceiveOrder"
                : "Unknown",

        Reason = m.ManualAdjustment != null
            ? m.ManualAdjustment.Reason
            : null,

        CreatedAt = m.CreatedAt
    };
}