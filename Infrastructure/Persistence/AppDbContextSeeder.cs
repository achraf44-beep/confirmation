using Domain.Customers;
using Domain.Delivery;
using Domain.Identity;
using Domain.Orders;
using Domain.Products;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public static class AppDbContextSeeder
{
    public static async Task SeedAsync(AppDbContext dbContext, CancellationToken cancellationToken = default)
    {
        await SeedDeliveryCompaniesAsync(dbContext, cancellationToken);
        await SeedAdminUserAsync(dbContext, cancellationToken);
        await SeedProductsAsync(dbContext, cancellationToken);
        await SeedCustomersAsync(dbContext, cancellationToken);
        await SeedOrdersAsync(dbContext, cancellationToken);
    }

    private static async Task SeedDeliveryCompaniesAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.DeliveryCompanies.AnyAsync(cancellationToken))
            return;

        var companies = new[]
        {
            DeliveryCompany.Create("Yalidine", "YALIDINE", "https://api.yalidine.app", null),
            DeliveryCompany.Create("ZR Express", "ZREXPRESS", "https://api.zrexpress.com", null),
            DeliveryCompany.Create("Maystro Delivery", "MAYSTRO", "https://api.maystro.com", null),
            DeliveryCompany.Create("EcoTrack", "ECOTRACK", "https://api.ecotrack.dz", null)
        };

        dbContext.DeliveryCompanies.AddRange(companies);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedAdminUserAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Users.AnyAsync(cancellationToken))
            return;

        var passwordHasher = new PasswordHasher<AppUser>();
        var admin = AppUser.Create("admin@wassel.dz", UserRole.Owner);
        admin.SetPasswordHash(passwordHasher.HashPassword(admin, "Admin123!"));
        admin.AddClaim("permissions", "orders.read");
        admin.AddClaim("permissions", "orders.write");

        var agent = AppUser.Create("agent@wassel.dz", UserRole.Agent);
        agent.SetPasswordHash(passwordHasher.HashPassword(agent, "Agent123!"));
        agent.AddClaim("permissions", "orders.read");
        agent.AddClaim("permissions", "orders.write");

        dbContext.Users.AddRange(admin, agent);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedProductsAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Products.AnyAsync(cancellationToken))
            return;

        var products = new[]
        {
            Product.Create("T-shirt Premium", 2500m, 100, "TSH-001", "T-shirt 100% coton", "Vêtements"),
            Product.Create("Casquette Sport", 1500m, 50, "CAP-001", "Casquette ajustable", "Accessoires"),
            Product.Create("Chaussures Running", 6500m, 30, "SHO-001", "Chaussures de course légères", "Chaussures"),
            Product.Create("Sac à dos", 3500m, 25, "BAG-001", "Sac à dos 25L imperméable", "Sacs"),
            Product.Create("Montre connectée", 12000m, 15, "WAT-001", "Smartwatch avec GPS", "Électronique")
        };

        dbContext.Products.AddRange(products);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedCustomersAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Customers.AnyAsync(cancellationToken))
            return;

        var customers = new[]
        {
            Customer.Create("Ahmed Benali", "0556000010", "Alger", "Alger Centre", "123 Rue Didouche Mourad", "ahmed@email.com"),
            Customer.Create("Fatima Zohra", "0556001020", "Oran", "Oran Centre", "45 Boulevard Front de Mer", "fatima@email.com"),
            Customer.Create("Karim Hadj", "0556002030", "Constantine", "Constantine Centre", "78 Rue Larbi Ben M'Hidi"),
            Customer.Create("Nadia Belkacem", "0556003040", "Blida", "Blida Centre", "12 Avenue de l'ALN"),
            Customer.Create("Youcef Mansouri", "0556004050", "Sétif", "Sétif Centre", "34 Rue du 8 Mai 1945")
        };

        dbContext.Customers.AddRange(customers);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    private static async Task SeedOrdersAsync(AppDbContext dbContext, CancellationToken cancellationToken)
    {
        if (await dbContext.Orders.AnyAsync(cancellationToken))
            return;

        var orders = new[]
        {
            Order.Create("#83041", OrderSource.Shopify, "Ahmed Benali", "0556000010", "Alger", "Alger Centre", "123 Rue Didouche Mourad",
                [new OrderItem("T-shirt Premium", 1, 2500m)], "Note for the manager", DateTime.UtcNow.AddHours(-18)),
            Order.Create("#83042", OrderSource.Facebook, "Fatima Zohra", "0556001020", "Oran", "Oran Centre", "45 Boulevard Front de Mer",
                [new OrderItem("Casquette Sport", 2, 1500m)], "Livraison urgente", DateTime.UtcNow.AddHours(-16)),
            Order.Create("#83043", OrderSource.WhatsApp, "Karim Hadj", "0556002030", "Constantine", "Constantine Centre", "78 Rue Larbi Ben M'Hidi",
                [new OrderItem("Chaussures Running", 1, 6500m)], "Taille 43", DateTime.UtcNow.AddHours(-14)),
            Order.Create("#83044", OrderSource.Instagram, "Nadia Belkacem", "0556003040", "Blida", "Blida Centre", "12 Avenue de l'ALN",
                [new OrderItem("Sac à dos", 1, 3500m), new OrderItem("Casquette Sport", 1, 1500m)], null, DateTime.UtcNow.AddHours(-12)),
            Order.Create("#83045", OrderSource.GoogleSheets, "Youcef Mansouri", "0556004050", "Sétif", "Sétif Centre", "34 Rue du 8 Mai 1945",
                [new OrderItem("Montre connectée", 1, 12000m)], "Emballage cadeau", DateTime.UtcNow.AddHours(-10)),
            Order.Create("#83046", OrderSource.Manual, "Client Test", "0556005060", "Béjaïa", "Béjaïa Centre", "Test Address",
                [new OrderItem("T-shirt Premium", 2, 2500m)], "Commande test", DateTime.UtcNow.AddHours(-8)),
            Order.Create("#83047", OrderSource.WooCommerce, "Sara Kaci", "0556006070", "Tizi Ouzou", "Tizi Ouzou Centre", "56 Rue Abane Ramdane",
                [new OrderItem("Chaussures Running", 1, 6500m)], null, DateTime.UtcNow.AddHours(-6))
        };

        orders[0].UpdateStatus(OrderStatus.Pending);
        orders[1].UpdateStatus(OrderStatus.Confirmed);
        orders[2].UpdateStatus(OrderStatus.CalledNoAnswer);
        orders[3].UpdateStatus(OrderStatus.Shipped);
        orders[4].UpdateStatus(OrderStatus.Delivered);
        orders[5].UpdateStatus(OrderStatus.Reprogrammed);
        orders[6].UpdateStatus(OrderStatus.Confirmed);

        dbContext.Orders.AddRange(orders);
        await dbContext.SaveChangesAsync(cancellationToken);
    }
}