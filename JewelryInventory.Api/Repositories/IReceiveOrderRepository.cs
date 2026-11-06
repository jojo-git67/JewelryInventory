using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Models;

namespace JewelryInventory.Api.Repositories;

public interface IReceiveOrderRepository
{
    Task<ReceiveOrderDto?> GetReceiveOrderDtoByIdAsync(int orderId);

    void Add(ReceiveOrder receiveOrder);
    
}