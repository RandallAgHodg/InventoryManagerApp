using InventoryManagerAPI.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace InventoryManagerAPI.Database.Configurations;

public sealed class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirstName)
            .HasColumnName("firstname")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.LastName)
            .HasColumnName("lastname")
            .HasMaxLength(255)
            .IsRequired();

        builder.Property(x => x.PictureUrl)
            .HasColumnName("picture_url")
            .IsRequired();

        builder.Property(x => x.Password)
            .HasColumnName("password")
            .IsRequired();

        builder.Property(x => x.Email)
            .HasColumnName("email")
            .HasMaxLength(255)
            .IsRequired();
    }
}
