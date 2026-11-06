using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Repositories;

public interface IComponentRepository
{
    Task<List<ComponentDto>> GetAllComponentDtoAsync();
    
    Task<List<ComponentDto>> GetAllActiveComponentDtoAsync();
    
    Task<ComponentDto?> GetComponentDtoByIdAsync(int id);
    
    Task<Component?> GetEntityByIdAsync(int id);
    
    Task<ComponentDto?> GetComponentByBarcodeAsync(string barcode);
    
    Task<bool> ComponentExistsAsync(int id);
    
    Task<bool> AllComponentsExistAsync(IEnumerable<int> componentIds);
    
    Task<bool> BarcodeExistsAsync(string barcode, int? excludeId = null);
    
    void Add(Component component);
}