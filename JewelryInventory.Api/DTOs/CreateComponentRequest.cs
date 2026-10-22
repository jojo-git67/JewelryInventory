using System.ComponentModel.DataAnnotations;

namespace JewelryInventory.Api.DTOs;

public class CreateComponentRequest
{
    [Required]
    public string Name { get; set; } = string.Empty;

    public string? Color { get; set; }
    public string? Category { get; set; }

    [Required]
    public string UnitOfMeasurement { get; set; } = string.Empty;
    
    [Range(0, double.MaxValue)] 
    public decimal UnitCost { get; set; }
    
    [Range(0, double.MaxValue)] 
    public decimal LowStockThreshold { get; set; }

    public string? StorageLocation { get; set; }
    public string? Barcode { get; set; }
    
    [Range(0, double.MaxValue)] 
    public decimal InitialStock { get; set; }
}