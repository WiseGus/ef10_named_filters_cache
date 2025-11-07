namespace EF10_QueryCache;

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

public class AppDbContext : DbContext
{
    private readonly DbConnection _connection;
    private readonly Guid? _tenantId;

    public AppDbContext(DbConnection connection, Guid? tenantId)
    {
        _connection = connection;
        _tenantId = tenantId;
    }

    public static async Task InitAndSeedDb(SqliteConnection con)
    {
        using var dbCtx = new AppDbContext(con, null);

        dbCtx.Database.EnsureCreated();

        dbCtx.Set<ProductEntity>().Add(new ProductEntity { Name = "Product 1", TenantId = Guid.NewGuid(), Price = 10.0m });
        dbCtx.Set<ProductEntity>().Add(new ProductEntity { Name = "Product 2", TenantId = Guid.NewGuid(), Price = 20.0m });
        dbCtx.Set<ProductEntity>().Add(new ProductEntity { Name = "Product 3", TenantId = Guid.NewGuid(), Price = 30.0m });
        dbCtx.Set<ProductEntity>().Add(new ProductEntity { Name = "Product 4", TenantId = Guid.NewGuid(), Price = 40.0m });
        dbCtx.Set<ProductEntity>().Add(new ProductEntity { Name = "Product 5", TenantId = Guid.NewGuid(), Price = 50.0m });
        
        await dbCtx.SaveChangesAsync();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite(_connection);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ProductEntity>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired();
            entity.Property(e => e.TenantId).IsRequired();
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");

            entity.HasQueryFilter("TenantFilter", e => e.TenantId == _tenantId);
        });
    }
}