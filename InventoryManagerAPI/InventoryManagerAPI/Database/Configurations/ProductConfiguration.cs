using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryManagerAPI.Entities;

namespace InventoryManagerAPI.Database.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("products");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder.Property(x => x.Cost)
            .HasColumnName("cost")
            .IsRequired();

        builder.Property(x => x.Price)
            .HasColumnName("price")
            .IsRequired();

        builder.Property(x => x.PictureUrl)
            .HasColumnName("picture_url")
            .IsRequired();

        builder.Property(x => x.Deleted)
            .HasColumnName("deleted")
            .HasDefaultValue(false);

        builder.Property(x => x.CreatedAt)
            .HasColumnName("inserted_at")
            .HasDefaultValue(DateTime.Now)
            .IsRequired();

        builder.Property(x => x.ProviderId)
            .HasColumnName("provider_id")
            .IsRequired();

        builder.HasOne(x => x.Provider)
            .WithMany(x => x.Products)
            .HasForeignKey(x => x.ProviderId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasQueryFilter(x => !x.Deleted);
    }

}
