using Domain.Customers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable("Customers");
        
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
        
        builder.Property(c => c.PhoneNumber)
            .IsRequired()
            .HasMaxLength(20);
        
        builder.Property(c => c.Email)
            .HasMaxLength(200);
        
        builder.Property(c => c.Wilaya)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Commune)
            .IsRequired()
            .HasMaxLength(100);
        
        builder.Property(c => c.Address)
            .IsRequired()
            .HasMaxLength(500);
        
        builder.HasIndex(c => c.PhoneNumber);
        builder.HasIndex(c => c.Email);
    }
}
