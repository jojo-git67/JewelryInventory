using System.ComponentModel.DataAnnotations;

namespace JewelryInventory.Api.DTOs;

public class UpdateComponentRequest
{
    [Required]
    public String Name { get; set; }

    public string? Color { get; set; }
    public string? Category { get; set; }

    [Required] 
    public string UnitOfMeasurement { get; set; } = "pcs";
    
    [Range(0, double.MaxValue)] 
    public decimal UnitCost { get; set; }
    
    [Range(0, double.MaxValue)] 
    public decimal LowStockThreshold { get; set; }

    public string? StorageLocation { get; set; }
    public string? Barcode { get; set; }
    
}