using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Xana.Domain.Entities;

namespace Xana.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.FirebaseUid)
            .IsRequired()
            .HasMaxLength(255);

        builder.HasIndex(x => x.FirebaseUid)
            .IsUnique();

        builder.Property(x => x.Email)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.DisplayName)
            .HasMaxLength(100);

        builder.Property(x => x.PhotoUrl)
            .HasMaxLength(2048);
    }
}
