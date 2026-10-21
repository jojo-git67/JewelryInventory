using JewelryInventory.Api.Data;
using JewelryInventory.Api.DTOs;
using JewelryInventory.Api.Services;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Tests.Services;

public class ComponentServiceTests
{
    [Fact]
    public async Task CreateComponentAsync_ShouldCreateComponentWithInitialStock()
    {
        // Arrange
        await using var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<InventoryDbContext>()
            .UseSqlite(connection)
            .Options;

        await using var context = new InventoryDbContext(options);
        await context.Database.EnsureCreatedAsync();

        var service = new ComponentService(context);

        var request = new CreateComponentRequest
        {
            Name = "Silver Bead",
            Barcode = "TEST-001",
            InitialStock = 100
        };

        // Act
        var result = await service.CreateComponentAsync(request);

        // Assert
        Assert.Equal("Silver Bead", result.Name);
        Assert.Equal("TEST-001", result.Barcode);
        Assert.Equal(100m, result.CurrentStock);
    }
}