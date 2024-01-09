using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using InventoryManagerAPI.Entities;

namespace InventoryManagerAPI.Database.Configurations;

public sealed class PurchaseDetailConfiguration : IEntityTypeConfiguration<PurchaseDetail>
{
    public void Configure(EntityTypeBuilder<PurchaseDetail> builder)
    {
        builder.ToTable("purchase_details");

        builder.HasKey(p => p.Id);

        builder.Property(x => x.Subtotal)
            .HasColumnName("subtotal")
            .IsRequired();

        builder.Property(x => x.Amount)
            .HasColumnName("amount")
            .IsRequired();

        builder.Property(x => x.PurchaseId)
            .HasColumnName("purchase_id")
            .IsRequired();

        builder.HasOne(x => x.Product)
            .WithMany()
            .HasForeignKey(x => x.ProductId)
            .OnDelete(DeleteBehavior.NoAction);

        builder.HasOne(x => x.Purchase)
            .WithMany(x => x.PurchaseDetails)
            .HasForeignKey(x => x.PurchaseId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
