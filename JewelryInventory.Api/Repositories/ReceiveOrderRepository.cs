using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Mappings;
using JewelryInventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Repositories;

public class ReceiveOrderRepository : IReceiveOrderRepository
{
    private readonly InventoryDbContext _context;
    
    public ReceiveOrderRepository(InventoryDbContext context)
    {
        _context = context;
    }
    
    public async Task<ReceiveOrderDto?> GetReceiveOrderDtoByIdAsync(int orderId)
    {
        return await _context.ReceiveOrders
            .Where(o => o.Id == orderId)
            .Select(ReceiveOrderMappings.ToDto)
            .SingleOrDefaultAsync();
    }

    public void Add(ReceiveOrder receiveOrder)
    {
        _context.ReceiveOrders.Add(receiveOrder);
    }
}