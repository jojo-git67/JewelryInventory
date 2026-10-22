using JewelryInventory.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace JewelryInventory.Api.Data;


public class InventoryDbContext : DbContext
{
    public InventoryDbContext(DbContextOptions<InventoryDbContext> options)
        :base(options){ }
    
    public DbSet<Component> Components =>  Set<Component>();
    public DbSet<ComponentInventoryMovement> ComponentInventoryMovements =>  Set<ComponentInventoryMovement>();
    
    public DbSet<ManualAdjustment> ManualAdjustments =>  Set<ManualAdjustment>();
    public DbSet<ReceiveOrderRow> ReceiveOrderRows =>  Set<ReceiveOrderRow>();
    public DbSet<ReceiveOrder> ReceiveOrders =>  Set<ReceiveOrder>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<ComponentInventoryMovement>()
            .HasOne(m => m.Component)
            .WithMany(c => c.InventoryMovements)
            .HasForeignKey(m => m.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReceiveOrderRow>()
            .HasOne(row => row.ReceiveOrder)
            .WithMany(order => order.ReceiveOrderRows)
            .HasForeignKey(row => row.ReceiveOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComponentInventoryMovement>()
            .HasOne(m => m.ManualAdjustment)
            .WithOne(a => a.InventoryMovement)
            .HasForeignKey<ComponentInventoryMovement>(m => m.ManualAdjustmentId)
            .OnDelete(DeleteBehavior.Restrict);
        
        modelBuilder.Entity<ComponentInventoryMovement>()
            .HasOne(m => m.ReceiveOrderRow)
            .WithOne(r => r.InventoryMovement)
            .HasForeignKey<ComponentInventoryMovement>(m => m.ReceiveOrderRowId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ManualAdjustment>()
            .HasOne(c => c.Component)
            .WithMany()
            .HasForeignKey(c => c.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ReceiveOrderRow>()
            .HasOne(row => row.Component)
            .WithMany()
            .HasForeignKey(row => row.ComponentId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<ComponentInventoryMovement>()
            .ToTable(t => t.HasCheckConstraint(
                "CK_InventoryMovement_ExactlyOneSource",
                """
                (ReceiveOrderRowId IS NOT NULL AND ManualAdjustmentId IS NULL)
                OR
                (ReceiveOrderRowId IS NULL AND ManualAdjustmentId IS NOT NULL)
                """));

        modelBuilder.Entity<Component>()
            .HasIndex(c => c.Barcode)
            .IsUnique();
    }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder.Properties<DateTime>()
            .HaveConversion<UtcDateTimeConverter>();
    }
}