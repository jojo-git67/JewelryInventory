namespace JewelryInventory.Api.Repositories;

public interface IUnitOfWork
{
    Task<int> SaveChangesAsync();
}