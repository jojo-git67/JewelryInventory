using System.Linq.Expressions;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Mappings;

public static class ReceiveOrderMappings
{
    public static readonly Expression<Func<ReceiveOrder, ReceiveOrderDto>> ToDto =
        order => new ReceiveOrderDto
        {
            Id = order.Id,
            CreatedAt = order.CreatedAt,

            Rows = order.ReceiveOrderRows
                .Select(row => new ReceiveOrderRowDto
                {
                    Id = row.Id,
                    ComponentId = row.ComponentId,
                    QuantityReceived = row.QuantityReceived,
                    UnitPurchasePrice = row.UnitPurchasePrice
                })
                .ToList()
        };
}