using Domain.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(300);
        
        builder.Property(p => p.Sku)
            .HasMaxLength(50);
        
        builder.Property(p => p.Description)
            .HasMaxLength(2000);
        
        builder.Property(p => p.Price)
            .HasPrecision(18, 2);
        
        builder.Property(p => p.Category)
            .HasMaxLength(100);

        builder.Property(p => p.CreatedAtUtc)
            .IsRequired();
        
        builder.HasIndex(p => p.Sku)
            .IsUnique()
            .HasFilter("[Sku] IS NOT NULL");
        
        builder.HasIndex(p => p.Category);
        builder.HasIndex(p => p.IsActive);
    }
}
