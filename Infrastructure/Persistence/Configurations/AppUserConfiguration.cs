using Domain.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> builder)
    {
        builder.ToTable("Users");
        builder.HasKey(user => user.Id);

        builder.HasIndex(user => user.Email)
            .IsUnique();

        builder.Property(user => user.Email)
            .HasMaxLength(160)
            .IsRequired();

        builder.Property(user => user.PasswordHash)
            .HasMaxLength(500)
            .IsRequired();

        builder.Property(user => user.Role)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(user => user.IsActive)
            .IsRequired();

        builder.Property(user => user.CreatedAtUtc)
            .IsRequired();

        builder.Navigation(user => user.Claims)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(user => user.Claims, claimBuilder =>
        {
            claimBuilder.ToTable("UserClaims");
            claimBuilder.WithOwner().HasForeignKey("UserId");
            claimBuilder.Property<int>("Id");
            claimBuilder.HasKey("Id");

            claimBuilder.Property(claim => claim.Type)
                .HasMaxLength(120)
                .IsRequired();

            claimBuilder.Property(claim => claim.Value)
                .HasMaxLength(240)
                .IsRequired();
        });
    }
}