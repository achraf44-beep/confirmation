using Domain.Orders;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class AppDbContextSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        if (await dbContext.Orders.AnyAsync(cancellationToken))
        {
            return;
        }

        var seedOrders = new[]
        {
            Order.Create(
                "#83041",
                OrderSource.Shopify,
                "Confirmix test",
                "0556000010",
                "Alger",
                "Alger Centre",
                "Stop desk",
                [new OrderItem("Confirmix Product", 1, 2500m)],
                "Note for the manager",
                DateTime.UtcNow.AddHours(-18)),
            Order.Create(
                "#73460",
                OrderSource.WooCommerce,
                "Confirmix test",
                "0556001020",
                "Oran",
                "Oran Centre",
                "Home delivery",
                [new OrderItem("Arabic text - Quantity", 1, 3500m)],
                "Caller tried twice",
                DateTime.UtcNow.AddHours(-16)),
            Order.Create(
                "#11209",
                OrderSource.Manual,
                "Nadia B.",
                "0556002030",
                "Blida",
                "Blida Centre",
                "Office pickup",
                [new OrderItem("T-shirt", 1, 900m), new OrderItem("Cap", 1, 900m)],
                "Call after Maghrib",
                DateTime.UtcNow.AddHours(-12))
        };

        seedOrders[0].UpdateStatus(OrderStatus.Pending);
        seedOrders[1].UpdateStatus(OrderStatus.Confirmed);
        seedOrders[2].UpdateStatus(OrderStatus.CalledNoAnswer);

        dbContext.Orders.AddRange(seedOrders);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}