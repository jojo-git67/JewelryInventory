using System.ComponentModel.DataAnnotations;

namespace JewelryInventory.Api.DTOs;

public class UpdateStatusRequest
{
    [Required]
    public bool? IsActive { get; set; }
}