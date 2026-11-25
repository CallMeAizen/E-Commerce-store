using ECommerceStore.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ECommerceStore.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<Category> Categories => Set<Category>();
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Product>().HasData(
    new Product { Id = 1, Name = "Headphones", Description = "Wireless headphones", Price = 79.99m, Stock = 10, CreatedAt = new DateTime(2026, 11, 24, 0, 0, 0, DateTimeKind.Utc), CategoryId = 1 },
    new Product { Id = 2, Name = "Keyboard", Description = "Mechanical keyboard", Price = 49.99m, Stock = 25, CreatedAt = new DateTime(2026, 11, 24, 0, 0, 0, DateTimeKind.Utc), CategoryId = 2 },
    new Product { Id = 3, Name = "Mouse", Description = "Wireless mouse", Price = 29.99m, Stock = 50, CreatedAt = new DateTime(2026, 11, 24, 0, 0, 0, DateTimeKind.Utc), CategoryId = 2 }
);
        modelBuilder.Entity<Product>()
        .Property(p => p.Price)
        .HasColumnType("decimal(18,2)");

        modelBuilder.Entity<Category>()
    .Property(c => c.Name)
    .HasMaxLength(100)
    .IsRequired();

        modelBuilder.Entity<Product>()
            .Property(p => p.CategoryId)
            .HasDefaultValue(1);

        modelBuilder.Entity<Product>()
            .HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices" },
            new Category { Id = 2, Name = "Accessories", Description = "Product accessories" }
        );
    }
}
