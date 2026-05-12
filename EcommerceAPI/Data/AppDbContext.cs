using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<Gift> Gifts => Set<Gift>();
    public DbSet<GiftRule> GiftRules => Set<GiftRule>();
    public DbSet<OrderGift> OrderGifts => Set<OrderGift>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Product configuration
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Price).HasPrecision(10, 2);
        });

        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(20);
        });

        // Order configuration
        modelBuilder.Entity<Order>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.TotalAmount).HasPrecision(10, 2);
            entity.Property(e => e.Status).HasMaxLength(50);

            // Relationship with Customer
            entity.HasOne(e => e.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(e => e.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            // Relationship with OrderItems
            entity.HasMany(e => e.OrderItems)
                .WithOne(oi => oi.Order)
                .HasForeignKey(oi => oi.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // OrderItem configuration
        modelBuilder.Entity<OrderItem>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasPrecision(10, 2);

            // Relationship with Product
            entity.HasOne(e => e.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(e => e.ProductId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<Gift>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            entity.Property(e => e.Description).HasMaxLength(500);
        });

        modelBuilder.Entity<GiftRule>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.ConditionValue).IsRequired().HasMaxLength(200);
            entity.Property(e => e.RuleType).HasConversion<int>();

            entity.HasOne(e => e.Gift)
                .WithMany(g => g.GiftRules)
                .HasForeignKey(e => e.GiftId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<OrderGift>(entity =>
        {
            entity.HasKey(e => e.Id);

            entity.HasOne(e => e.Order)
                .WithMany(o => o.OrderGifts)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Gift)
                .WithMany(g => g.OrderGifts)
                .HasForeignKey(e => e.GiftId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.GiftRule)
                .WithMany()
                .HasForeignKey(e => e.GiftRuleId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        // Seed initial data
        SeedData(modelBuilder);
    }

    private void SeedData(ModelBuilder modelBuilder)
    {
        // Seed Products
        modelBuilder.Entity<Product>().HasData(
            new Product { Id = 1, Name = "Laptop", Description = "High-performance laptop", Price = 999.99m, StockQuantity = 10 },
            new Product { Id = 2, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, StockQuantity = 50 },
            new Product { Id = 3, Name = "Keyboard", Description = "Mechanical keyboard", Price = 79.99m, StockQuantity = 30 },
            new Product { Id = 4, Name = "Monitor", Description = "27-inch 4K monitor", Price = 299.99m, StockQuantity = 15 },
            new Product { Id = 5, Name = "USB-C Cable", Description = "High-speed USB-C cable", Price = 14.99m, StockQuantity = 100 }
        );

        // Seed Customers
        modelBuilder.Entity<Customer>().HasData(
            new Customer { Id = 1, Name = "Ahmed Hassan", Email = "ahmed@example.com", Phone = "+966501234567" },
            new Customer { Id = 2, Name = "Fatima Al-Rashid", Email = "fatima@example.com", Phone = "+966509876543" },
            new Customer { Id = 3, Name = "Mohammed Ali", Email = "mohammed@example.com", Phone = "+966551234567" }
        );
    }
}