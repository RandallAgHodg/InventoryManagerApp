using InventoryManagerAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagerAPI.Database.Configurations;

public sealed class SaleConfiguration : IEntityTypeConfiguration<Sale>
{
    public void Configure(EntityTypeBuilder<Sale> builder)
    {
        builder.ToTable("sales");

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
            .WithMany(x => x.Sales)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasQueryFilter(x => !x.Deleted);
    }
}
