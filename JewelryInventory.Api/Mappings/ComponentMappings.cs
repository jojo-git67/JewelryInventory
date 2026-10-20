using System.Linq.Expressions;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Mappings;

public static class ComponentMappings
{
    public static readonly Expression<Func<Component, ComponentDto>> ToDto =
        c => new ComponentDto
        {
            Id = c.Id,
            Name = c.Name,
            Color = c.Color,
            Category = c.Category,
            UnitOfMeasurement = c.UnitOfMeasurement,
            UnitCost = c.UnitCost,
            LowStockThreshold = c.LowStockThreshold,
            StorageLocation = c.StorageLocation,
            IsActive = c.IsActive,
            Barcode = c.Barcode,

            CurrentStock = c.InventoryMovements.Sum(m => m.QuantityChange)
        };
}