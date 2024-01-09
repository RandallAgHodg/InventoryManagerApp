using InventoryManagerAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagerAPI.Database.Configurations;

public sealed class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder.ToTable("clients");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
           .HasColumnName("name")
           .HasMaxLength(252)
           .IsRequired();

        builder.Property(x => x.Deleted)
            .HasColumnName("deleted")
            .HasDefaultValue(false);

        builder.HasQueryFilter(x => !x.Deleted);
    }
}
