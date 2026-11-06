using JewelryInventory.Api.Data;

namespace JewelryInventory.Api.Repositories;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;
    
    public EfUnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }


    public Task<int> SaveChangesAsync()
    {
        return _context.SaveChangesAsync();
    }
}
