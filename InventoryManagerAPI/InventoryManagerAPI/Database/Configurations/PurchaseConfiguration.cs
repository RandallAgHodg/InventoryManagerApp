using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryManagerAPI.Entities;


namespace InventoryManagerAPI.Database.Configurations;

public sealed class PurchaseConfiguration : IEntityTypeConfiguration<Purchase>
{
    public void Configure(EntityTypeBuilder<Purchase> builder)
    {
        builder.ToTable("purchases");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Total)
            .HasColumnName("total")
            .HasDefaultValue(0);

        builder.Property(x => x.UserId)
            .HasColumnName("user_id")
            .IsRequired();

        builder.Property(x => x.CreatedAt)
            .HasColumnName("created_at")
            .HasDefaultValue(DateTime.UtcNow)
            .IsRequired();

        builder.Property(x => x.Deleted)
            .HasColumnName("deleted")
            .HasDefaultValue(false);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Purchases)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasQueryFilter(x => !x.Deleted);
    }
}
