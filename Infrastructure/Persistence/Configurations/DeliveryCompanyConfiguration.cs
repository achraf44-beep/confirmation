using Domain.Delivery;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class DeliveryCompanyConfiguration : IEntityTypeConfiguration<DeliveryCompany>
{
    public void Configure(EntityTypeBuilder<DeliveryCompany> builder)
    {
        builder.ToTable("DeliveryCompanies");
        
        builder.HasKey(d => d.Id);
        
        builder.Property(d => d.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(d => d.Code)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(d => d.ApiEndpoint)
            .HasMaxLength(500);
        
        builder.Property(d => d.ApiKey)
            .HasMaxLength(500);

        builder.Property(d => d.CreatedAtUtc)
            .IsRequired();
        
        builder.HasIndex(d => d.Code)
            .IsUnique();
    }
}
