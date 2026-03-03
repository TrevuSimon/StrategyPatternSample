using Microsoft.EntityFrameworkCore;
using SalesApi.Models.Entities;

namespace SalesApi.Data;

public class SalesDbContext : DbContext
{
    public SalesDbContext(DbContextOptions<SalesDbContext> options) : base(options) { }

    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Sale> Sales => Set<Sale>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).HasMaxLength(200);
        });

        modelBuilder.Entity<Sale>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.CustomerName).HasMaxLength(200);
            entity.Property(e => e.RuleApplied).HasMaxLength(50);
            entity.Property(e => e.OriginalAmount).HasPrecision(18, 2);
            entity.Property(e => e.DiscountApplied).HasPrecision(18, 2);
            entity.Property(e => e.FinalAmount).HasPrecision(18, 2);
        });

        SeedData(modelBuilder);
    }

    private static void SeedData(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>().HasData(
            new Customer
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "VIP Customer",
                Type = Models.Enums.CustomerType.Vip,
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Employee John",
                Type = Models.Enums.CustomerType.Intern,
                CreatedAt = DateTime.UtcNow
            },
            new Customer
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Regular Customer",
                Type = Models.Enums.CustomerType.Customer,
                CreatedAt = DateTime.UtcNow
            }
        );
    }
}
