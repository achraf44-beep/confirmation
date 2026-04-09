using Domain.Orders;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(order => order.Id);

        builder.Property(order => order.OrderNumber)
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(order => order.Source)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(order => order.CustomerName)
            .HasMaxLength(150)
            .IsRequired();

        builder.Property(order => order.PhoneNumber)
            .HasMaxLength(30)
            .IsRequired();

        builder.Property(order => order.Wilaya)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(order => order.Commune)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(order => order.DeliveryAddress)
            .HasMaxLength(250)
            .IsRequired();

        builder.Property(order => order.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(order => order.Note)
            .HasMaxLength(500);

        builder.Property(order => order.CreatedAtUtc)
            .IsRequired();

        builder.Navigation(order => order.Items)
            .UsePropertyAccessMode(PropertyAccessMode.Field);

        builder.OwnsMany(order => order.Items, itemBuilder =>
        {
            itemBuilder.ToTable("OrderItems");
            itemBuilder.WithOwner().HasForeignKey("OrderId");
            itemBuilder.Property<int>("Id");
            itemBuilder.HasKey("Id");

            itemBuilder.Property(item => item.ProductName)
                .HasMaxLength(150)
                .IsRequired();

            itemBuilder.Property(item => item.Quantity)
                .IsRequired();

            itemBuilder.Property(item => item.UnitPrice)
                .HasPrecision(18, 2)
                .IsRequired();
        });
    }
}