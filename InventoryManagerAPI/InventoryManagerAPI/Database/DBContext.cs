using Microsoft.EntityFrameworkCore;

namespace InventoryManagerAPI.Database;

public sealed class DBContext : DbContext
{
    public DBContext(DbContextOptions options) : base(options) {  }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DBContext).Assembly);
    }
}
