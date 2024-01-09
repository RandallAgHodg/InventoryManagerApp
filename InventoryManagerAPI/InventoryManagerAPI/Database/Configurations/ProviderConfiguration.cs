using InventoryManagerAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagerAPI.Database.Configurations;

public sealed class ProviderConfiguration : IEntityTypeConfiguration<Provider>
{
    public void Configure(EntityTypeBuilder<Provider> builder)
    {
        builder.ToTable("providers");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .HasColumnName("name")
            .HasMaxLength(252)
            .IsRequired();

        builder.Property(x => x.Deleted)
            .HasColumnName("deleted")
            .HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.Deleted);

        builder.HasMany(x => x.Products)
            .WithOne(x => x.Provider)
            .HasForeignKey(x => x.ProviderId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}
