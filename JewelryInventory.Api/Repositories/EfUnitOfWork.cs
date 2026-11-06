using JewelryInventory.Api.Data;
using JewelryInventory.Api.Exceptions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Repositories;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly InventoryDbContext _context;
    
    public EfUnitOfWork(InventoryDbContext context)
    {
        _context = context;
    }


    public async Task<int> SaveChangesAsync()
    {
        try
        {
            return await _context.SaveChangesAsync();
        }
        catch (DbUpdateException ex)
            when (ex.InnerException is SqliteException sqliteEx &&
                  sqliteEx.SqliteExtendedErrorCode == 2067)
        {
            throw new UniqueConstraintViolationException(ex);
        }
    }
}
